using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Life")]
    public GameObject gameoverPanelUi;
    public Slider sliderLife;
    public float startLife = 50f;
    public float maxLife = 100f;
    public float regenLifePerSeconds = 1f;
    public float damagePerHit = 10f;
    public float lifePerBonus = 20f;

    [Header("Bonus")]
    public float bonusSpeedDuration = 10f;
    public float bonusSpeedVelocity = 12.5f;

    private float _bonusSpeedStartTime;
    private float _currentLife = 0f;

    private float _shootChargeTimer;
    private float _shootChargeVelocity;
    private float _lastShootTime;

    private Rigidbody _rigidbody;
    private Bezier _shootFeedback;

    private float _horizontal = 0f;
    private float _vertical = 0f;

    private bool _isShooting;
    private bool _isBonusSpeed = false;
    private bool _isRunning = false;
    private bool _isDead = false;

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _shootFeedback = GetComponentInChildren<Bezier>();

        SetLife(startLife);
    }

    private void FixedUpdate()
    {
        if (regenLifePerSeconds > 0f)
            SetLife(regenLifePerSeconds * Time.fixedDeltaTime);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        if (_isDead)
        {
            if (gameoverPanelUi != null && !gameoverPanelUi.activeSelf)
            {
                gameoverPanelUi.SetActive(true);
            }
        }
        else
        {
            UpdateMovement();
            UpdateBonusSpeed();
        }
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

            if (_isBonusSpeed)
                transform.Translate(move * bonusSpeedVelocity * Time.fixedDeltaTime);
            else
                transform.Translate(move * (_isRunning ? runningSpeed : movingSpeed) * Time.fixedDeltaTime);
        }
    }

    private void UpdateBonusSpeed()
    {
        if (_isBonusSpeed && Time.time - _bonusSpeedStartTime > bonusSpeedDuration)
        {
            _isBonusSpeed = false;
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
        if ((Time.time - _lastShootTime) >= (shootBufferTime * 2f) && !_isDead)
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();
            bb.transform.position = Vector3.up + transform.position + (transform.forward * 1.5f);
            bb.transform.rotation = transform.rotation;
            bb.Init(shootMinVelocity, "PlayerBall");

            _lastShootTime = Time.time;
        }
    }

    public void InstantShootAttack(float x, float z)
    {
       if ((Time.time - _lastShootTime) >= (shootBufferTime * 2f) && (x != 0f || z != 0f) && !_isDead)
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();
            bb.transform.position = Vector3.up + transform.position + (new Vector3(x, 0f, z) * 1.5f);
            bb.transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(x, z) * Mathf.Rad2Deg, 0f);
            bb.Init(shootMinVelocity, "PlayerBall");

            _lastShootTime = Time.time;
        }
    }

    public void InitShootAttack()
    {
        if (Time.time - _lastShootTime >= shootBufferTime && !_isDead)
        {
            _shootChargeVelocity = shootMinVelocity;
            _shootChargeTimer = Time.time;
            
            _shootFeedback.lineProgress = _shootFeedback.lineOrientation = 0f;

            _isShooting = true;
        }
    }

    public void UpdateShootAttack()
    {
        if (_isShooting && !_isDead)
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
        if (_isShooting && !_isDead)
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();
            bb.transform.position = Vector3.up + transform.position + (transform.forward * 1.5f);
            bb.transform.rotation = transform.rotation;
            bb.Init(_shootChargeVelocity, "PlayerBall");

            _shootFeedback.lineProgress = _shootFeedback.lineOrientation = _shootChargeTimer = _shootChargeVelocity = 0f;
            _lastShootTime = Time.time;
            _isShooting = false;   
        }
    }

    private void SetLife(float add)
    {
        _currentLife = Mathf.Clamp(_currentLife + add, 0f, maxLife);

        if (sliderLife != null)
        {
            float relativeLife = _currentLife / maxLife;
            sliderLife.value = relativeLife;
        }

        if (_currentLife == 0f) _isDead = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBall")
        {
            SetLife(-damagePerHit);

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "LifeBonus")
        {
            SetLife(lifePerBonus);

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "SpeedBonus")
        {
            _bonusSpeedStartTime = Time.time;
            _isBonusSpeed = true;

            collision.gameObject.SetActive(false);
        }
    }
}
