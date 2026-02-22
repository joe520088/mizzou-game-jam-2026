using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform weaponTransform;
    public bool isEquipped = false;
    public float offset;
    public float damage = 10f;
    public float attackCooldown = 0.5f;
    public Hand hand;

    protected float lastAttackTime;
    protected InputHandler inputHandler;

    protected virtual void Start()
    {
        // Try parent first, if not found search the whole scene
        inputHandler = GetComponentInParent<InputHandler>();

        if (inputHandler == null)
            inputHandler = FindObjectOfType<InputHandler>();

        if (hand == null) hand = GetComponentInChildren<Hand>();
    }

    protected virtual void Update()
    {
        if (isEquipped)
        {
            RotateTowardMouse();
        }
    }

    void RotateTowardMouse()
    {
        if (inputHandler == null) return;

        Vector3 mousePos = inputHandler.MousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 displacement = worldMouse - weaponTransform.position;
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(0f, 0f, (angle + offset));

        Vector3 scale = Vector3.one;

        if (angle > 90 || angle < -90)
        {
            scale.y = -1f;
            scale.x = -1f;
        }
        else
        {
            scale.y = 1f;
            scale.x = 1f;
        }

        weaponTransform.localScale = scale;
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        Debug.Log("Base weapon attack!");
    }
}