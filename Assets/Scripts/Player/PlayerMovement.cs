using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController player;
    
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private InputHandler input;
    private bool wasRunning = false;
    private Camera cam;

    void Start()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<InputHandler>();
    }

    void Update()
    {
        bool isRunning = animator.GetBool("isRunning");

        // Use the MoveInput from our InputHandler
        float currentSpeed = player.Stats.MoveSpeed;
        rb.linearVelocity = input.MoveInput * currentSpeed;

        if (input.MoveInput.magnitude > 0)
        {
            animator.SetBool("isRunning", true);
        } 
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (isRunning && !wasRunning)
        {
            FindFirstObjectByType<AudioManager>().Play("Running");
        }
        else if (!isRunning && wasRunning)
        {
            FindFirstObjectByType<AudioManager>().Stop("Running");
        }

        wasRunning = isRunning;
        
        FlipTowardMouse();
    }

    void FlipTowardMouse()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        Vector3 mousePos = cam.ScreenToWorldPoint(input.MousePosition);
    
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
    }
}