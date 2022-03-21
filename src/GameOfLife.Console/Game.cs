namespace GameOfLife;

public sealed class Game
{
    private const int SpeedMin = 0;
    private const int SpeedStart = 1;
    private const int SpeedMax = 6;

    private readonly Random _rng = new();

    private int _speed = SpeedStart;

    public int Speed
    {
        get => _speed;
        private set
        {
            _speed = value switch
            {
                < SpeedMin => SpeedMin,
                > SpeedMax => SpeedMax,
                _ => value
            };
        }
    }
    
    public int Tick { get; private set; }

    public World World { get; }

    public Game(World world)
    {
        World = world;
    }

    public void NextTick()
    {
        if (Speed == SpeedMin)
        {
            return;
        }

        World.Tick();
        Tick++;
    }

    public void Faster() => Speed++;

    public void Slower() => Speed--;

    public void SpawnNoise()
    {
        for (var x = 0; x < World.Width; x++)
        {
            for (var y = 0; y < World.Height; y++)
            {
                if (_rng.Next(2) > 0)
                {
                    World.MakeAlive(x, y);
                }
                else
                {
                    World.MakeDead(x, y);
                }
            }
        }
    }

    public void SpawnGlider()
    {
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
            {
                World.MakeDead(x, y);
            }
        }

        World.MakeAlive(1, 0);
        World.MakeAlive(2, 1);
        World.MakeAlive(0, 2);
        World.MakeAlive(1, 2);
        World.MakeAlive(2, 2);
    }

    public void KillEverything()
    {
        for (var x = 0; x < World.Width; x++)
        {
            for (var y = 0; y < World.Height; y++)
            {
                World.MakeDead(x, y);
            }
        }
    }
}