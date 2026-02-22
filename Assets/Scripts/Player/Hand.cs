using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject heldItem;
    public UIInventory inventory;
    private GameObject player;
    private InputHandler inputHandler;
    private PlayerControls controls;

    private int maxItems = 4;
    public GameObject[] heldItems;

    public void Start()
    {
        player = transform.parent.gameObject;
        inputHandler = GetComponentInParent<InputHandler>();
        heldItems = new GameObject[maxItems];

        if (inputHandler == null)
            inputHandler = FindAnyObjectByType<InputHandler>();

        controls = new PlayerControls();

        controls.Player.EquipSlot1.performed += ctx => Equip(0);
        controls.Player.EquipSlot2.performed += ctx => Equip(1);
        controls.Player.EquipSlot3.performed += ctx => Equip(2);
        controls.Player.EquipSlot4.performed += ctx => Equip(3);

        controls.Player.Drop.performed += ctx => Drop();

        controls.Enable();

        if (heldItem != null)
        {
            Grab(heldItem);
        }
    }

    private void OnDisable()
    {
        if (controls != null)
            controls.Disable();
    }

    public void Update()
    {
        if (inputHandler == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        mousePos.z = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Grab(collision.gameObject);
        }
    }

    public void Grab(GameObject item)
    {
        for (int i = 0; i < heldItems.Length; i++)
        {         
            if (heldItems[i] == null)
            {
                heldItems[i] = item;
                item.transform.SetParent(this.transform);
                item.transform.localPosition = Vector3.zero;
                item.SetActive(false);

                inventory.updateInventory();

                return;
            }
        }
    }

    public void Equip(int choice)
    {
        if (choice < 0 || choice >= heldItems.Length) return;

        if (heldItem != null)
        {
            heldItem.SetActive(false);
        }

        if (heldItems[choice] != null)
        {
            heldItem = heldItems[choice];
            heldItem.SetActive(true);

            heldItem.transform.SetParent(this.transform);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localScale = Vector3.one;
            heldItem.GetComponentInChildren<SpriteRenderer>().transform.localRotation = Quaternion.identity;
            ChangeLayer(10);
        }
        else
        {
            heldItem = null;
        }
    }

    public void Drop()
    {
        for (int i = 0; i < 4; i++)
        {
            if (heldItem == heldItems[i])
            {
                heldItems[i] = null;
                heldItem.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0);

                inventory.updateInventory();
            }
        }
    }

    private void ChangeLayer(int layer)
    {
        if (heldItem == null) return;

        var sr = heldItem.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            sr.sortingOrder = layer;

            // Ensure Z-offset stays consistent
            heldItem.transform.localPosition = new Vector3(0, 0, -0.1f);
        }
    }
}