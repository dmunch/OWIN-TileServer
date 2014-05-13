using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileServer
{
    public interface ITileRepository
    {
        byte[] GetTile(TileIndex index);
        Task<byte[]> GetTileAsync(TileIndex index);
    }
}
