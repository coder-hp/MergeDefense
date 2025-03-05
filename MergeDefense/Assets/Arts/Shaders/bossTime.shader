Shader "Kein/UI/bossTime"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType " = "Transparent"}

        Pass
        {
            Blend SrcAlpha One
			//ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			//ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;
                //fixed4 col = _Color * i.color;
                col.a *= abs(sin(_Time.y * 1.5));

                return col;
            }
            ENDCG
        }
    }
}
