using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    private PlayerControls controls;
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public bool IsAttackHeld => controls.Player.Attack.IsPressed();
    public event Action OnAttackPerformed;
    private Weapon equippedWeapon;
    public GameObject starterWeaponPrefab;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribing to the Move action
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();  
    }

    private void Start()
    {
        DetectWeapon();
    }

    private void Update()
    {
        DetectWeapon();
    }

    private void DetectWeapon()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        
        foreach (Weapon w in weapons)
        {
            if (equippedWeapon == null || w.GetType() != typeof(Weapon) && w.GetType() != typeof(Gun))
                equippedWeapon = w;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}