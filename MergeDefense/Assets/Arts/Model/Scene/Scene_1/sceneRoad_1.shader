Shader "Kein/Scene/Road_1"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Background+1"}
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
             #include "Lighting.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldLightDir : TEXCOORD2;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                if(v.color.r < 0.95)
                {
                    v.vertex.y -= v.color.r * 3;

                }
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.worldLightDir = WorldSpaceLightDir(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(i.worldLightDir);
                fixed3 diffuse = (dot(worldNormal, worldLightDir) * 0.5 + 0.5) * _LightColor0;
                col.rgb = lerp(col.rgb,col.rgb * diffuse,0.5);
                return col;
            }
            ENDCG
        }
    }
}
