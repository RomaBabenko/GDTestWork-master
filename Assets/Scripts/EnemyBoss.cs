using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBoss : Enemy
{
    [SerializeField] private GameObject _regularEnemyPrefab;

    protected override void Die()
    {
        SpawnRegularEnemies();
        base.Die();
    }

    private void SpawnRegularEnemies()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(i * 2, 0, 0);
            GameObject newEnemy = Instantiate(_regularEnemyPrefab, spawnPosition, Quaternion.identity);
            SceneManager.Instance.AddEnemy(newEnemy.GetComponent<Enemy>());
        }
    }
}
