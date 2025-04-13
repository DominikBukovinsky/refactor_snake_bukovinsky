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
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            Random randomNumber = new Random();
            int score = 5;
            int gameOver = 0;
            
            Pixel head = new Pixel();
            head.XPos = screenWidth / 2;
            head.YPos = screenHeight / 2;
            head.Color = ConsoleColor.Red;
            
            string movement = "RIGHT";
            List<int> bodyXPositions = new List<int>();
            List<int> bodyYPositions = new List<int>();
            
            int berryX = randomNumber.Next(0, screenWidth);
            int berryY = randomNumber.Next(0, screenHeight);
            
            DateTime startTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            string buttonPressed = "no";
            
            while (true)
            {
                Console.Clear();
                if (head.XPos == screenWidth - 1 || head.XPos == 0 || 
                    head.YPos == screenHeight - 1 || head.YPos == 0)
                {
                    gameOver = 1;
                }

                // Draw border
                for (int i = 0; i < screenWidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                    Console.SetCursorPosition(i, screenHeight - 1);
                    Console.Write("■");
                }
                for (int i = 0; i < screenHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                    Console.SetCursorPosition(screenWidth - 1, i);
                    Console.Write("■");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                if (berryX == head.XPos && berryY == head.YPos)
                {
                    score++;
                    berryX = randomNumber.Next(1, screenWidth - 2);
                    berryY = randomNumber.Next(1, screenHeight - 2);
                }

                // Draw snake body
                for (int i = 0; i < bodyXPositions.Count(); i++)
                {
                    Console.SetCursorPosition(bodyXPositions[i], bodyYPositions[i]);
                    Console.Write("■");
                    if (bodyXPositions[i] == head.XPos && bodyYPositions[i] == head.YPos)
                    {
                        gameOver = 1;
                    }
                }

                if (gameOver == 1)
                {
                    break;
                }

                // Draw head
                Console.SetCursorPosition(head.XPos, head.YPos);
                Console.ForegroundColor = head.Color;
                Console.Write("■");

                // Draw food
                Console.SetCursorPosition(berryX, berryY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");

                startTime = DateTime.Now;
                buttonPressed = "no";
                while (true)
                {
                    currentTime = DateTime.Now;
                    if (currentTime.Subtract(startTime).TotalMilliseconds > 500) { break; }
                    
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key.Equals(ConsoleKey.UpArrow) && movement != "DOWN" && buttonPressed == "no")
                        {
                            movement = "UP";
                            buttonPressed = "yes";
                        }
                        if (key.Key.Equals(ConsoleKey.DownArrow) && movement != "UP" && buttonPressed == "no")
                        {
                            movement = "DOWN";
                            buttonPressed = "yes";
                        }
                        if (key.Key.Equals(ConsoleKey.LeftArrow) && movement != "RIGHT" && buttonPressed == "no")
                        {
                            movement = "LEFT";
                            buttonPressed = "yes";
                        }
                        if (key.Key.Equals(ConsoleKey.RightArrow) && movement != "LEFT" && buttonPressed == "no")
                        {
                            movement = "RIGHT";
                            buttonPressed = "yes";
                        }
                    }
                }

                bodyXPositions.Add(head.XPos);
                bodyYPositions.Add(head.YPos);

                switch (movement)
                {
                    case "UP": head.YPos--; break;
                    case "DOWN": head.YPos++; break;
                    case "LEFT": head.XPos--; break;
                    case "RIGHT": head.XPos++; break;
                }

                if (bodyXPositions.Count() > score)
                {
                    bodyXPositions.RemoveAt(0);
                    bodyYPositions.RemoveAt(0);
                }
            }

            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
        }
    }

    class Pixel
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public ConsoleColor Color { get; set; }
    }
}