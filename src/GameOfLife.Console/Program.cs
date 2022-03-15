using System.Collections.Concurrent;
using System.Text;

namespace GameOfLife
{
    public static class Program
    {
        private static int _tick;
        private static bool _doExit;
        private static int _speed = 1; 
        private static readonly ConcurrentQueue<ConsoleKeyInfo> KeyBuffer = new();
        private static readonly List<int> PopulationHistory = new();
        private static readonly World World = BuildWorld();

        public static void Main()
        {
            Console.Clear();
            RunGame();
        }

        private static void RunGame()
        {
            BufferInput();

            while (!_doExit)
            {
                Bookkeeping();
                DrawFrame(BuildFrame());
                Tick();
                HoldFrame();
                FlushInput();
            }
        }

        private static World BuildWorld()
        {
            var world = new World(100, 25);
            SpawnNoise(world);
            return world;
        }

        private static void BufferInput()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_doExit)
                {
                    KeyBuffer.Enqueue(Console.ReadKey(true));
                }
            });
        }

        private static void FlushInput()
        {
            while(KeyBuffer.TryDequeue(out var keyInfo))
            {
                switch(keyInfo.KeyChar)
                {
                    case 'q':
                        _doExit = true;
                        return;
                    case 'g':
                        SpawnGlider(World);
                        break;
                    case 'n':
                        SpawnNoise(World);
                        break;
                    case 'c':
                        KillEverything(World);
                        break;
                    case '+':
                        GoFaster();
                        break;
                    case '-':
                        GoSlower();
                        break;
                }
            }
        }

        private static void Bookkeeping()
        {
            if (_speed <= 0)
            {
                return;
            }

            PopulationHistory.Add(World.Population);
        }

        private static string BuildFrame()
        {
            var frame = new StringBuilder();

            for (var y = 0; y < World.Height; y++)
            {
                var line = new char[World.Width];
                for (var x = 0; x < World.Width; x++)
                {
                    line[x] = World.IsAlive(x, y) ? 'X' : '-';
                }
                frame.AppendLine(new string(line));
            }

            var rollingData = new
            {
                avg3 = (int)PopulationHistory.TakeLast(3).Average(),
                avg5 = (int)PopulationHistory.TakeLast(5).Average(),
                avg10 = (int)PopulationHistory.TakeLast(10).Average(),
                avg100 = (int)PopulationHistory.TakeLast(100).Average(),
                avg500 = (int)PopulationHistory.TakeLast(500).Average(),
                avg1000 = (int)PopulationHistory.TakeLast(1000).Average()
            };

            string GetSpeedString(int speed) => speed > 5 ? "MAX" : Convert.ToString(speed);

            const int valuesAreaWidth = 30;
            frame.AppendLine();
            frame.AppendLine($"tick:             {_tick,valuesAreaWidth}");
            frame.AppendLine($"speed:            {GetSpeedString(_speed),valuesAreaWidth}");
            frame.AppendLine();
            frame.AppendLine($"population:       {World.Population,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-3:    {rollingData.avg3,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-5:    {rollingData.avg5,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-10:   {rollingData.avg10,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-100:  {rollingData.avg100,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-500:  {rollingData.avg500,valuesAreaWidth}");
            frame.AppendLine($"rolling avg-1000: {rollingData.avg1000,valuesAreaWidth}");

            return frame.ToString();
        }

        private static void DrawFrame(string frame)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(ClampFrameDimensions(frame, Console.WindowWidth, Console.WindowHeight - 1));
        }

        private static string ClampFrameDimensions(string frame, int maxWidth, int maxHeight)
        {
            var clamped = new StringBuilder();

            var lines = 0;
            foreach (var line in frame.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                if (lines >= maxHeight)
                {
                    break;
                }
                clamped.AppendLine(line.Length > maxWidth ? line[..maxWidth] : line);
                lines++;
            }

            return clamped.ToString();
        }

        private static void Tick()
        {
            if (_speed <= 0)
            {
                return;
            }

            World.Tick();
            _tick++;
        }

        private static void HoldFrame()
        {
            if (_speed > 5)
            {
                // Max speed
                return;
            }

            var speedDelayMillisecondsMap = new [] { 1000, 1000, 500, 250, 100, 10 };

            // Slow down to let people watch
            Thread.Sleep(speedDelayMillisecondsMap[_speed]);
        }

        private static void GoSlower() => _speed = ClampSpeed(_speed - 1);

        private static void GoFaster() => _speed = ClampSpeed(_speed + 1);

        private static int ClampSpeed(int speed)
        {
            return speed switch
            {
                < 0 => 0,
                > 5 => 6,
                _ => speed
            };
        }

        private static void SpawnNoise(World world)
        {
            var rng = new Random();
            for (var x = 0; x < world.Width; x++)
            {
                for (var y = 0; y < world.Height; y++)
                {
                    if (rng.Next(2) > 0)
                    {
                        world.MakeAlive(x, y);
                    }
                    else
                    {
                        world.MakeDead(x, y);
                    }
                }
            }
        }

        private static void SpawnGlider(World world)
        {
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    world.MakeDead(x, y);
                }
            }
            world.MakeAlive(1, 0);
            world.MakeAlive(2, 1);
            world.MakeAlive(0, 2);
            world.MakeAlive(1, 2);
            world.MakeAlive(2, 2);
        }

        private static void KillEverything(World world)
        {
            for (var x = 0; x < world.Width; x++)
            {
                for (var y = 0; y < world.Height; y++)
                {
                    world.MakeDead(x, y);
                }
            }
        }
    }
}
