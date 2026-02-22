using UnityEngine;

public class AutoGun : Gun
{
    public float fireRate = 0.1f;
    private float timer = 0f;

    protected override void Update()
    {
        if (isEquipped == false)
        {
            return;
        }

        base.Update();
        timer += Time.deltaTime;

        if(inputHandler.IsAttackHeld && timer >= fireRate)
        {
            Debug.Log("TRIGGERED BULLET SPAWN");

            SpawnBullet("MachineGun");
            timer = 0f;
        }
    }
}