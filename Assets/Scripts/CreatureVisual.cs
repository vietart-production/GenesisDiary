using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class CreatureVisual : MonoBehaviour
{
    const int MaxLegs = 6;

    [Header("Body")]
    public float bodyLength = 0.66f;
    public float bodyWidth = 0.26f;
    public float headSize = 0.19f;
    public float headOffsetX = 0.42f;
    public float headOffsetY = 0.06f;

    [Header("Ears")]
    public float earHeight = 0.13f;
    public float earBaseWidth = 0.045f;
    public float earSpacing = 0.06f;
    public float earLean = 0.35f;

    [Header("Face")]
    public float eyeSize = 0.022f;
    public float noseSize = 0.016f;

    [Header("Legs")]
    [Range(0, MaxLegs)] public int legCount = 4;
    public float legLength = 0.24f;
    public float legThickness = 0.036f;
    public float legAttachY = -0.1f;
    public float pawSize = 0.042f;

    [Header("Tail")]
    public float tailLength = 0.42f;
    public float tailCurl = 1.05f;
    public float tailThickness = 0.045f;

    [Header("Animation")]
    public float walkSpeed = 3f;
    public float walkAmplitude = 0.3f;
    public float tailSwaySpeed = 1.2f;
    public float tailSwayAmount = 0.12f;

    [Header("Look")]
    public Color bodyColor = new Color(0.92f, 0.55f, 0.28f);
    public Color rimColor = new Color(0.5f, 0.28f, 0.12f);
    public Color bellyColor = new Color(0.97f, 0.92f, 0.82f);
    public float bellyHeight = 0.05f;
    public Color stripeColor = new Color(0.35f, 0.18f, 0.08f);
    public float stripeFreq = 14f;
    public float stripeStrength = 0.35f;
    public Color eyeColor = new Color(0.08f, 0.09f, 0.06f);
    public Color noseColor = new Color(0.7f, 0.35f, 0.32f);
    public float smoothBlend = 0.1f;
    public float edgeSoftness = 0.018f;

    static readonly int ColorID = Shader.PropertyToID("_Color");
    static readonly int RimColorID = Shader.PropertyToID("_RimColor");
    static readonly int BellyColorID = Shader.PropertyToID("_BellyColor");
    static readonly int BellyHeightID = Shader.PropertyToID("_BellyHeight");
    static readonly int StripeColorID = Shader.PropertyToID("_StripeColor");
    static readonly int StripeFreqID = Shader.PropertyToID("_StripeFreq");
    static readonly int StripeStrengthID = Shader.PropertyToID("_StripeStrength");
    static readonly int EyeColorID = Shader.PropertyToID("_EyeColor");
    static readonly int NoseColorID = Shader.PropertyToID("_NoseColor");

    static readonly int BodyLengthID = Shader.PropertyToID("_BodyLength");
    static readonly int BodyWidthID = Shader.PropertyToID("_BodyWidth");
    static readonly int HeadSizeID = Shader.PropertyToID("_HeadSize");
    static readonly int HeadOffsetXID = Shader.PropertyToID("_HeadOffsetX");
    static readonly int HeadOffsetYID = Shader.PropertyToID("_HeadOffsetY");

    static readonly int EarHeightID = Shader.PropertyToID("_EarHeight");
    static readonly int EarBaseWidthID = Shader.PropertyToID("_EarBaseWidth");
    static readonly int EarSpacingID = Shader.PropertyToID("_EarSpacing");
    static readonly int EarLeanID = Shader.PropertyToID("_EarLean");

    static readonly int EyeSizeID = Shader.PropertyToID("_EyeSize");
    static readonly int NoseSizeID = Shader.PropertyToID("_NoseSize");

    static readonly int LegThicknessID = Shader.PropertyToID("_LegThickness");
    static readonly int LegCountID = Shader.PropertyToID("_LegCount");
    static readonly int LegDataID = Shader.PropertyToID("_LegData");
    static readonly int PawSizeID = Shader.PropertyToID("_PawSize");

    static readonly int TailThicknessID = Shader.PropertyToID("_TailThickness");
    static readonly int TailSeg1ID = Shader.PropertyToID("_TailSeg1");
    static readonly int TailSeg2ID = Shader.PropertyToID("_TailSeg2");

    static readonly int SmoothKID = Shader.PropertyToID("_SmoothK");
    static readonly int EdgeSoftnessID = Shader.PropertyToID("_EdgeSoftness");

    MeshRenderer meshRenderer;
    MaterialPropertyBlock mpb;
    readonly Vector4[] legData = new Vector4[MaxLegs];
    Vector4 tailSeg1;
    Vector4 tailSeg2;

    public void ApplyDNA(CreatureDNA dna)
    {
        bodyLength = dna.bodyLength;
        bodyWidth = dna.bodyWidth;
        headSize = dna.headSize;
        headOffsetX = dna.headOffsetX;
        headOffsetY = dna.headOffsetY;

        earHeight = dna.earHeight;
        earBaseWidth = dna.earBaseWidth;
        earSpacing = dna.earSpacing;
        earLean = dna.earLean;

        eyeSize = dna.eyeSize;
        noseSize = dna.noseSize;

        legCount = dna.legCount;
        legLength = dna.legLength;
        legThickness = dna.legThickness;
        legAttachY = dna.legAttachY;
        pawSize = dna.pawSize;

        tailLength = dna.tailLength;
        tailCurl = dna.tailCurl;
        tailThickness = dna.tailThickness;

        bodyColor = dna.bodyColor;
        rimColor = dna.rimColor;
        bellyColor = dna.bellyColor;
        bellyHeight = dna.bellyHeight;
        stripeColor = dna.stripeColor;
        stripeFreq = dna.stripeFreq;
        stripeStrength = dna.stripeStrength;
        eyeColor = dna.eyeColor;
        noseColor = dna.noseColor;

        smoothBlend = dna.smoothBlend;
        edgeSoftness = dna.edgeSoftness;
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        UpdateLegs();
        UpdateTail();
        Apply();
    }

    void UpdateLegs()
    {
        float t = Time.realtimeSinceStartup * walkSpeed;
        for (int i = 0; i < MaxLegs; i++)
        {
            if (i >= legCount)
            {
                legData[i] = Vector4.zero;
                continue;
            }

            float along = legCount <= 1
                ? 0f
                : Mathf.Lerp(-bodyLength * 0.32f, bodyLength * 0.28f, (float)i / (legCount - 1));
            Vector2 attach = new Vector2(along, legAttachY);

            float phase = (i % 2 == 0) ? 0f : Mathf.PI;
            float swing = Mathf.Sin(t + phase) * walkAmplitude;
            Vector2 end = attach + new Vector2(swing * 0.25f, -legLength);

            legData[i] = new Vector4(attach.x, attach.y, end.x, end.y);
        }
    }

    void UpdateTail()
    {
        float t = Time.realtimeSinceStartup * tailSwaySpeed;
        float sway = Mathf.Sin(t) * tailSwayAmount;

        const float baseAngle = 2.55f; // ~146deg: up and back from the body's rear
        Vector2 basePoint = new Vector2(-bodyLength * 0.48f, bodyWidth * 0.1f);

        float a1 = baseAngle + sway;
        Vector2 dir1 = new Vector2(Mathf.Cos(a1), Mathf.Sin(a1));
        Vector2 mid = basePoint + dir1 * (tailLength * 0.55f);

        float a2 = a1 + tailCurl;
        Vector2 dir2 = new Vector2(Mathf.Cos(a2), Mathf.Sin(a2));
        Vector2 tip = mid + dir2 * (tailLength * 0.45f);

        tailSeg1 = new Vector4(basePoint.x, basePoint.y, mid.x, mid.y);
        tailSeg2 = new Vector4(mid.x, mid.y, tip.x, tip.y);
    }

    void Apply()
    {
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ColorID, bodyColor);
        mpb.SetColor(RimColorID, rimColor);
        mpb.SetColor(BellyColorID, bellyColor);
        mpb.SetFloat(BellyHeightID, bellyHeight);
        mpb.SetColor(StripeColorID, stripeColor);
        mpb.SetFloat(StripeFreqID, stripeFreq);
        mpb.SetFloat(StripeStrengthID, stripeStrength);
        mpb.SetColor(EyeColorID, eyeColor);
        mpb.SetColor(NoseColorID, noseColor);

        mpb.SetFloat(BodyLengthID, bodyLength);
        mpb.SetFloat(BodyWidthID, bodyWidth);
        mpb.SetFloat(HeadSizeID, headSize);
        mpb.SetFloat(HeadOffsetXID, headOffsetX);
        mpb.SetFloat(HeadOffsetYID, headOffsetY);

        mpb.SetFloat(EarHeightID, earHeight);
        mpb.SetFloat(EarBaseWidthID, earBaseWidth);
        mpb.SetFloat(EarSpacingID, earSpacing);
        mpb.SetFloat(EarLeanID, earLean);

        mpb.SetFloat(EyeSizeID, eyeSize);
        mpb.SetFloat(NoseSizeID, noseSize);

        mpb.SetFloat(LegThicknessID, legThickness);
        mpb.SetFloat(LegCountID, legCount);
        mpb.SetVectorArray(LegDataID, legData);
        mpb.SetFloat(PawSizeID, pawSize);

        mpb.SetFloat(TailThicknessID, tailThickness);
        mpb.SetVector(TailSeg1ID, tailSeg1);
        mpb.SetVector(TailSeg2ID, tailSeg2);

        mpb.SetFloat(SmoothKID, smoothBlend);
        mpb.SetFloat(EdgeSoftnessID, edgeSoftness);
        meshRenderer.SetPropertyBlock(mpb);
    }
}
