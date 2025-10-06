using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangmanGameMVC.Model
{
	public enum GameStatus
	{
		Ongoing,
		PlayerWon,
		PlayerLost
	}

	public static class WordBank
	{
		private static Dictionary<string, List<string>> Categories = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		private static Random random = new Random();

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

	public class HangmanGameModel
	{
		public string WordToGuess { get; }
		public List<char> CorrectGuesses { get; } = new();
		public List<char> AllGuesses { get; } = new();
		public int WrongStreak { get; private set; } = 0;
		public GameStatus Status { get; private set; } = GameStatus.Ongoing;

		public HangmanGameModel(string wordToGuess)
		{
			WordToGuess = wordToGuess.ToUpper();
		}

		public void Guess(char character)
		{
			character = char.ToUpper(character);

			if (AllGuesses.Contains(character) || Status != GameStatus.Ongoing)
				return;

			AllGuesses.Add(character);

			if (WordToGuess.Contains(character))
			{
				CorrectGuesses.Add(character);
				WrongStreak = 0;
			}
			else
			{
				WrongStreak++;
			}

			CheckGameStatus();
		}

		private void CheckGameStatus()
		{
			if (WrongStreak >= 3)
			{
				Status = GameStatus.PlayerLost;
			}
			else if (WordToGuess.All(letter => CorrectGuesses.Contains(letter)))
			{
				Status = GameStatus.PlayerWon;
			}
		}

		public int AttemptsLeft => 3 - WrongStreak;
	}
}
