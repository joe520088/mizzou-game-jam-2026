using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private WeaponInterface currentWeapon;
    private InputHandler input;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<InputHandler>();
        currentWeapon = GetComponent<WeaponInterface>();

        input.OnAttackPerformed += HandleAttack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void equipWeapon(WeaponInterface newWeapon)
    {
        currentWeapon = newWeapon;
    }

    void HandleAttack()
    {
        Debug.Log("Attack");
        currentWeapon?.Attack();
    }
}
