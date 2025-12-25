using System.Collections.Generic;
using UnityEngine;

namespace Core.Frameworks.StateMachine
{
    /// <summary>
    /// 通用状态机
    /// </summary>
    public class StateMachine
    {
        private State currentState;
        private Dictionary<string, State> states = new Dictionary<string, State>();
        
        /// <summary>
        /// 添加状态
        /// </summary>
        public void AddState(State state)
        {
            if (!states.ContainsKey(state.StateName))
            {
                states.Add(state.StateName, state);
                state.SetStateMachine(this);
            }
        }
        
        /// <summary>
        /// 切换到指定状态
        /// </summary>
        public void ChangeState(string stateName)
        {
            if (states.TryGetValue(stateName, out State newState))
            {
                currentState?.Exit();
                currentState = newState;
                currentState.Enter();
            }
            else
            {
                Debug.LogWarning($"State {stateName} not found!");
            }
        }
        
        /// <summary>
        /// 更新当前状态
        /// </summary>
        public void Update(float deltaTime)
        {
            currentState?.Update(deltaTime);
        }
        
        /// <summary>
        /// 获取当前状态
        /// </summary>
        public State GetCurrentState()
        {
            return currentState;
        }
        
        /// <summary>
        /// 获取状态名称
        /// </summary>
        public string GetCurrentStateName()
        {
            return currentState?.StateName ?? "None";
        }
    }
}