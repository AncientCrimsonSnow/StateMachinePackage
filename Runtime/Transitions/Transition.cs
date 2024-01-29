using System;

namespace StateMachinePackage.Runtime.Transitions
{
    public class Transition
    {
        public StateMachineManager _stateMachineManager;

        private Type _from;
        private Type _to;

        private Condition _condition;
        
        internal Transition(StateMachineManager stateMachineManager, Type from, Type to, Condition condition)
        {
            _stateMachineManager = stateMachineManager;
            _from = from;
            _to = to;
            _condition = condition;

            _condition.AddListener(() => Execute());
        }

        private void Execute()
        {
            var fromState = _stateMachineManager.GetStateByType(_from);
            
            if (!_stateMachineManager.CurrentState.Equals(fromState))
            {
                if(_stateMachineManager.CurrentState.IsChildOf(fromState))
                    _stateMachineManager.SwitchState(_to);
                else
                    return;
            }
            _stateMachineManager.SwitchState(_to);
        }

        public void Dispose()
        {
            _condition.RemoveAllListeners();
        }
    }
}