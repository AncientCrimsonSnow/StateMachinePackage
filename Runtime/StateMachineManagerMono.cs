using Assets.StateMachinePackage.Runtime;
using UnityEngine;

namespace StateMachinePackage.Runtime
{
    [DefaultExecutionOrder(-1)]
    public class StateMachineManagerMono : MonoBehaviour
    {
        [SerializeField]
        private StateMachineConfig _config;
        public StateMachineManager StateMachineManager => _stateMachineManager;
        
        private StateMachineManager _stateMachineManager;

        private void Awake() => _stateMachineManager = new StateMachineManager(_config.Data);
        private void Update() => _stateMachineManager.Update();
    }
}