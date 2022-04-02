namespace GameOfLife.Console;

public interface IConsoleLayoutContentNode : IConsoleLayoutNode
{
    Func<IEnumerable<char>> Content { get; }
}