Shader "BF/Effect/eff_dissolve_add" {
	Properties {
		[Enum(CullMode)] _CULLENUM ("剔除模式", Float) = 0
		[Enum(show_all,0,hide_cut,4,show_cut,7)] _ztest_on ("遮挡显示", Float) = 4
		[Space(8)] [Header(texture _____________________________________________________________)] [Space(2)] [HDR] _tex_color ("贴图颜色", Vector) = (1,1,1,1)
		_tex ("贴图", 2D) = "white" {}
		_tex_uv_speed ("贴图UV速度向量", Vector) = (0,0,0,0)
		_tex_rotate ("贴图旋转【0到360°】", Range(0, 360)) = 0
		_tex_mask ("贴图遮罩", 2D) = "white" {}
		[Toggle(_MASK_UV_SPEED_CUSTOM)] _MASK_UV_SPEED_CUSTOM ("粒子custom1.zw(uv1.xy)控制mask uv偏移", Float) = 0
		_tex_mask_uv_speed ("贴图遮罩UV速度向量", Vector) = (0,0,0,0)
		_tex_mask_rotate ("贴图遮罩旋转【0到360°】", Range(0, 360)) = 0
		[Space(8)] [Header(dissove _____________________________________________________________)] [Space(2)] _diss_tex ("溶解贴图", 2D) = "white" {}
		_diss_tex_rotate ("溶解贴图旋转【0到360°】", Range(0, 360)) = 0
		[ScaleOffset] _diss_tex_offset ("溶解贴图偏移", Vector) = (0,0,0,0)
		[HDR] _diss_edge_color ("溶解边颜色", Vector) = (1,1,1,1)
		_diss_edge_width ("溶解边宽度", Range(0, 1)) = 0.01
		_diss_edge_smoothness ("溶解边外侧平滑", Range(0, 3)) = 1.8
		_diss_smoothness ("溶解边内侧平滑", Range(0, 1)) = 0.01
		[IntRange] _diss_alpha_clip ("溶解使用alpha做阀值", Range(0, 1)) = 0
		_diss_clip ("溶解阀值", Range(-0.2, 1.2)) = 0.2
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