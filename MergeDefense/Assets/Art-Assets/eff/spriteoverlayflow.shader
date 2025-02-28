Shader "UI/SpriteOverlayFlow" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[Toggle(_MAIN_UV_FLOW)] _MAIN_UV_FLOW ("开启贴图UV移动", Float) = 0
		_MainFlowSpeed ("贴图UV移动速度", Vector) = (0,0,0,0)
		[Toggle(_USE_OVERLAY)] _USE_OVERLAY ("开启覆盖", Float) = 0
		_OverlayTex ("覆盖贴图", 2D) = "white" {}
		[HDR] _OverlayColor ("覆盖颜色", Vector) = (1,1,1,1)
		_OverlayFlowSpeed ("覆盖移动速度", Vector) = (0,0,0,0)
		_OverlayRotation ("覆盖旋转角度", Range(0, 360)) = 0
		[HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector] _ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}