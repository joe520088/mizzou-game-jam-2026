using UnityEngine;
using System.Collections.Generic;
using Game.Entanglement;

namespace Game.Mobs
{
    [DisallowMultipleComponent]
    public class MobEntangle : MonoBehaviour
    {
        [Header("Type (auto if MobController exists)")]
        public MobType fallbackType = MobType.Medium;

        [Header("Buff/debuff range (fractions)")]
        [Range(0.05f, 0.08f)] public float minPct = 0.05f;
        [Range(0.05f, 0.08f)] public float maxPct = 0.08f;

        [Header("Variant overlays (can stack multiple)")]
        public List<MobVariant> variants = new List<MobVariant> { MobVariant.Normal };

        [Header("Roll behavior")]
        public bool lockEffectAfterFirstRoll = true; // per-mob identity
        public bool rerollEachEntangle = false;       // if true, ignores lock above

        private MobController mobController;
        private bool rolled;
        private EntanglementEffect cached;

        public MobType Type => mobController != null ? mobController.Type : fallbackType;

        private void Awake()
        {
            mobController = GetComponent<MobController>(); // ok if missing
        }

        public EntanglementEffect GetEntanglementEffect()
        {
            if (rerollEachEntangle)
            {
                // Force re-roll every time
                rolled = false;
            }

            if (lockEffectAfterFirstRoll && rolled)
                return cached;

            float buff = Random.Range(minPct, maxPct);      // +0.05..+0.08
            float debuff = -Random.Range(minPct, maxPct);   // -0.05..-0.08

            // 1) Base effect from MobType
            EntanglementEffect e = BuildBaseEffect(buff, debuff);

            // 2) Apply each variant overlay (layered)
            if (variants != null)
            {
                for (int i = 0; i < variants.Count; i++)
                    e = ApplyVariantOverlay(e, variants[i], buff, debuff);
            }

            // 3) Cache if desired
            if (lockEffectAfterFirstRoll)
            {
                cached = e;
                rolled = true;
            }

            return e;
        }

        private EntanglementEffect BuildBaseEffect(float buff, float debuff)
        {
            switch (Type)
            {
                case MobType.Heavy:
                    // Heavy: +Strength, -Speed
                    return new EntanglementEffect(healthPct: 0f, speedPct: debuff, attackSpeedPct: 0f, strengthPct: buff);

                case MobType.Light:
                    // Light: +Speed, -Strength
                    return new EntanglementEffect(healthPct: 0f, speedPct: buff, attackSpeedPct: 0f, strengthPct: debuff);

                case MobType.Medium:
                default:
                    // Medium: +AttackSpeed, -Speed (milder)
                    return new EntanglementEffect(healthPct: 0f, speedPct: debuff * 0.5f, attackSpeedPct: buff * 0.75f, strengthPct: 0f);
            }
        }

        private EntanglementEffect ApplyVariantOverlay(EntanglementEffect e, MobVariant v, float buff, float debuff)
        {
            // buff is +0.05..+0.08
            // debuff is -0.05..-0.08

            switch (v)
            {
                case MobVariant.Normal:
                    return e;

                case MobVariant.Berserk:
                    // extra STR, lose HP
                    e.strengthPct += buff * 0.75f;
                    e.healthPct += debuff * 0.75f;
                    return e;

                case MobVariant.Phase:
                    // extra speed, slower attacks
                    e.speedPct += buff * 0.75f;
                    e.attackSpeedPct += debuff * 0.5f;
                    return e;

                case MobVariant.Leech:
                    // extra health, less strength
                    e.healthPct += buff * 0.75f;
                    e.strengthPct += debuff * 0.5f;
                    return e;

                case MobVariant.Chrono:
                    // faster attacks, slower movement
                    e.attackSpeedPct += buff * 0.75f;
                    e.speedPct += debuff * 0.5f;
                    return e;

                case MobVariant.GlassCannon:
                    // big STR up, big HP down
                    e.strengthPct += buff;
                    e.healthPct += debuff;
                    return e;

                case MobVariant.Tank:
                    // big HP up, slower movement
                    e.healthPct += buff;
                    e.speedPct += debuff;
                    return e;

                case MobVariant.Trickster:
                    // small random wobble on speed/strength
                    if (Random.value < 0.5f) e.speedPct += (Random.value < 0.5f ? buff * 0.5f : debuff * 0.5f);
                    else e.strengthPct += (Random.value < 0.5f ? buff * 0.5f : debuff * 0.5f);
                    return e;

                case MobVariant.Resonant:
                    // amplifies the BASE tradeoff of the mob type (feels "more itself")
                    // heavy: even slower + stronger, light: even faster + weaker, medium: faster attacks + slightly slower
                    switch (Type)
                    {
                        case MobType.Heavy:
                            e.strengthPct += buff * 0.5f;
                            e.speedPct += debuff * 0.5f;
                            break;
                        case MobType.Light:
                            e.speedPct += buff * 0.5f;
                            e.strengthPct += debuff * 0.5f;
                            break;
                        default:
                            e.attackSpeedPct += buff * 0.5f;
                            e.speedPct += debuff * 0.25f;
                            break;
                    }
                    return e;

                case MobVariant.Unstable:
                    // adds small random noise to all stats (could be +/-)
                    e.healthPct += Random.Range(-0.02f, 0.02f);
                    e.speedPct += Random.Range(-0.02f, 0.02f);
                    e.attackSpeedPct += Random.Range(-0.02f, 0.02f);
                    e.strengthPct += Random.Range(-0.02f, 0.02f);
                    return e;

                case MobVariant.Adaptive:
                    // simple: add buff to the "least affected" stat right now (closest to 0)
                    float absH = Mathf.Abs(e.healthPct);
                    float absS = Mathf.Abs(e.speedPct);
                    float absAS = Mathf.Abs(e.attackSpeedPct);
                    float absSTR = Mathf.Abs(e.strengthPct);

                    if (absH <= absS && absH <= absAS && absH <= absSTR) e.healthPct += buff * 0.5f;
                    else if (absS <= absAS && absS <= absSTR) e.speedPct += buff * 0.5f;
                    else if (absAS <= absSTR) e.attackSpeedPct += buff * 0.5f;
                    else e.strengthPct += buff * 0.5f;

                    // pay a small cost
                    e.speedPct += debuff * 0.25f;
                    return e;

                default:
                    return e;
            }
        }
    }
}