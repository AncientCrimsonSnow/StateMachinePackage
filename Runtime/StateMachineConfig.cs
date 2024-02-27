using System.Collections;
using UnityEngine;

namespace Assets.StateMachinePackage.Runtime
{
    [CreateAssetMenu(menuName = "StateMachine/Config", fileName = "StateMachineConfig")]
    public class StateMachineConfig : ScriptableObject
    {
        [SerializeField]
        private StateMachineConfigData _data;

        internal StateMachineConfigData Data => _data;
    }
}