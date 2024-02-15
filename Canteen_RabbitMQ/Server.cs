namespace Servers;

using NLog;


/// <summary>
/// Server
/// </summary>
class Server
{
	/// <summary>
	/// Logger for this class.
	/// </summary>
	private Logger log = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Configure loggin subsystem.
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

		while( true )
		{
			try 
			{
				//start service
				var service = new CanteenService();

				//
				log.Info("Server has been started.");

				//hang main thread						
				while( true ) {
					Thread.Sleep(1000);
				}
			}
			catch( Exception e ) 
			{
				//log exception
				log.Error(e, "Unhandled exception caught. Server will now restart.");

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
		var self = new Server();
		self.Run();
	}
}