using System;
using HangmanGameMVC.Model;
using HangmanGameMVC.View;

namespace HangmanGameMVC.Controller
{
    public class GameController
    {
        private readonly HangmanGameModel _model;
        private readonly GameView _view;
        private readonly string _category;

        public GameController(HangmanGameModel model, GameView view, string category)
        {
            _model = model;
            _view = view;
            _category = category;
        }

        public void Run()
        {
            while (_model.Status == GameStatus.Ongoing)
            {
                _view.DisplayGame(_model, _category);
                char guess = _view.GetGuessFromUser();
                _model.Guess(guess);
            }

            _view.DisplayEnd(_model, _category);
        }
    }
}
