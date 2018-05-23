using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouretBehavior : MonoBehaviour {

    [Header("Shoot")]
    public float shootVelocity = 20f;
    public float shootBufferTime = 0.5f;

    public Transform playerTransform;

    private float _lastShootTime;
    private MeshRenderer _mr;

	private void Awake()
	{
        _mr = GetComponentInChildren<MeshRenderer>();
	}

	private void Update()
    {
        ShootAttack();

        Vector3 forward = (playerTransform.position - transform.position).normalized;
        _mr.transform.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg, 0f);
    }

    public void ShootAttack()
    {
        if ((Time.time - _lastShootTime) >= shootBufferTime)
        {
            BallBehavior bb = SpawnManager.Instance.GetPooledBall();

            Vector3 forward = (playerTransform.position - transform.position).normalized;

            bb.transform.position = Vector3.up + transform.position + (forward * 1.5f);
            bb.transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg, 0f);
            bb.Init(shootVelocity, "EnemyBall");

            _lastShootTime = Time.time;
        }
    }

}
