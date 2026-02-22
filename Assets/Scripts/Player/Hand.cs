using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject heldItem;
    private GameObject player;
    private InputHandler inputHandler;

    public void Start()
    {
        player = transform.parent.gameObject;

        inputHandler = GetComponentInParent<InputHandler>();

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
        heldItem = item.gameObject;

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
    }
}
