using System;

namespace StateMachinePackage.Runtime.Transitions
{
    public class Transition
    {
        public StateMachineManager _stateMachineManager;

        internal readonly Type From;
        internal readonly Type To;

        private readonly Condition _condition;
        
        internal Transition(StateMachineManager stateMachineManager, Type from, Type to, Condition condition)
        {
            _stateMachineManager = stateMachineManager;
            From = from;
            To = to;
            _condition = condition;

            _condition.AddListener(Execute);
        }

        private void Execute()
        {
            _stateMachineManager.TransitionExecuteQueue.Enqueue(this);
        }

        public void Dispose() => _condition.RemoveAllListeners();
    }
}