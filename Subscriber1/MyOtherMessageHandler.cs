using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Subscriber1
{
    public class MyOtherMessageHandler : IHandleMessages<MyOtherMessage>
    {
        public Task Handle(MyOtherMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("Received MyOtherMessage as well");

            return Task.CompletedTask;
        }
    }
}