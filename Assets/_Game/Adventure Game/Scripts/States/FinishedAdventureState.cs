using System;
using FSM;
using SaveLoad;
using SceneLoader;

namespace Adventure_Game.Scripts.States
{
    public class FinishedAdventureState: FsmState
    {
        private readonly AdventureUI _adventureUI;
        private readonly Timer _timer;
        private readonly ISaveLoad _saveLoad;

        public FinishedAdventureState(FiniteStateMachine finiteStateMachine, AdventureUI adventureUI, Timer timer, ISaveLoad saveload) : base(finiteStateMachine)
        {
            _adventureUI = adventureUI;
            _timer = timer;
            _saveLoad = saveload;
        }

        public override void Enter()
        {
            base.Enter();
            _adventureUI.Enable(UI.Finished);
            _adventureUI.OnRestartPressed += OnOnRestartPressed;
            _adventureUI.OnExitPressed += OnOnExitPressed;

            if (_saveLoad.SaveData.isFirstPlay || _timer.GetTime()< _saveLoad.SaveData.bestTime)
            {
                _saveLoad.SaveData.bestTime = _timer.GetTime();
                _saveLoad.SaveData.isFirstPlay = false;
                _saveLoad.Save();
                _adventureUI.ShowRecord("New record!");
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(_saveLoad.SaveData.bestTime);
                _adventureUI.ShowRecord(string.Format($"Best time: {time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds:D3}"));
            }
            
        }

        private void OnOnExitPressed()
        {
            SceneLoaderService.LoadMainMenu();
        }

        private void OnOnRestartPressed()
        {
            FiniteStateMachine.ChangeStateTo<BeginAdventureState>();
        }
    }
}