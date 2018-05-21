using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement")]
    public float movingSpeed = 5f;
    public float runningSpeed = 7.5f;
    public float stationaryTurnSpeed = 180f;
    public float movingTurnSpeed = 360f;

    [Header("Shoot")]
    public BallBehavior shootBallPrefab;
    public float shootMinChargeTime = 0.5f;
    public float shootMaxChargeTime = 5f;
    public float shootMinVelocity = 10f;
    public float shootMaxVelocity = 60f;
    public float shootBufferTime = 0.25f;

    private float _shootChargeTimer;
    private float _shootChargeVelocity;
    private float _lastShootTime;

    private Rigidbody _rigidbody;
    private Bezier _shootFeedback;

    private float _horizontal = 0f;
    private float _vertical = 0f;

    private bool _isShooting;

    private bool _isRunning = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _shootFeedback = GetComponentInChildren<Bezier>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector3 move = (_vertical * Vector3.forward) + (_horizontal * Vector3.right);
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);

        float turnAmount = Mathf.Atan2(move.x, move.z);
        float forwardAmount = move.z;
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);

        if (!_isShooting)
        {
            transform.Rotate(0f, turnAmount * turnSpeed * Time.fixedDeltaTime, 0f);
            transform.Translate(move * (_isRunning ? runningSpeed : movingSpeed) * Time.fixedDeltaTime);
        }
    }

    public void SetMoveValue(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    public void SetRunValue(bool isRunning)
    {
        _isRunning = isRunning;
    }

    public void InstantShootAttack()
    {
        if ((Time.time - _lastShootTime) >= (shootBufferTime * 2f))
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();
            bb.transform.position = transform.position + (transform.forward * 1.5f);
            bb.transform.rotation = transform.rotation;
            bb.Init(shootMinVelocity, "PlayerBall");

            _lastShootTime = Time.time;
        }
    }

    public void InitShootAttack()
    {
        if (Time.time - _lastShootTime >= shootBufferTime)
        {
            _shootChargeVelocity = shootMinVelocity;
            _shootChargeTimer = Time.time;
            
            _shootFeedback.lineProgress = _shootFeedback.lineOrientation = 0f;

            _isShooting = true;
        }
    }

    public void UpdateShootAttack()
    {
        if (_isShooting)
        {
            float curTime = Time.time - _shootChargeTimer;

            _shootFeedback.lineProgress = Mathf.Min(curTime / shootMaxChargeTime, 1f);

            if (curTime > shootMinChargeTime && curTime < shootMaxChargeTime)
                _shootChargeVelocity = shootMinVelocity + ((curTime / shootMaxChargeTime) * (shootMaxVelocity - shootMinVelocity));
            else if (curTime >= shootMaxChargeTime)
                _shootChargeVelocity = shootMaxVelocity;
        }
    }

    public void FinalizeShootAttack()
    {
        if (_isShooting)
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();
            bb.transform.position = transform.position + (transform.forward * 1.5f);
            bb.transform.rotation = transform.rotation;
            bb.Init(_shootChargeVelocity, "PlayerBall");

            _shootFeedback.lineProgress = _shootFeedback.lineOrientation = _shootChargeTimer = _shootChargeVelocity = 0f;
            _lastShootTime = Time.time;
            _isShooting = false;
            
        }
    }
}
