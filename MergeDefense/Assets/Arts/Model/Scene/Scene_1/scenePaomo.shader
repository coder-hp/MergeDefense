Shader "Kein/Scene/Paomo"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Range(0.1,100)) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Background+1"}
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            v2f vert (appdata v)
            {
                v2f o;
                float x = v.vertex.x;
                float y = v.vertex.y;
                float xy = v.vertex.y * v.vertex.x * _Speed;
                x += sin(_Time.y * 0.05 * xy ) * 0.05;
                y += sin(_Time.y * 0.05 * xy) * 0.05;
                v.vertex.x = x;
                v.vertex.y = y;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //i.uv.x -= sin(_Time.y * 0.01);
                //i.uv.y -= sin(_Time.y) * 0.05;
                fixed4 col = tex2D(_MainTex, i.uv);
                // if(col.r < 0.98)
                // discard;
                return _Color;
            }
            ENDCG
        }
    }
}
