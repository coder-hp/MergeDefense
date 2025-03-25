Shader "Kein/Scene/Ground_1"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DarkenTex ("DarkenTex", 2D) = "white" {}
        _BrightenTex ("BrightenTex", 2D) = "white" {}
        _WaveSpeed("WaveSpeed",Range(0.1,10)) = 1
        _DayAndNight("DayAndNight",Range(0,1)) = 0
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
                float2 uv2 : TEXCOORD1;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldLightDir : TEXCOORD2;
                fixed4 color : COLOR;
            };
            fixed4 _Color;
            sampler2D _MainTex,_DarkenTex,_BrightenTex;
            float _WaveSpeed,_DayAndNight;
            v2f vert (appdata v)
            {
                v2f o;
                o.color = fixed4(1,1,1,1);
                if(v.color.r < 0.95)
                {
                    v.vertex.y -= v.color.r * 3;
                }

                if(v.color.g < 0.5)
                {
                    float x = v.vertex.x;
                    float y = v.vertex.y;
                    float xy = v.vertex.y * v.vertex.x * _WaveSpeed;
                    x += sin(_Time.y * 0.05 * xy ) * 0.05;
                    y += sin(_Time.y * 0.05 * xy) * 0.05;
                    v.vertex.x = x;
                    v.vertex.y = y;
                }
                if(v.color.b < 0.5)
                {
                    v.vertex.x += sin(_Time.y * 0.2 + v.vertex.z) * 0.5;
                    v.vertex.y += sin(_Time.y * 0.5 + v.vertex.z) * 0.2;
                    o.color.rgb *= fixed3(0.5,0.8,0.8);
                }

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.worldLightDir = WorldSpaceLightDir(v.vertex);

                o.uv.xy = v.uv;
                o.uv.zw = v.uv2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy) * _Color;
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(i.worldLightDir);
                fixed3 diffuse = (dot(worldNormal, worldLightDir) * 0.5 + 0.5) * _LightColor0;
                col.rgb = lerp(col.rgb,col.rgb * diffuse,0.2);


                fixed4 darkenMap = tex2D(_DarkenTex, i.uv.zw);
                fixed4 brightenMap = tex2D(_BrightenTex, i.uv.zw);


                fixed4 finalCol;
                fixed4 nightCol;
                nightCol.rgb = lerp(col.rgb * darkenMap.rgb,col.rgb * brightenMap * 1.1 * i.color.rgb ,brightenMap.r);

                finalCol.rgb = lerp(col.rgb,nightCol.rgb,_DayAndNight);
                return finalCol;
            }
            ENDCG
        }
    }
}
