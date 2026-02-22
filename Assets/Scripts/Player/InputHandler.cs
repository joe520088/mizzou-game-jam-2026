using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    private PlayerControls controls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    // Fixed: This must be checked in Update or via the Action, 
    // it cannot be assigned with '=>' inside Awake like that.
    public bool IsAttackHeld { get; private set; }

    public event Action OnAttackPerformed;

    [Header("Weapon References")]
    public Weapon equippedWeapon;
    public GameObject starterWeaponPrefab;

    private void Awake()
    {
        controls = new PlayerControls();

        // 1. Setup Movement
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        // 2. Setup Look/Mouse
        controls.Player.Look.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();

        // 3. Setup Attack (Events)
        controls.Player.Attack.performed += ctx =>
        {
            Debug.Log("Attack Performed");
            OnAttackPerformed?.Invoke();
        };
    }

    private void Start()
    {
        // Initial detection
        DetectWeapon();
    }

    private void Update()
    {
        // Fixed: Read the 'IsPressed' state every frame
        IsAttackHeld = controls.Player.Attack.IsPressed();

        // Rigorous Check: Only search for a weapon if we don't have one
        // Running GetComponentsInChildren every frame is very bad for performance!
        if (equippedWeapon == null)
        {
            DetectWeapon();
        }
    }

    private void DetectWeapon()
    {
        // Use the Grandparent class 'Weapon' to find any inherited script (Gun, Pistol, etc.)
        Weapon foundWeapon = GetComponentInChildren<Weapon>();

        // Guard Clause: If nothing is found, stop here
        if (foundWeapon == null)
        {
            // Debug.LogWarning("[InputHandler] No weapon found in children.");
            return;
        }

        // Assignment logic
        equippedWeapon = foundWeapon;
        Debug.Log($"[InputHandler] Weapon Detected: {equippedWeapon.GetType().Name} on {equippedWeapon.gameObject.name}");
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}