using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Serializable]
    public class SpawnRule
    {
        public string name;
        public GameObject prefab;
        [Tooltip("Fixed world position. Randomized axes use their ranges instead.")]
        public Vector3 position = new Vector3(0f, 10f, 0f);
        public bool randomizeX;
        [Tooltip("Minimum and maximum world X coordinates.")]
        public Vector2 xRange = new Vector2(-5f, 5f);
        public bool randomizeY;
        [Tooltip("Minimum and maximum world Y coordinates.")]
        public Vector2 yRange = new Vector2(8f, 12f);
        [Min(0f)] public float firstSpawnTime = 3f;
        public bool repeat;
        [Min(0.1f)] public float interval = 10f;

        [NonSerialized] internal float nextSpawnTime;
        [NonSerialized] internal bool finished;
    }

    [SerializeField] private bool startAutomatically = true;
    [SerializeField] private Transform spawnedItemsParent;
    [SerializeField] private SpawnRule[] rules = new SpawnRule[0];

    public bool IsRunning { get; private set; }
    public float ElapsedTime { get; private set; }

    private void Start()
    {
        if (startAutomatically) StartSpawning();
    }

    private void Update()
    {
        if (!IsRunning) return;
        ElapsedTime += Time.deltaTime;

        foreach (SpawnRule rule in rules)
        {
            if (rule == null || rule.finished || ElapsedTime < rule.nextSpawnTime) continue;
            Spawn(rule);
            if (rule.repeat)
            {
                float interval = Mathf.Max(0.1f, rule.interval);
                float missedTicks = Mathf.Floor((ElapsedTime - rule.nextSpawnTime) / interval) + 1f;
                rule.nextSpawnTime += missedTicks * interval;
            }
            else rule.finished = true;
        }
    }

    public void StartSpawning()
    {
        ElapsedTime = 0f;
        foreach (SpawnRule rule in rules)
        {
            if (rule == null) continue;
            rule.nextSpawnTime = Mathf.Max(0f, rule.firstSpawnTime);
            rule.finished = false;
            if (rule.prefab == null)
            {
                rule.finished = true;
                Debug.LogWarning($"ItemManager: rule '{rule.name}' needs a prefab.", this);
            }
        }
        IsRunning = true;
    }

    public void StopSpawning()
    {
        IsRunning = false;
    }

    public GameObject SpawnNow(int ruleIndex)
    {
        if (ruleIndex < 0 || ruleIndex >= rules.Length) return null;
        return Spawn(rules[ruleIndex]);
    }

    private GameObject Spawn(SpawnRule rule)
    {
        if (rule == null || rule.prefab == null) return null;
        Vector3 position = rule.position;
        if (rule.randomizeX) position.x = RandomInRange(rule.xRange);
        if (rule.randomizeY) position.y = RandomInRange(rule.yRange);
        return Instantiate(rule.prefab, position,
            rule.prefab.transform.rotation, spawnedItemsParent);
    }

    private static float RandomInRange(Vector2 range)
    {
        return UnityEngine.Random.Range(Mathf.Min(range.x, range.y), Mathf.Max(range.x, range.y));
    }
}
