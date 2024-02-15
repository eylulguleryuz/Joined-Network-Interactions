namespace Clients;

using NLog;
using Grpc.Net.Client;

//this comes from GRPC generated code
using Services;


/// <summary>
/// Client
/// </summary>
class Baker
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
				//connect to the server, get service proxy
				var channel = GrpcChannel.ForAddress("http://127.0.0.1:5003");
				var canteen = new Service.ServiceClient(channel);

			
				//do the baker stuff
				while( true )
				{
					int food = rnd.Next(1,6); //bake 1 to 6 portions of food
					bool closed = canteen.CloseCanteen(new Empty()).Value;
                    if (!closed)
					{
						bool bakeTime = canteen.BakerBakes(new Empty()).Value;
						if (bakeTime)
						{
							canteen.Bake(new IntMsg { Value = food });
							mLog.Info($"Baker baked {food} portions");
							mLog.Info(" ");
					
						}else{
							
							canteen.BakerLeaves(new Empty());
							int hour = canteen.GetHour(new Empty()).Value;
							if(hour > 16){
								Thread.Sleep (TimeSpan.FromSeconds((double)((24-hour)+7)));
							}else{
								Thread.Sleep (TimeSpan.FromSeconds((double)(Math.Abs(7-hour))));
							}
						}

						Thread.Sleep(2500);
					}else{
						Thread.Sleep (TimeSpan.FromSeconds(4.0));
					}
					Thread.Sleep(2000);					
			}}
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
		var self = new Baker();
		self.Run();
	}
}