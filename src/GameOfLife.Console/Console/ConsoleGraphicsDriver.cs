using System.Diagnostics;
using System.Text;

namespace GameOfLife.Console;

public static class ConsoleGraphicsDriver
{
    public static Func<IConsoleLayoutContentNode> Build(Game game)
    {
        // This should build a layout that can be called repeatedly to generate
        // a fresh view of the world instance in it's state at the time the layout is invoked

        const char aliveChar = 'X';
        const char deadChar = '-';

        var world = game.World;

        var worldContent = new ConsoleLayoutContent
        {
            Width = world.Width,
            Height = world.Height,
            OffsetX = 1,
            OffsetY = 1,
            ZIndex = 100,
            Content = () =>
            {
                var content = new StringBuilder();

                for (var y = 0; y < world.Height; y++)
                {
                    for (var x = 0; x < world.Width; x++)
                    {
                        var alive = world.IsAlive(x, y);
                        var symbol = alive ? aliveChar : deadChar;
                        content.Append(symbol);
                    }
                }

                return content.ToString();
            }
        };

        var stateInfoWidth = worldContent.Width;
        var stateInfoContent = new ConsoleLayoutContent
        {
            Width = stateInfoWidth,
            Height = 3,
            OffsetX = 1,
            OffsetY = worldContent.Height + 2,
            ZIndex = 101,
            Content = () =>
            {
                var info = $" Tick: {game.Tick}   Speed: {game.Speed}   Population: {game.World.Population} ";
                
                var blankLine = Enumerable.Repeat(' ', stateInfoWidth).ToList();
                var padding = Enumerable.Repeat(' ', stateInfoWidth - info.Length);
                return blankLine.Concat(info).Concat(padding).Concat(blankLine);
            }
        };

        var controlsInfoWidth = worldContent.Width;
        var controlsInfoContent = new ConsoleLayoutContent
        {
            Width = controlsInfoWidth,
            Height = 3,
            OffsetX = 1,
            OffsetY = worldContent.Height + 2 + stateInfoContent.Height,
            ZIndex = 101,
            Content = () =>
            {
                const string info = " [+] Faster | [-] Slower | [c] Clear | [g] Spawn Glider | [n] Spawn Noise | [q] Quit ";
                if (info.Length > controlsInfoWidth)
                {
                    return info.Take(controlsInfoWidth);
                }

                var blankLine = Enumerable.Repeat(' ', controlsInfoWidth).ToList();
                var padding = Enumerable.Repeat(' ', controlsInfoWidth - info.Length);
                return info.Concat(padding).Concat(blankLine);
            }
        };

        var layoutWidth = worldContent.Width + 2;
        var layoutHeight = worldContent.Height + 8;
        
        const char backgroundChar = '▓';
        var backgroundContent = new ConsoleLayoutContent
        {
            Width = layoutWidth,
            Height = layoutHeight,
            ZIndex = 0,
            Content = () => Enumerable.Repeat(backgroundChar, layoutWidth * layoutHeight)
        };

        var layout = new ConsoleLayoutContainer
        {
            Width = layoutWidth,
            Height = layoutHeight,
            Children = () => new IConsoleLayoutNode[]
            {
                backgroundContent,
                worldContent,
                stateInfoContent,
                controlsInfoContent
            }
        };

        return () => Collapse(layout);
    }

    public static void Draw(IConsoleLayoutContentNode content)
    {
        var x = 0;
        var y = 0;
        var line = new char[content.Width];
        
        System.Console.SetCursorPosition(content.OffsetX, content.OffsetY);
        
        foreach (var symbol in content.Content())
        {
            line[x] = symbol;
            x++;

            if (x < content.Width)
            {
                continue;
            }
            
            System.Console.Write(new string(line));
            x = 0;
            y++;

            if (y < content.Height)
            {
                var nextLeft = content.OffsetX;
                var nextTop = y + content.OffsetY;

                if (nextLeft < System.Console.WindowWidth && nextTop < System.Console.WindowHeight)
                {
                    System.Console.SetCursorPosition(nextLeft, nextTop);
                    continue;
                }
                
                Debug.WriteLine("Attempted to draw content outside of console bounds");
            }

            Debug.WriteLine("Attempted to draw content outside of declared bounds");
            break;
        }
    }

    private static IConsoleLayoutContentNode Collapse(IConsoleLayoutNode node)
    {
        return node switch
        {
            IConsoleLayoutContainerNode container => Collapse(container),
            IConsoleLayoutContentNode content => content,
            _ => throw new NotImplementedException()
        };
    }

    private static IConsoleLayoutContentNode Collapse(IConsoleLayoutContainerNode container)
    {
        const char blankChar = ' ';
        
        // Start everything with 2d array of blanks
        var cells = new char[container.Width, container.Height];
        for (var y = 0; y < container.Height; y++)
        {
            for (var x = 0; x < container.Width; x++)
            {
                cells[x, y] = blankChar;
            }
        }

        // Render the children into the 2d array in declared stacking order
        foreach (var child in container.Children().OrderBy(child => child.ZIndex))
        {
            var x = child.OffsetX;
            var y = child.OffsetY;

            foreach (var symbol in Collapse(child).Content())
            {
                cells[x, y] = symbol;

                // Move horizontally
                x++;

                if (x < child.OffsetX + child.Width)
                {
                    if (x < container.Width)
                    {
                        // Still within our horizontal bounds... keep going
                        continue;
                    }
                    
                    // We are about to overflow the output area... bail out
                    Debug.WriteLine("Attempted to draw content outside of container horizontal bounds");
                    break;
                }
                
                // Move vertically and reset horizontally
                x = child.OffsetX;
                y++;

                if (y < child.OffsetY + child.Height)
                {
                    if (y < container.Height)
                    {
                        // Still within our vertical bounds... keep going
                        continue;
                    }
                    
                    // We are about to overflow the output area... bail out
                    Debug.WriteLine("Attempted to draw content outside of container vertical bounds");
                    break;
                }
                
                // Something has gone wrong and there is too much content
                Debug.WriteLine("Encountered container content exceeding declared dimensions");
                break;
            }
        }

        // Flatten 2d array we've been building so that we can return it
        var content = new StringBuilder();
        for (var y = 0; y < container.Height; y++)
        {
            for (var x = 0; x < container.Width; x++)
            {
                content.Append(cells[x, y]);
            }
        }

        return new ConsoleLayoutContent
        {
            Width = container.Width,
            Height = container.Height,
            OffsetX = container.OffsetX,
            OffsetY = container.OffsetY,
            Content = content.ToString
        };
    }
}

