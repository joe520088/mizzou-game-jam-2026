using UnityEngine;

namespace Game.Mobs
{
    public class MobSpawner : MonoBehaviour
    {
        [Header("Wave 1 Count")]
        public int minMobs = 3;
        public int maxMobs = 5;

        [Header("Spawn Points (use transforms)")]
        public Transform[] spawnPoints;

        [Header("Mob Prefabs (drag in Heavy/Light/Medium + specials)")]
        public GameObject[] mobPrefabs;

        [Header("Options")]
        public bool spawnOnStart = true;
        public bool preventDuplicateSpawnPoints = true;

        private void Start()
        {
            if (spawnOnStart)
                SpawnWave();
        }

        [ContextMenu("Spawn Wave Now")]
        public void SpawnWave()
        {
            if (mobPrefabs == null || mobPrefabs.Length == 0)
            {
                Debug.LogError("[MobSpawner] No mobPrefabs assigned.");
                return;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("[MobSpawner] No spawnPoints assigned.");
                return;
            }

            int count = Random.Range(minMobs, maxMobs + 1);

            // If you prevent duplicates, we can't spawn more than we have points.
            if (preventDuplicateSpawnPoints)
                count = Mathf.Min(count, spawnPoints.Length);

            for (int i = 0; i < count; i++)
            {
                Transform point = PickSpawnPoint(i);
                GameObject prefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];

                Instantiate(prefab, point.position, Quaternion.identity);
            }

            Debug.Log($"[MobSpawner] Spawned wave with {count} mobs.");
        }

        private Transform PickSpawnPoint(int i)
        {
            if (!preventDuplicateSpawnPoints)
                return spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Simple non-alloc Fisher-Yates style: swap chosen with current index.
            int j = Random.Range(i, spawnPoints.Length);
            (spawnPoints[i], spawnPoints[j]) = (spawnPoints[j], spawnPoints[i]);
            return spawnPoints[i];
        }
    }
}