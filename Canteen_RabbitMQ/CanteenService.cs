namespace Servers;

using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NLog;
using Newtonsoft.Json;
using Commons.MQs;
using Services;


/// <summary>
/// Service
/// </summary>
class CanteenService
{
	/// <summary>
	/// Name of the request exchange.
	/// </summary>
	private static readonly String ExchangeName = "T120B180.Canteen.Exchange";

	/// <summary>
	/// Name of the request queue.
	/// </summary>
	private static readonly String ServerQueueName = "T120B180.Canteen.CanteenService";


	/// <summary>
	/// Logger for this class.
	/// </summary>
	private Logger log = LogManager.GetCurrentClassLogger();


	/// <summary>
	/// Connection to RabbitMQ message broker.
	/// </summary>
	private IConnection rmqConn;

	/// <summary>
	/// Communications channel to RabbitMQ message broker.
	/// </summary>
	private IModel rmqChann;

	/// <summary>
	/// Service logic.
	/// </summary>
	private CanteenLogic logic = new CanteenLogic();


	/// <summary>
	/// Constructor.
	/// </summary>
	public CanteenService()
	{
		//connect to the RabbitMQ message broker
		var rmqConnFact = new ConnectionFactory();
		rmqConn = rmqConnFact.CreateConnection();

		//get channel, configure exchanges and request queue
		rmqChann = rmqConn.CreateModel();

		rmqChann.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
		rmqChann.QueueDeclare(queue: ServerQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
		rmqChann.QueueBind(queue: ServerQueueName, exchange: ExchangeName, routingKey: ServerQueueName, arguments: null);

		//connect to the queue as consumer
		//XXX: see https://www.rabbitmq.com/dotnet-api-guide.html#concurrency for threading issues
		var rmqConsumer = new EventingBasicConsumer(rmqChann);
		rmqConsumer.Received += (consumer, delivery) => OnMessageReceived(((EventingBasicConsumer)consumer).Model, delivery);
		rmqChann.BasicConsume(queue: ServerQueueName, autoAck: true, consumer : rmqConsumer);
	}

	/// <summary>
	/// Is invoked to process messages received.
	/// </summary>
	/// <param name="channel">Related communications channel.</param>
	/// <param name="msgIn">Message deliver data.</param>
	private void OnMessageReceived(IModel channel, BasicDeliverEventArgs msgIn)
	{
		try
		{
			//get call request
			var request =
				JsonConvert.DeserializeObject<RPCMessage>(
					Encoding.UTF8.GetString(
						msgIn.Body.ToArray()
					)
				);

			//set response as undefined by default
			RPCMessage response = null;

			//process the call
			switch( request.Action )
			{
				case $"Call_{nameof(logic.Count24H)}":
				{
					//make the call
					logic.Count24H();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.Count24H)}",
							Data = JsonConvert.SerializeObject(new {}) 
						};

					//
					break;
				}

				case $"Call_{nameof(logic.Bake)}":
				{
					//deserialize arguments
					var args = JsonConvert.DeserializeAnonymousType(request.Data, new {Portions = 0});

					//make the call
					logic.Bake(args.Portions);

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.Bake)}",
							Data = JsonConvert.SerializeObject(new {}) 
						};

					//
					break;
				}
				
				case $"Call_{nameof(logic.Eat)}":
				{
					//deserialize arguments
					var args = JsonConvert.DeserializeAnonymousType(request.Data, new {Portions = 0});

					//make the call
					logic.Eat(args.Portions);

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.Eat)}",
							Data = JsonConvert.SerializeObject(new {}) 
						};

					//
					break;
				}

				case $"Call_{nameof(logic.BakerBakes)}":
				{
					//make the call
					var result = logic.BakerBakes();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.BakerBakes)}",
							Data = JsonConvert.SerializeObject(new {Value = result})
						};

					//
					break;
				}

				case $"Call_{nameof(logic.EaterEats)}":
				{
					//make the call
					var result = logic.EaterEats();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.EaterEats)}",
							Data = JsonConvert.SerializeObject(new {Value = result})
						};

					//
					break;
				}

				case $"Call_{nameof(logic.CloseCanteen)}":
				{
					//make the call
					var result = logic.CloseCanteen();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.CloseCanteen)}",
							Data = JsonConvert.SerializeObject(new {Value = result})
						};

					//
					break;
				}

				case $"Call_{nameof(logic.BakerLeaves)}":
				{
					//make the call
					logic.BakerLeaves();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.BakerLeaves)}",
							Data = JsonConvert.SerializeObject(new {})
						};

					//
					break;
				}
				
				case $"Call_{nameof(logic.EaterLeaves)}":
				{
					//make the call
					logic.EaterLeaves();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.EaterLeaves)}",
							Data = JsonConvert.SerializeObject(new {})
						};

					//
					break;
				}
				case $"Call_{nameof(logic.GetHour)}":
				{
					//make the call
					var result = logic.GetHour();

					//create response
					response =
						new RPCMessage() {
							Action = $"Result_{nameof(logic.GetHour)}",
							Data = JsonConvert.SerializeObject(new {Value = result})
						};

					//
					break;
				}

				default:
				{
					log.Info($"Unsupported type of RPC action '{request.Action}'. Ignoring the message.");
					break;
				}
			}

			//response is defined? send reply message
			if( response != null )
			{
				//prepare metadata for outgoing message
				var msgOutProps = channel.CreateBasicProperties();
				msgOutProps.CorrelationId = msgIn.BasicProperties.CorrelationId;

				//send reply message to the client queue
				channel.BasicPublish(
					exchange : ExchangeName,
					routingKey : msgIn.BasicProperties.ReplyTo,
					basicProperties : msgOutProps,
					body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response))
				);
			}
		}
		catch( Exception e )
		{
			log.Error(e, "Unhandled exception caught when processing a message. The message is now lost.");
		}
	}	
}