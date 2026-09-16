using UnityEngine;

[System.Serializable]
public struct CreatureDNA
{
    public float bodyLength;
    public float bodyWidth;
    public float headSize;
    public float headOffsetX;
    public float headOffsetY;

    public float earHeight;
    public float earBaseWidth;
    public float earSpacing;
    public float earLean;

    public float eyeSize;
    public float noseSize;

    public int legCount;
    public float legLength;
    public float legThickness;
    public float legAttachY;
    public float pawSize;

    public float tailLength;
    public float tailCurl;
    public float tailThickness;

    public Color bodyColor;
    public Color rimColor;
    public Color bellyColor;
    public float bellyHeight;
    public Color stripeColor;
    public float stripeFreq;
    public float stripeStrength;
    public Color eyeColor;
    public Color noseColor;

    public float smoothBlend;
    public float edgeSoftness;

    public static CreatureDNA Random(System.Random rng)
    {
        var d = new CreatureDNA();

        d.bodyLength = Range(rng, 0.5f, 0.85f);
        d.bodyWidth = Range(rng, 0.18f, 0.34f);
        d.headSize = Range(rng, 0.14f, 0.24f);
        d.headOffsetX = d.bodyLength * Range(rng, 0.55f, 0.7f);
        d.headOffsetY = Range(rng, -0.02f, 0.12f);

        d.earHeight = Range(rng, 0.06f, 0.18f);
        d.earBaseWidth = Range(rng, 0.03f, 0.06f);
        d.earSpacing = Range(rng, 0.03f, 0.08f);
        d.earLean = Range(rng, 0.1f, 0.6f);

        d.eyeSize = Range(rng, 0.015f, 0.028f);
        d.noseSize = Range(rng, 0.012f, 0.02f);

        double legRoll = rng.NextDouble();
        d.legCount = legRoll < 0.08 ? 2 : legRoll < 0.16 ? 6 : 4;
        d.legLength = Range(rng, 0.16f, 0.34f);
        d.legThickness = Range(rng, 0.028f, 0.05f);
        d.legAttachY = -d.bodyWidth * Range(rng, 0.3f, 0.5f);
        d.pawSize = Range(rng, 0.032f, 0.06f);

        d.tailLength = Range(rng, 0.2f, 0.55f);
        d.tailCurl = Range(rng, 0.4f, 1.6f);
        d.tailThickness = Range(rng, 0.03f, 0.06f);

        float hue = (float)rng.NextDouble();
        float sat = Range(rng, 0.35f, 0.75f);
        float val = Range(rng, 0.55f, 0.95f);
        d.bodyColor = Color.HSVToRGB(hue, sat, val);
        d.rimColor = Color.HSVToRGB(hue, Mathf.Min(1f, sat + 0.15f), val * 0.55f);
        d.stripeColor = Color.HSVToRGB(hue, Mathf.Min(1f, sat + 0.25f), val * 0.32f);
        d.bellyColor = Color.HSVToRGB(hue, sat * 0.15f, Range(rng, 0.85f, 0.98f));
        d.bellyHeight = Range(rng, 0.03f, 0.16f);
        d.stripeFreq = Range(rng, 6f, 22f);
        d.stripeStrength = (float)rng.NextDouble() < 0.25 ? 0f : Range(rng, 0.15f, 0.5f);

        float[] eyeHues = { 0.33f, 0.14f, 0.55f, 0.08f };
        float eyeHue = eyeHues[rng.Next(eyeHues.Length)];
        d.eyeColor = Color.HSVToRGB(eyeHue, Range(rng, 0.6f, 0.9f), Range(rng, 0.35f, 0.6f));
        d.noseColor = Color.HSVToRGB(Range(rng, 0.9f, 1.0f) % 1f, Range(rng, 0.3f, 0.6f), Range(rng, 0.45f, 0.75f));

        d.smoothBlend = Range(rng, 0.07f, 0.13f);
        d.edgeSoftness = 0.018f;

        return d;
    }

    static float Range(System.Random rng, float min, float max)
    {
        return min + (float)rng.NextDouble() * (max - min);
    }
}
