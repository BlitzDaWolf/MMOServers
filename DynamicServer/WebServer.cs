using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace DynamicServer
{
    public class WebServer : BackgroundService
    {
        public WebServer()
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);

            TcpListener listener = new TcpListener(IPAddress.Any, 1302);
            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                TcpClient client = listener.AcceptTcpClient();
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());

                try
                {
                    string request = sr.ReadLine();
                    Console.WriteLine(request);
                    string[] tokens = request.Split(' ');

                    string page = tokens[1];
                    if (page == "/")
                    {

                    }

                    sw.WriteLine("HTTP/1.0 200 OK\n");
                    sw.WriteLine("{\"Test\": 5}");
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                client.Close();
            }
        }
    }
}
