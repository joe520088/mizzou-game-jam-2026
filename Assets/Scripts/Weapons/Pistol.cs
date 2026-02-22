using UnityEngine;

public class Pistol : Gun
{
    public int magazineSize = 12;
    public float reloadTime = 1.5f;
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
        Debug.Log($"Pistol fired! Ammo: {currentAmmo}/{magazineSize}");
        SpawnBullet("Pistol");
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading pistol...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}