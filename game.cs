using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangmanGame
{
    public delegate void GameStatusDelegate();

    public enum GameStatus
    {
        Ongoing,
        PlayerWon,
        PlayerLost
    }

    // abstract base class 
    public abstract class GameBase
    {
        public GameStatus GameStatus { get; protected set; } = GameStatus.Ongoing;
        public event GameStatusDelegate GameEnded;

        public abstract void Guess(char character);
        public abstract void DisplayUI();

        protected void OnGameEnded()
        {
            GameEnded?.Invoke();
        }
    }

    public static class WordBank
    {
        private static Dictionary<string, List<string>> Categories = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        private static Random random = new Random();

        // load words from csv file
        public static void LoadFromCSV(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: CSV file not found at {filePath}");
                Environment.Exit(1);
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1)) 
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');

                if (parts.Length != 2) continue;

                var category = parts[0].Trim();
                var word = parts[1].Trim().ToUpper();

                if (!Categories.ContainsKey(category))
                    Categories[category] = new List<string>();

                Categories[category].Add(word);
            }

            if (Categories.Count == 0)
            {
                Console.WriteLine("No words loaded from the CSV file.");
                Environment.Exit(1);
            }
        }

        // return a random word or category:
        public static (string word, string category) GetRandomWord()
        {
            if (Categories.Count == 0)
            {
                Console.WriteLine("Word list is empty. Did you forget to call LoadFromCSV?");
                Environment.Exit(1);
            }

            var category = Categories.Keys.ElementAt(random.Next(Categories.Count));
            var words = Categories[category];
            var word = words[random.Next(words.Count)];
            return (word, category);
        }
    }

    public class HangmanGame : GameBase
    {
        private readonly string _wordToGuess;
        private readonly List<char> _correctGuesses = new();
        private readonly List<char> _allGuesses = new();
        private int _wrongStreak;

        public HangmanGame(string wordToGuess)
        {
            _wordToGuess = wordToGuess.ToUpper();
        }

        public override void Guess(char character)
        {
            character = char.ToUpper(character);

            if (_allGuesses.Contains(character) || GameStatus != GameStatus.Ongoing)
                return;

            _allGuesses.Add(character);

            if (_wordToGuess.Contains(character))
            {
                _correctGuesses.Add(character);
                _wrongStreak = 0; // reset wrong streak on correct guess
            }
            else
            {
                _wrongStreak++;
            }

            CheckGameStatus();
        }

        private void CheckGameStatus()
        {
            if (_wrongStreak >= 3)
            {
                GameStatus = GameStatus.PlayerLost;
                OnGameEnded();
                return;
            }

            if (_wordToGuess.All(letter => _correctGuesses.Contains(letter)))
            {
                GameStatus = GameStatus.PlayerWon;
                OnGameEnded();
            }
        }

        public int AttemptsLeft => 3 - _wrongStreak;

        public override void DisplayUI()
        {
            Console.WriteLine($"Attempts Left: {AttemptsLeft} / 3\n");
            DrawHangman(_wrongStreak);
            Console.WriteLine("\nWord: " + DisplayWord(_wordToGuess, _correctGuesses));
            Console.WriteLine("\nGuessed Letters: " + string.Join(", ", _allGuesses.OrderBy(c => c)));
        }

        private string DisplayWord(string word, List<char> correctGuesses)
        {
            return string.Join(" ", word.Select(c => correctGuesses.Contains(c) ? c : '_'));
        }

        private void DrawHangman(int wrongStreak)
        {
            if (wrongStreak == 0)
                return;

            string[] hangman = new string[8];

            // Scaffold base
            hangman[0] = "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _";
            hangman[1] = "           |";
            hangman[2] = "           |";
            hangman[3] = "           |";
            hangman[4] = "           |";
            hangman[5] = "           |";
            hangman[6] = "           |";
            hangman[7] = "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _";

            if (wrongStreak == 2)
            {
                hangman[1] = "           |            |";
                hangman[2] = "           |            |";
                hangman[3] = "           |            O";
            }

            if (wrongStreak >= 3)
            {
                hangman[1] = "           |            |";
                hangman[2] = "           |            |";
                hangman[3] = "           |            O";
                hangman[4] = "           |           /|\\";
                hangman[5] = "           |           / \\";
            }

            foreach (var line in hangman)
                Console.WriteLine(line);
        }
    }

    class Program
    {
        static GameBase game;
        static string category;

        static void Main()
        {
            WordBank.LoadFromCSV("words.csv");
            var (word, selectedCategory) = WordBank.GetRandomWord();
            category = selectedCategory;

            game = new HangmanGame(word);
            game.GameEnded += OnGameEnded;

            while (game.GameStatus == GameStatus.Ongoing)
            {
                Console.Clear();
                Console.WriteLine("HANGMAN GAME \n");
                Console.WriteLine($"Category: {category}");
                game.DisplayUI();

                Console.Write("\nEnter a letter: ");
                char guess = Console.ReadKey().KeyChar;
                game.Guess(guess);
            }

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        static void OnGameEnded()
        {
            Console.Clear();
            Console.WriteLine("HANGMAN GAME\n");
            Console.WriteLine($"Category: {category}");
            game.DisplayUI();

            if (game.GameStatus == GameStatus.PlayerWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nCongratulations! You guessed the word!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (game is HangmanGame hangmanGame)
                {
                    Console.WriteLine("\nSorry, you lost. The word was: " + GetWordFromGame(hangmanGame));
                }
                else
                {
                    Console.WriteLine("\nSorry, you lost.");
                }
            }
            Console.ResetColor();
        }
        private static string GetWordFromGame(HangmanGame game)
        {
            var field = typeof(HangmanGame).GetField("_wordToGuess", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(game)?.ToString() ?? "";
        }
    }
}
