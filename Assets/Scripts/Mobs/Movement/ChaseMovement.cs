using UnityEngine;
using Game.Mobs.Targeting;

namespace Game.Mobs.Movement
{
    [DisallowMultipleComponent]
    public class ChaseMovement : MonoBehaviour, IMovementStrategy
    {
        private MobController mob;
        private PlayerTargetProvider targetProvider;

        private void Awake()
        {
            mob = GetComponent<MobController>();
            targetProvider = GetComponent<PlayerTargetProvider>();
            if (targetProvider == null)
                targetProvider = gameObject.AddComponent<PlayerTargetProvider>(); // auto-add for convenience
        }

        public bool TargetInRange(out Transform target)
        {
            target = targetProvider.Get();
            if (target == null) return false;

            float dist = Vector2.Distance(mob.transform.position, target.position);
            return dist <= mob.Stats.detectionRadius;
        }

        public Vector2 GetDesiredVelocity()
        {
            if (!TargetInRange(out Transform target) || target == null)
                return Vector2.zero;

            Vector2 dir = ((Vector2)target.position - mob.RB.position).normalized;
            return dir * mob.Stats.moveSpeed;
        }
    }
}