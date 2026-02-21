using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    private PlayerControls controls;
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public event Action OnAttackPerformed;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribing to the Move action
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();  

        controls.Player.Attack.performed += ctx => OnAttackPerformed?.Invoke();
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