using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

	public enum EnemyState { Follow, Shoot, FollowAndShoot, PlayerIsDead }
    private EnemyState _currentState;

    private GameObject _currentTarget;

    public void Init(GameObject target)
    {
        _currentTarget = target;
    }

	void OnEnable ()
    {
        SetState(EnemyState.Follow);
    }

    private void Update ()
    {
        float dist = Vector3.Distance(transform.position, _currentTarget.transform.position);

		switch(_currentState)
        {
            case EnemyState.Follow:

                break;
            case EnemyState.Shoot:

                break;
            case EnemyState.FollowAndShoot:

                break;
            case EnemyState.PlayerIsDead:

                break;
        }
	}

    public void SetState(EnemyState newState)
    {
        switch (_currentState)
        {
            case EnemyState.Follow:

                break;
            case EnemyState.Shoot:

                break;
            case EnemyState.FollowAndShoot:
                
                break;
            case EnemyState.PlayerIsDead:

                break;
        }

        _currentState = newState;
    }
}
