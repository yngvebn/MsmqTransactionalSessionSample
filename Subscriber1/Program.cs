using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Subscriber1
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new EndpointConfiguration("Subscriber1");
            var transport = config.UseTransport(new MsmqTransport());
            transport.DisablePublishing();

            transport.RegisterPublisher(typeof(MyMessage), "Publisher");

            config.SendFailedMessagesTo("error");

            config.EnableInstallers();

            var endpoint = await Endpoint.Start(config);

            Console.WriteLine("Press key to quit");
            Console.ReadKey();
        }
    }
}