Shader "GenesisDiary/CreatureSDF"
{
    Properties
    {
        _Color ("Body Color", Color) = (0.92, 0.55, 0.28, 1)
        _RimColor ("Rim/Shadow Color", Color) = (0.5, 0.28, 0.12, 1)
        _BellyColor ("Belly/Sock Color", Color) = (0.97, 0.92, 0.82, 1)
        _BellyHeight ("Belly Height", Float) = 0.12
        _StripeColor ("Stripe Color", Color) = (0.35, 0.18, 0.08, 1)
        _StripeFreq ("Stripe Frequency", Float) = 14
        _StripeStrength ("Stripe Strength", Float) = 0.35
        _EyeColor ("Eye Color", Color) = (0.08, 0.09, 0.06, 1)
        _NoseColor ("Nose Color", Color) = (0.7, 0.35, 0.32, 1)

        _BodyLength ("Body Length", Float) = 0.66
        _BodyWidth ("Body Width", Float) = 0.26
        _HeadSize ("Head Size", Float) = 0.19
        _HeadOffsetX ("Head Offset X", Float) = 0.42
        _HeadOffsetY ("Head Offset Y", Float) = 0.06

        _EarHeight ("Ear Height", Float) = 0.13
        _EarBaseWidth ("Ear Base Width", Float) = 0.045
        _EarSpacing ("Ear Spacing", Float) = 0.06
        _EarLean ("Ear Lean (rad)", Float) = 0.35

        _EyeSize ("Eye Size", Float) = 0.022
        _NoseSize ("Nose Size", Float) = 0.016

        _LegThickness ("Leg Thickness", Float) = 0.042
        _LegCount ("Leg Count", Float) = 4
        _PawSize ("Paw Size", Float) = 0.05

        _TailThickness ("Tail Thickness", Float) = 0.045
        _SmoothK ("Smooth Blend", Float) = 0.1
        _EdgeSoftness ("Edge Softness", Float) = 0.018
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define MAX_LEGS 6

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _RimColor;
                float4 _BellyColor;
                float _BellyHeight;
                float4 _StripeColor;
                float _StripeFreq;
                float _StripeStrength;
                float4 _EyeColor;
                float4 _NoseColor;

                float _BodyLength;
                float _BodyWidth;
                float _HeadSize;
                float _HeadOffsetX;
                float _HeadOffsetY;

                float _EarHeight;
                float _EarBaseWidth;
                float _EarSpacing;
                float _EarLean;

                float _EyeSize;
                float _NoseSize;

                float _LegThickness;
                float _LegCount;
                float4 _LegData[MAX_LEGS];
                float _PawSize;

                float _TailThickness;
                float4 _TailSeg1;
                float4 _TailSeg2;

                float _SmoothK;
                float _EdgeSoftness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 positionLocal : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Unity's default Quad primitive is 1x1 (-0.5..0.5); scale to a -1..1 SDF domain.
                OUT.positionLocal = IN.positionOS.xy * 2.0;
                return OUT;
            }

            float sdCapsule(float2 p, float2 a, float2 b, float r)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / max(dot(ba, ba), 1e-5));
                return length(pa - ba * h) - r;
            }

            float sdTaperedCapsule(float2 p, float2 a, float2 b, float ra, float rb)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / max(dot(ba, ba), 1e-5));
                float r = lerp(ra, rb, h);
                return length(pa - ba * h) - r;
            }

            float smin(float a, float b, float k)
            {
                float h = saturate(0.5 + 0.5 * (b - a) / k);
                return lerp(b, a, h) - k * h * (1.0 - h);
            }

            float2 HeadCenter()
            {
                return float2(_HeadOffsetX, _HeadOffsetY);
            }

            float MapCreature(float2 p)
            {
                float body = sdCapsule(p, float2(-_BodyLength * 0.5, 0), float2(_BodyLength * 0.5, 0), _BodyWidth * 0.5);

                float2 headC = HeadCenter();
                float head = length(p - headC) - _HeadSize;
                float d = smin(body, head, _SmoothK);

                float earBaseY = _HeadSize * 0.7;
                float2 earLBase = headC + float2(-_EarSpacing, earBaseY);
                float2 earRBase = headC + float2(_EarSpacing, earBaseY);
                float2 earLTip = earLBase + float2(-sin(_EarLean), cos(_EarLean)) * _EarHeight;
                float2 earRTip = earRBase + float2(sin(_EarLean), cos(_EarLean)) * _EarHeight;
                float earL = sdTaperedCapsule(p, earLBase, earLTip, _EarBaseWidth, _EarBaseWidth * 0.12);
                float earR = sdTaperedCapsule(p, earRBase, earRTip, _EarBaseWidth, _EarBaseWidth * 0.12);
                d = smin(d, earL, _SmoothK * 0.35);
                d = smin(d, earR, _SmoothK * 0.35);

                float tail1 = sdTaperedCapsule(p, _TailSeg1.xy, _TailSeg1.zw, _TailThickness, _TailThickness * 0.7);
                d = smin(d, tail1, _SmoothK * 0.6);
                float tail2 = sdTaperedCapsule(p, _TailSeg2.xy, _TailSeg2.zw, _TailThickness * 0.7, _TailThickness * 0.15);
                d = smin(d, tail2, _SmoothK * 0.5);

                int legCount = min((int)_LegCount, MAX_LEGS);
                for (int i = 0; i < legCount; i++)
                {
                    float2 legEnd = _LegData[i].zw;
                    float leg = sdCapsule(p, _LegData[i].xy, legEnd, _LegThickness);
                    d = smin(d, leg, _SmoothK * 0.45);
                    float paw = length(p - legEnd) - _PawSize;
                    d = smin(d, paw, _SmoothK * 0.3);
                }
                return d;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 p = IN.positionLocal;
                float d = MapCreature(p);

                float2 e = float2(0.01, 0.0);
                float2 n = normalize(float2(
                    MapCreature(p + e.xy) - MapCreature(p - e.xy),
                    MapCreature(p + e.yx) - MapCreature(p - e.yx)
                ));

                float light = saturate(dot(n, normalize(float2(-0.4, 0.6))) * 0.5 + 0.5);
                float3 col = lerp(_RimColor.rgb, _Color.rgb, light);

                float bellyT = smoothstep(-_BodyWidth * 0.9, -_BodyWidth * 0.9 + _BellyHeight, p.y);
                bellyT = 1.0 - bellyT;
                col = lerp(col, _BellyColor.rgb, bellyT);

                float stripe = sin(p.x * _StripeFreq + sin(p.y * 3.0) * 0.6) * 0.5 + 0.5;
                stripe = smoothstep(0.55, 0.78, stripe) * _StripeStrength * (1.0 - bellyT);
                col = lerp(col, _StripeColor.rgb, stripe);

                float2 headC = HeadCenter();
                float2 eyePos = headC + float2(_HeadSize * 0.55, _HeadSize * 0.2);
                float eyeD = length(p - eyePos) - _EyeSize;
                float eyeMask = 1.0 - smoothstep(-0.004, 0.004, eyeD);
                col = lerp(col, _EyeColor.rgb, eyeMask);

                float2 nosePos = headC + float2(_HeadSize * 0.92, -0.01);
                float noseD = length(p - nosePos) - _NoseSize;
                float noseMask = 1.0 - smoothstep(-0.004, 0.004, noseD);
                col = lerp(col, _NoseColor.rgb, noseMask);

                float alpha = 1.0 - smoothstep(0.0, _EdgeSoftness, d);
                clip(alpha - 0.01);

                return float4(col, alpha);
            }
            ENDHLSL
        }
    }
}
