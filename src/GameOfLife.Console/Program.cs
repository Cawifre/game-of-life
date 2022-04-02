using System.Collections.Concurrent;
using System.Diagnostics;
using GameOfLife.Console.Graphics;
using GameOfLife.Console.Graphics.Layout.Interface;
using GameOfLife.Console.Simulation;

namespace GameOfLife.Console;

public static class Program
{
    private static bool _doExit;
    private static readonly ConcurrentQueue<ConsoleKeyInfo> KeyBuffer = new();
    private static readonly Game Game = new(BuildWorld());

    private static readonly Func<IConsoleLayoutContentNode> GenerateDisplayContent =
        ConsoleGraphicsDriver.Build(Game);

    private static int MsPerTick => Game.Speed switch
    {
        0 => int.MaxValue,
        1 => 1_000,
        2 => 500,
        3 => 100,
        4 => 10,
        5 => 1,
        _ => throw new NotImplementedException()
    };

    public static void Main()
    {
        System.Console.Clear();
        RunGame();
    }

    private static void RunGame()
    {
        BeginBufferingInput();

        var stopwatch = Stopwatch.StartNew();

        var clockPrevious = DateTime.Now;
        var clockLag = TimeSpan.Zero;
        while (!_doExit)
        {
            var clockCurrent = DateTime.Now;
            var clockElapsed = clockCurrent - clockPrevious;
            clockPrevious = clockCurrent;
            clockLag += clockElapsed;

            FlushInput();

            if (Game.Speed > 0)
            {
                stopwatch.Restart();
                var ticksElapsed = 0;

                var tickLength = TimeSpan.FromMilliseconds(MsPerTick);
                while (clockLag >= tickLength)
                {
                    Game.NextTick();
                    clockLag -= tickLength;
                    ticksElapsed++;
                }

                Debug.WriteLine($"Performed {ticksElapsed} ticks in {stopwatch.ElapsedMilliseconds} ms");
            }
            else
            {
                clockLag = TimeSpan.Zero;
                Debug.WriteLine($"Game paused for {stopwatch.ElapsedMilliseconds} ms");
            }

            Render();
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

    private static void Render()
    {
        var content = GenerateDisplayContent();
        ConsoleGraphicsDriver.Draw(content);
    }
}