Shader "Custom/ProceduralNebulaSky"
{
    Properties
    {
        _NoiseScale ("Noise Scale", Float) = 3

        // LAYER 1
        _ColorA1 ("Layer1 Color A", Color) = (0.6,0.2,1,1)
        _ColorB1 ("Layer1 Color B", Color) = (0.2,0.6,1,1)
        _Center1 ("Layer1 Center", Vector) = (0.5,0.5,0,0)
        _Radius1 ("Layer1 Radius", Float) = 0.6
        _Soft1 ("Layer1 Softness", Float) = 0.3
        _Intensity1 ("Layer1 Intensity", Float) = 1

        // LAYER 2
        _ColorA2 ("Layer2 Color A", Color) = (1,0.4,0.2,1)
        _ColorB2 ("Layer2 Color B", Color) = (1,0.1,0.5,1)
        _Center2 ("Layer2 Center", Vector) = (0.2,0.7,0,0)
        _Radius2 ("Layer2 Radius", Float) = 0.5
        _Soft2 ("Layer2 Softness", Float) = 0.3
        _Intensity2 ("Layer2 Intensity", Float) = 1

        // LAYER 3
        _ColorA3 ("Layer3 Color A", Color) = (0.2,1,0.6,1)
        _ColorB3 ("Layer3 Color B", Color) = (0.1,0.5,0.3,1)
        _Center3 ("Layer3 Center", Vector) = (0.7,0.4,0,0)
        _Radius3 ("Layer3 Radius", Float) = 0.55
        _Soft3 ("Layer3 Softness", Float) = 0.3
        _Intensity3 ("Layer3 Intensity", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }

        Cull Front
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _NoiseScale;

            float4 _ColorA1,_ColorB1;
            float4 _ColorA2,_ColorB2;
            float4 _ColorA3,_ColorB3;

            float4 _Center1,_Center2,_Center3;

            float _Radius1,_Radius2,_Radius3;
            float _Soft1,_Soft2,_Soft3;

            float _Intensity1,_Intensity2,_Intensity3;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            float hash(float3 p)
            {
                return frac(sin(dot(p,float3(12.9898,78.233,45.164)))*43758.5453);
            }

            float noise(float3 p)
            {
                float3 i=floor(p);
                float3 f=frac(p);

                float n000=hash(i);
                float n100=hash(i+float3(1,0,0));
                float n010=hash(i+float3(0,1,0));
                float n110=hash(i+float3(1,1,0));

                float n001=hash(i+float3(0,0,1));
                float n101=hash(i+float3(1,0,1));
                float n011=hash(i+float3(0,1,1));
                float n111=hash(i+float3(1,1,1));

                float3 u=f*f*(3-2*f);

                return lerp(
                    lerp(lerp(n000,n100,u.x),lerp(n010,n110,u.x),u.y),
                    lerp(lerp(n001,n101,u.x),lerp(n011,n111,u.x),u.y),
                    u.z
                );
            }

            float radialMask(float2 uv,float2 center,float radius,float soft)
            {
                float d=distance(uv,center);
                return 1-smoothstep(radius-soft,radius,d);
            }

            float3 nebulaLayer(
                float3 dir,
                float2 uv,
                float4 colA,
                float4 colB,
                float2 center,
                float radius,
                float soft,
                float intensity
            )
            {
                float n=noise(dir*_NoiseScale);
                float3 col=lerp(colA.rgb,colB.rgb,n);

                float mask=radialMask(uv,center,radius,soft);

                return col*mask*intensity;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.dir=normalize(v.vertex.xyz);
                return o;
            }

            fixed4 frag(v2f i):SV_Target
            {
                float3 dir=normalize(i.dir);

                float2 uv=dir.xy*0.5+0.5;

                float3 n1=nebulaLayer(dir,uv,_ColorA1,_ColorB1,_Center1.xy,_Radius1,_Soft1,_Intensity1);
                float3 n2=nebulaLayer(dir*1.7,uv,_ColorA2,_ColorB2,_Center2.xy,_Radius2,_Soft2,_Intensity2);
                float3 n3=nebulaLayer(dir*2.3,uv,_ColorA3,_ColorB3,_Center3.xy,_Radius3,_Soft3,_Intensity3);

                float3 col=n1+n2+n3;

                return float4(col,1);
            }

            ENDCG
        }
    }
}
