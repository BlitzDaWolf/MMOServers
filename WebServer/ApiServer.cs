using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleHttp;

namespace WebServer
{
    public class ApiServer : BackgroundService
    {
        public ILogger<ApiServer> Logger { get; }

        public ApiServer(ILogger<ApiServer> logger)
        {
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Route.Add("/", (req, res, props) =>
            {
                res.AsText("[0,8,6,875.847]", "application/json");
            });
            /*Route.Add("/", (req, res, props) =>
            {

            }, "POST");*/

            await HttpServer.ListenAsync(7580, stoppingToken, Route.OnHttpRequestAsync);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1);
            }
        }
    }
}
