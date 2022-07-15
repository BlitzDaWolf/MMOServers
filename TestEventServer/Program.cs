using DynamicServer;
using MainServer;
using TestEventServer;

IHost host = Host.CreateDefaultBuilder(args)
    /*.ConfigureServices(services =>
    {
        services.AddSingleton<Network>();
        services.AddSingleton<ReciveHandler>();
        services.AddSingleton<ServerHandler>();

        services.AddHostedService<CustomServer>();
    })*/
    .ConfigureServices(services =>
    {
        services.AddHostedService<WebServer>();
    })
    .Build();

await host.RunAsync();
