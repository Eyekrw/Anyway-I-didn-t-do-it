using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class TurretSpawner: MonoBehaviour
    {
        private List<Transform> _spawnPoints;
        
        public TurretSpawner(Transform mapCenter, float spawnDistance, int totalSpawnPoints)
        {
            _spawnPoints = new List<Transform>();
            for (int i = 0; i < totalSpawnPoints; i++)
            {
                float angle = i * Mathf.PI * 2 / totalSpawnPoints;
                Vector3 spawnPosition = new Vector3(
                    mapCenter.position.x + Mathf.Cos(angle) * spawnDistance,
                    mapCenter.position.y,
                    mapCenter.position.z + Mathf.Sin(angle) * spawnDistance
                );
                GameObject spawnPointObject = new GameObject($"SpawnPoint_{i}");
                spawnPointObject.transform.position = spawnPosition;
                _spawnPoints.Add(spawnPointObject.transform);
            }
        }

        public List<Transform> GetRandomSpawnPoints(int count)
        {
            return _spawnPoints.OrderBy(x => Random.value).Take(count).ToList();
        }
    }
}