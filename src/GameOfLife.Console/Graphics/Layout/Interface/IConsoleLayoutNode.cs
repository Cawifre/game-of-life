namespace GameOfLife.Console.Graphics.Layout.Interface;

public interface IConsoleLayoutNode
{
    string Name { get; }
    int Width { get; }
    int Height { get; }
    int OffsetX { get; }
    int OffsetY { get; }
    int ZIndex { get; }
}