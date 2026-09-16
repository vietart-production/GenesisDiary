using UnityEngine;

// LIFE SIMULATION loop: wander by default, seek food when hungry, seek a
// mate and reproduce (via CreatureDNA.Crossbreed) when well-fed, starve if
// energy hits zero. Deliberately polls FindObjectsByType on a throttled
// timer rather than every frame - fine at prototype population sizes, will
// need a spatial index if this ever needs to scale to thousands.
//
// Per-frame logic lives in Tick(dt) rather than directly in Update() so it
// can also be driven manually (fixed dt, many steps) from an Editor script
// for deterministic testing - the Editor's Play-mode clock can't be trusted
// to advance at real wall-clock speed when the window isn't focused.
//
// Default values below are validated by a 60 simulated-second population
// test (12 start -> 21 born, 19 starved, 14 alive at end) - tune from here,
// not from scratch; the original guessed defaults produced total die-offs.
[RequireComponent(typeof(CreatureVisual))]
public class CreatureAgent : MonoBehaviour
{
    [Header("Needs")]
    public float energy = 100f;
    public float maxEnergy = 150f;
    public float energyDrainPerSecond = 3f;
    public float hungryThreshold = 80f;
    public float reproduceThreshold = 90f;
    public float reproduceCost = 30f;
    public float reproduceCooldown = 5f;

    [Header("Movement")]
    public float moveSpeed = 2.2f;
    public float wanderRadius = 3f;
    public float senseRadius = 10f;
    public float interactRadius = 0.4f;

    public bool Alive { get; private set; } = true;
    public float Cooldown { get; private set; }

    float decisionTimer;
    Vector3 originPoint;
    Vector3 targetPoint;
    Transform targetFood;
    Transform targetMate;

    CreatureVisual visualCache;

    // Non-serialized cache fields go null after every domain reload (script
    // recompile) without Awake() re-running on pre-existing objects, which is
    // routine when driving the Editor via repeated RunCommand calls - so this
    // re-fetches lazily instead of trusting Awake() to have set it once.
    CreatureVisual Visual => visualCache != null ? visualCache : (visualCache = GetComponent<CreatureVisual>());

    void Start()
    {
        originPoint = transform.position;
        PickWanderTarget();
    }

    void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float dt)
    {
        if (!Alive)
        {
            return;
        }

        energy -= energyDrainPerSecond * dt;
        if (energy <= 0f)
        {
            Die();
            return;
        }

        if (Cooldown > 0f)
        {
            Cooldown -= dt;
        }

        decisionTimer -= dt;
        if (decisionTimer <= 0f)
        {
            decisionTimer = 0.4f;
            Decide();
        }

        Vector3 goal = targetFood != null ? targetFood.position
            : targetMate != null ? targetMate.position
            : targetPoint;
        MoveToward(goal, dt);

        if (targetFood != null && Vector3.Distance(transform.position, targetFood.position) < interactRadius)
        {
            EatFood(targetFood.GetComponent<FoodSource>());
        }
        else if (targetMate != null && Vector3.Distance(transform.position, targetMate.position) < interactRadius)
        {
            TryBreed(targetMate.GetComponent<CreatureAgent>());
        }
    }

    /// Resets a starved-and-deactivated creature back to a live state. Reactivating the
    /// GameObject alone is not enough - Alive is a separate internal flag Tick() checks first.
    public void Revive(float startEnergy)
    {
        Alive = true;
        Cooldown = 0f;
        decisionTimer = 0f;
        targetFood = null;
        targetMate = null;
        energy = startEnergy;
        originPoint = transform.position;
        PickWanderTarget();
    }

    void Decide()
    {
        targetFood = null;
        targetMate = null;

        if (energy < hungryThreshold)
        {
            targetFood = FindNearest<FoodSource>();
        }

        if (targetFood == null && energy >= reproduceThreshold && Cooldown <= 0f)
        {
            targetMate = FindNearestMate();
        }

        if (targetFood == null && targetMate == null)
        {
            if (Vector3.Distance(transform.position, targetPoint) < 0.2f || Random.value < 0.05f)
            {
                PickWanderTarget();
            }
        }
    }

    Transform FindNearest<T>() where T : Component
    {
        T[] all = FindObjectsByType<T>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDist = senseRadius;
        foreach (var t in all)
        {
            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t.transform;
            }
        }
        return best;
    }

    Transform FindNearestMate()
    {
        CreatureAgent[] all = FindObjectsByType<CreatureAgent>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDist = senseRadius;
        foreach (var a in all)
        {
            if (a == this || !a.Alive || a.energy < a.reproduceThreshold || a.Cooldown > 0f)
            {
                continue;
            }
            float d = Vector3.Distance(transform.position, a.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = a.transform;
            }
        }
        return best;
    }

    void MoveToward(Vector3 goal, float dt)
    {
        Vector3 toGoal = goal - transform.position;
        toGoal.z = 0f;
        if (toGoal.magnitude < 0.02f)
        {
            return;
        }

        Vector3 dir = toGoal.normalized;
        transform.position += dir * moveSpeed * dt;

        Vector3 scale = transform.localScale;
        float facing = dir.x >= 0f ? 1f : -1f;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }

    void PickWanderTarget()
    {
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        targetPoint = originPoint + new Vector3(offset.x, offset.y, 0f);
    }

    void EatFood(FoodSource food)
    {
        if (food == null)
        {
            targetFood = null;
            return;
        }
        energy = Mathf.Min(maxEnergy, energy + food.energyValue);
        DestroyImmediate(food.gameObject);
        targetFood = null;
    }

    void TryBreed(CreatureAgent mate)
    {
        if (mate == null || !mate.Alive || mate.Cooldown > 0f || mate.energy < mate.reproduceThreshold)
        {
            targetMate = null;
            return;
        }

        Cooldown = reproduceCooldown;
        mate.Cooldown = reproduceCooldown;
        energy -= reproduceCost;
        mate.energy -= reproduceCost;

        // Only the lower-instance-ID side spawns so two mutually detecting
        // agents don't both instantiate a child on the same frame.
        if (GetInstanceID() < mate.GetInstanceID())
        {
            SpawnChild(mate);
        }

        targetMate = null;
    }

    void SpawnChild(CreatureAgent mate)
    {
        var rng = new System.Random();
        CreatureDNA childDna = CreatureDNA.Crossbreed(Visual.GetDNA(), mate.Visual.GetDNA(), rng);

        GameObject go = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        go.name = "Creature_child";

        var childVisual = go.GetComponent<CreatureVisual>();
        childVisual.ApplyDNA(childDna);

        var childAgent = go.GetComponent<CreatureAgent>();
        childAgent.Revive(childAgent.maxEnergy * 0.5f);
        childAgent.Cooldown = childAgent.reproduceCooldown;

        Debug.LogFormat("{0} + {1} bred {2}", name, mate.name, go.name);
    }

    void Die()
    {
        Alive = false;
        Debug.LogFormat("{0} died of starvation at {1}", name, transform.position);
        gameObject.SetActive(false);
    }
}
