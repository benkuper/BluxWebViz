Shader "Custom/Grid" {
	Properties {
		_LineColor ("Line Color", Color) = (1,1,1,.5)
		_BaseColor("Base Color",Color) = (0,0,0,.1)
		_Spacing("Spacing",Float) = 1
		_LineWidth("Line Width",Range(0,1)) = .1
		_NumSteps("Steps",Range(1,16)) = 1
		_StepFade("StepFade",Range(0,1)) = .1
		_Radius("Radius",Range(0,1)) = 5
		_Fade("Fade",Range(0,1)) = 1

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_MainTex("MainTex",2D) = "white"
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="AlphaTest" "IgnoreProjector"="True" }
		LOD 200
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		 
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		
		half _Glossiness;
		half _Metallic;

		fixed4 _LineColor;
		fixed4 _BaseColor;
		float _Spacing;
		fixed _LineWidth;
		fixed _Radius;
		fixed _Fade;
		fixed _NumSteps;
		fixed _StepFade;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = _BaseColor;
			
			float2 relXY = abs(float2(IN.uv_MainTex.x - .5, IN.uv_MainTex.y - .5));
			fixed dist = length(relXY);
			clip(_Radius - dist);

			int numSteps = pow(2, _NumSteps-1);
			int p = 0;

			for (int i = 1; i <= numSteps; i*=2)
			{
				float sp = _Spacing / i;
				float lw = _LineWidth / 100;
				float2 mp = fmod(relXY.xy, sp);

				if (mp.x < lw/2 || mp.y < lw/2 || mp.x > 1-lw/2 || mp.y > 1-lw/2)
				{
					c = _LineColor;
					c.a -= p*_StepFade;
					break;
				}
				p++;
			}
			
			
			if (_Fade > 0)
			{
				fixed fadeRad = _Radius - _Fade;
				c.a = min(lerp(0, 1, 1 - ((dist - fadeRad) / _Fade)), c.a);
			}

			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
