﻿using System.Text;

namespace GameOfLife
{
    public sealed class Program
    {
        private static int _tick = 0;
        private static readonly List<int> _populationHistory = new List<int>();
        private static readonly World _world = BuildWorld();

        public static void Main()
        {
            Console.Clear();
            RunGame();
        }

        private static void RunGame()
        {
            while (true)
            {
                Bookkeeping();
                DrawFrame(BuildFrame());
                Tick();

                // Slow down to let people watch
                Thread.Sleep(10);
            }
        }

        private static World BuildWorld()
        {
            var world = new World(100, 25);

            // Build a glider
            // world.MakeAlive(1, 0);
            // world.MakeAlive(2, 1);
            // world.MakeAlive(0, 2);
            // world.MakeAlive(1, 2);
            // world.MakeAlive(2, 2);

            // Build random life
            var rng = new Random();
            for (var x = 0; x < world.Width; x++)
            {
                for (var y = 0; y < world.Height; y++)
                {
                    if (rng.Next(2) > 0)
                    {
                        world.MakeAlive(x, y);
                    }
                }
            }

            return world;
        }

        private static void Bookkeeping()
        {
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

            frame.AppendLine();
            frame.AppendLine($"tick:             {AlignRight(_tick)}");
            frame.AppendLine();
            frame.AppendLine($"population:       {AlignRight(_populationHistory[_tick])}");
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
            Console.Write(frame);
        }

        private static void Tick()
        {
            _world.Tick();
            _tick++;
        }
    }
}
