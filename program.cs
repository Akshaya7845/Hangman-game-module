using System;
using HangmanGameMVC.Model;
using HangmanGameMVC.View;
using HangmanGameMVC.Controller;

namespace HangmanGameMVC
{
    class Program
    {
        static void Main(string[] args)
        {
            WordBank.LoadFromCSV("words.csv");
            var (word, category) = WordBank.GetRandomWord();

            var model = new HangmanGameModel(word);
            var view = new GameView();
            var controller = new GameController(model, view, category);

            controller.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
