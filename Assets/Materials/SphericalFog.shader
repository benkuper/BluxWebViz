Shader "Unlit/SphericalFog"
{
    Properties
    {
        _FogCenter("FogCenter/Radius", Vector) = (0,0,0,0.5)
        _FogColor("FogColor", Color) = (1,1,1)
        _InnerRatio("InnerRatio", Range(0.0,0.9)) = 0.5
        _Density("Density", Range(0.0,1.0)) = 0.5
        _Steps("Steps", Range(0.0,100.0)) = 10.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _FogCenter;
            fixed4 _FogColor;
            float _InnerRatio;
            float _Density;
            sampler2D _CameraDepthTexture;
            float _Steps;

            float CalculateFogIntensity(
                float3 sphereCenter,
                float sphereRadius,
                float innerRatio,
                float density,
                float3 cameraPos,
                float3 viewDir,
                float maxDis)
            {
                float3 localCam = cameraPos - sphereCenter;
                float a = dot(viewDir, viewDir);
                float b = 2 * dot(viewDir, localCam);
                float c = dot(localCam, localCam) - sphereRadius * sphereRadius;
                float d = b * b - 4 * a * c;
                if (d <= 0.0f)
                {
                    return 0;
                }
                float DS = sqrt(d);
                float dis = max((-b-DS)/2*a,0);
                float dis2 = max((-b+DS)/2*a,0);

                float backDepth = min(maxDis, dis2);
                float sample = dis;
                float stepDis = (backDepth - dis) / _Steps;
                float stepContribution = density;

                float centerValue = 1/(1-innerRatio);

                float clarity = 1;
                for (int i = 0; i < _Steps; i++)
                {
                    float3 position = localCam + viewDir * sample;
                    float val = saturate(centerValue * (1-length(position)/sphereRadius));
                    float fogAmount = saturate(val * stepContribution);
                    clarity *= (1-fogAmount);
                    sample += stepDis;
                }
                return 1-clarity;
            }

            struct v2f
            {
                float3 view : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                float4 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.view = wPos.xyz -_WorldSpaceCameraPos;
                o.projPos = ComputeScreenPos(o.pos);
                float inFrontOf = (o.pos.z/o.pos.w) > 0;
                o.pos.z *= inFrontOf;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
                float depth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj (_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
                float3 viewDir = normalize(i.view);
                float fog = CalculateFogIntensity(
                    _FogCenter.xyz,
                    _FogCenter.w,
                    _InnerRatio,
                    _Density,
                    _WorldSpaceCameraPos,
                    viewDir,
                    depth
                );
                col.rgb = _FogColor.rgb;
                col.a = fog;
                return col;
            }
            ENDCG
        }
    }
}
