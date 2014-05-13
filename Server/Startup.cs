using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var tileRepository = new TileServer.MbTiles.MbTileRepository(@"X:\VirtualMachines\Vagrant\ParisToner.mbtiles");
            builder.Use(new TileServer.TileServerMiddleware(tileRepository));
        }
    }
}
