using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private PathGenDFS _pathGenerator;
    [SerializeField] private GameObject RatPrefab;

    private int enemyCount;

    private void Start()
    {
        SpawnEnemy();

        ScoreKeepr.DifficultyIncrease += SpawnAnother;
    }

    private void SpawnAnother(object sender, System.EventArgs e)
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        enemyCount ++;

        GameObject r = Instantiate(RatPrefab, transform.position, Quaternion.identity);
        r.GetComponent<PathFinder>().SetPathGen(_pathGenerator);
    }

    private void OnDisable()
    {
        ScoreKeepr.DifficultyIncrease -= SpawnAnother;
    }
}
