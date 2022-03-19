using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace RectangleGame
{
    /// <summary>
    /// simple console game
    /// </summary>
    static class Game
    {
        static int WindowSizeX;
        static int WindowSizeY;

        static Rectangle rectMessageWindow;//rectangle represents message window
        static List<string> Messages;//strings of message window
        static Rectangle rectGameField;//rectangle represents game field
        //static List<Rectangle> player1Rectangles;//list of player1's rectangles
        //static List<Rectangle> player2Rectangles;//list of player2's rectangles
        static List<Rectangle> freeRectangles;//list of neutral rectangles of max square that may intersect
        /// <summary>
        /// move of player <param name="numPlayer"></param>
        /// </summary>
        static void MakeMove(ref Player _player)
        {
            int dice1 = 0, dice2 = 0;
            if (_player.MovesCount > 0)
            {
                bool canMakeMove = false;
                for (int i = 0; i < 2; i++)//2 tries to make move
                {
                    GetRandomNumbers(out dice1, out dice2);
                    WriteMessages(rectMessageWindow, Messages, $"Player {_player.Name}'s move. Dice: hight {dice1} and width {dice2}");
                    foreach (var rect in freeRectangles)
                    {
                        if ((rect.Width >= dice2) && (rect.Height >= dice1))
                        {
                            canMakeMove = true;
                            break;
                        }
                    }
                    if (canMakeMove)
                    {
                        break;
                    }
                    else
                    {
                        WriteMessages(rectMessageWindow, Messages, "Cannot draw such rectangle in free area.");
                    }
                }

                if (canMakeMove)
                {
                    WriteMessages(rectMessageWindow, Messages, "Use arrows to select left-top of a rectangle then press {Enter}");
                    Console.CursorVisible = true;
                    bool moveDone = false;

                    Console.CursorLeft = rectGameField.LeftTopX + (rectGameField.Width / 2);
                    Console.CursorTop = rectGameField.LeftTopY + (rectGameField.Height / 2);

                    while (!moveDone)
                    {
                        if (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo pressedKey = Console.ReadKey(true);

                            switch (pressedKey.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (Console.CursorTop > rectGameField.LeftTopY)
                                    {
                                        Console.CursorTop--;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    if (Console.CursorTop < rectGameField.LeftTopY + rectGameField.Height - 1)
                                    {
                                        Console.CursorTop++;
                                    }
                                    break;
                                case ConsoleKey.LeftArrow:
                                    if (Console.CursorLeft > rectGameField.LeftTopX)
                                    {
                                        Console.CursorLeft--;
                                    }
                                    break;
                                case ConsoleKey.RightArrow:
                                    if (Console.CursorLeft < rectGameField.LeftTopX + rectGameField.Width - 1)
                                    {
                                        Console.CursorLeft++;
                                    }
                                    break;
                                case ConsoleKey.Enter:
                                    var rectPlayersMove = new Rectangle(Console.CursorLeft, Console.CursorTop, dice2, dice1);
                                    foreach (var rectFree in freeRectangles)
                                    {
                                        if (Rectangle.Intersection(rectPlayersMove, rectFree) == rectPlayersMove)
                                        {
                                            DrawRectangle(rectPlayersMove, _player.Ch, true);
                                            _player.RectanglesList.Add(rectPlayersMove);
                                            _player.Score += rectPlayersMove.Square;
                                            moveDone = true;
                                            break;
                                        }
                                    }

                                    if (moveDone)
                                    {
                                        var maxIndex = freeRectangles.Count - 1;
                                        int index = 0;
                                        while (index <= maxIndex)
                                        {
                                            var newList = freeRectangles[index].SliceWith(rectPlayersMove);
                                            if (newList.Count > 0)
                                            {
                                                foreach (var item in newList)
                                                {
                                                    freeRectangles.Add(item);
                                                }
                                                freeRectangles.RemoveAt(index);
                                                maxIndex--;
                                            }
                                            else
                                            {
                                                index++;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    Console.CursorVisible = false;
                }
                else
                {
                    WriteMessages(rectMessageWindow, Messages, "Player passed");
                }

                _player.MovesCount--;
            }
        }
        /// <summary>
        /// returns 2 random numbers from 1 to 6
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        static void GetRandomNumbers(out int x, out int y)
        {
            var rand = new Random();
            x = rand.Next(1, 6);
            y = rand.Next(1, 6);
        }
        /// <summary>
        /// updates and rewrites message window <param name="rectMessageWindow"></param>
        /// with <param name="listMessages"></param> and adds <param name="strMessage"></param>
        /// to the bottom of it
        /// </summary>
        static void WriteMessages(Rectangle rectMessageWindow, List<string> listMessages, string strMessage)
        {
            listMessages.Add(strMessage);

            if ((listMessages.Count) > rectMessageWindow.Height - 3)// 2 lines for border, 1 line for input
            {
                listMessages.RemoveAt(0);
            }

            char[] chars;
            char ch;

            for (int line = 0; line < rectMessageWindow.Height - 2; line++)// 2 lines for border
            {
                if (line < listMessages.Count)
                {
                    chars = listMessages[line].ToCharArray();
                }
                else
                {
                    chars = null;
                }

                for (int column = 1; column < rectMessageWindow.Width - 1; column++)
                {
                    if (chars == null)
                    {
                        ch = ' ';
                    }
                    else if (column <= chars.Length)
                    {
                        ch = chars[column - 1];
                    }
                    else
                    {
                        ch = ' ';
                    }

                    Console.SetCursorPosition(rectMessageWindow.LeftTopX + column, rectMessageWindow.LeftTopY + line + 1);
                    Console.Write(ch);
                }
            }
        }
        static string ReadLineFromMessageWindow(Rectangle rectMessageWindow, List<string> listMessages, string strMessage)
        {
            WriteMessages(rectMessageWindow, listMessages, strMessage);
            Console.SetCursorPosition(rectMessageWindow.LeftTopX + 1, rectMessageWindow.LeftTopY + rectMessageWindow.Height - 2);
            Console.CursorVisible = true;
            string userInput = Console.ReadLine();
            Console.CursorVisible = false;
            WriteMessages(rectMessageWindow, listMessages, $"{userInput} entered");
            return userInput;
        }
        /// <summary>
        /// Draws a rectangle <param name="rectangle"></param>
        /// with given symbol <param name="Ch"></param>
        /// and fills it if <param name="fill"> is true
        /// </summary>
        static void DrawRectangle(Rectangle rectangle, char Ch, Boolean fill)
        {
            int absoluteX = rectangle.LeftTopX;
            if ((absoluteX < 0) || (absoluteX > WindowSizeX))
            {
                return;
            }

            int absoluteY = rectangle.LeftTopY;
            if ((absoluteY < 0) || (absoluteY > WindowSizeY))
            {
                return;
            }

            for (int line = 0; line < rectangle.Height; line++)
            {
                if (absoluteY + line > WindowSizeY)
                {
                    break;
                }

                if ((line == 0) || (line == rectangle.Height - 1) || fill)
                {
                    for (int column = 0; column < rectangle.Width; column++)
                    {
                        if (absoluteX + column > WindowSizeX)
                        {
                            break;
                        }

                        Console.SetCursorPosition(absoluteX + column, absoluteY + line);
                        Console.Write(Ch);
                    }
                }
                else
                {
                    if (absoluteY + line <= WindowSizeY)
                    {
                        Console.SetCursorPosition(absoluteX, absoluteY + line);
                        Console.Write(Ch);
                    }

                    if (absoluteX + rectangle.Width - 1 <= WindowSizeX)
                    {
                        Console.SetCursorPosition(absoluteX + rectangle.Width - 1, absoluteY + line);
                        Console.Write(Ch);
                    }
                }
            }
        }

        static void Main()
        {
#if DEBUG
            WindowSizeX = 80;
            WindowSizeY = 20;
#else
                WindowSizeX = Console.LargestWindowWidth - 20;
                WindowSizeY = Console.LargestWindowHeight - 10;
#endif

            //Console.WriteLine("LargestWindowWidth = " + Console.LargestWindowWidth);240
            //Console.WriteLine("LargestWindowHeight = " + Console.LargestWindowHeight);73

            //while (Console.ReadKey().Key != ConsoleKey.Enter) {}

            try
            {
                Console.SetWindowSize(WindowSizeX, WindowSizeY);
                Console.SetBufferSize(WindowSizeX, WindowSizeY);
                Console.CursorVisible = false;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                Console.Write("Press <Enter> to exit... ");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                return;
            }
            finally
            {
            }

            Console.Clear();

            var player1 = new Player
            {
                Name = "Player1",
                Ch = '1',
                RectanglesList = new List<Rectangle>()
            };

            var player2 = new Player()
            {
                Name = "Player2",// TODO "Computer"
                Ch = '2',
                RectanglesList = new List<Rectangle>()
            };

            Messages = new List<string>();

            rectMessageWindow = new Rectangle(0, WindowSizeY - 8 - 1, WindowSizeX, 8);
#if DEBUG
#else
            if (((WindowSizeX - 2) < 30) || (20 > WindowSizeY - rectMessageWindow.Height - 2))//2 symbols for border, minimal game field 20x20 
            {
                Console.WriteLine("Your display is not good enough. Play some other game.");
                return;
            }
#endif
            DrawRectangle(rectMessageWindow, '#', false);

            int numberMoves, height, width;
#if DEBUG
            numberMoves = 2;
            height = WindowSizeY - rectMessageWindow.Height - 3;
            width = WindowSizeX - 2;
#else
            int maxValue = 100, minValue = 1;
            string strUserInput;
            while (true)
            {
                strUserInput = ReadLineFromMessageWindow(rectMessageWindow, Messages, $"Enter number of moves for each player (max {maxValue})");
                bool resultParsed = int.TryParse(strUserInput, NumberStyles.None, null as IFormatProvider, out numberMoves);
                if (resultParsed)
                {
                    if ((numberMoves >= minValue) && (numberMoves <= maxValue))
                    {
                        break;
                    }
                }
            }

            (minValue, maxValue) = (20, WindowSizeY - rectMessageWindow.Height - 2 - 1);//2 symbols for border
            while (true)
            {
                strUserInput = ReadLineFromMessageWindow(rectMessageWindow, Messages, $"Enter height of game field (min {minValue} max {maxValue})");
                bool resultParsed = int.TryParse(strUserInput, NumberStyles.None, null as IFormatProvider, out height);
                if (resultParsed)
                {
                    if ((height >= minValue) && (height <= maxValue))
                    {
                        break;
                    }
                }
            }

            (minValue, maxValue) = (30, WindowSizeX - 2);//2 symbols for border
            while (true)
            {
                strUserInput = ReadLineFromMessageWindow(rectMessageWindow, Messages, $"Enter width of game field (min {minValue} max {maxValue})");
                bool resultParsed = int.TryParse(strUserInput, NumberStyles.None, null as IFormatProvider, out width);
                if (resultParsed)
                {
                    if ((width >= minValue) && (width <= maxValue))
                    {
                        break;
                    }
                }
            }
#endif
            player1.MovesCount = numberMoves;
            player2.MovesCount = numberMoves;

            var rectGameFieldBorder = new Rectangle(0, 0, width + 2, height + 2);
            DrawRectangle(rectGameFieldBorder, '*', false);

            rectGameField = new Rectangle(1, 1, width, height);

            //game start

            freeRectangles = new List<Rectangle>();

            freeRectangles.Add(new Rectangle(rectGameField.LeftTopX, rectGameField.LeftTopY, rectGameField.Width, rectGameField.Height));

            while ((player1.MovesCount > 0) || (player2.MovesCount > 0))
            {
                MakeMove(ref player1);
                MakeMove(ref player2);
            }

            if (player1.Score > player2.Score)
            {
                WriteMessages(rectMessageWindow, Messages, $"{player1.Name} wins with score {player1.Score}");
                WriteMessages(rectMessageWindow, Messages, $"{player2.Name} looses with score {player2.Score}");
            }
            else if (player2.Score > player1.Score)
            {
                WriteMessages(rectMessageWindow, Messages, $"{player2.Name} wins with score {player2.Score}");
                WriteMessages(rectMessageWindow, Messages, $"{player1.Name} looses with score {player1.Score}");
            }
            else
            {
                WriteMessages(rectMessageWindow, Messages, $"Nobody wins score {player1.Score} = {player2.Score}");
            }

            WriteMessages(rectMessageWindow, Messages, "Press {Esc} to exit...");

            ConsoleKeyInfo pressedKey;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    pressedKey = Console.ReadKey(true);

                    if (pressedKey.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// represents player of the game
    /// </summary>
    struct Player
    {
        public string Name;
        public int MovesCount;
        public char Ch;// player's rectangles marked with this symbol
        public List<Rectangle> RectanglesList;
        public int Score;
    }
}