Shader "Custom/VolumetricNebulaMulticolor"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (0.4,0.2,1,1)
        _Color2 ("Color 2", Color) = (1,0.2,0.5,1)
        _Color3 ("Color 3", Color) = (0.2,0.8,1,1)
        _Color4 ("Color 4", Color) = (1,0.6,0.2,1)

        _NoiseScale ("Noise Scale", Float) = 1.5
        _Density ("Density", Float) = 1.2
        _Brightness ("Brightness", Float) = 2

        _Steps ("Ray Steps", Float) = 32
        _StepSize ("Step Size", Float) = 0.2

        _FlowSpeed ("Gas Flow Speed", Float) = 0.02
        _ColorMixScale ("Color Mix Scale", Float) = 2
    }

    SubShader
    {
        Tags { "Queue"="Background" }

        Cull Front
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color1,_Color2,_Color3,_Color4;

            float _NoiseScale;
            float _Density;
            float _Brightness;

            int _Steps;
            float _StepSize;

            float _FlowSpeed;
            float _ColorMixScale;

            struct appdata
            {
                float4 vertex:POSITION;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float3 dir:TEXCOORD0;
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

            float fbm(float3 p)
            {
                float f=0;
                float amp=0.5;

                for(int i=0;i<5;i++)
                {
                    f+=noise(p)*amp;
                    p*=2;
                    amp*=0.5;
                }

                return f;
            }

            float densityField(float3 p)
            {
                float d=fbm(p*_NoiseScale);
                d=saturate(d-0.4);
                return d*_Density;
            }

            float3 nebulaColor(float3 p)
            {
                float n1=fbm(p*_ColorMixScale);
                float n2=fbm((p+5)*_ColorMixScale);
                float n3=fbm((p+10)*_ColorMixScale);

                float4 w=float4(n1,n2,n3,1-n1);

                w=max(w,0);
                w/=dot(w,1);

                return
                    _Color1.rgb * w.x +
                    _Color2.rgb * w.y +
                    _Color3.rgb * w.z +
                    _Color4.rgb * w.w;
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
                float3 rayDir=normalize(i.dir);
                float3 rayPos=_WorldSpaceCameraPos;

                float t=0;
                float3 col=0;

                float time=_Time.y*_FlowSpeed;

                for(int step=0;step<_Steps;step++)
                {
                    float3 p=rayPos+rayDir*t;

                    p+=float3(time,time*0.5,time*0.2);

                    float density=densityField(p);

                    float3 gasColor=nebulaColor(p);

                    col+=gasColor*density*0.04;

                    t+=_StepSize;
                }

                col*=_Brightness;

                return float4(col,1);
            }

            ENDCG
        }
    }
}
