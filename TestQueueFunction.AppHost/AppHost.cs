var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
storage.AddQueue("test-queue", "queue"); // This creates the queue in the storage account
var queue = storage.AddQueues("queue"); // this creates a reference to the queue, the above should be the only thing needed though :(

builder.AddAzureFunctionsProject<Projects.TestQueueFunction>("function")
    .WithReference(queue)
    .WaitFor(queue);

builder.Build().Run();
