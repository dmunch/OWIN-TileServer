using NUnit.Framework;
using SQLite;
using System;
using System.Threading.Tasks;

namespace TileServer.MbTiles.Test
{
    [TestFixture]
    public class MbTilesRepositoryTest
    {
        string sqliteTmpFilePath;

        public static string CreateAndPopulateTestDatabase()
        {
            string sqliteTmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".sqlite";
            using (var conn = new SQLiteConnection(sqliteTmpFilePath))
            {
                conn.Execute("CREATE TABLE tiles (zoom_level INTEGER, tile_column INTEGER, tile_row INTEGER, tile_data BLOB)");
                string insertCmd = "INSERT INTO tiles (tile_column, tile_row, zoom_level, tile_data) VALUES(?, ?, ?, ?)";

                conn.Execute(insertCmd, 1, 2, 3, new byte[] { 1, 2, 3 });
                conn.Execute(insertCmd, 3125, 24522, 51, new byte[] { 4, 5, 6 });
                conn.Execute(insertCmd, 12, 2155, 125, new byte[] { 7, 8, 9 });
            }

            return sqliteTmpFilePath;
        }

        [TestFixtureSetUp]
        public void CreateTestDatabase()
        {
            sqliteTmpFilePath = CreateAndPopulateTestDatabase();            
        }

        /// <summary>
        /// Deletes temporary database. 
        /// If there's a problem it probably means that not all SQLite instances where disposed properly
        /// </summary>
        [TestFixtureTearDown]
        public void DeleteDatabase()
        {
            System.IO.File.Delete(sqliteTmpFilePath);
        }

        [Test]
        public void Should_Retreive_Correct_tile_data_for_given_indizes()
        {
            using (var rep = new TileServer.MbTiles.MbTileRepository(sqliteTmpFilePath))
            {
                Assert.AreEqual(rep.GetTile(TileServer.TileIndex.WithFlippedY(1, 2, 3)), new byte[] { 1, 2, 3 });
                Assert.AreEqual(rep.GetTile(TileServer.TileIndex.WithFlippedY(3125, 24522, 51)), new byte[] { 4, 5, 6 });
                Assert.AreEqual(rep.GetTile(TileServer.TileIndex.WithFlippedY(12, 2155, 125)), new byte[] { 7, 8, 9 });
            }
        }
        
        [Test]
        public async Task Should_Retreive_Correct_tile_data_for_given_indizes_async_1()
        {
            using(var rep = new TileServer.MbTiles.MbTileRepository(sqliteTmpFilePath))
            {
                Assert.AreEqual(await rep.GetTileAsync(TileServer.TileIndex.WithFlippedY(1, 2, 3)), new byte[] { 1, 2, 3 });               
                Assert.AreEqual(await rep.GetTileAsync(TileServer.TileIndex.WithFlippedY(3125, 24522, 51)), new byte[] { 4, 5, 6 });
                Assert.AreEqual(await rep.GetTileAsync(TileServer.TileIndex.WithFlippedY(12, 2155, 125)), new byte[] { 7, 8, 9 });
            }
        }
    }
}
