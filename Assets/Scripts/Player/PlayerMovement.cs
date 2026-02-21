using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private InputHandler input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<InputHandler>();
    }

    void Update()
    {
        // Use the MoveInput from our InputHandler
        rb.linearVelocity = input.MoveInput * speed;
    }
}