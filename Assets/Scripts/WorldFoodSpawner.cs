using UnityEngine;

// Lean stand-in for the eventual WORLD layer (terrain/climate-driven food
// distribution). For now: keep a roughly constant number of edible dots
// scattered in a rectangle around this transform.
public class WorldFoodSpawner : MonoBehaviour
{
    public int maxFood = 20;
    public Vector2 areaSize = new Vector2(14f, 10f);
    public float respawnInterval = 1.5f;

    float timer;
    SimulationClock clockCache;
    SimulationClock Clock => clockCache != null ? clockCache : (clockCache = FindFirstObjectByType<SimulationClock>());

    void Update()
    {
        Tick(Clock != null ? Clock.ScaledDeltaTime() : Time.deltaTime);
    }

    public void Tick(float dt)
    {
        timer -= dt;
        if (timer <= 0f)
        {
            timer = respawnInterval;
            if (transform.childCount < maxFood)
            {
                SpawnOne();
            }
        }
    }

    [ContextMenu("Spawn Initial Food")]
    public void SpawnInitial()
    {
        for (int i = 0; i < maxFood; i++)
        {
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        Vector3 pos = transform.position + new Vector3(
            Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
            Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f),
            0f);

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Food";
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * 0.18f;

        var collider = go.GetComponent<Collider>();
        if (collider != null)
        {
            if (Application.isPlaying)
            {
                Destroy(collider);
            }
            else
            {
                DestroyImmediate(collider);
            }
        }

        var mr = go.GetComponent<MeshRenderer>();
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = new Color(0.35f, 0.75f, 0.25f);
        mr.sharedMaterial = mat;

        go.AddComponent<FoodSource>();
    }
}
