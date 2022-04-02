namespace GameOfLife.Console;

public interface IConsoleLayoutNode
{
    string Name { get; }
    int Width { get; }
    int Height { get; }
    int OffsetX { get; }
    int OffsetY { get; }
    int ZIndex { get; }
}

public interface IConsoleLayoutContainerNode : IConsoleLayoutNode
{
    Func<IEnumerable<IConsoleLayoutNode>> Children { get; }
}

public interface IConsoleLayoutContentNode : IConsoleLayoutNode
{
    Func<IEnumerable<char>> Content { get; }
}

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