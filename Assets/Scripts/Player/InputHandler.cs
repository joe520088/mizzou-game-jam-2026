using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    private PlayerControls controls;
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
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

        controls.Player.Attack.performed += ctx => OnAttackPerformed?.Invoke();
        controls.Player.Attack.performed += ctx => equippedWeapon?.TryAttack();
    }

    private void Start()
    {
        equippedWeapon = GetComponentInChildren<Weapon>();

        // If no weapon found, spawn the starter a
        // if (equippedWeapon == null)
        // {
        //     Debug.Log("No weapon found, spawning starter axe!");
        //     // You'll drag your Axe prefab into this slot in the Inspector
        //     equippedWeapon = Instantiate(starterWeaponPrefab, transform.position, Quaternion.identity)
        //                      .GetComponent<Weapon>();
        //     equippedWeapon.transform.SetParent(transform);
        // }

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