using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManagement : MonoBehaviour
{
    // General Setup
    private SpriteRenderer sr;
    private InputHandler inputHandler;

    // Cursors
    public Sprite[] cursors;

    void Start()
    {
        Cursor.visible = false;
        sr = gameObject.GetComponent<SpriteRenderer>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        sr.sprite = cursors[0];
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;
    }
}