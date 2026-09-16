using UnityEngine;

// TIME layer (GDD section 13): a single tunable knob for how fast the whole
// simulation runs, so a play session isn't stuck watching real-time-speed
// wandering when the interesting part (speciation, drift) is generations
// away. Deliberately separate from Time.timeScale, which would also warp
// physics/animation/UI in ways we don't want here.
public class SimulationClock : MonoBehaviour
{
    [Range(0f, 1000f)]
    public float speedMultiplier = 1f;

    public float ScaledDeltaTime()
    {
        return Time.deltaTime * speedMultiplier;
    }
}
