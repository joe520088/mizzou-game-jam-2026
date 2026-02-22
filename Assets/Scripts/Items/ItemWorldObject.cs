using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour
{
    private ItemData currentData;
    private List<ItemData> possibleItems; // To know what else it can turn into
    private GameObject currentVisual;

    [Header("Switch Settings")]
    public float minSwitchTime = 2.0f;
    public float maxSwitchTime = 5.0f;

    public void Setup(ItemData data, List<ItemData> pool)
    {
        possibleItems = pool;
        currentData = data;
        ApplyVisual(data);

        // Start the random switching loop
        StartCoroutine(SwitchRoutine());
    }

    private void ApplyVisual(ItemData data)
    {
        // Remove old visual
        if (currentVisual != null) Destroy(currentVisual);

        // Spawn new visual from the Prefab slot in ItemData
        if (data.prefab != null)
        {
            currentVisual = Instantiate(data.prefab, transform.position, transform.rotation, transform);
            currentVisual.transform.localPosition = Vector3.zero;
        }

        gameObject.name = "Item_" + data.itemName;

        // Pick a random angle between 0 and 360 degrees
        float randomZ = Random.Range(0f, 360f);
        // Apply it to the visual's local rotation
        currentVisual.transform.localRotation = Quaternion.Euler(0, 0, randomZ);
    }

    IEnumerator SwitchRoutine()
    {
        while (true)
        {
            // 1. Wait for a random amount of time between your constants
            float waitTime = Random.Range(minSwitchTime, maxSwitchTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Pick a new random item from the pool
            if (possibleItems != null && possibleItems.Count > 0)
            {
                currentData = possibleItems[Random.Range(0, possibleItems.Count)];
                ApplyVisual(currentData);
            }
        }
    }
}