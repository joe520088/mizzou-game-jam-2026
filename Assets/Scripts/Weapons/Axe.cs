using UnityEngine;

public class Axe : Weapon
{
    private SpriteRenderer sr;
    private Transform playerTransform;
    private AxeSwingAttack swingAttack;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        swingAttack = GetComponent<AxeSwingAttack>();
    }

    protected override void Update()
    {
        // AxeSwingAttack handles all positioning now
        HandleSortingOrder();
    }

    void HandleSortingOrder()
    {
        if (sr == null || playerTransform == null || inputHandler == null) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        sr.sortingOrder = mousePos.x >= playerTransform.position.x ? 2 : 0;
    }

    protected override void Attack() { }
}