using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TileServer
{    
    public class TileServerMiddleware
    {
        protected Func<IDictionary<string, object>, Task> next;
        protected ITileRepository repository;

        public TileServerMiddleware(ITileRepository repository)
        {
            this.repository = repository;
        }
        
        public TileServerMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            Initialize(next);
        }

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {                       
            var responseHeader = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
            var responseStream = (Stream)environment["owin.ResponseBody"];

            responseHeader.Add("Content-Type", new string[]{"image/png"});
            environment["owin.ResponseStatusCode"] = 200;

            try
            {
                var tileIndex = TileIndex.ParseRequestPath(environment["owin.RequestPath"] as string);           
                var pngByteArray = await repository.GetTileAsync(tileIndex);

                using (Stream stream = new MemoryStream(pngByteArray))
                {
                    await stream.CopyToAsync(responseStream);
                }
            } 
            catch(Exception ex)
            {
                environment["owin.ResponseStatusCode"] = 404;
            }

            await next.Invoke(environment);            
        }        
    }
}
