namespace Servers;

using System;
using System.Text;
using System.Threading;
using Services;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using NLog;

using Commons.MQs;


/// <summary>
/// <para>RPC style wrapper for the service.</para>
/// <para>Static members are thread safe, instance members are not.</para>
/// </summary>
class ServiceClient : ICanteenService
{/// <summary>
	/// Name of the message exchange.
	/// </summary>
	private static readonly String ExchangeName = "T120B180.Canteen.Exchange";

	/// <summary>
	/// Name of the server queue.
	/// </summary>
	private static readonly String ServerQueueName = "T120B180.Canteen.CanteenService";

	/// <summary>
	/// Prefix for the name of the client queue.
	/// </summary>
	private static readonly String ClientQueueNamePrefix = "T120B180.Canteen.CanteenClient_";



    /// <summary>
    /// Logger for this class.
    /// </summary>
    private Logger log = LogManager.GetCurrentClassLogger();


    /// <summary>
    /// Service client ID.
    /// </summary>
    public String ClientId { get; }

    /// <summary>
    /// Name of the client queue.
    /// </summary>
    private String ClientQueueName { get; }


    /// <summary>
    /// Connection to RabbitMQ message broker.
    /// </summary>
    private IConnection rmqConn;

    /// <summary>
    /// Communications channel to RabbitMQ message broker.
    /// </summary>
    private IModel rmqChann;


    /// <summary>
    /// Constructor.
    /// </summary>
    public ServiceClient()
    {
        //initialize properties
        ClientId = Guid.NewGuid().ToString();
        ClientQueueName = ClientQueueNamePrefix + ClientId;

        //connect to the RabbitMQ message broker
        var rmqConnFact = new ConnectionFactory();
        rmqConn = rmqConnFact.CreateConnection();

        //get channel, configure exchange and queue
        rmqChann = rmqConn.CreateModel();

        rmqChann.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        rmqChann.QueueDeclare(queue: ClientQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        rmqChann.QueueBind(queue: ClientQueueName, exchange: ExchangeName, routingKey: ClientQueueName, arguments: null);

        //XXX: see https://www.rabbitmq.com/dotnet-api-guide.html#concurrency for threading issues
    }

    /// <summary>
    /// Generic method to call a remove operation on a server.
    /// </summary>
    /// <param name="requestAction">Name of the request action.</param>
    /// <param name="requestDataProvider">Request data provider.</param>
    /// <param name="responseAction">Name of the response action.</param>
    /// <param name="resultExtractor">Result extractor.</param>
    /// <typeparam name="RESULT">Type of the result.</typeparam>
    /// <returns>Result of the call.</returns>
    private RESULT Call<RESULT>(
        string methodName,
		Func<String> requestDataProvider,
		Func<String, RESULT> resultDataExtractor
	) {
		//validate inputs
		if( methodName == null )
			throw new ArgumentException("Argument 'methodName' is null.");

		//declare result storage
		RESULT result = default;

		//declare stuff used to avoid result owerwriting and to signal when result is ready
		var isResultReady = false;
		var resultReadySignal = new AutoResetEvent(false);

		//create request
		var request =
			new RPCMessage()
			{
				Action = $"Call_{methodName}",
				Data = requestDataProvider != null ? requestDataProvider() : null
			};

		var requestProps = rmqChann.CreateBasicProperties();
		requestProps.CorrelationId = Guid.NewGuid().ToString();
		requestProps.ReplyTo = ClientQueueName;

		//result data extractor set? set-up receiver for response message
		string consumerTag = null;

		if( resultDataExtractor != null )
		{
			//ensure contents of variables set in main thread, are loadable by receiver thread
			Thread.MemoryBarrier();

			//create response message consumer
			var consumer = new EventingBasicConsumer(rmqChann);
			consumer.Received +=
				(channel, delivery) => {
					//ensure contents of variables set in main thread are loaded into this thread
					Thread.MemoryBarrier();

					//prevent owerwriting of result, check if the expected message is received
					if( !isResultReady && (delivery.BasicProperties.CorrelationId == requestProps.CorrelationId) )
					{
						var response = JsonConvert.DeserializeObject<RPCMessage>(Encoding.UTF8.GetString(delivery.Body.ToArray()));
						if( response.Action == $"Result_{methodName}" )
						{
							//extract the result
							result = resultDataExtractor(response.Data);

							//indicate result has been received, ensure it is loadable by main thread
							isResultReady = true;
							Thread.MemoryBarrier();

							//signal main thread that result has been received
							resultReadySignal.Set();
						}
						else
						{
							log.Info($"Unsupported type of RPC action '{request.Action}'. Ignoring the message.");
						}
					}
				};

			//attach message consumer to the response queue
			consumerTag = rmqChann.BasicConsume(ClientQueueName, true, consumer);
		}

		//send request
		rmqChann.BasicPublish(
			exchange : ExchangeName,
			routingKey : ServerQueueName,
			basicProperties : requestProps,
			body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))
		);

