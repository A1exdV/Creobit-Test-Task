using Adventure_Game.Scripts.States;
using Adventure_Game.ThirdPersonController.Scripts;
using FSM;
using Reflex.Attributes;
using SaveLoad;
using UnityEngine;

namespace Adventure_Game.Scripts
{
    public class Adventure : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private CustomCharacterController playerPrefab;
        [SerializeField] private AdventureUI adventureUI;

        [Inject] private PlayerInputSystem _input;
        [Inject] private Timer _timer;
        [Inject] private ISaveLoad _saveLoad;
        
        private CustomCharacterController _characterController;
        private FiniteStateMachine _finiteStateMachine;
        private void Awake()
        {
            SetupGame();
        }

        private void SetupGame()
        {
            _characterController = Instantiate(playerPrefab);
            _input.Enable();
            SetupFsm();
            
            _finiteStateMachine.ChangeStateTo<BeginAdventureState>();
        }
        

        private void SetupFsm()
        {
            _finiteStateMachine = new FiniteStateMachine();
            _finiteStateMachine.AddState(new BeginAdventureState(_finiteStateMachine, _characterController, adventureUI, _input, startPoint));
            _finiteStateMachine.AddState(new RunningAdventureState(_finiteStateMachine, _characterController, adventureUI, _timer));
            _finiteStateMachine.AddState(new FinishedAdventureState(_finiteStateMachine, adventureUI, _timer, _saveLoad));
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}