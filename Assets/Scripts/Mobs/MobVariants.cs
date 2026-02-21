namespace Game.Mobs
{
    public enum MobVariant
    {
        // Default
        Normal = 0,

        // "Quantum / special" modifiers
        Berserk,        // more STR, less HP
        Phase,          // more SPD, less AttackSpeed
        Leech,          // more HP, less STR
        Chrono,         // more AttackSpeed, less SPD
        GlassCannon,    // big STR up, big HP down
        Tank,           // big HP up, SPD down
        Trickster,      // small random wobble
        Resonant,       // amplifies the base mob-type tradeoff
        Unstable,       // adds extra random +/- noise to all stats
        Adaptive        // buff the stat you are currently lowest in (simple rule)
    }
}