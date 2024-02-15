using System;
using System.Threading;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Server
{
    /// <summary>
    /// class of application's entry point
    /// </summary>
    public class Server
    {
        /// <summary>
        /// program's body
        /// </summary>
        /// <param name="args"> program's arguments </param>
        void Run(string[] args)
        {
            // configure and start the server
            CreateHostBuilder(args).Build().RunAsync();

            // suspend the main thread
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - gRPC adapter is running");
            Console.WriteLine();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// a method for configuring and running the server
        /// </summary>
        /// <param name="args"> arguments </param>
        /// <returns> IHostBuilder item </returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return
                Host
                    .CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(hcfg =>
                    {
                        hcfg.UseKestrel();
                        hcfg.ConfigureKestrel(kcfg =>
                        {
                            kcfg.Listen(IPAddress.Loopback, 5003);
                        });
                        hcfg.UseStartup<Configuration>();
                    });
        }

        /// <summary>
        /// program's entry point
        /// </summary>
        /// <param name="args"> program's arguments </param>
        static void Main(string[] args)
        {
            var self = new Server();
            self.Run(args);
        }
    }
}