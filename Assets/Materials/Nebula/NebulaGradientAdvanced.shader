Shader "Custom/NebulaGradientAdvanced"
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
        Tags { "Queue"="Transparent" }
        Blend One One
        ZWrite Off
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

            float4 _Scroll;

            float4 _Center;
            float _Radius;
            float _Softness;

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

            float4 nebulaGradient(float n)
            {
                float4 col;

                if(n < 0.33)
                    col = lerp(_Color1,_Color2,n*3);
                else if(n < 0.66)
                    col = lerp(_Color2,_Color3,(n-0.33)*3);
                else
                    col = lerp(_Color3,_Color4,(n-0.66)*3);

                return col;
            }

            float radialMask(float2 uv)
            {
                float d = distance(uv,_Center.xy);

                float mask = 1 - smoothstep(_Radius - _Softness, _Radius, d);

                return mask;
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
                float2 uv = i.uv;

                float time = _Time.y * _Speed;

                uv += _Scroll.xy * _Time.y;

                float2 noiseUV = uv * _Scale;

                float2 warp;
                warp.x = fbm(noiseUV + time);
                warp.y = fbm(noiseUV - time);

                noiseUV += warp * _Warp;

                float n = fbm(noiseUV);

                float4 col = nebulaGradient(n);

                float mask = radialMask(i.uv);

                col.rgb *= mask;
                col.a *= mask;

                return col * col.a;
            }

            ENDCG
        }
    }
}
