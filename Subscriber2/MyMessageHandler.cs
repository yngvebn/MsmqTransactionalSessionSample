using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Subscriber2
{
    public class MyMessageHandler : IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("MyMessage ontvangen");

            return Task.CompletedTask;
        }
    }
}