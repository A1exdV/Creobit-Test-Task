using System;
using System.Collections.Generic;

namespace FSM
{
    public class FiniteStateMachine
    {
        private FsmState _currentFsmState;

        private Dictionary<Type, FsmState> _states = new Dictionary<Type, FsmState>();

        public void AddState(FsmState newFsmState)
        {
            _states.Add(newFsmState.GetType(),  newFsmState);
        }

        public void ChangeStateTo<TState>() where TState : FsmState
        {
            if (_states.TryGetValue(typeof(TState), out var state))
            {
                _currentFsmState?.Exit();

                _currentFsmState = state;
                
                _currentFsmState.Enter();
            }
        }

        public void Update()
        {
            _currentFsmState?.Update();
        }
    }
}