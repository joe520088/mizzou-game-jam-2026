using UnityEngine;
using UnityEngine.InputSystem; // NEW input system
using Game.Mobs;
using Game.Entanglement;


public class PlayerEntangleInteractor : MonoBehaviour
{
    [Header("Debug")]
    public bool debugLogs = true;

    [Header("Input")]
    public Key entangleKey = Key.E;   // NEW input system key enum

    [Header("Detection")]
    public float entangleRange = 2.0f;
    public LayerMask mobLayer;

    [Header("Cooldown")]
    public float cooldownSeconds = 0.25f;

    private PlayerController player;
    private float nextAllowedTime;

    private void Start()
    {
        Debug.Log("[Entangle] Start fired", this);
    }
    private void Awake()
    {
        player = GetComponent<PlayerController>();
        if (debugLogs)
            Debug.Log($"[Entangle] Awake on {name}. PlayerController found? {player != null}", this);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[Entangle] E pressed detected", this);
        }
        
        if (player == null) return;

        // Key press check (NEW input system)
        if (Keyboard.current == null)
        {
            if (debugLogs) Debug.LogWarning("[Entangle] Keyboard.current is null (no keyboard?)");
            return;
        }

        if (!Keyboard.current[entangleKey].wasPressedThisFrame) return;

        if (debugLogs) Debug.Log("[Entangle] Key pressed", this);

        if (Time.time < nextAllowedTime)
        {
            if (debugLogs) Debug.Log("[Entangle] On cooldown", this);
            return;
        }

        var entangle = FindClosestEntangleable();
        if (entangle == null)
        {
            if (debugLogs) Debug.Log("[Entangle] No MobEntangle found in range/layer", this);
            return;
        }

        EntanglementEffect effect = entangle.GetEntanglementEffect();

        // Apply FRACTIONS: 0.08 = +8%
        player.Stats.AddHealthPercent(effect.healthPct);
        player.Stats.AddSpeedPercent(effect.speedPct);
        player.Stats.AddAttackSpeedPercent(effect.attackSpeedPct);
        player.Stats.AddStrengthPercent(effect.strengthPct);

        nextAllowedTime = Time.time + cooldownSeconds;

        Debug.Log($"[Entangle] SUCCESS with {entangle.Type} -> " +
                  $"HP {effect.healthPct:+0.###;-0.###;0}, " +
                  $"SPD {effect.speedPct:+0.###;-0.###;0}, " +
                  $"AS {effect.attackSpeedPct:+0.###;-0.###;0}, " +
                  $"STR {effect.strengthPct:+0.###;-0.###;0}", this);
    }

    private MobEntangle FindClosestEntangleable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, entangleRange, mobLayer);

        if (debugLogs) Debug.Log($"[Entangle] Overlap hits: {(hits == null ? 0 : hits.Length)}", this);

        if (hits == null || hits.Length == 0) return null;

        MobEntangle best = null;
        float bestDist = float.PositiveInfinity;

        foreach (var h in hits)
        {
            var ent = h.GetComponentInParent<MobEntangle>();
            if (ent == null) continue;

            float d = Vector2.Distance(transform.position, ent.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = ent;
            }
        }

        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, entangleRange);
    }
}