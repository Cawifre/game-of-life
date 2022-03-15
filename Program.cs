using System.Collections.Concurrent;
using System.Text;

namespace GameOfLife
{
    public sealed class Program
    {
        private static int _tick = 0;
        private static bool _doExit = false;
        private static int _speed = 1; 
        private static ConcurrentQueue<ConsoleKeyInfo> _keyBuffer = new ConcurrentQueue<ConsoleKeyInfo>();
        private static readonly List<int> _populationHistory = new List<int>();
        private static readonly World _world = BuildWorld();

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
                while (true)
                {
                    _keyBuffer.Enqueue(Console.ReadKey(true));
                }
            });
        }

        private static void FlushInput()
        {
            while(_keyBuffer.TryDequeue(out var keyInfo))
            {
                switch(keyInfo.KeyChar)
                {
                    case 'q':
                        _doExit = true;
                        return;
                    case 'g':
                        SpawnGlider(_world);
                        break;
                    case 'n':
                        SpawnNoise(_world);
                        break;
                    case 'c':
                        KillEverything(_world);
                        break;
                    case '+':
                        GoFaster();
                        break;
                    case '-':
                        GoSlower();
                        break;
                    default:
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

            _populationHistory.Add(_world.Population);
        }

        private static string BuildFrame()
        {
            var frame = new StringBuilder();

            for (var y = 0; y < _world.Height; y++)
            {
                var line = new char[_world.Width];
                for (var x = 0; x < _world.Width; x++)
                {
                    line[x] = _world.IsAlive(x, y) ? 'X' : '-';
                }
                frame.AppendLine(new string(line));
            }

            string AlignRight(object content) => string.Format("{0,30}", content);
            string GetSpeedString(int speed) => speed > 5 ? "MAX" : Convert.ToString(speed);

            frame.AppendLine();
            frame.AppendLine($"tick:             {AlignRight(_tick)}");
            frame.AppendLine($"speed:            {AlignRight(GetSpeedString(_speed))}");
            frame.AppendLine();
            frame.AppendLine($"population:       {AlignRight(_world.Population)}");
            frame.AppendLine($"rolling avg-3:    {AlignRight((int)_populationHistory.TakeLast(3).Average())}");
            frame.AppendLine($"rolling avg-5:    {AlignRight((int)_populationHistory.TakeLast(5).Average())}");
            frame.AppendLine($"rolling avg-10:   {AlignRight((int)_populationHistory.TakeLast(10).Average())}");
            frame.AppendLine($"rolling avg-100:  {AlignRight((int)_populationHistory.TakeLast(100).Average())}");
            frame.AppendLine($"rolling avg-500:  {AlignRight((int)_populationHistory.TakeLast(500).Average())}");
            frame.AppendLine($"rolling avg-1000: {AlignRight((int)_populationHistory.TakeLast(1000).Average())}");

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
                clamped.AppendLine(line.Length > maxWidth ? line.Substring(0, maxWidth) : line);
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

            _world.Tick();
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
