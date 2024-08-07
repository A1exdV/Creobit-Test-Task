using Adventure_Game.ThirdPersonController.Scripts;
using FSM;
using SceneLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Adventure_Game.Scripts.States
{
    public class BeginAdventureState : FsmState
    {
        private readonly CustomCharacterController _customCharacterController;
        private readonly AdventureUI _adventureUI;
        private readonly PlayerInputSystem _input;
        private readonly Transform _startPosition;

        public BeginAdventureState(FiniteStateMachine finiteStateMachine, CustomCharacterController customCharacterController,
            AdventureUI adventureUI, PlayerInputSystem input, Transform startPosition) : base(finiteStateMachine)
        {
            _customCharacterController = customCharacterController;
            _adventureUI = adventureUI;
            _input = input;
            _startPosition = startPosition;
        }

        public override void Enter()
        {
            base.Enter();
            _adventureUI.Enable(UI.Await);
            _adventureUI.OnExitPressed += OnOnExitPressed;
            _customCharacterController.transform.position = _startPosition.position;
            _input.Player.Move.started += OnPlayerStartMoving;
        }
        private void OnOnExitPressed()
        {
            SceneLoaderService.LoadMainMenu();
        }
        private void OnPlayerStartMoving(InputAction.CallbackContext callbackContext)
        {
            FiniteStateMachine.ChangeStateTo<RunningAdventureState>();
        }

        public override void Exit()
        {
            base.Exit();
            _adventureUI.OnExitPressed -= OnOnExitPressed;
            _input.Player.Move.started -= OnPlayerStartMoving;
        }
    }
}