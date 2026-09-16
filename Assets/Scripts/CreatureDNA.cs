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

    /// Recombines two parents' genes (random blend point per trait, not a flat average) with
    /// a chance of mutation per trait. This is the only place new variation enters the gene pool
    /// beyond what Random() seeds initially.
    public static CreatureDNA Crossbreed(CreatureDNA a, CreatureDNA b, System.Random rng, float mutationRate = 0.15f, float mutationStrength = 0.25f)
    {
        var c = new CreatureDNA();

        c.bodyLength = Mix(rng, a.bodyLength, b.bodyLength, mutationRate, mutationStrength, 0.5f, 0.85f);
        c.bodyWidth = Mix(rng, a.bodyWidth, b.bodyWidth, mutationRate, mutationStrength, 0.18f, 0.34f);
        c.headSize = Mix(rng, a.headSize, b.headSize, mutationRate, mutationStrength, 0.14f, 0.24f);
        c.headOffsetX = Mix(rng, a.headOffsetX, b.headOffsetX, mutationRate, mutationStrength, c.bodyLength * 0.4f, c.bodyLength * 0.78f);
        c.headOffsetY = Mix(rng, a.headOffsetY, b.headOffsetY, mutationRate, mutationStrength, -0.02f, 0.12f);

        c.earHeight = Mix(rng, a.earHeight, b.earHeight, mutationRate, mutationStrength, 0.06f, 0.18f);
        c.earBaseWidth = Mix(rng, a.earBaseWidth, b.earBaseWidth, mutationRate, mutationStrength, 0.03f, 0.06f);
        c.earSpacing = Mix(rng, a.earSpacing, b.earSpacing, mutationRate, mutationStrength, 0.03f, 0.08f);
        c.earLean = Mix(rng, a.earLean, b.earLean, mutationRate, mutationStrength, 0.1f, 0.6f);

        c.eyeSize = Mix(rng, a.eyeSize, b.eyeSize, mutationRate, mutationStrength, 0.015f, 0.028f);
        c.noseSize = Mix(rng, a.noseSize, b.noseSize, mutationRate, mutationStrength, 0.012f, 0.02f);

        c.legCount = rng.NextDouble() < 0.5 ? a.legCount : b.legCount;
        if (rng.NextDouble() < mutationRate * 0.3)
        {
            c.legCount = c.legCount == 4 ? (rng.NextDouble() < 0.5 ? 2 : 6) : 4;
        }
        c.legLength = Mix(rng, a.legLength, b.legLength, mutationRate, mutationStrength, 0.16f, 0.34f);
        c.legThickness = Mix(rng, a.legThickness, b.legThickness, mutationRate, mutationStrength, 0.028f, 0.05f);
        c.legAttachY = -c.bodyWidth * Range(rng, 0.3f, 0.5f);
        c.pawSize = Mix(rng, a.pawSize, b.pawSize, mutationRate, mutationStrength, 0.032f, 0.06f);

        c.tailLength = Mix(rng, a.tailLength, b.tailLength, mutationRate, mutationStrength, 0.2f, 0.55f);
        c.tailCurl = Mix(rng, a.tailCurl, b.tailCurl, mutationRate, mutationStrength, 0.4f, 1.6f);
        c.tailThickness = Mix(rng, a.tailThickness, b.tailThickness, mutationRate, mutationStrength, 0.03f, 0.06f);

        float ha, sa, va, hb, sb, vb;
        Color.RGBToHSV(a.bodyColor, out ha, out sa, out va);
        Color.RGBToHSV(b.bodyColor, out hb, out sb, out vb);
        float hue = MixHue(rng, ha, hb, mutationRate);
        float sat = Mathf.Clamp(Mix(rng, sa, sb, mutationRate, mutationStrength, 0.35f, 0.75f), 0.15f, 0.9f);
        float val = Mathf.Clamp(Mix(rng, va, vb, mutationRate, mutationStrength, 0.55f, 0.95f), 0.35f, 1f);
        c.bodyColor = Color.HSVToRGB(hue, sat, val);
        c.rimColor = Color.HSVToRGB(hue, Mathf.Min(1f, sat + 0.15f), val * 0.55f);
        c.stripeColor = Color.HSVToRGB(hue, Mathf.Min(1f, sat + 0.25f), val * 0.32f);
        c.bellyColor = Color.HSVToRGB(hue, sat * 0.15f, Range(rng, 0.85f, 0.98f));
        c.bellyHeight = Mix(rng, a.bellyHeight, b.bellyHeight, mutationRate, mutationStrength, 0.03f, 0.16f);
        c.stripeFreq = Mix(rng, a.stripeFreq, b.stripeFreq, mutationRate, mutationStrength, 6f, 22f);
        c.stripeStrength = Mix(rng, a.stripeStrength, b.stripeStrength, mutationRate, mutationStrength, 0f, 0.5f);

        c.eyeColor = Color.Lerp(a.eyeColor, b.eyeColor, (float)rng.NextDouble());
        c.noseColor = Color.Lerp(a.noseColor, b.noseColor, (float)rng.NextDouble());

        c.smoothBlend = Mix(rng, a.smoothBlend, b.smoothBlend, mutationRate, mutationStrength, 0.07f, 0.13f);
        c.edgeSoftness = 0.018f;

        return c;
    }

    static float Mix(System.Random rng, float a, float b, float mutationRate, float mutationStrength, float min, float max)
    {
        float value = Mathf.Lerp(a, b, (float)rng.NextDouble());
        if (rng.NextDouble() < mutationRate)
        {
            value += (max - min) * mutationStrength * ((float)rng.NextDouble() * 2f - 1f);
        }
        return Mathf.Clamp(value, min, max);
    }

    static float MixHue(System.Random rng, float ha, float hb, float mutationRate)
    {
        float diff = Mathf.DeltaAngle(ha * 360f, hb * 360f) / 360f;
        float hue = ha + diff * (float)rng.NextDouble();
        if (rng.NextDouble() < mutationRate)
        {
            hue += ((float)rng.NextDouble() * 2f - 1f) * 0.15f;
        }
        return Mathf.Repeat(hue, 1f);
    }

    static float Range(System.Random rng, float min, float max)
    {
        return min + (float)rng.NextDouble() * (max - min);
    }
}
