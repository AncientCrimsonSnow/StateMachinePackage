using StateMachinePackage.Runtime.Transitions;
using StateMachinePackage.Runtime.Transitions.Conditions;
using StateMachinePackage.Runtime.Transitions.Wrapper;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StateMachinePackage.Runtime
{
    public abstract class State
    {
        public State Parent
        {
            private set;
            get;
        }

        internal HashSet<State> Children = new HashSet<State>();
        internal HashSet<Transition> Transitions = new HashSet<Transition>();
        internal State LastChild = null;

        public StateMachineManager StateMachineManager { get; internal set; }

        private UnityEvent _onInit = new UnityEvent();
        private UnityEvent _onEnter = new UnityEvent();
        private UnityEvent _onUpdate = new UnityEvent();
        private UnityEvent _onExit = new UnityEvent();

        public State(State parent = null)
        {
            Init(parent);
        }

        public State(
            UnityEvent onInit,
            UnityEvent onEnter,
            UnityEvent onUpdate,
            UnityEvent onExit,
            State parent = null)
        {
            Init(parent);
            _onInit = onInit;
            _onEnter = onEnter;
            _onUpdate = onUpdate;
            _onExit = onExit;
        }

        #region StateManagement

        public void AddListenerOnInit(UnityAction action)
        {
            _onInit.AddListener(action);
        }
        public void AddListenerOnEnter(UnityAction action)
        {
            _onEnter.AddListener(action);
        }
        public void AddListenerOnUpdate(UnityAction action)
        {
            _onUpdate.AddListener(action);
        }
        public void AddListenerOnExit(UnityAction action)
        {
            _onExit.AddListener(action);
        }

        public virtual void Init()
        {
            if(StateMachineManager.ConfigData.PrintDebug)
                Debug.Log($"INIT {this}");
            _onInit.Invoke();
        }

        public virtual void Enter()
        {
            if (StateMachineManager.ConfigData.PrintDebug)
                Debug.Log($"ENTER {this}");
            _onEnter.Invoke();
        }
        public virtual void Update()
        {
            _onUpdate.Invoke();
        }

        public virtual void Exit()
        {
            if (StateMachineManager.ConfigData.PrintDebug)
                Debug.Log($"EXIT {this}");
            _onExit.Invoke();
        }

        private void Init(
            State parent = null)
        {
            Parent = parent;
        }

        #endregion

        #region TreeManagement
        internal void AddChild(State state)
        {
            if (Children.Contains(state))
                Debug.LogWarning($"StateOverrider: State {state.GetType()} is already in {GetType()} as substate");

            Children.Add(state);
        }
        #endregion

        #region overrives
        public override bool Equals(object obj)
        {
            if (obj == null)    return false;
            if (obj is not State) return false;
            return GetType().Equals(obj.GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var type = GetType();
            return type.ToString();
        }

        protected new abstract Type GetType();
        #endregion

#if ENABLE_INPUT_SYSTEM
        protected void CreateTransition(Type to, InputAction inputAction) => CreateTransition(to, new EventCondition(inputAction));
#endif
        protected void CreateTransition(Type to, BooleanWrapper booleanWrapper) => CreateTransition(to, new BooleanCondition(booleanWrapper));
        protected void CreateTransition(Type to, ref Action eventTriggerMethod) => CreateTransition(to, new EventCondition(ref eventTriggerMethod));
        protected void CreateTransition(Type to, UnityEvent unityEvent) => CreateTransition(to, new EventCondition(unityEvent));
        protected void CreateTransition(Type to, FloatWrapper value, float targetValue, ComparisonType comparisonType) => CreateTransition(to, new FloatCondition(value, targetValue, comparisonType));
        protected void CreateTransition(Type to, IntWrapper value, int targetValue, ComparisonType comparisionType) => CreateTransition(to, new IntCondition(value, targetValue, comparisionType));

        protected void CreateTransition(Type to, Condition condition)
        {
            var transition = new Transition(StateMachineManager, GetType(), to, condition);
            Transitions.Add(transition);
        }

        internal void CreateTransitionInternal(Type to, Condition condition)
        {
            CreateTransition(to, condition);
        }

        internal bool IsChildOf(State parent)
        {
            if (Parent == null) 
                return false;
            var crrParent = Parent;
            while (parent != crrParent) {
                if (crrParent.Parent == null)
                    return false;
                crrParent = crrParent.Parent;
            }
            return true;
        }

        public void ClearTransitions()
        {
            foreach (var transition in Transitions)
                transition.Dispose();

            Transitions = new HashSet<Transition>();
        }
    }
}
