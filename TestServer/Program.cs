using MainServer;
using TestServer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<Network>();
        services.AddSingleton<ReciveHandler>();
        services.AddSingleton<EventConnection>();

        services.AddSingleton<CCommandHandler>();
        services.AddSingleton<ServerHandler>();
        services.AddHostedService<CustomServer>();
    })
    .Build();

await host.RunAsync();
