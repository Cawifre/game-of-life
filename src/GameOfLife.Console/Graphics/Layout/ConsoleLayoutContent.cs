using GameOfLife.Console.Graphics.Layout.Interface;

namespace GameOfLife.Console.Graphics.Layout;

public class ConsoleLayoutContent : IConsoleLayoutContentNode
{
    public string Name { get; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int OffsetX { get; init; }
    public int OffsetY { get; init; }
    public int ZIndex { get; init; }

    public Func<IEnumerable<char>> Content { get; init; } = Array.Empty<char>;

    public ConsoleLayoutContent(string name)
    {
        Name = name;
    }
}