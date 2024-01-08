using UnityEngine;

namespace StateMachinePackage.Runtime
{
    [DefaultExecutionOrder(-1)]
    public class StateMachineManagerMono : MonoBehaviour
    {
        public StateMachineManager StateMachineManager => _stateMachineManager;
        
        private StateMachineManager _stateMachineManager;

        private void Awake() => _stateMachineManager = new StateMachineManager();
        private void Update() => _stateMachineManager.Update();
    }
}