using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main game loop
            GameState gameState = new GameState();
            
            while (true)
            {
                Console.Clear();
                
                if (gameState.IsGameOver())
                {
                    break;
                }

                gameState.DrawBorder();
                gameState.UpdateFood();
                gameState.DrawSnake();
                gameState.DrawFood();
                
                gameState.HandleInput();
                gameState.MoveSnake();
            }

            gameState.ShowGameOverScreen();
        }
    }

    public enum Direction //A list of all possible directions 
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public class GameState
    {   
        //Screen config
        private const int ScreenWidth = 32;
        private const int ScreenHeight = 16;
        
        private readonly Random _random = new Random();
        private Direction _currentDirection = Direction.Right;
        private Direction _nextDirection = Direction.Right;
        
        public Snake Snake { get; }
        public Food Food { get; }
        public int Score { get; private set; } = 5;

        public GameState()
        {
            Console.WindowHeight = ScreenHeight;
            Console.WindowWidth = ScreenWidth;
            
            //Creates a new snake in the starting position
            Snake = new Snake(
                headX: ScreenWidth / 2,
                headY: ScreenHeight / 2,
                initialBodyLength: 5
            );

            Food = new Food(
                random: _random,
                maxX: ScreenWidth - 2,
                maxY: ScreenHeight - 2
            );
        }

        //Checks if the head if colliding with the body of the snaek
        public bool IsGameOver()
        {
            var head = Snake.Head;
            return head.XPos == 0 || head.XPos == ScreenWidth - 1 ||
                   head.YPos == 0 || head.YPos == ScreenHeight - 1 ||
                   Snake.IsHeadCollidingWithBody();
        }

        //Rendering of borders on console
        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.White;
            
            for (int x = 0; x < ScreenWidth; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("■");
                Console.SetCursorPosition(x, ScreenHeight - 1);
                Console.Write("■");
            }

            for (int y = 0; y < ScreenHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("■");
                Console.SetCursorPosition(ScreenWidth - 1, y);
                Console.Write("■");
            }
        }

        public void UpdateFood()
        {
            if (Snake.Head.XPos == Food.XPos && Snake.Head.YPos == Food.YPos)
            {
                Score++;
                Food.Regenerate();
                Snake.Grow();
            }
        }

        //Logic of spawning the head and body of the snake
        public void DrawSnake()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var body in Snake.BodySegments)
            {
                Console.SetCursorPosition(body.XPos, body.YPos);
                Console.Write("■");
            }

            Console.SetCursorPosition(Snake.Head.XPos, Snake.Head.YPos);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("■");
        }

        public void DrawFood()
        {
            Console.SetCursorPosition(Food.XPos, Food.YPos);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");
        }

        public void HandleInput()
        {
            DateTime start = DateTime.Now;
            bool inputProcessed = false;
            
            while ((DateTime.Now - start).TotalMilliseconds < 500)
            {
                if (!Console.KeyAvailable) continue;
                
                var key = Console.ReadKey(true).Key;
                var newDirection = GetDirectionFromKey(key);

                if (IsValidDirectionChange(newDirection))
                {
                    _nextDirection = newDirection;
                    inputProcessed = true;
                    break;
                }
            }

            if (!inputProcessed)
            {
                _nextDirection = _currentDirection;
            }
        }

        public void MoveSnake()
        {
            _currentDirection = _nextDirection;
            Snake.Move(_currentDirection);
        }

        public void ShowGameOverScreen()
        {
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2);
            Console.WriteLine($"Game over, Score: {Score}");
        }

        private Direction GetDirectionFromKey(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.UpArrow => Direction.Up,
                ConsoleKey.DownArrow => Direction.Down,
                ConsoleKey.LeftArrow => Direction.Left,
                ConsoleKey.RightArrow => Direction.Right,
                _ => Direction.None
            };
        }

        private bool IsValidDirectionChange(Direction newDirection)
        {
            return newDirection != Direction.None && 
                   newDirection != GetOppositeDirection(_currentDirection);
        }

        private Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.None
            };
        }
    }

    //Class that handles the movement, collision and updating of the snake's body
    public class Snake
    {
        public Pixel Head { get; }
        public List<Pixel> BodySegments { get; } = new List<Pixel>();

        public Snake(int headX, int headY, int initialBodyLength)
        {
            Head = new Pixel(headX, headY, ConsoleColor.Red);
            
            for (int i = 1; i <= initialBodyLength; i++)
            {
                BodySegments.Add(new Pixel(headX - i, headY, ConsoleColor.Green));
            }
        }

        public void Move(Direction direction)
        {
            UpdateBody();
            UpdateHead(direction);
        }

        public void Grow()
        {
            var lastSegment = BodySegments.LastOrDefault();
            BodySegments.Add(new Pixel(
                lastSegment?.XPos ?? Head.XPos,
                lastSegment?.YPos ?? Head.YPos,
                ConsoleColor.Green
            ));
        }

        public bool IsHeadCollidingWithBody()
        {
            return BodySegments.Any(segment => 
                segment.XPos == Head.XPos && 
                segment.YPos == Head.YPos
            );
        }

        private void UpdateHead(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: Head.YPos--; break;
                case Direction.Down: Head.YPos++; break;
                case Direction.Left: Head.XPos--; break;
                case Direction.Right: Head.XPos++; break;
            }
        }

        private void UpdateBody()
        {
            if (!BodySegments.Any()) return;

            BodySegments.Insert(0, new Pixel(Head.XPos, Head.YPos, ConsoleColor.Green));
            BodySegments.RemoveAt(BodySegments.Count - 1);
        }
    }

    //Class that handles the logic of food (tiles that the snake eats)
    public class Food
    {
        private readonly Random _random;
        private readonly int _maxX;
        private readonly int _maxY;

        public int XPos { get; private set; }
        public int YPos { get; private set; }

        public Food(Random random, int maxX, int maxY)
        {
            _random = random;
            _maxX = maxX;
            _maxY = maxY;
            Regenerate();
        }

        //Randomly generates new food
        public void Regenerate()
        {
            XPos = _random.Next(1, _maxX);
            YPos = _random.Next(1, _maxY);
        }
    }

    public class Pixel
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public ConsoleColor Color { get; }

        public Pixel(int xPos, int yPos, ConsoleColor color)
        {
            XPos = xPos;
            YPos = yPos;
            Color = color;
        }
    }
}