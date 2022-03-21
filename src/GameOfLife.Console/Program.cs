using System.Collections.Concurrent;
using GameOfLife.Console;

namespace GameOfLife
{
    public static class Program
    {
        private static bool _doExit;
        private static readonly ConcurrentQueue<ConsoleKeyInfo> KeyBuffer = new();
        private static readonly Game Game = new(BuildWorld());

        private static readonly Func<IConsoleLayoutContentNode> GenerateDisplayContent =
            ConsoleGraphicsDriver.Build(Game);

        public static void Main()
        {
            System.Console.Clear();
            RunGame();
        }

        private static void RunGame()
        {
            BeginBufferingInput();

            Game.SpawnNoise();
            Draw();
            HoldFrame();
            FlushInput();

            while (!_doExit)
            {
                Game.NextTick();
                Draw();
                HoldFrame();
                FlushInput();
            }
        }

        private static World BuildWorld() => new(100, 25);

        private static void BeginBufferingInput()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_doExit)
                {
                    KeyBuffer.Enqueue(System.Console.ReadKey(true));
                }
            });
        }

        private static void FlushInput()
        {
            while (KeyBuffer.TryDequeue(out var keyInfo))
            {
                switch (keyInfo.KeyChar)
                {
                    case 'q':
                        _doExit = true;
                        return;
                    case 'g':
                        Game.SpawnGlider();
                        break;
                    case 'n':
                        Game.SpawnNoise();
                        break;
                    case 'c':
                        Game.KillEverything();
                        break;
                    case '+':
                        Game.Faster();
                        break;
                    case '-':
                        Game.Slower();
                        break;
                }
            }
        }

        private static void Draw()
        {
            var content = GenerateDisplayContent();
            ConsoleGraphicsDriver.Draw(content);
        }

        private static void HoldFrame()
        {
            if (Game.Speed > 5)
            {
                // Max speed
                return;
            }

            var speedDelayMillisecondsMap = new[] {1000, 1000, 500, 250, 100, 10};

            // Slow down to let people watch
            Thread.Sleep(speedDelayMillisecondsMap[Game.Speed]);
        }
    }
}