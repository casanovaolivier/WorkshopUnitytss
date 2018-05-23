using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public PlayerBehavior player;

    [Header("=== Ball ===")]
    public BallBehavior ballPrefab;
    public int ballAmount;
    public bool ballShouldExpand;

    [Header("=== Enemy ===")]
    public EnemyBehavior enemyPrefab;
    public int enemyAmount;
    public bool enemyShouldExpand;

    public static SpawnManager Instance;
    private List<BallBehavior> _pooledBalls;
    private List<EnemyBehavior> _pooledEnemys;

    private void Awake()
    {
        Instance = this;
        StartObjectPooling();
    }

    private void StartObjectPooling()
    {
        _pooledBalls = new List<BallBehavior>();
        for (int i = 0; i < ballAmount; i++)
        {
            BallBehavior b = Instantiate<BallBehavior>(ballPrefab, transform);
            b.gameObject.SetActive(false);
            _pooledBalls.Add(b);
        }

        _pooledEnemys = new List<EnemyBehavior>();
        for (int i = 0; i < enemyAmount; i++)
        {
            EnemyBehavior e = Instantiate<EnemyBehavior>(enemyPrefab, transform);
            e.Init(player.gameObject);
            e.gameObject.SetActive(false);
            _pooledEnemys.Add(e);
        }
    }

    public BallBehavior GetPooledBall()
    {
        for (int i = 0; i < _pooledBalls.Count; i++)
            if (!_pooledBalls[i].gameObject.activeInHierarchy)
                return _pooledBalls[i];

        //Pool List Shoud Expand
        if (ballShouldExpand)
        {
            BallBehavior b = Instantiate<BallBehavior>(ballPrefab, transform);
            b.gameObject.SetActive(false);
            _pooledBalls.Add(b);
            return b;
        }
        else return null;
    }

    public EnemyBehavior GetPooledEnemy()
    {
        for (int i = 0; i < _pooledEnemys.Count; i++)
            if (!_pooledEnemys[i].gameObject.activeInHierarchy)
                return _pooledEnemys[i];

        //Pool List Shoud Expand
        if (ballShouldExpand)
        {
            EnemyBehavior e = Instantiate<EnemyBehavior>(enemyPrefab, transform);
            e.Init(player.gameObject);
            e.gameObject.SetActive(false);
            _pooledEnemys.Add(e);
            return e;
        }
        else return null;
    }

    public void Clear()
    {
        _pooledBalls.ForEach(t => t.gameObject.SetActive(false));
        _pooledEnemys.ForEach(t => t.gameObject.SetActive(false));
    }
}
