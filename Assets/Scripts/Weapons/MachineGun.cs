using UnityEngine;

public class AutoGun : Gun
{
    public float fireRate = 0.1f;
    private float nextFireTime = 0f;

    protected override void Update()
    {
        base.Update();

        if (inputHandler.IsAttackHeld && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            SpawnBullet("MachineGun");
        }
    }
}