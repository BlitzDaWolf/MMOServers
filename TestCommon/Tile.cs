namespace TestCommon
{
    public enum TileType
    {
        Normal
    }

    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }

        public TileType Type { get; set; } = TileType.Normal;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
