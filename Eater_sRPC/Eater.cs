namespace Clients;

using Microsoft.Extensions.DependencyInjection;

using SimpleRpc.Serialization.Hyperion;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;

using NLog;

using Services;


/// <summary>
/// Client example.
/// </summary>
class Eater
{

	/// <summary>
	/// Logger for this class.
	/// </summary>
	Logger mLog = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Configures logging subsystem.
	/// </summary>
	private void ConfigureLogging()
	{
		var config = new NLog.Config.LoggingConfiguration();

		var console =
			new NLog.Targets.ConsoleTarget("console")
			{
				Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
			};
		config.AddTarget(console);
		config.AddRuleForAllLevels(console);

		LogManager.Configuration = config;
	}

	/// <summary>
	/// Program body.
	/// </summary>
	private void Run() {
		//configure logging
		ConfigureLogging();

		//initialize random number generator
		var rnd = new Random();

		//run everythin in a loop to recover from connection errors
		while( true )
		{
			try {
				//connect to the server, get service client proxy
				var sc = new ServiceCollection();
				sc
					.AddSimpleRpcClient(
						"CanteenService", //must be same as on line 86
						new HttpClientTransportOptions
						{
							Url = "http://127.0.0.1:5002/simplerpc",
							Serializer = "HyperionMessageSerializer"
						}
					)
					.AddSimpleRpcHyperionSerializer();

				sc.AddSimpleRpcProxy<ICanteenService>("CanteenService"); 

				var sp = sc.BuildServiceProvider();

				var canteen = sp.GetService<ICanteenService>();

				//do the eater stuff
				while( true )
				{
					int food = rnd.Next(1,7); //eat 1 to 7 portions of food
					bool closed = canteen.CloseCanteen();
					int hour = canteen.GetHour();
                    if (!closed)
					{
						bool eatTime = canteen.EaterEats();
						if (eatTime)
						{
							mLog.Info($"Eaters ate {food} portions");
							canteen.Eat(food);
							mLog.Info(" ");

						}else{
							canteen.EaterLeaves();
							if(hour > 18){
								Thread.Sleep (TimeSpan.FromSeconds((double)((24-hour)+11)));
							}else{
								try{
								Thread.Sleep (TimeSpan.FromSeconds((double)(Math.Abs(11-hour))));
								}
								catch(Exception e)
								{mLog.Info($"!!!! EXCEPTION at {hour} ");
									mLog.Info($"{e} ");
								}
							}
						}

						Thread.Sleep(1500);
					}else{
						Thread.Sleep (TimeSpan.FromSeconds(4.0));
					}
					Thread.Sleep(2000);
									
				}
			}
			catch( Exception e )
			{
				//log whatever exception to console
				mLog.Warn(e, "Unhandled exception caught. Will restart main loop.");

				//prevent console spamming
				Thread.Sleep(2000);
			}
		}
	}

	/// <summary>
	/// Program entry point.
	/// </summary>
	/// <param name="args">Command line arguments.</param>
	static void Main(string[] args)
	{
		var self = new Eater();
		self.Run();
	}
}
