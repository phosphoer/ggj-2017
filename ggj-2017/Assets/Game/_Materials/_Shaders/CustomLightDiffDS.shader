Shader "Custom/Opaque/CustomLightDiffDS" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Lighting ("Light Ramp", 2D) = "grey" {}

		_BubbleFade ("Bubble Fade", Range(0,1)) = 0
		_BubbleColor ("Bubble Color", Color) = (1,1,1,1)
		_ColorDist ("Color Distance", float) = 0
		_BubbleCenter ("Bubble Cetner", Vector) = (0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Toon
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;
		float3 _BubbleCenter;
		half _ColorDist;
		fixed4 _BubbleColor;
		half _BubbleFade;

		struct SurfaceOutCustom{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Alpha;
			fixed2 UVs;
		};

		void surf (Input IN, inout SurfaceOutCustom o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Alpha = c.a;
			o.UVs = IN.uv_MainTex;

			float d = distance(IN.worldPos,_BubbleCenter);
			if(d<_ColorDist) c.rgb = lerp(c.rgb,_BubbleColor,_BubbleFade);

			o.Albedo = c.rgb;
		}

		sampler2D _Lighting;

		fixed4  LightingToon (SurfaceOutCustom s, fixed3 lightDir, fixed atten){
			half NdotL = dot(s.Normal, lightDir);
			NdotL = tex2D(_Lighting, fixed2(s.UVs.x,NdotL));

			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * NdotL * atten * 2;
			c.a = s.Alpha;

			return c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
