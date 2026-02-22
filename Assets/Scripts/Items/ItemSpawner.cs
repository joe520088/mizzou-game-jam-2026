using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Pool")]
    public List<ItemData> itemPool;
    public GameObject baseItemPrefab; // A generic prefab with the ItemController script

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints;

    void Start()
    {
        SpawnRandomItems();
    }

    public void SpawnRandomItems()
    {
        foreach (Transform point in spawnPoints)
        {
            ItemData randomData = itemPool[Random.Range(0, itemPool.Count)];
            GameObject newItem = Instantiate(baseItemPrefab, point.position, point.rotation);

            // PASS BOTH the starting item AND the full list of items
            newItem.GetComponent<ItemWorldObject>().Setup(randomData, itemPool);
        }
    }
}