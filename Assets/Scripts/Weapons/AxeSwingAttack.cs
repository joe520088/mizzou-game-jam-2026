using UnityEngine;
using System.Collections.Generic;

public class AxeSwingAttack : MonoBehaviour
{
    public float orbitRadius = 0.6f;
    public float swingAngle = 90f;
    public float swingSpeed = 300f;
    public float axeRange = 0.4f;
    public float restingAngleRight = 90f;
    public float restingAngleLeft = 90f;

    private Transform playerTransform;
    private InputHandler inputHandler;
    private SpriteRenderer sr;

    private bool isSwinging = false;
    private bool isReturning = false;

    private float swingStartAngle;
    private float swingTargetAngle;
    private float swingDirection;
    private float currentAngle;

    public bool IsSwinging => isSwinging;
    public bool IsReturning => isReturning;

    private HashSet<Collider2D> hitThisSwing = new HashSet<Collider2D>();

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();

        inputHandler = FindFirstObjectByType<InputHandler>();
        if (inputHandler != null)
            inputHandler.OnAttackPerformed += StartSwing;

        currentAngle = restingAngleRight;
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
            inputHandler.OnAttackPerformed -= StartSwing;
    }

    private void Update()
    {
        Debug.Log($"isSwinging: {isSwinging}, isReturning: {isReturning}, currentAngle: {currentAngle}, pos: {transform.position}");

        if (isSwinging)
            ContinueSwing();
        else if (isReturning)
            ContinueReturn();
        else
            SnapToRestingPosition();
    }

    void SnapToRestingPosition()
    {
        if (playerTransform == null || inputHandler == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        bool facingRight = mousePos.x >= playerTransform.position.x;

        currentAngle = facingRight ? restingAngleRight : restingAngleLeft;
        PlaceAxeAtAngle(currentAngle);
    }

    public void StartSwing()
    {
        if (isSwinging || isReturning) return;

        hitThisSwing.Clear();

        // Lock direction at moment of click - mouse is never read again after this
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        bool facingRight = mousePos.x >= playerTransform.position.x;

        swingDirection = facingRight ? 1f : -1f;
        swingStartAngle = facingRight ? restingAngleRight : restingAngleLeft;
        swingTargetAngle = swingStartAngle + swingAngle * swingDirection;

        currentAngle = swingStartAngle;
        isSwinging = true;
    }

    void ContinueSwing()
    {
        currentAngle += swingSpeed * swingDirection * Time.deltaTime;

        // Keep axe glued to player position during swing
        PlaceAxeAtAngle(currentAngle);
        CheckDamage();

        bool done = swingDirection > 0
            ? currentAngle >= swingTargetAngle
            : currentAngle <= swingTargetAngle;

        if (done)
        {
            currentAngle = swingTargetAngle;
            isSwinging = false;
            isReturning = true;
        }
    }

    void ContinueReturn()
    {
        currentAngle -= swingSpeed * 1.5f * swingDirection * Time.deltaTime;

        // Keep axe glued to player position during return
        PlaceAxeAtAngle(currentAngle);

        bool done = swingDirection > 0
            ? currentAngle <= swingStartAngle
            : currentAngle >= swingStartAngle;

        if (done)
        {
            currentAngle = swingStartAngle;
            isReturning = false;
        }
    }

    void PlaceAxeAtAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        transform.position = playerTransform.position +
            new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * orbitRadius;

        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        sr.flipX = (angle % 360 + 360) % 360 > 180f;
        sr.flipY = false;
    }


    void CheckDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, axeRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && !hitThisSwing.Contains(hit))
            {
                hitThisSwing.Add(hit);
                Debug.Log("Axe hit: " + hit.name);
                // hit.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            }
        }
    }
}