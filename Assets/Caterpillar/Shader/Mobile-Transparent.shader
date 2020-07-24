// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/Transparent" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd alpha:fade

		sampler2D _MainTex;
		fixed _Alpha;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = _Alpha;
		}
		ENDCG
	}

	Fallback "Mobile/Diffuse"
}
