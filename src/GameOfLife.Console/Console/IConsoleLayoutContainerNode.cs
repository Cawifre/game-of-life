namespace GameOfLife.Console;

public interface IConsoleLayoutContainerNode : IConsoleLayoutNode
{
    Func<IEnumerable<IConsoleLayoutNode>> Children { get; }
}