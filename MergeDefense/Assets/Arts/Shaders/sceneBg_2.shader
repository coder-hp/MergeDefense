Shader "Kein/Scene/bg_2"
{
    Properties
    {
        _CullUpDown("上下裁切",Range(0,1)) = 0
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Background+3"}
        //Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            ZWrite Off
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
                float4 pos : SV_POSITION;
            };
            float _CullUpDown;
            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if(i.uv.y > _CullUpDown || (1 - i.uv.y) > _CullUpDown)
                discard;
                return col;
            }
            ENDCG
        }
    }
}
