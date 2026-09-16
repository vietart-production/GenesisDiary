using UnityEngine;

// First slice of the LIFE SIMULATION layer: wandering movement plus an energy
// stat that drains over time and kills the creature at zero. No food/feeding
// yet - that's the next piece, once this loop is proven.
[RequireComponent(typeof(CreatureVisual))]
public class CreatureAgent : MonoBehaviour
{
    public float moveSpeed = 0.6f;
    public float wanderRadius = 4f;
    public float energy = 100f;
    public float energyDrainPerSecond = 15f;

    Vector3 originPoint;
    Vector3 targetPoint;
    bool alive = true;

    void Start()
    {
        originPoint = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        if (!alive)
        {
            return;
        }

        energy -= energyDrainPerSecond * Time.deltaTime;
        if (energy <= 0f)
        {
            Die();
            return;
        }

        Vector3 toTarget = targetPoint - transform.position;
        toTarget.z = 0f;
        if (toTarget.magnitude < 0.15f)
        {
            PickNewTarget();
            return;
        }

        Vector3 dir = toTarget.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        Vector3 scale = transform.localScale;
        float facing = dir.x >= 0f ? 1f : -1f;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }

    void PickNewTarget()
    {
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        targetPoint = originPoint + new Vector3(offset.x, offset.y, 0f);
    }

    void Die()
    {
        alive = false;
        Debug.LogFormat("{0} died of starvation at {1}", name, transform.position);
        gameObject.SetActive(false);
    }
}
