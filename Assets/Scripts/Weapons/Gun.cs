using UnityEngine;

public class Gun : Weapon
{
    public Transform shotPoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    private AudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        Debug.Log($"Gun started - inputHandler={inputHandler}, shotPoint={shotPoint}, prefab={projectilePrefab}");
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void SpawnBullet(string soundName)
    {
        // 1. Check for essential prefab/transform references
        if (projectilePrefab == null)
        {
            Debug.LogError($"[Gun] SpawnBullet failed: 'projectilePrefab' is null on {gameObject.name}.");
            return;
        }
        if (shotPoint == null)
        {
            Debug.LogError($"[Gun] SpawnBullet failed: 'shotPoint' (Transform) is null on yo mama.");
            return;
        }

        // 2. Calculate Direction and Rotation
        if (Camera.main == null)
        {
            Debug.LogError("[Gun] SpawnBullet: No Main Camera found in the scene! Cannot calculate mouse position.");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - shotPoint.position).normalized;

        if (direction == Vector2.zero)
        {
            Debug.LogWarning($"[Gun] Shot direction is Zero. Mouse and ShotPoint are at the same position: {mousePos}");
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shotPoint.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Instantiate Projectile
        GameObject bullet = Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation);
        Debug.Log($"[Gun] Successfully instantiated {bullet.name} at {shotPoint.position}.");

        // 4. Audio Management
        if (!string.IsNullOrEmpty(soundName))
        {
            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            if (audioManager != null)
            {
               // audioManager.Play(soundName);
            }
            else
            {
                Debug.LogWarning($"[Gun] Sound '{soundName}' requested, but AudioManager was not found in scene.");
            }
        }

        // 5. Physics Application
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            Debug.Log($"[Gun] Applied velocity {rb.linearVelocity} to {bullet.name}.");
        }
        else
        {
            Debug.LogError($"[Gun] CRITICAL: Prefab '{projectilePrefab.name}' is missing a Rigidbody2D component! Velocity cannot be applied.");
        }
    }

    protected override void Attack()
    {
        SpawnBullet("Rifle");
    }
}