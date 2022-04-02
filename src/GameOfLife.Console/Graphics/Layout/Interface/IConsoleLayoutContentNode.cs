namespace GameOfLife.Console.Graphics.Layout.Interface;

public interface IConsoleLayoutContentNode : IConsoleLayoutNode
{
    Func<IEnumerable<char>> Content { get; }
}