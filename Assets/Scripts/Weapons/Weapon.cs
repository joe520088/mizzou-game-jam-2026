using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform weaponTransform;
    public float offset;
    public float damage = 10f;
    public float attackCooldown = 0.5f;

    protected float lastAttackTime;

    void Update()
    {
        RotateTowardMouse();

        if (Input.GetMouseButtonDown(0))
            TryAttack();
    }

    void RotateTowardMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 displacement = weaponTransform.position - worldMouse;
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(0f, 0f, angle + offset);
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    // Virtual means child classes can override this
    protected virtual void Attack()
    {
        Debug.Log("Base weapon attack!");
    }
}