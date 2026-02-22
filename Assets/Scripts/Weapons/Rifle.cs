using UnityEngine;

public class Rifle : Gun
{
    public int magazineSize = 30;
    public float reloadTime = 2.5f;
    private int currentAmmo;
    private bool isReloading = false;

    protected override void Start()
    {
        base.Start();
        currentAmmo = magazineSize;
    }

    protected override void Attack()
    {
        if (isReloading) { Debug.Log("Reloading..."); return; }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        Debug.Log($"Rifle fired! Ammo: {currentAmmo}/{magazineSize}");
        SpawnBullet("Rifle");
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading rifle...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}