		//result data extractor set? await for response message
		if( resultDataExtractor != null )
		{
			//wait for the result to be ready
			resultReadySignal.WaitOne();

			//ensure contents of variables set by the receiver are loaded into this thread
			Thread.MemoryBarrier();

			//detach message consumer from the response queue
			rmqChann.BasicCancel(consumerTag);

			//
			return result;		
		}

		//we did not wait for response, return default value of whatever is expected
		return default;
	}


	/// <summary>
	/// Countdown for a day (24hours)
	/// </summary>
	public void Count24H()
	{
		Call(
			nameof(Count24H),
			() => JsonConvert.SerializeObject(new {}),
			(data) => JsonConvert.DeserializeAnonymousType(data, new {})
		);
	} 
	

    /// <summary>
    /// Baker bakes new portions of food
    /// </summary>
    /// <param name="portions">amount of portions to bake</param>
	public void Bake(int portions)
	{
		var result =
			Call(
				nameof(Bake),
				() => JsonConvert.SerializeObject(new {Portions = portions}),
				(data) => JsonConvert.DeserializeAnonymousType(data, new {})
			);
	} 

    /// <summary>
    /// Eater eats indicated amount of portions and necessary actions are implemented 
    /// </summary>
    /// <param name="portions">amount of portions to bake</param>
	public void Eat(int portions)
	{
		var result =
			Call(
				nameof(Eat),
				() => JsonConvert.SerializeObject(new {Portions = portions}),
				(data) => JsonConvert.DeserializeAnonymousType(data, new {})
			);
	} 

    /// <summary>
    /// Baking time
    /// </summary>
    /// <returns>if it is time or not</returns>
	public bool BakerBakes()
	{
		var result =
			Call(
			nameof(BakerBakes),
			() => JsonConvert.SerializeObject(new {}),
            (data) => JsonConvert.DeserializeAnonymousType(data, new { Value = false }).Value
		);
		return result;
	}    
	
    /// <summary>
    /// Eating time
    /// </summary>
    /// <returns>If it is time or not</returns>
	public bool EaterEats()
	{
		var result =
			Call(
			nameof(EaterEats),
			() => JsonConvert.SerializeObject(new {}),
            (data) => JsonConvert.DeserializeAnonymousType(data, new { Value = false }).Value
		);
		return result;
	}
		
    /// <summary>
    /// Checks if the canteen is closed
    /// </summary>
    /// <returns>if canteen is closed or not</returns>
    public bool CloseCanteen()
	{
		var result =
			Call(
			nameof(CloseCanteen),
			() => JsonConvert.SerializeObject(new {}),
            (data) => JsonConvert.DeserializeAnonymousType(data, new { Value = false}).Value
			);
		return result;
	}
	    
    /// <summary>
    /// Logs baker state (leaving)
    /// </summary>
    public void BakerLeaves()
	{	
		Call(
			nameof(BakerLeaves),
			() => JsonConvert.SerializeObject(new {}),
			(data) => JsonConvert.DeserializeAnonymousType(data, new {})
		);
	}

    /// <summary>
    /// Logs eater state (leaving)
    /// </summary>
    public void EaterLeaves()
	{
		Call(
			nameof(EaterLeaves),
			() => JsonConvert.SerializeObject(new {}),
			(data) => JsonConvert.DeserializeAnonymousType(data, new {})
		);
	}

	/// <summary>
    /// Gets the time according to the 24hr system
    /// </summary>
    /// <returns>the current hour</returns>
	public int GetHour()
	{
		var result =
			Call(
			nameof(GetHour),
			() => JsonConvert.SerializeObject(new {}),
			(data) => JsonConvert.DeserializeAnonymousType(data, new {Value = 0}).Value
		);
		return result;
	}


}