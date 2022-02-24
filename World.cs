namespace GameOfLife
{
    public sealed class World
    {
        public int Width { get; }
        public int Height { get; }
        private bool[] _cells;

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new bool[Width * Height];
        }

        public void Tick()
        {
            var nextGeneration = new bool[_cells.Length];

            for (var i = 0; i < _cells.Length; i++)
            {
                var y = i / Width;
                var x = i - y * Width;

                var alive = IsAlive(x, y);
                var localPopulation = LiveNeighborCount(x, y);

                bool aliveNext;
                if (alive)
                {
                    if (localPopulation < 2)
                    {
                        // Any live cell with fewer than two live neighbors dies, as if caused by under-population.
                        aliveNext = false;
                    }
                    else if (localPopulation > 3)
                    {
                        // Any live cell with more than three live neighbors dies, as if by over-population.
                        aliveNext = false;
                    }
                    else
                    {
                        // Any live cell with two or three live neighbors lives on to the next generation.
                        aliveNext = true;
                    }
                }
                else
                {
                    if (localPopulation == 3)
                    {
                        // Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
                        aliveNext = true;
                    }
                    else
                    {
                        // Dead things usually stay dead
                        aliveNext = false;
                    }
                }

                nextGeneration[i] = aliveNext;
            }

            _cells = nextGeneration;
        }

        public void MakeAlive(int x, int y)
        {
            if (!TryNormalizeCoords(x, y, out var normalized))
            {
                return;
            }
            _cells[normalized] = true;
        }

        public void MakeDead(int x, int y)
        {
            if (!TryNormalizeCoords(x, y, out var normalized))
            {
                return;
            }
            _cells[normalized] = false;
        }

        public bool IsAlive(int x, int y) => TryNormalizeCoords(x, y, out var normalized) && _cells[normalized];

        private bool TryNormalizeCoords(int x, int y, out int index)
        {
            if ( x < 0 || x >= Width || y < 0 || y >= Height)
            {
                index = -1;
                return false;
            }
            index = x + Width * y;
            return true;
        }

        private int LiveNeighborCount(int x, int y) => Neighbors(x, y).Count(isAlive => isAlive);

        private IEnumerable<bool> Neighbors(int x, int y)
        {
            if (!TryNormalizeCoords(x, y, out var normalized))
            {
                yield break;
            }

            yield return IsAlive(x - 1, y - 1);
            yield return IsAlive(x - 1, y);
            yield return IsAlive(x - 1, y + 1);

            yield return IsAlive(x, y - 1);
            yield return IsAlive(x, y + 1);

            yield return IsAlive(x + 1, y - 1);
            yield return IsAlive(x + 1, y);
            yield return IsAlive(x + 1, y + 1);
        }
    }
}