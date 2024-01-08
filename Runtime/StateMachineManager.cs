using StateMachinePackage.Runtime.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateMachinePackage.Runtime
{
    public class StateMachineManager
    {
        public State CurrentState { private set; get; }

        private readonly Dictionary<Type, State> _allStates = new();

        private readonly HashSet<State> _leafStates = new();

        private readonly UniqueQueue<State> _initQueue = new();
        private readonly Queue<TransitionData> _transitionQueue = new();

        public void Update()
        {
            var newStates = _initQueue.Count > 0;
            while (_initQueue.Count > 0)
            {
                State state = _initQueue.Dequeue();
                state.Init();
            }
            if (newStates)
            {
                CurrentState.Enter();
                while (!_leafStates.Contains(CurrentState))
                {
                    CurrentState = CurrentState.Children.First();
                    CurrentState.Enter();
                }
            }
            while(_transitionQueue.Count > 0)
            {
                TransitionData transitionData = _transitionQueue.Dequeue();
                var state = GetStateByType(transitionData.from);
                state.CreateTransitionInternal(transitionData.to, transitionData.condition);
            }
            CurrentState.Update();
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

            CurrentState ??= entryState;

            return newStates;
        }

        public void AddTransition(Type from, Type to, Condition condition)
        {
            _transitionQueue.Enqueue(
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
                state = state.Children.First();

            var (pathState1ToLCA, pathLCAtoState2) = FindPathsToLCA(CurrentState, state);
            foreach (var pathState in pathState1ToLCA)
                pathState.Exit();

            CurrentState = state;
            foreach (var pathState in pathLCAtoState2)
                pathState.Enter();
        }

        internal State GetStateByType(Type type)
        {
            if(_allStates.TryGetValue(type, out State value))
                return value;
            return null;
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
            List<State> path1 = new List<State>();
            List<State> path2 = new List<State>();
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

        private struct TransitionData
        {
            public Type from;
            public Type to;
            public Condition condition;
        }
    }
}