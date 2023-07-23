using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanAgentStates
{
    Idle,
    Moving,
    Attacking,
    Patrolling,
    DefendingPoint
};
public class HumanUintFSM : MonoBehaviour
{
    public IState _currentState;
    public Dictionary<HumanAgentStates,IState> _allStates=new Dictionary<HumanAgentStates, IState>();
    public void OnUpdate()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdate();
        }
        else
        {
            if (_allStates[0] != null)
            {
                _currentState = _allStates[0];
            }
            else
            {
                Debug.Log("No tiene estados");
            }
        }
    }
    public void AddState(HumanAgentStates key,IState state)
    {
        if (!_allStates.ContainsKey(key))
        {
            _allStates.Add(key, state);
        }
    }
    public void ChangeState(HumanAgentStates key)
    {
        if (!_allStates.ContainsKey(key))
        {
            return;
        }
        if (_currentState != null)
        {
            _currentState.OnExit();
            _currentState = _allStates[key];
            _currentState.OnEnter();
        }
    }
}
