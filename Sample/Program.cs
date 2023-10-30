using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.TransactionalSession;
using Shared;

class Program
{
    const string ConnectionString =
        $@"Data Source=localhost;Initial Catalog=nservicebus;Integrated Security=True;Encrypt=false";

    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        host.Start();
        using (var childScope = host.Services.CreateScope())
        {
            var session = childScope.ServiceProvider.GetService<ITransactionalSession>();
            await session.Open(new SqlPersistenceOpenSessionOptions())
                .ConfigureAwait(false);

            var myMessage = new MyMessage();
            await session.Publish(myMessage).ConfigureAwait(false);

            var myOtherMessage = new MyOtherMessage();
            await session.Publish(myOtherMessage).ConfigureAwait(false);

            await session.Commit().ConfigureAwait(false);
        }

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await host.StopAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .ConfigureLogging(logging => { logging.AddConsole(); })
            .UseNServiceBus(ctx =>
            {
                Console.Title = "Publisher";


                var endpointConfiguration = new EndpointConfiguration("Publisher");
                var transport = endpointConfiguration.SetupTransportReceiveOnly(1, ConnectionString);

                // endpointConfiguration.SendOnly();

                transport.RegisterPublisher(typeof(MyMessage), "Publisher");
                endpointConfiguration.EnableOutbox();


                var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                persistence.SqlDialect<SqlDialect.MsSqlServer>();
                persistence.ConnectionBuilder(() => new SqlConnection(ConnectionString));

                persistence.EnableTransactionalSession();

                var subscriptions = persistence.SubscriptionSettings();
                subscriptions.DisableCache();


                endpointConfiguration.SendFailedMessagesTo("error");

                endpointConfiguration.EnableInstallers();
                endpointConfiguration.Recoverability().Delayed(d => d.NumberOfRetries(0));

                return endpointConfiguration;
            });
    }
}