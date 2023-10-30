using System.Collections.Generic;
using NServiceBus;

public static class Extensions
{
    public static RoutingSettings<MsmqTransport> SetupTransportReceiveOnly(this EndpointConfiguration configuration, int numberOfWorkerThreads, string connectionString)
    {
        var transport = new MsmqTransport();
        
        transport.DelayedDelivery = GetTransportDelayedDeliverySettings(connectionString);
        transport.TransportTransactionMode = TransportTransactionMode.ReceiveOnly;
        configuration.LimitMessageProcessingConcurrencyTo(numberOfWorkerThreads);
        
        return configuration.UseTransport(transport);
    }
    private static DelayedDeliverySettings GetTransportDelayedDeliverySettings(string connectionString)
    {
        // https://docs.particular.net/transports/msmq/delayed-delivery
        var messageStore = new SqlServerDelayedMessageStore(
            connectionString: connectionString);

        return new DelayedDeliverySettings(messageStore)
        {
            NumberOfRetries = 3,
            MaximumRecoveryFailuresPerSecond = 2
        };
    }

}