Shader "Custom/NebulaGradientAdvancedMobile"
{
    Properties
    {
        _Scale ("Noise Scale", Float) = 2
        _Speed ("Animation Speed", Float) = 0.05
        _Warp ("Warp Strength", Float) = 0.5
        _Scroll ("Scroll XY", Vector) = (0.01,0.0,0,0)
        _Center ("Nebula Center", Vector) = (0.5,0.5,0,0)
        _Radius ("Nebula Radius", Float) = 0.6
        _Softness ("Edge Softness", Float) = 0.3
        _Color1 ("Color 1", Color) = (0.02,0.02,0.1,0)
        _Color2 ("Color 2", Color) = (0.2,0.3,0.9,0.5)
        _Color3 ("Color 3", Color) = (0.7,0.2,1,0.8)
        _Color4 ("Color 4", Color) = (1,0.6,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend One One
        ZWrite Off
        Cull Front // Optimized for sphere interior

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Using half precision for mobile performance
            half _Scale;
            half _Speed;
            half _Warp;
            half4 _Scroll;
            half4 _Center;
            half _Radius;
            half _Softness;
            half4 _Color1;
            half4 _Color2;
            half4 _Color3;
            half4 _Color4;

            // Faster hash function for mobile
            inline float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // Reduced FBM iterations for mobile (3 instead of 5)
            float fbm(float2 uv)
            {
                float v = 0.0;
                float a = 0.5;
                for(int i = 0; i < 3; i++)
                {
                    v += noise(uv) * a;
                    uv *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            // Optimized gradient without if/else branch
            fixed4 nebulaGradient(float n)
            {
                fixed4 col = lerp(_Color1, _Color2, saturate(n * 3.0));
                col = lerp(col, _Color3, saturate((n - 0.33) * 3.0));
                col = lerp(col, _Color4, saturate((n - 0.66) * 3.0));
                return col;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;
                float2 uv = i.uv + _Scroll.xy * _Time.y;
                float2 noiseUV = uv * _Scale;

                // Warp effect
                float2 warp;
                warp.x = fbm(noiseUV + time);
                warp.y = fbm(noiseUV - time);

                // Apply warp and final FBM
                float n = fbm(noiseUV + warp * _Warp);

                fixed4 col = nebulaGradient(n);

                // Radial Mask optimization
                half d = distance(i.uv, _Center.xy);
                half mask = 1.0 - smoothstep(_Radius - _Softness, _Radius, d);

                col *= mask;
                return col * col.a;
            }
            ENDCG
        }
    }
}