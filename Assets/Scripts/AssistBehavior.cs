using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistBehavior : MonoBehaviour
{
    public PlayerBehavior playerToFollow;
    public BallBehavior shootBallPrefab;
    public Vector3 offsetToPlayer;
    [Range(0f, 1f)] public float smoothSpeed = 0.125f;

    private Rigidbody _rigidbody;

    private float _lastInputAngle;
    private Quaternion _lastInputRotation;
    private bool _forceAlignToPlayer;

    private float _horizontal = 0f;
    private float _vertical = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBall")
        {
            BallBehavior bb0 = other.gameObject.GetComponent<BallBehavior>();
            bool createByAssist = bb0.GetCreateByAssist();
            float velocity = bb0.GetVelocity();
            other.gameObject.SetActive(false);

            if (!createByAssist)
            {
                BallBehavior bb1 = SpawnManager.Instance.GetPooledBall();
                bb1.transform.position = transform.position + (-transform.right * 1.25f);
                bb1.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, -90f, 0f));
                bb1.Init(velocity, "PlayerBall", true);

                BallBehavior bb2 = SpawnManager.Instance.GetPooledBall();
                bb2.transform.position = transform.position + (-transform.forward * 1.25f);
                bb2.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 180f, 0f));
                bb2.Init(velocity, "PlayerBall", true);

                BallBehavior bb3 = SpawnManager.Instance.GetPooledBall();
                bb3.transform.position = transform.position + (transform.right * 1.25f);
                bb3.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 90f, 0f));
                bb3.Init(velocity, "PlayerBall", true);
            }
        }
        else if (other.gameObject.tag == "EnemyBall")
        {
            other.gameObject.SetActive(false);
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
        //_forceAlignToPlayer = false;
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

}
