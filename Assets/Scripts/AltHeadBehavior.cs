using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltHeadBehavior : MonoBehaviour
{
    public AltPlayerBehavior playerToAttach;

    [Header("Transition")]
    public float transitionDuration = 0.5f;

    [Header("State")]
    public GameObject shield;
    public GameObject autoFire;
    public GameObject multiplyFire;

    [Header("Parameters")]
    [Range(0.01f, 5f)] public float autoFireTime = 0.2f;
    [Range(1f, 100f)] public float autoFireVelocity = 25f;

    private Transform _startTransitionTr;
    private Transform _endTransitionTr;

    private float _transitionStartTime;
    private float _lastAutoFireTime;

    private AltAssistBehavior.AssistState _currentState;

    private bool _isInTransition = false;
    private bool _isInPosition = false;

    public void Awake()
    {
        transform.parent = playerToAttach.headPosition;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        shield.SetActive(false);
        autoFire.SetActive(false);
        multiplyFire.SetActive(false);
    }

    public bool SetHeadInPosition(Transform tr, AltAssistBehavior.AssistState state)
    {
        if (!_isInPosition && !_isInTransition)
        {
            _startTransitionTr = playerToAttach.headPosition;
            _endTransitionTr = tr;

            _currentState = state;
            transform.parent = null;

            _transitionStartTime = Time.time;
            _isInTransition = true;
            return true;
        }
        else return false;
    }

    public bool GetBackHead()
    {
        if (_isInPosition && !_isInTransition)
        {
            _startTransitionTr = transform;
            _endTransitionTr = playerToAttach.headPosition;

            shield.SetActive(false);
            autoFire.SetActive(false);
            multiplyFire.SetActive(false);

            _transitionStartTime = Time.time;
            _isInTransition = true;
            return true;
        }
        else return false;
    }

    public void Update()
    {
        if (_isInTransition)
        {
            if (Time.time - _transitionStartTime > transitionDuration)
            {
                _isInPosition = !_isInPosition;

                if (_isInPosition)
                {
                    transform.position = _endTransitionTr.position;
                    transform.rotation = _endTransitionTr.rotation;

                    shield.SetActive(_currentState == AltAssistBehavior.AssistState.Shield);
                    autoFire.SetActive(_currentState == AltAssistBehavior.AssistState.AutoFire);
                    multiplyFire.SetActive(_currentState == AltAssistBehavior.AssistState.MultiplyFire);
                }
                else
                {
                    transform.parent = playerToAttach.headPosition;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }

                _isInTransition = false;
            }
            else
            {
                float progress = (Time.time - _transitionStartTime) / transitionDuration;
                transform.position = Vector3.Lerp(_startTransitionTr.position, _endTransitionTr.position, progress);
                transform.rotation = Quaternion.Lerp(_startTransitionTr.rotation, _endTransitionTr.rotation, progress);
            }
        }

       if (_isInPosition && _currentState == AltAssistBehavior.AssistState.AutoFire && !_isInTransition)
        {
            if (Time.time - _lastAutoFireTime > autoFireTime)
            {
                BallBehavior bb = SpawnManager.Instance.GetPooledBall();
                bb.transform.position = Vector3.up + transform.position + (-transform.forward * 1.25f);
                bb.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 180f, 0f));
                bb.Init(autoFireVelocity, "PlayerBall", true);
                _lastAutoFireTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBall" && _isInPosition && _currentState == AltAssistBehavior.AssistState.MultiplyFire && !_isInTransition) 
        {
            BallBehavior bb = other.gameObject.GetComponent<BallBehavior>();
            bool createByAssist = bb.GetCreateByAssist();
            float velocity = bb.GetVelocity();

            if (!createByAssist)
            {
                bb.gameObject.SetActive(false);

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

                BallBehavior bb4 = SpawnManager.Instance.GetPooledBall();
                bb4.transform.position = Vector3.up + transform.position + (transform.forward * 1.25f);
                bb4.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, 0f));
                bb4.Init(velocity, "PlayerBall", true);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBall" && _isInPosition && _currentState == AltAssistBehavior.AssistState.MultiplyFire && !_isInTransition)
            collision.gameObject.SetActive(false);
    }
}
