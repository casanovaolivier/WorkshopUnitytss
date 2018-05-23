using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistBehavior : MonoBehaviour
{
    public enum AssistState { Shield, AutoFire, MultiplyFire, MultiplyVelocity }

    public PlayerBehavior playerToFollow;
    public BallBehavior shootBallPrefab;

    [Header("Movement")]
    public Vector3 offsetToPlayer;
    [Range(0f, 1f)] public float smoothSpeed = 0.125f;

    [Header("Actions")]
    public AssistState startState;
    [Range(0.01f, 5f)] public float autoFireTime = 0.2f;
    [Range(1f, 100f)] public float autoFireVelocity = 25f;
    [Range(1f, 10f)] public float multiplyVelocity = 2f;

    private AssistState _currentState;
    private Rigidbody _rigidbody;

    private float _lastAutoFireTime;

    private float _lastInputAngle;
    private Quaternion _lastInputRotation;
    private bool _forceAlignToPlayer;

    private float _horizontal = 0f;
    private float _vertical = 0f;

    private int _assistStateLength;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentState = startState;
        _assistStateLength = System.Enum.GetValues(typeof(AssistState)).Length;

        Debug.Log("=> Assit State Change : " + _currentState.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBall" && (_currentState == AssistState.MultiplyFire || _currentState == AssistState.MultiplyVelocity))
        {
            BallBehavior bb = other.gameObject.GetComponent<BallBehavior>();
            bool createByAssist = bb.GetCreateByAssist();
            float velocity = bb.GetVelocity();

            switch (_currentState)
            {
                case AssistState.MultiplyFire:
                    if (!createByAssist)
                    {
                        other.gameObject.SetActive(false);

                        BallBehavior bb1 = SpawnManager.Instance.GetPooledBall();
                        bb1.transform.position = Vector3.up + transform.position + (-transform.right * 1.25f);
                        bb1.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, -90f, 0f));
                        bb1.Init(velocity, "PlayerBall", true);

                        BallBehavior bb2 = SpawnManager.Instance.GetPooledBall();
                        bb2.transform.position = Vector3.up + transform.position + (-transform.forward * 1.25f);
                        bb2.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 180f, 0f));
                        bb2.Init(velocity, "PlayerBall", true);

                        BallBehavior bb3 = SpawnManager.Instance.GetPooledBall();
                        bb3.transform.position = Vector3.up + transform.position + (transform.right * 1.25f);
                        bb3.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 90f, 0f));
                        bb3.Init(velocity, "PlayerBall", true);
                    }
                    break;
                case AssistState.MultiplyVelocity:
                    bb.SetVelocity(2f);
                    break;
            }


        }
        else if (other.gameObject.tag == "EnemyBall" && (_currentState == AssistState.Shield))
        {
            switch(_currentState)
            {
                case AssistState.Shield:
                    other.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void Update()
    {
        switch(_currentState)
        {
            case AssistState.AutoFire:
                if (Time.time - _lastAutoFireTime > autoFireTime)
                {
                    BallBehavior bb = SpawnManager.Instance.GetPooledBall();
                    bb.transform.position = Vector3.up + transform.position + (-transform.forward * 1.25f);
                    bb.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 180f, 0f));
                    bb.Init(autoFireVelocity, "PlayerBall", true);
                    _lastAutoFireTime = Time.time;
                }
                break;
        }
    }

    private void LateUpdate()
    {
        if (_forceAlignToPlayer)
            _lastInputAngle = Mathf.Atan2(playerToFollow.transform.forward.x, playerToFollow.transform.forward.z) * Mathf.Rad2Deg;
        else if(_horizontal != 0f || _vertical != 0f)
            _lastInputAngle = Mathf.Atan2(_horizontal, _vertical) * Mathf.Rad2Deg;
        
        _lastInputRotation = Quaternion.Lerp(_lastInputRotation, Quaternion.AngleAxis(_lastInputAngle, Vector3.up), smoothSpeed);
        _rigidbody.transform.position = playerToFollow.transform.position + (_lastInputRotation * offsetToPlayer);
        _rigidbody.transform.LookAt(playerToFollow.transform.position);
    }

    public void SetMoveValue(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    public void AlignWithPlayer(bool a)
    {
        _forceAlignToPlayer = a;
    }

    public void SetState(AssistState ast)
    {
        _currentState = ast;
        Debug.Log("=> Assit State Change : " + _currentState.ToString());
    }

    public void SetState(float f)
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
