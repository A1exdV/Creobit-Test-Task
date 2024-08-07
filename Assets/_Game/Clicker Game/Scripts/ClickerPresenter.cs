namespace Clicker_Game.Scripts
{
    public class ClickerPresenter
    {
        private readonly ClickerModel _model;
        private readonly ClickerView _view;

        public ClickerPresenter(ClickerModel model, ClickerView view)
        {
            _model = model;
            _view = view;

            _view.OnClickButton += HandleClickButton;
            UpdateView();
        }

        private void HandleClickButton()
        {
            _model.IncrementScore();
            UpdateView();
        }

        private void UpdateView()
        {
            _view.UpdateScore(_model.Score);
        }
    }
}