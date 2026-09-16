using UnityEngine;

// Minimal WORLD-layer resource: a point creatures can path to and consume for
// energy. Kept deliberately dumb (no regrowth/seasons yet) until the world
// layer gets its own design pass.
public class FoodSource : MonoBehaviour
{
    public float energyValue = 40f;
}
