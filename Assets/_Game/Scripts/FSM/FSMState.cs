using UnityEngine;

namespace FSM
{
    public abstract class FsmState
    {
        protected FiniteStateMachine FiniteStateMachine;

        protected FsmState(FiniteStateMachine finiteStateMachine)
        {
            FiniteStateMachine = finiteStateMachine;
        }

        public virtual void Enter()
        {
            Debug.Log($"[FSM]: Enter {GetType().Name}");
        }
        public virtual void Update() { }

        public virtual void Exit()
        {
            Debug.Log($"[FSM]: Exit {GetType().Name}");
        }
    }

}