using StateMachinePackage.Runtime.Transitions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            Debug.Log($"INIT {this}");
            _onInit.Invoke();
        }

        public virtual void Enter()
        {
            Debug.Log($"ENTER {this}");
            _onEnter.Invoke();
        }
        public virtual void Update()
        {
            _onUpdate.Invoke();
        }

        public virtual void Exit()
        {
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
