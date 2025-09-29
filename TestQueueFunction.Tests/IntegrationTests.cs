using Azure.Storage.Queues;

namespace TestQueueFunction.Tests;

public class IntegrationTests(TestApplicationFactory testApplicationFactory)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private readonly TestApplicationFactory _testApplicationFactory = testApplicationFactory;

    [Fact]
    public async Task WhenMessageIsPublished_TestFunctionConsumesIt()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await _testApplicationFactory.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        var connString = await _testApplicationFactory.GetConnectionString("queue");

        var queueClient = new QueueClient(connString, "queue");
        var response = await queueClient.SendMessageAsync("TEST", cancellationToken);

        // Assert
        Assert.NotNull(response.Value?.MessageId);

        var message = await queueClient.PeekMessageAsync(cancellationToken);
        var count = 0;
        while (count++ < 5)
        {
            if (message is null) break;

            Thread.Sleep(1000); // Wait for the function to process the message
            message = await queueClient.PeekMessageAsync(cancellationToken);
        }
        Assert.Null(message?.Value);
    }
}
