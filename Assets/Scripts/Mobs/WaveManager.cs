using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public enum WaveState { Intermission, Spawning, Fighting }
    [Header("Runtime (Read Only)")]
    [SerializeField] private WaveState state = WaveState.Intermission;

    [Header("Wave")]
    [Tooltip("Starts at 0. Wave 1 begins after the initial intermission.")]
    [SerializeField] private int currentRound = 0;

    [Header("Intermission")]
    [Tooltip("Wait this long AFTER the wave is fully cleared (all spawned + all dead).")]
    public float intermissionAfterClear = 15f;

    [Header("Spawning")]
    [Tooltip("Spawn points placed around the scene.")]
    public Transform[] spawnPoints;

    [Header("Mob Prefabs (explicit, order-independent)")]
    [Tooltip("Common early mob.")]
    public GameObject lightPrefab;

    [Tooltip("Medium mob (appears more as rounds increase).")]
    public GameObject mediumPrefab;

    [Tooltip("Heavy mob (rare early, common later).")]
    public GameObject heavyPrefab;

    [Tooltip("Time between spawn attempts (zombies trickle).")]
    public float spawnInterval = 0.5f;

    [Tooltip("Scaling rules for mob runtime stats per round.")]
    public MobWaveStatsSO waveScaling;

    [Header("Count Scaling")]
    [Tooltip("Total enemies spawned in wave 1.")]
    public int baseEnemyCount = 10;

    [Tooltip("Additional enemies added each wave.")]
    public int enemyCountGrowth = 3;

    [Header("Zombies Style")]
    [Tooltip("Max number of enemies allowed alive at once. When you kill one, another can spawn.")]
    public int maxAliveAtOnce = 6;

    // Tracking
    private int enemiesAlive = 0;
    private int remainingToSpawn = 0;
    private bool isSpawning = false;
    private Coroutine waveLoopRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        waveLoopRoutine = StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            // Intermission before starting the next wave (also happens before Wave 1).
            state = WaveState.Intermission;
            if (intermissionAfterClear > 0f)
                yield return new WaitForSeconds(intermissionAfterClear);

            // Start next wave (sets remainingToSpawn, then trickles spawns under cap).
            StartNextWave();

            // Fight until the wave is fully complete:
            // - nothing left to spawn
            // - no enemies alive
            state = WaveState.Fighting;
            while (remainingToSpawn > 0 || enemiesAlive > 0 || isSpawning)
                yield return null;

            // loop repeats -> intermission -> next wave
        }
    }

    private void StartNextWave()
    {
        // Validation
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] No spawnPoints assigned.");
            return;
        }

        if (lightPrefab == null && mediumPrefab == null && heavyPrefab == null)
        {
            Debug.LogError("[WaveManager] No mob prefabs assigned (light/medium/heavy are all null).");
            return;
        }

        if (waveScaling == null)
        {
            Debug.LogError("[WaveManager] No waveScaling assigned.");
            return;
        }

        currentRound++;

        int totalToSpawn = baseEnemyCount + (currentRound - 1) * enemyCountGrowth;
        if (totalToSpawn < 0) totalToSpawn = 0;

        remainingToSpawn = totalToSpawn;

        // Start the trickle spawner
        if (!isSpawning)
            StartCoroutine(TrickleSpawnRoutine(currentRound));
    }

    private IEnumerator TrickleSpawnRoutine(int roundForThisWave)
    {
        state = WaveState.Spawning;
        isSpawning = true;

        // Keep trying to spawn until we've spawned the whole wave quota
        while (remainingToSpawn > 0)
        {
            // Only spawn if we're under the alive cap
            if (enemiesAlive < maxAliveAtOnce)
            {
                SpawnMob(roundForThisWave);
                remainingToSpawn--;
            }

            if (spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
            else
                yield return null;
        }

        isSpawning = false;
        state = WaveState.Fighting;
    }

    private void SpawnMob(int round)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] SpawnMob called but spawnPoints is empty.");
            return;
        }

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = ChooseMobPrefab(round);

        if (prefab == null)
        {
            Debug.LogError($"[WaveManager] ChooseMobPrefab returned null (round {round}). Check prefabs.");
            return;
        }

        GameObject obj = Instantiate(prefab, sp.position, sp.rotation);

        // Look on children too (common prefab hierarchy)
        global::Mob mob = obj.GetComponentInChildren<global::Mob>();

        // If the prefab isn't configured right, destroy it and DO NOT count it
        if (mob == null)
        {
            Debug.LogError($"[WaveManager] Prefab '{prefab.name}' has NO Mob component (on root or children). Fix prefab.");
            Destroy(obj);
            return;
        }

        if (mob.baseStats == null)
        {
            Debug.LogError($"[WaveManager] Mob on prefab '{prefab.name}' has NO baseStats assigned. Fix prefab.");
            Destroy(obj);
            return;
        }

        // Only now do we count it as alive
        enemiesAlive++;

        var runtime = MobRuntimeStatBuilder.Build(mob.baseStats, waveScaling, round);
        mob.Init(runtime);
    }

    /// <summary>
    /// Zombies-style ramp:
    /// - Rounds 1-3: mostly Light, small chance Medium
    /// - Rounds 4-7: Light + Medium, small chance Heavy
    /// - Round 8+: all three, Heavy becomes more common
    /// This is order-independent because it uses explicit prefab fields.
    /// </summary>
    private GameObject ChooseMobPrefab(int round)
    {
        // Build a safe fallback chain in case some prefabs are missing
        GameObject fallback = lightPrefab != null ? lightPrefab :
                              (mediumPrefab != null ? mediumPrefab : heavyPrefab);

        if (fallback == null)
            return null;

        float r = Random.value;

        // Rounds 1-3
        if (round < 4)
        {
            if (mediumPrefab != null && r < 0.15f) return mediumPrefab;
            return fallback;
        }
        // Rounds 4-7
        else if (round < 8)
        {
            if (heavyPrefab != null && r < 0.10f) return heavyPrefab;
            if (mediumPrefab != null && r < 0.50f) return mediumPrefab;
            return fallback;
        }
        // Round 8+
        else
        {
            if (heavyPrefab != null && r < 0.35f) return heavyPrefab;
            if (mediumPrefab != null && r < 0.75f) return mediumPrefab;
            return fallback;
        }
    }

    // Call exactly once when a mob dies
    public void NotifyMobDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }

    // Optional getters for UI/debug
    public WaveState GetState() => state;
    public int GetEnemiesAlive() => enemiesAlive;
    public int GetRemainingToSpawn() => remainingToSpawn;
    public int GetCurrentRound() => currentRound;
    public bool IsSpawning() => isSpawning;
}