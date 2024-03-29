﻿using Assets.StateMachinePackage.Runtime;
using StateMachinePackage.Runtime.Transitions;
using StateMachinePackage.Runtime.Transitions.Conditions;
using StateMachinePackage.Runtime.Transitions.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace StateMachinePackage.Runtime
{
    public class StateMachineManager
    {
        public State CurrentState { private set; get; }

        internal readonly UniqueQueue<Transition> TransitionExecuteQueue = new();
        internal readonly StateMachineConfigData ConfigData;

        private readonly Dictionary<Type, State> _allStates = new();

        private readonly HashSet<State> _leafStates = new();

        private readonly UniqueQueue<State> _initQueue = new();
        private readonly UniqueQueue<State> _exitQueue = new();
        private readonly UniqueQueue<State> _enterQueue = new();
     
        private readonly Queue<TransitionData> _transitionAddQueue = new();

        internal StateMachineManager(StateMachineConfigData configData)
        {
            ConfigData = configData;
        }

        public void Update()
        {
            ProcessQueues();

            var crrState = CurrentState;
            while (crrState != null)
            {
                crrState.Update();
                crrState = crrState.Parent;
            }
        }

        public State[] GetLeafStates()
        {
            var result = new State[_leafStates.Count];
            _leafStates.CopyTo(result);
            return result;
        }

        public State AddState<T>(T entryState) where T : State
        {
            return AddState(entryState, Array.Empty<State>())[0];
        }
        
        public State[] AddState(State entryState, params State[] states)
        {
            State[] newStates = new State[] { entryState }.Concat(states).ToArray();

            foreach (var state in newStates)
            {
                if (_allStates.ContainsKey(state.GetType()))
                    continue;

                state.StateMachineManager = this;
                _initQueue.Enqueue(state);

                if (state.Parent != null)
                {
                    state.Parent.AddChild(state);
                    _leafStates.Remove(state.Parent);
                }
                _leafStates.Add(state);
                _allStates.Add(state.GetType(), state);
            }

            var stack = new Stack<State>();
            if(CurrentState == null)
            {
                var crrState = entryState;
                while(crrState != null)
                {
                    stack.Push(crrState);
                    crrState = crrState.Parent;
                }
                foreach (var state in stack)
                    _enterQueue.Enqueue(state);
                CurrentState = entryState;
            }
            else if(IsParentOf(entryState, CurrentState))
            {
                var crrState = entryState;
                while (crrState != CurrentState)
                {
                    stack.Push(crrState);
                    crrState = crrState.Parent;
                }
                foreach (var state in stack)
                    _enterQueue.Enqueue(state);
                CurrentState = entryState;
            }

            return newStates;
        }

        public void AddTransition(Type from, Type to, BooleanWrapper booleanWrapper) => AddTransition(from, to, new BooleanCondition(booleanWrapper));

        public void AddTransition(Type from, Type to, ref Action eventTriggerMethod) => AddTransition(from, to, new EventCondition(ref eventTriggerMethod));

        public void AddTransition(Type from, Type to, UnityEvent unityEvent) => AddTransition(from, to, new EventCondition(unityEvent));

        public void AddTransition(Type from, Type to, FloatWrapper value, float targetValue, ComparisonType comparisonType) => AddTransition(from, to, new FloatCondition(value, targetValue, comparisonType));

        public void AddTransition(Type from, Type to, IntWrapper value, int targetValue, ComparisonType comparisonType) => AddTransition(from, to, new IntCondition(value, targetValue, comparisonType));

        public void AddTransition(Type from, Type to, Condition condition)
        {
            _transitionAddQueue.Enqueue(
                new TransitionData
                {
                    from = from,
                    to = to,
                    condition = condition
                });
        }



        internal void SwitchState(Type stateType) => SwitchState(GetStateByType(stateType));
        internal void SwitchState(State state)
        {
            if (state.Equals(CurrentState))
            {
                Debug.LogWarning($"State {state} is already active");
                return;
            }
            while (!_leafStates.Contains(state))
            {
                state.LastChild ??= state.Children.First();
                state = state.LastChild;
            }
                           
            var (pathState1ToLCA, pathLCAtoState2) = FindPathsToLCA(CurrentState, state);
            foreach (var pathState in pathState1ToLCA)
                _exitQueue.Enqueue(pathState);

            foreach (var pathState in pathLCAtoState2)
                _enterQueue.Enqueue(pathState);


            while (state.Parent != null)
            {
                state.Parent.LastChild = state;
                state = state.Parent;
            }
        }

        internal State GetStateByType(Type type)
        {
            if(_allStates.TryGetValue(type, out State value))
                return value;
            return null;
        }

        private void ProcessQueues()
        {
            ProcessInitQueue();

            ProcessTransitionAddQueue();
            ProcessTransitionExecuteQueue();

            ProcessExitQueue();
            ProcessEnterQueue();
        }

        private void ProcessInitQueue()
        {
            while (_initQueue.Count > 0) _initQueue.Dequeue().Init();
        }

        private void ProcessTransitionAddQueue()
        {
            var transitionAddQueueCopy = new Queue<TransitionData>(_transitionAddQueue);
            _transitionAddQueue.Clear();
            while (transitionAddQueueCopy.Count > 0)
            {
                var transitionData = transitionAddQueueCopy.Dequeue();
                var state = GetStateByType(transitionData.from);
                state.CreateTransitionInternal(transitionData.to, transitionData.condition);
            }
        }

        private void ProcessTransitionExecuteQueue()
        {
            var transitionExecuteQueueCopy = TransitionExecuteQueue.ShallowCopy();
            TransitionExecuteQueue.Clear();
            while(transitionExecuteQueueCopy.Count > 0)
            {
                var transition = transitionExecuteQueueCopy.Dequeue();
                var fromState = GetStateByType(transition.From);

                if (!CurrentState.Equals(fromState))
                {
                    if (CurrentState.IsChildOf(fromState))
                        SwitchState(transition.To);
                    else
                        continue;
                }
                SwitchState(transition.To);
            }
        }

        private void ProcessExitQueue()
        {
            var exitQueueCopy = _exitQueue.ShallowCopy();
            _exitQueue.Clear();
            while (exitQueueCopy.Count > 0) exitQueueCopy.Dequeue().Exit();
        }

        private void ProcessEnterQueue()
        {
            var enterQueueCopy = _enterQueue.ShallowCopy();
            _enterQueue.Clear();

            State newCurrentState = null;
            while (enterQueueCopy.Count > 0)
            {
                newCurrentState = enterQueueCopy.Dequeue();
                newCurrentState.Enter();
            }
            if(newCurrentState != null)
                CurrentState = newCurrentState;
        }

        private int GetDepth(State state)
        {
            int depth = 0;
            while (state != null)
            {
                state = state.Parent;
                depth++;
            }
            return depth;
        }

        private (List<State> pathState1ToLCA, List<State> pathLCAtoState2) FindPathsToLCA(State state1, State state2)
        {
            List<State> path1 = new();
            List<State> path2 = new();
            _ = FindLowestCommonAncestor(state1, state2, path1, path2);

            path2.Reverse();

            return (path1, path2);
        }

        private State FindLowestCommonAncestor(State state1, State state2, List<State> path1, List<State> path2)
        {
            var depth1 = GetDepth(state1);
            var depth2 = GetDepth(state2);

            while (depth1 > depth2)
            {
                path1.Add(state1);
                state1 = state1.Parent;
                depth1--;
            }

            while (depth2 > depth1)
            {
                path2.Add(state2);
                state2 = state2.Parent;
                depth2--;
            }

            while (state1 != state2)
            {
                path1.Add(state1);
                path2.Add(state2);
                state1 = state1.Parent;
                state2 = state2.Parent;
            }

            return state1;
        }

        private bool IsParentOf(State child, State parent)
        {
            if (child == null || parent == null)
                return false;

            State current = child;
            while (current.Parent != null)
            {
                if (current.Parent == parent)
                    return true;

                current = current.Parent;
            }

            return false;
        }

        private struct TransitionData
        {
            public Type from;
            public Type to;
            public Condition condition;
        }
    }
}