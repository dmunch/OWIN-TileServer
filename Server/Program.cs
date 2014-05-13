using Microsoft.Owin.Hosting;
using System;
using System.Threading;

namespace Server
{
    public class Program
    {
        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            /*
            var path = @"X:\VirtualMachines\Vagrant\ParisToner.mbtiles";
            var rep = new TileServer.MbTiles.MbTileRepository(path);
            var d = rep.GetTile(new TileServer.TileIndex() { z = 15, x = 16601, y = 11275 });

            return;
            */

            //On Mono in Docker, when binding to localhost we can't reach server from the outside
            //So we pass the binding url as parameter, making it http://*:5000 on Mono
            //On Windows, we need to run: netsh http add urlacl url=http://localhost:5000/ user=AZ101\daniel
            //or: netsh http add urlacl=http://+:5000/ user=AZ101\daniel for * (yet to test)
            var startUrl = string.Format("http://192.168.1.17:{0}", 5000);
            if (args.Length > 0)
            {
                startUrl = args[0];
            }

            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            var options = new StartOptions(startUrl)
            {
                ServerFactory = "Microsoft.Owin.Host.HttpListener",
            };
            
            using (var app = WebApp.Start<Startup>(options))
            {
                Console.WriteLine("Started");
                _quitEvent.WaitOne();
            }
        }
    }
}