using DynamicServer;
using TestServer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<CCommandHandler>();
    })
    .AddClientConnection<EventConnection>()
    .AddServerServices<CustomServer>()
    .Build();

await host.RunAsync();
