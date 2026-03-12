Shader "Custom/NebulaGradient"
{
    Properties
    {
        _Scale ("Noise Scale", Float) = 2
        _Speed ("Animation Speed", Float) = 0.05
        _Warp ("Warp Strength", Float) = 0.5

        _Color1 ("Color 1 (dark)", Color) = (0.02,0.02,0.1,1)
        _Color2 ("Color 2 (blue)", Color) = (0.2,0.3,0.9,1)
        _Color3 ("Color 3 (purple)", Color) = (0.7,0.2,1,1)
        _Color4 ("Color 4 (core)", Color) = (1,0.6,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend One One
        ZWrite Off
        // ZTest Always
        Cull Off

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

            float _Scale;
            float _Speed;
            float _Warp;

            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;

            float hash(float2 p)
            {
                return frac(sin(dot(p,float2(127.1,311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));

                float2 u = f*f*(3-2*f);

                return lerp(a,b,u.x) +
                       (c-a)*u.y*(1-u.x) +
                       (d-b)*u.x*u.y;
            }

            float fbm(float2 uv)
            {
                float v = 0.0;
                float a = 0.5;

                for(int i=0;i<5;i++)
                {
                    v += noise(uv) * a;
                    uv *= 2.0;
                    a *= 0.5;
                }

                return v;
            }

            float3 nebulaGradient(float n)
            {
                float3 col;

                if(n < 0.33)
                    col = lerp(_Color1.rgb,_Color2.rgb,n*3);
                else if(n < 0.66)
                    col = lerp(_Color2.rgb,_Color3.rgb,(n-0.33)*3);
                else
                    col = lerp(_Color3.rgb,_Color4.rgb,(n-0.66)*3);

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
                float2 uv = i.uv * _Scale;

                float time = _Time.y * _Speed;

                float2 warp;
                warp.x = fbm(uv + time);
                warp.y = fbm(uv - time);

                uv += warp * _Warp;

                float n = fbm(uv);

                float3 col = nebulaGradient(n);

                return float4(col * n, n);
            }

            ENDCG
        }
    }
}
