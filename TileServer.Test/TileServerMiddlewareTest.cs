using Microsoft.Owin.Testing;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TileServer.Test
{
    public class TileRepositoryMock : ITileRepository
    {
        public byte[] GetTile(TileIndex index)
        {
            byte[] d = new byte[index.z];

            for(int i = 0; i < index.z; i++)
            {
                d[i] = (byte) (i + index.x);
            }
            
            return d;
        }

        public Task<byte[]> GetTileAsync(TileIndex index)
        {
            return Task.Factory.StartNew(() =>
            {
                return GetTile(index);
            });
        }
    }

    [TestFixture]
    public class TileServerMiddlewareTest
    {
        [Test]
        public async Task Should_respond_correct_content_type()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Use(new TileServer.TileServerMiddleware(new TileRepositoryMock()));

            }))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/1/2/3.png");
                
                Assert.AreEqual(response.Content.Headers.GetValues("Content-Type").Count(), 1);
                Assert.AreEqual(response.Content.Headers.GetValues("Content-Type").First(), "image/png");
            }
        }

        [Test]
        public async Task Should_respond_correct_content_for_given_indizes()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Use(new TileServer.TileServerMiddleware(new TileRepositoryMock()));

            }))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/1/2/3.png");
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                Assert.AreEqual(content, new byte[] { 2 });

                response = await server.HttpClient.GetAsync("/2/2/3.png");
                content = await response.Content.ReadAsByteArrayAsync();
                Assert.AreEqual(content, new byte[] { 2, 3 });

                response = await server.HttpClient.GetAsync("/2/3/3.png");
                content = await response.Content.ReadAsByteArrayAsync();
                Assert.AreEqual(content, new byte[] { 3, 4 });
            }
        }
    }
}
