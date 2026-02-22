using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Hand hand;
    private InputHandler inputHandler;
    private PlayerControls controls;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        hand = GetComponent<Hand>();
        inputHandler = GetComponent<InputHandler>();
        
        controls = new PlayerControls();

        controls.Player.EquipSlot1.performed += ctx => hand.Equip(1);
        controls.Player.EquipSlot2.performed += ctx => hand.Equip(2);
        controls.Player.EquipSlot3.performed += ctx => hand.Equip(3);
        controls.Player.EquipSlot4.performed += ctx => hand.Equip(4);
    }

    // Update is called once per frame
    void Update()
    {
        
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
