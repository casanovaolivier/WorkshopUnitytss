using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public bool bounceOnWall = true;

    private float _initialVelocity;
    private int _bounceCount;
    private Vector3 _lastFrameVelocity;
    private bool _createByAssist;

    private Rigidbody _rigidbody;
    private TrailRenderer _trail;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnDisable()
    {
        _trail.Clear();
    }

    public void Init (float initVel, string tag, bool assist = false)
    {
        _initialVelocity = initVel;
        _createByAssist = assist;

        gameObject.tag = tag;
        gameObject.SetActive(true);

        _rigidbody.velocity = transform.forward * _initialVelocity;
    }

    public float GetVelocity()
    {
        return _rigidbody.velocity.magnitude;
    }

    public void SetVelocity(float multiplier)
    {
        _rigidbody.velocity *= multiplier;
    }

    public bool GetCreateByAssist()
    {
        return _createByAssist;
    }

    private void Update()
    {
        _lastFrameVelocity = _rigidbody.velocity;

        if (_lastFrameVelocity.magnitude <= 0f || transform.position.y <= -5f)
            this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bounceOnWall)
        {
            if (collision.gameObject.tag == "Bounce")
                Bounce(collision.contacts[0].normal);
            else
                gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

    private void Bounce(Vector3 collisionNormal)
    {
        float speed = _lastFrameVelocity.magnitude;
        Vector3 direction = Vector3.Reflect(_lastFrameVelocity.normalized, collisionNormal);
        _rigidbody.velocity = direction * Mathf.Max(speed, (_initialVelocity / (1f * (_bounceCount + 2))));
        _bounceCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

}
