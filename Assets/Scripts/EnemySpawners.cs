using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawners : MonoBehaviour
{
    public List<Transform> spawnPoints;

    [Header("Spawn Limit Settings")]
    [Range(10, 100)] public int maxSimultaneousEnemy;

    [Header("Spawn Time Settings")]
    [Range(0f, 30f)] public float startDelay;
    [Range(0f, 30f)] public float spawnDelay;
    [Range(0f, 30f)] public float waveDelay;

    private List<EnemyBehavior> _spawnEnemys;

    private void Start()
    {
        _spawnEnemys = new List<EnemyBehavior>();

        if (spawnPoints.Count != 0) StartCoroutine(SpawnEnemyWaves());
    }

    private IEnumerator SpawnEnemyWaves()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                EnemyBehavior e = SpawnManager.Instance.GetPooledEnemy();
                if (e != null)
                {
                    e.transform.position = spawnPoints[i].position;
                    e.transform.rotation = spawnPoints[i].rotation;
                    e.transform.localScale = spawnPoints[i].localScale;
                    e.gameObject.SetActive(true);
                    _spawnEnemys.Add(e);
                }

                yield return new WaitForSeconds(spawnDelay);

                //_spawnEnemys.RemoveAll(s => s.gameObject.activeSelf == false);
                //Debug.Log(_spawnEnemys.Count);

                yield return new WaitUntil(() => _spawnEnemys.RemoveAll(s => s.gameObject.activeSelf == false) == 1000 ||  _spawnEnemys.Count < maxSimultaneousEnemy);
            }
            
            yield return new WaitForSeconds(waveDelay);

            /*if (gameOver)
            {
                restart = true;
                yield break;
            }*/
        }
    }

}
