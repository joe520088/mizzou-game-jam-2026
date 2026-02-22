using System;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject heldItem;
    private GameObject player;
    private InputHandler inputHandler;
    private PlayerControls controls;

    int maxItems = 4;
    public GameObject[]heldItems;
    private int itemCount = 0;

    public void Start()
    {
        player = transform.parent.gameObject;

        inputHandler = GetComponentInParent<InputHandler>();

        heldItems = new GameObject[maxItems];

        if (inputHandler == null)
            inputHandler = FindAnyObjectByType<InputHandler>();

        controls = new PlayerControls();

        // Subscribe to the buttons. 
        // We use 0, 1, 2, 3 because arrays start at 0.
        controls.Player.EquipSlot1.performed += ctx => Equip(0);
        controls.Player.EquipSlot2.performed += ctx => Equip(1);
        controls.Player.EquipSlot3.performed += ctx => Equip(2);
        controls.Player.EquipSlot4.performed += ctx => Equip(3);

        controls.Enable();

        if (heldItem != null)
        {
            Grab(heldItem);
        }
    }

    public void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        mousePos.z = 0f;

        if (player.transform.position.x < mousePos.x)
        {
            ChangeLayer(true);
        }
        else
        {
            ChangeLayer(false);
        }
    }

    private void ChangeLayer(bool isFront)
    {
        if (heldItem == null) return;

        // Look for the renderer in children
        var sr = heldItem.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            // Force the Z position to be towards the camera
            // In 2D, moving Z closer to the camera (usually negative) helps
            heldItem.transform.localPosition = new Vector3(0, 0, -0.1f);
        }
        else
        {
            // If it returns null, it prints a warning in yellow
            Debug.LogWarning("Could not find a SpriteRenderer on " + heldItem.name + " or any of its children!");
        }
    }
    
    // This runs automatically when the Hand's collider touches an Item's trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we touched is an item
        if (collision.CompareTag("Item"))
        {
            GameObject itemToPickUp = collision.gameObject;

            // Call your Grab function
            Grab(itemToPickUp);
        }
    }
    
    public void Grab(GameObject item)
    {
        for (int i = 0; i < heldItems.Length; i++)
        {
            if (heldItems[i] == null)
            {
                // 1. Add it to the inventory array
                heldItems[i] = item;

                // 2. Make it a child of the hand so it moves with the player
                item.transform.SetParent(this.transform);
                item.transform.localPosition = Vector3.zero;

                // 3. Turn it off (Hide it) until we Equip it
                item.SetActive(false);

                Debug.Log("Picked up " + item.name + " into slot " + i);
                return;
            }
        }
        Debug.LogWarning("Inventory Full!");
    }

    public void Equip(int choice)
    {
        Debug.Log($"<color=cyan>=== Equip Start (Slot: {choice}) ===</color>");

        if (choice < 0 || choice >= heldItems.Length)
        {
            Debug.LogError($"Choice {choice} is out of bounds for heldItems array.");
            return;
        }

        // 1. Check current heldItem before swapping
        if (heldItem != null)
        {
            Debug.Log($"Hiding currently held item: {heldItem.name}");
            heldItem.SetActive(false);
            Debug.Log($"{heldItem.name} activeSelf is now: {heldItem.activeSelf}");
        }

        // 2. Check the item in the inventory slot
        GameObject itemInSlot = heldItems[choice];

        if (itemInSlot != null)
        {
            heldItem = itemInSlot;

            // Log state BEFORE SetActive
            Debug.Log($"Target item found: {heldItem.name}. State before SetActive: {heldItem.activeSelf}");

            // THE COMMAND
            heldItem.SetActive(true);

            // Log state AFTER SetActive
            Debug.Log($"Target item: {heldItem.name}. State after SetActive: {heldItem.activeSelf}");

            // 3. Parenting and Positioning
            heldItem.transform.SetParent(this.transform);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localScale = Vector3.one;

            // 4. Thorough check for SpriteRenderer and Children
            SpriteRenderer sr = heldItem.GetComponentInChildren<SpriteRenderer>(true); // 'true' includes inactive
            if (sr != null)
            {
                Debug.Log($"[Visual Check] Found SpriteRenderer on object: {sr.gameObject.name}");
                Debug.Log($"[Visual Check] SpriteRenderer component enabled: {sr.enabled}");
                Debug.Log($"[Visual Check] Actual Sprite assigned: {(sr.sprite != null ? sr.sprite.name : "NULL")}");
                Debug.Log($"[Visual Check] Child Object Active: {sr.gameObject.activeSelf}");
                Debug.Log($"[Visual Check] Child Object Active in Hierarchy: {sr.gameObject.activeInHierarchy}");
            }
            else
            {
                Debug.LogError($"[Visual Check] NO SpriteRenderer found in {heldItem.name} or children!");
            }

            Debug.Log($"<color=green>Successfully Equipped: {heldItem.name}</color>");
        }
        else
        {
            heldItem = null;
            Debug.LogWarning($"Equip failed: Slot {choice} is empty (null).");
        }

        Debug.Log("<color=cyan>=== Equip End ===</color>");
    }
}
