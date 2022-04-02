namespace GameOfLife.Console;

public class ConsoleLayoutContainer : IConsoleLayoutContainerNode
{
    public string Name { get; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int OffsetX { get; init; }
    public int OffsetY { get; init; }
    public int ZIndex { get; init; }

    public Func<IEnumerable<IConsoleLayoutNode>> Children { get; init; } = Array.Empty<IConsoleLayoutNode>;

    public ConsoleLayoutContainer(string name)
    {
        Name = name;
    }
}