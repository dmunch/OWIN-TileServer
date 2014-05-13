using System.Text.RegularExpressions;

namespace TileServer
{
    public class TileIndex
    {
        public int x { get; set; }
        public int y { get; set; }

        /// <summary>
        /// Calculates the flipped Y coordinate
        /// </summary>
        public int yFlipped
        {
            get
            {
                return FlipY(y, z);
            }
        }

        public byte z { get; set; }

        protected static Regex tileRegex;
        static TileIndex()
        {
            tileRegex = new Regex("/([0-9]+)/([0-9]+)/([0-9]+).png$");
        }

        protected TileIndex()
        {

        }

        public TileIndex(int x, int y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static TileIndex ParseRequestPath(string requestPath)
        {
            var match = tileRegex.Match(requestPath);
            var tile = new TileIndex();

            tile.z = byte.Parse(match.Groups[1].Value);
            tile.x = int.Parse(match.Groups[2].Value);
            tile.y = int.Parse(match.Groups[3].Value);

            return tile;
        }

        public override string ToString()
        {
            return string.Format("/{0}/{1}/{2}.png", z, x, y);
        }
        
        public static TileIndex WithFlippedY(int x, int y, byte z)
        {
            var tileIndex = new TileIndex();
            tileIndex.x = x;
            tileIndex.y = FlipY(y, z);
            tileIndex.z = z;

            return tileIndex;
        }

        public static int FlipY(int y, byte z)
        {
            return IntPow(z, 2) - 1 - y;
        }
        
        public static int IntPow(int x, byte pow)
        {
            var ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
    }
}
