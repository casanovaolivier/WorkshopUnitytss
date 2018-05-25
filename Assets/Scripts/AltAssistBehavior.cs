using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltAssistBehavior : MonoBehaviour
{
    public enum AssistState { Shield, AutoFire, MultiplyFire }

    public AltPlayerBehavior playerToFollow;
    public AltHeadBehavior headToControl;

    [Header("Movement")]
    public Vector3 offsetToPlayer;
    [Range(0f, 1f)] public float smoothSpeed = 0.125f;

    [Header("State")]
    public AssistState startState;

    private AssistState _currentState;

    private float _lastInputAngle;
    private Quaternion _lastInputRotation;

    private float _horizontal = 0f;
    private float _vertical = 0f;

    private int _assistStateLength;
    private bool _isVisible = true;

    private Transform[] _children;

    private void Awake()
    {
        _children = GetComponentsInChildren<Transform>();
        _currentState = startState;
        _assistStateLength = System.Enum.GetValues(typeof(AssistState)).Length;

        Debug.Log("=> Assit State Change : " + _currentState.ToString());
    }

    private void LateUpdate()
    {
        if(_horizontal != 0f || _vertical != 0f)
            _lastInputAngle = Mathf.Atan2(_horizontal, _vertical) * Mathf.Rad2Deg;
        
        _lastInputRotation = Quaternion.Lerp(_lastInputRotation, Quaternion.AngleAxis(_lastInputAngle, Vector3.up), smoothSpeed);
        transform.position = playerToFollow.transform.position + (_lastInputRotation * offsetToPlayer);
        transform.LookAt(playerToFollow.transform.position);
    }

    public void SetMoveValue(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    public void SetHead(bool set)
    {
        if (set) // Set Head
        {
            if (headToControl.SetHeadInPosition(transform, _currentState))
            {
                Array.ForEach(_children, a => a.gameObject.SetActive((a == transform))); // false
                _isVisible = false;
            }
                
        }
        else // Get Back Head
        {
            if (headToControl.GetBackHead())
            {
                Array.ForEach(_children, a => a.gameObject.SetActive(true)); //true
                _isVisible = true;
            }
        }
    }

    public void SetState(AssistState ast)
    {
        if (_isVisible)
        {
            _currentState = ast;
            Debug.Log("=> Assit State Change : " + _currentState.ToString());
        } 
    }

    public void SetState(float f)
    {
        if (_isVisible)
        {
            if (f >= 1)
            {
                int ns = (int)_currentState + 1;

                if (ns >= _assistStateLength) _currentState = (AssistState)0;
                else _currentState = (AssistState)ns;

                Debug.Log("=> Assit State Change : " + _currentState.ToString());
            }
            else if (f <= -1)
            {
                int ns = (int)_currentState - 1;

                if (ns < 0) _currentState = (AssistState)(_assistStateLength - 1);
                else _currentState = (AssistState)ns;

                Debug.Log("=> Assit State Change : " + _currentState.ToString());
            }
        }
    }

}
