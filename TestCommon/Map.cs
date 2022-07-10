namespace TestCommon
{
    public class Map
    {
        public Tile[,] Tiles { get; set; }
        public readonly int Width;
        public readonly int Height;

        public Map(int w, int h)
        {
            Width = w;
            Height = h;
            Tiles = new Tile[Width, Height];
        }

        private void InitiliazeTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new Tile(x, y);
                }
            }
        }
    }
}
