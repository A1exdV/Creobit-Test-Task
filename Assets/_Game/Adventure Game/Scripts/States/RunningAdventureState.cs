using Adventure_Game.ThirdPersonController.Scripts;
using FSM;
using UnityEngine;

namespace Adventure_Game.Scripts.States
{
    public class RunningAdventureState : FsmState
    {
        private readonly CustomCharacterController _customCharacterController;
        private readonly AdventureUI _adventureUI;
        private readonly Timer _timer;

        public RunningAdventureState(FiniteStateMachine finiteStateMachine, CustomCharacterController customCharacterController,
            AdventureUI adventureUI, Timer timer) : base(finiteStateMachine)
        {
            _customCharacterController = customCharacterController;
            _adventureUI = adventureUI;
            _timer = timer;
        }

        public override void Enter()
        {
            base.Enter();
            _customCharacterController.EnableControl(true);
            _timer.StartTimer();
            _customCharacterController.OnFinishTriggerEnter += PlayerReachFinish;
            _adventureUI.Enable(UI.Running);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void PlayerReachFinish()
        {
            FiniteStateMachine.ChangeStateTo<FinishedAdventureState>();
        }

        public override void Exit()
        {
            base.Exit();
            _customCharacterController.EnableControl(false);
            _timer.StopTimer();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}