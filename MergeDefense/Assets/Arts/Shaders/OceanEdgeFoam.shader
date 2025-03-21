Shader "Custom/CartoonFoam"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0, 0.5, 1, 1)
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamIntensity ("Foam Intensity", Float) = 1.0
        _FoamWidth ("Foam Width", Float) = 0.1
        _WaterHeight ("Water Height", Float) = 0.0 // 水面高度
        _NoiseTex ("Noise Texture", 2D) = "white" {} // 噪声纹理
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float4 screenPos : TEXCOORD1;
            };

            float4 _WaterColor;
            float4 _FoamColor;
            float _FoamIntensity;
            float _FoamWidth;
            float _WaterHeight;
            sampler2D _NoiseTex;
            sampler2D _CameraDepthTexture; // 深度纹理

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex); // 计算屏幕空间坐标
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 获取深度纹理中的深度值
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
                float surfaceDepth = i.screenPos.w; // 水面深度

                // 计算深度差值
                float depthDifference = sceneDepth - surfaceDepth;

                // 检测接触区域
                float foam = smoothstep(0, _FoamWidth, depthDifference) * _FoamIntensity;

                // 采样噪声纹理
                float noise = tex2D(_NoiseTex, screenUV * 10).r; // 调整噪声纹理的缩放

                // 卡通化泡沫效果
                foam *= noise; // 使用噪声纹理增强泡沫效果

                // 混合颜色
                fixed4 waterColor = _WaterColor;
                fixed4 foamColor = _FoamColor * foam;
                fixed4 finalColor = waterColor + foamColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
