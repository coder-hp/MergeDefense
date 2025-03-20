Shader "Kein/Scene/Boat_1"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _WaterDown("WaterDown",Range(-1,2)) = 0
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
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldLightDir : TEXCOORD2;
                fixed4 color : COLOR;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaterDown;
            v2f vert (appdata v)
            {
                v2f o;
                
                float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
                worldPos.y -= v.vertex.z * 0.8;
                o.color = worldPos.z > _WaterDown ? fixed4(0.8,0.8,0.8,1) : fixed4(1,1,1,1);
                
                

                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                //o.vertex = UnityObjectToClipPos(v.vertex);

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
                col.rgb = lerp(col.rgb,col.rgb * diffuse,0.2);
                col.rgb *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
