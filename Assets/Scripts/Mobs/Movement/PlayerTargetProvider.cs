using UnityEngine;

namespace Game.Mobs.Targeting
{
    [DisallowMultipleComponent]
    public class PlayerTargetProvider : MonoBehaviour
    {
        [Header("Find Strategy")]
        public bool preferTag = true;
        public string playerTag = "Player";

        private Transform cached;

        public Transform Get()
        {
            if (cached != null) return cached;

            // 1) Prefer tag (fastest/cleanest for teams)
            if (preferTag)
            {
                var go = GameObject.FindGameObjectWithTag(playerTag);
                if (go != null) { cached = go.transform; return cached; }
            }

            // 2) Fallback: find PlayerController in scene
            var pc = Object.FindFirstObjectByType<PlayerController>();
            if (pc != null) { cached = pc.transform; return cached; }

            return null;
        }

        public void ClearCache() => cached = null;
    }
}