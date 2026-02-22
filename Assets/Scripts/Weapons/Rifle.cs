using UnityEngine;
using System.Collections;

public class Rifle : Gun
{
    public int magazineSize = 30;
    public float reloadTime = 2.5f;
    private int currentAmmo;
    private bool isReloading = false;

    // Keep a reference to the coroutine to prevent "Double Reloading"
    private Coroutine reloadCoroutine;

    protected override void Start()
    {
        base.Start();
        currentAmmo = magazineSize;

        // Rigorous Check: Ensure inputHandler exists before subscribing
        if (inputHandler != null)
        {
            inputHandler.OnAttackPerformed += HandleAttackInput;
        }
        else
        {
            Debug.LogError($"[Rifle] {gameObject.name} is missing an InputHandler reference!");
        }
    }

    // Use a separate method for the event to make it cleaner
    private void HandleAttackInput()
    {
        Attack();
    }

    protected override void Attack()
    {
        if (isEquipped == false)
        {
            return;
        }

        // Guard Clause 1: Already reloading
        if (isReloading)
        {
            Debug.Log("[Rifle] Action blocked: Currently reloading.");
            return;
        }

        // Guard Clause 2: Out of ammo
        if (currentAmmo <= 0)
        {
            Debug.Log("[Rifle] Out of ammo! Starting reload...");
            // Start reload and exit
            StartReload();
            return;
        }

        // Logic: Fire weapon
        currentAmmo--;
        Debug.Log($"[Rifle] Fired! Ammo: {currentAmmo}/{magazineSize}");

        // Pass the sound name to the parent SpawnBullet method
        SpawnBullet("Rifle");
    }

    public void StartReload()
    {
        // Guard Clause: Don't start a second reload if one is in progress
        if (isReloading) return;

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("[Rifle] Reloading started...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        reloadCoroutine = null;
        Debug.Log("[Rifle] Reload complete. Ammo reset.");
    }

    // Important: Clean up events when object is destroyed to prevent memory leaks/ghost shots
    private void OnDestroy()
    {
        if (inputHandler != null)
        {
            inputHandler.OnAttackPerformed -= HandleAttackInput;
        }
    }
}