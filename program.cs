using System;
using System.IO;
using HangmanGameMVC.Model;
using HangmanGameMVC.View;
using HangmanGameMVC.Controller;

namespace HangmanGameMVC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hangman Game!");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. User");
            Console.Write("Select mode (1 or 2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter new category: ");
                string category = Console.ReadLine()?.Trim();

                Console.Write("Enter new word: ");
                string word = Console.ReadLine()?.Trim().ToUpper(); 

                if (!string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(word))
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter("words.csv", append: true))
                        {
                            sw.WriteLine($"{category},{word}");
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n Word added successfully to words.csv!");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error writing to file: {ex.Message}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Category or word cannot be empty.");
                    Console.ResetColor();
                }
            }
            else if (choice == "2")
            {
                WordBank.LoadFromCSV("words.csv");
                var (word, category) = WordBank.GetRandomWord();

                var model = new HangmanGameModel(word);
                var view = new GameView();
                var controller = new GameController(model, view, category);

                controller.Run();
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
