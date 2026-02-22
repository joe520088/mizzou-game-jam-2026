using System;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject heldItem;
    private GameObject player;
    private InputHandler inputHandler;

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
        var sr = heldItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = isFront ? 1 : -1;
        }
    }

    public void Grab(GameObject item)
    {
        for (int i = 0; i < heldItems.Length; i++)
        {
            if (heldItems[i] == null)
            {
                heldItems[i] = item;
                item.transform.SetParent(transform);
                item.transform.localPosition = Vector3.zero;
                return;
            }
        }
        Debug.LogWarning("Cannot hold more than " + maxItems + " items!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Grab(other.gameObject);
        }
    }
}
