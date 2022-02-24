using System.Text;

namespace GameOfLife
{
    public sealed class Program
    {
        public static void Main()
        {
            RunGame();
        }

        private static void RunGame()
        {
            var world = new World(25, 25);

            // Build a glider
            world.MakeAlive(1, 0);
            world.MakeAlive(2, 1);
            world.MakeAlive(0, 2);
            world.MakeAlive(1, 2);
            world.MakeAlive(2, 2);

            do
            {
                var frame = new StringBuilder();
                for(var y = 0; y < world.Height; y++)
                {
                    var line = new char[world.Width];
                    for(var x = 0; x < world.Width; x++)
                    {
                        line[x] = world.IsAlive(x, y) ? 'X' : '.';
                    }
                    frame.AppendLine(new string(line));
                }

                Console.Clear();
                Console.Write(frame);

                Thread.Sleep(100);

                world.Tick();
            }
            while (true);
        }
    }
}