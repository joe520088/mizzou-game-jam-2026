namespace Game.Entanglement
{
    // Fractions: +0.08 = +8%, -0.06 = -6%
    [System.Serializable]
    public struct EntanglementEffect
    {
        public float healthPct;
        public float speedPct;
        public float attackSpeedPct;
        public float strengthPct;

        public static EntanglementEffect Zero => new EntanglementEffect(0f, 0f, 0f, 0f);

        public EntanglementEffect(float healthPct, float speedPct, float attackSpeedPct, float strengthPct)
        {
            this.healthPct = healthPct;
            this.speedPct = speedPct;
            this.attackSpeedPct = attackSpeedPct;
            this.strengthPct = strengthPct;
        }
    }
}