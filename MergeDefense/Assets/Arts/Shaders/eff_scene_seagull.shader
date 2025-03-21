Shader "Kein/Scene/Seagull"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DirectionX("DirectionX",float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent+5" "IgnoreProjector" = "True" "RenderType " = "Transparent"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual

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
            float _DirectionX;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x *= i.color.r < 0.5 ? -1 : 1;
                //if(i.color.r<0.5)
                //i.uv.x *= -1;
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                return col;
            }
            ENDCG
        }
    }
}
