using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace TileServer.MbTiles
{
    public class MbTileRepository :  ITileRepository, IDisposable
    {
        private SQLiteConnectionPool connectionPool;
        private const string SQLTileCommand = "SELECT tile_data FROM tiles WHERE zoom_level=? AND tile_column=? AND tile_row = ?";

        protected string filePath;

        public MbTileRepository(string filePath)
        {            
            connectionPool = new SQLite.SQLiteConnectionPool();
            this.filePath = filePath;
        }

        private SQLiteConnectionWithLock GetConnection()
        {
            var cs = new SQLiteConnectionString(filePath, false);            
            return connectionPool.GetConnection(cs, SQLite.SQLiteOpenFlags.ReadOnly);
        }

        public byte[] GetTile(TileIndex index)
        {            
            var c = GetConnection();
            return c.ExecuteScalar<byte[]>(SQLTileCommand, index.z, index.x, index.yFlipped);
        }
        
        public async Task<byte[]> GetTileAsync(TileIndex index)
        {            
            var c = new SQLiteAsyncConnection(filePath, SQLiteOpenFlags.ReadOnly, false, connectionPool);
            return await c.ExecuteScalarAsync<byte[]>(SQLTileCommand, index.z, index.x, index.yFlipped);                                
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);   
        }
        protected bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear all property values that maybe have been set
                    // when the class was instantiated                    
                    
                }
                connectionPool.Reset();

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}