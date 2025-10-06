using System;
using System.Collections.Generic;
using System.Linq;
using HangmanGameMVC.Model;

namespace HangmanGameMVC.View
{
    public class GameView
    {
        public void DisplayGame(HangmanGameModel model, string category)
        {
            Console.Clear();
            Console.WriteLine(" HANGMAN GAME\n");
            Console.WriteLine($"Category: {category}");
            Console.WriteLine($"\nAttempts Left: {model.AttemptsLeft} / 3\n");

            DrawHangman(model.WrongStreak);
            Console.WriteLine("\nWord: " + DisplayWord(model.WordToGuess, model.CorrectGuesses));
            Console.WriteLine("\nGuessed Letters: " + string.Join(", ", model.AllGuesses.OrderBy(c => c)));
        }

        public void DisplayEnd(HangmanGameModel model, string category)
        {
            Console.Clear();
            Console.WriteLine("HANGMAN GAME \n");
            Console.WriteLine($"Category: {category}");
            DisplayGame(model, category);

            if (model.Status == GameStatus.PlayerWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nCongratulations! You guessed the word!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nSorry, you lost. The word was: " + model.WordToGuess);
            }
            Console.ResetColor();
        }

        public char GetGuessFromUser()
        {
            Console.Write("\nEnter a letter: ");
            return Console.ReadKey().KeyChar;
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
}
