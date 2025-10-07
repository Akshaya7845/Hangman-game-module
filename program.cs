using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("\n Admin ");
                    Console.WriteLine("1. Add a category and word");
                    Console.WriteLine("2. View existing categories and words");
                    Console.WriteLine("3. Update a category or word");
                    Console.WriteLine("4. Exit");
                    Console.Write("Choose an option: ");
                    string adminChoice = Console.ReadLine();

                    if (adminChoice == "1")
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
                                Console.WriteLine("Word added successfully!");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"error writing to file: {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("category or word cannot be empty.");
                            Console.ResetColor();
                        }
                    }
                    else if (adminChoice == "2")
                    {
                        if (!File.Exists("words.csv"))
                        {
                            Console.WriteLine("CSV file not found.");
                        }
                        else
                        {
                            Console.WriteLine("\nexisting categories and words");
                            foreach (var line in File.ReadAllLines("words.csv").Skip(1))
                            {
                                Console.WriteLine(line);
                            }
                        }
                    }
                    else if (adminChoice == "3")
                    {
                        var lines = File.ReadAllLines("words.csv").ToList();
                        if (lines.Count <= 1)
                        {
                            Console.WriteLine("no data to update.");
                            continue;
                        }

                        Console.WriteLine("\nExisting Entries");
                        for (int i = 1; i < lines.Count; i++) 
                        {
                            Console.WriteLine($"{i}. {lines[i]}");
                        }

                        Console.Write("Enter entry number to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateIndex) && updateIndex > 0 && updateIndex < lines.Count)
                        {
                            Console.Write("Enter new category: ");
                            string newCategory = Console.ReadLine()?.Trim();

                            Console.Write("Enter new word: ");
                            string newWord = Console.ReadLine()?.Trim().ToUpper();

                            if (!string.IsNullOrWhiteSpace(newCategory) && !string.IsNullOrWhiteSpace(newWord))
                            {
                                lines[updateIndex] = $"{newCategory},{newWord}";
                                File.WriteAllLines("words.csv", lines);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("entry updated successfully!");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine("category or word cannot be empty.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("invalid selection.");
                        }
                    }
                    else if (adminChoice == "4")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("invalid option.");
                    }

                    Console.WriteLine("\nPress any key to return to admin menu...");
                    Console.ReadKey();
                }
            }
            else if (choice == "2")
            {
                WordBank.LoadFromCSV("words.csv");

                while (true)
                {
                    var (word, category) = WordBank.GetRandomWord();

                    var model = new HangmanGameModel(word);
                    var view = new GameView();
                    var controller = new GameController(model, view, category);

                    controller.Run();

                    Console.Write("\nDo you want to play again? (yes/no): ");
                    string again = Console.ReadLine()?.Trim().ToLower();
                    if (again != "yes")
                    {
                        break;
                    }
                }
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

