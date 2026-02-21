using UnityEngine;
using Game.Mobs.Movement;

namespace Game.Mobs
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MobController))]
    public class MobBrain : MonoBehaviour
    {
        private MobController mob;
        private WanderMovement wander;
        private ChaseMovement chase;

        private enum State { Wander, Chase }
        [SerializeField] private State state = State.Wander;

        private void Awake()
        {
            mob = GetComponent<MobController>();
            wander = GetComponent<WanderMovement>();
            chase = GetComponent<ChaseMovement>();

            if (wander == null) Debug.LogError($"{name}: Missing WanderMovement.", this);
            if (chase == null) Debug.LogError($"{name}: Missing ChaseMovement.", this);
        }

        private void Update()
        {
            // Transition logic
            if (chase != null && chase.TargetInRange(out _))
                state = State.Chase;
            else
                state = State.Wander;

            // Animator hooks (optional)
            if (mob.Animator != null)
            {
                mob.Animator.SetBool("IsChasing", state == State.Chase);
                mob.Animator.SetInteger("MobType", (int)mob.Type);
                mob.Animator.SetFloat("MoveSpeed", mob.RB.linearVelocity.magnitude);
            }
        }

        private void FixedUpdate()
        {
            Vector2 vel = Vector2.zero;

            switch (state)
            {
                case State.Chase:
                    vel = chase != null ? chase.GetDesiredVelocity() : Vector2.zero;
                    break;

                case State.Wander:
                default:
                    vel = wander != null ? wander.GetDesiredVelocity() : Vector2.zero;
                    break;
            }

            mob.SetVelocity(vel);
        }

        private void OnDrawGizmosSelected()
        {
            if (mob == null || mob.Stats == null) return;
            Gizmos.DrawWireSphere(transform.position, mob.Stats.detectionRadius);
        }
    }
}