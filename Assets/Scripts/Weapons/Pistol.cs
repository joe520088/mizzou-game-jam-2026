using UnityEngine;
using System.Collections;

public class Pistol : Gun
{
    [Header("Pistol Settings")]
    public int magazineSize = 12;
    public float reloadTime = 1.5f;

    private int currentAmmo;
    private bool isReloading = false;

    // Track the coroutine to prevent multiple reloads overlapping
    private Coroutine reloadRoutine;

    protected override void Start()
    {
        base.Start();
        currentAmmo = magazineSize;

        // Subscribing to the attack event from InputHandler
        if (inputHandler != null)
        {
            inputHandler.OnAttackPerformed += HandleAttack;
        }
        else
        {
            Debug.LogError($"[Pistol] InputHandler not found on {gameObject.name}!");
        }
    }

    private void HandleAttack()
    {
        Attack();
    }

    protected override void Attack()
    {
        if (isEquipped == false)
        {
            return;
        }

        // 1. Guard: Check if already reloading
        if (isReloading)
        {
            Debug.Log("[Pistol] Cannot fire: Reloading in progress.");
            return;
        }

        // 2. Guard: Check if out of ammo
        if (currentAmmo <= 0)
        {
            Debug.Log("[Pistol] Magazine empty! Starting auto-reload.");
            StartManualReload();
            return;
        }

        // 3. Logic: Fire Bullet
        currentAmmo--;
        Debug.Log($"[Pistol] Shot fired! {currentAmmo}/{magazineSize} remaining.");

        // Use the base Gun's method to handle physics and sound
        SpawnBullet("Pistol_Fire");
    }

    public void StartManualReload()
    {
        // Prevent starting a reload if one is already running
        if (isReloading) return;

        // Reset and start the reload process
        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
        reloadRoutine = StartCoroutine(ReloadLogic());
    }

    private IEnumerator ReloadLogic()
    {
        isReloading = true;
        Debug.Log($"[Pistol] Reloading... ({reloadTime}s)");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        reloadRoutine = null;
        Debug.Log("[Pistol] Reload complete. Magazine full.");
    }

    // CRITICAL: Prevent memory leaks and "ghost shots" when swapping weapons
    private void OnDestroy()
    {
        if (inputHandler != null)
        {
            inputHandler.OnAttackPerformed -= HandleAttack;
        }
    }

    // Safety check for UI or other scripts to see ammo
    public int GetCurrentAmmo() => currentAmmo;
    public bool IsReloading() => isReloading;
}