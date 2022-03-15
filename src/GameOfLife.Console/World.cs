using System.Collections;

namespace GameOfLife
{
    public sealed class World
    {
        public int Width { get; }
        public int Height { get; }

        public int Population
        {
            get
            {
                var val = 0;
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (IsAlive(x, y))
                        {
                            val++;
                        }
                    }
                }
                return val;
            }
        }

        private BitArray _cells;

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new BitArray(Width * Height);
        }

        public void Tick()
        {
            var nextGeneration = new BitArray(_cells.Length);

            for (var i = 0; i < _cells.Length; i++)
            {
                var y = i / Width;
                var x = i - y * Width;

                var alive = IsAlive(x, y);
                var localPopulation = LiveNeighborCount(x, y);

                bool aliveNext;
                if (alive)
                {
                    switch (localPopulation)
                    {
                        case < 2:
                            // Any live cell with more than three live neighbors dies, as if by over-population.
                        case > 3:
                            // Any live cell with fewer than two live neighbors dies, as if caused by under-population.
                            aliveNext = false;
                            break;
                        default:
                            // Any live cell with two or three live neighbors lives on to the next generation.
                            aliveNext = true;
                            break;
                    }
                }
                else
                {
                    // Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
                    aliveNext = localPopulation == 3;
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
            if (x < 0 || x >= Width || y < 0 || y >= Height)
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
            if (!TryNormalizeCoords(x, y, out _))
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
