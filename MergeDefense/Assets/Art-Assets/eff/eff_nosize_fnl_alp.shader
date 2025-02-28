Shader "BF/Effect/eff_nosize_fnl_alp" {
	Properties {
		[Enum(CullMode)] _CULLENUM ("剔除模式", Float) = 0
		[Enum(show_all,0,hide_cut,4,show_cut,7)] _ztest_on ("遮挡显示", Float) = 4
		[Space(8)] [Header(texture _____________________________________________________________)] [Space(2)] [HDR] _tex_color ("贴图颜色", Vector) = (1,1,1,1)
		_tex ("贴图", 2D) = "white" {}
		_tex_uv_speed ("贴图UV速度向量", Vector) = (0,0,0,0)
		_tex_rotate ("贴图旋转【0到360°】", Range(0, 360)) = 0
		_tex_mask ("贴图遮罩", 2D) = "white" {}
		[Toggle(_MASK_UV_SPEED_CUSTOM)] _MASK_UV_SPEED_CUSTOM ("粒子custom1.xy(uv1.xy)控制mask uv偏移", Float) = 0
		_tex_mask_uv_speed ("贴图遮罩UV速度向量", Vector) = (0,0,0,0)
		_tex_mask_rotate ("贴图遮罩旋转【0到360°】", Range(0, 360)) = 0
		[Space(8)] [Header(noise _____________________________________________________________)] [Space(2)] [Toggle(_USE_UV_NOSIZE)] _use_uv_nosize ("使用UV扰动", Float) = 0
		_uv_nosize_tex ("UV扰动贴图", 2D) = "white" {}
		_uv_nosize_strength ("UV扰动横向宽度", Range(0, 1)) = 0.01
		_uv_nosize_speed ("UV扰动贴图速度向量", Vector) = (0,0,0,0)
		[Space(8)] [Header(fresnel _____________________________________________________________)] [Space(2)] [Toggle(_USE_FNL)] _use_fnl ("使用菲涅尔", Float) = 0
		[HDR] _fnl_color ("菲涅尔颜色", Vector) = (1,1,1,1)
		_fnl_size ("菲涅尔范围", Range(0.0001, 8)) = 1
		_fnl_intensity ("菲涅尔强度", Range(0, 1)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}