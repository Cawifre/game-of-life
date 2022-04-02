namespace GameOfLife.Console.Graphics.Layout.Interface;

public interface IConsoleLayoutContainerNode : IConsoleLayoutNode
{
    Func<IEnumerable<IConsoleLayoutNode>> Children { get; }
}