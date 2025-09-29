using Aspire.Hosting;
using Microsoft.Extensions.Logging;

[assembly: AssemblyFixture(typeof(TestQueueFunction.Tests.TestApplicationFactory))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestQueueFunction.Tests;

public class TestApplicationFactory()
    : DistributedApplicationFactory(typeof(Projects.TestQueueFunction_AppHost))
{
    protected override void OnBuilderCreated(DistributedApplicationBuilder builder)
    {
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole(); // Outputs logs to console
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            logging.AddXUnit(TestContext.Current.TestOutputHelper!);
        });
        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    }
}