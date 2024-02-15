using System;
using System.Threading;
using System.Net.Http;
using NLog;

namespace Server
{
    public class AdapterLogic : IAdapter
    {
        // private static readonly HttpClient Client = new();

        public int GetHour()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding GetHour() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            return service.GetHour();
        }

        public bool EaterEats()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding EaterEats() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            return service.EaterEats();
        }
        public bool BakerBakes()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding BakerBakes() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            return service.BakerBakes();
        }

        public bool CloseCanteen()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding CloseCanteen() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            return service.CloseCanteen();
        }
        public void Count24H()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding Count24H() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            service.Count24H();
        }
        public void Bake(int portions)
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding Bake() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            service.Bake(portions);
        }
        public void Eat(int portions)
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding Eat() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            service.Eat(portions);
        }
        public void BakerLeaves()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding BakerLeaves() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            service.BakerLeaves();
        }
        public void EaterLeaves()
        {
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}| Forwarding EaterLeaves() request from gRPC client to RabbitMQ server");
            Console.WriteLine();

            var service = new ServiceClient();
            service.EaterLeaves();
        }
    
    }
}
