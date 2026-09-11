using System.Collections.Generic;
using Gameplay.Enemy;
using UnityEngine;

namespace Core
{
    public class LevelGenerator : MonoBehaviour
    {
        [System.Serializable]
        private struct EnemySpawnData
        {
            public IEnemy Enemy;
            public Vector3 position;
            public Quaternion rotation;

            public EnemySpawnData(IEnemy enemy, Vector3 position, Quaternion rotation)
            {
                Enemy = enemy;
                this.position = position;
                this.rotation = rotation;
            }
        }

        [Header("Road Settings")]
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private GameObject finishPrefab;
        [SerializeField] private int levelLength = 10;
        private const float SegmentLength = 75f;

        [Header("Enemy Settings")]
        [SerializeField] private EnemyBase[] enemyPrefabs;
        [SerializeField] private int enemiesPerSegment = 3;
        [SerializeField] private float roadWidth = 3.5f;

        private readonly List<GameObject> _spawnedSegments = new();
        private readonly List<EnemySpawnData> _spawnedEnemies = new();

        public void GenerateLevel()
        {
            ClearLevel();

            for (var i = 0; i < levelLength; i++)
            {
                var position = new Vector3(0, 0, i * SegmentLength);
                var prefab = (i == levelLength - 1 && finishPrefab != null) ? finishPrefab : levelPrefab;
                var segment = Instantiate(prefab, position, Quaternion.identity);
                _spawnedSegments.Add(segment);

                if (i > 0 && i < levelLength - 1)
                {
                    SpawnEnemiesOnSegment(position.z);
                }
            }
        }

        private void SpawnEnemiesOnSegment(float segmentPositionZ)
        {
            var step = SegmentLength / (enemiesPerSegment + 1);

            for (var j = 1; j < enemiesPerSegment; j++)
            {
                var z = segmentPositionZ + j * step;
                var x = Random.Range(-roadWidth, roadWidth);

                var spawnPos = new Vector3(x, 0, z);
                var spawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

                var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                var enemy = Instantiate(prefab, spawnPos, spawnRot);
                _spawnedEnemies.Add(new EnemySpawnData(enemy, spawnPos, spawnRot));
            }
        }

        public void ResetEnemies()
        {
            foreach (var enemyData in _spawnedEnemies)
            {
                enemyData.Enemy.ResetEnemy(enemyData.position, enemyData.rotation);
            }
        }

        private void ClearLevel()
        {
            foreach (var segment in _spawnedSegments)
            {
                Destroy(segment);
            }
            _spawnedSegments.Clear();

            foreach (var enemyData in _spawnedEnemies)
            {
                Destroy(enemyData.Enemy.GameObject);
            }
            _spawnedEnemies.Clear();
        }
    }
}

