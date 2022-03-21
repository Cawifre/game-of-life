namespace GameOfLife.Console;

public interface IConsoleLayoutNode
{
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
    public int Width { get; init; }
    public int Height { get; init; }
    public int OffsetX { get; init; }
    public int OffsetY { get; init; }
    public int ZIndex { get; init; }

    public Func<IEnumerable<IConsoleLayoutNode>> Children { get; init; } = Array.Empty<IConsoleLayoutNode>;
}

public class ConsoleLayoutContent : IConsoleLayoutContentNode
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int OffsetX { get; init; }
    public int OffsetY { get; init; }
    public int ZIndex { get; init; }
    
    public Func<IEnumerable<char>> Content { get; init; } = Array.Empty<char>;
}