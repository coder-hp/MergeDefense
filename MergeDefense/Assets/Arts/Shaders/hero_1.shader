Shader "Kein/Hero/hero_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _MatCap ("MatCap", 2D) = "white" {}
        _RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPower", Range(0.000001, 3.0)) = 0.1

        _Specular("Specular", Color) = (1,1,1,1)
        _Gloss("Gloss", Range(8.0,256)) = 20

        [Toggle]_IsOutLine("IsOutLine",float) = 1.0
        _OutlineCol("OutlineCol", Color) = (1,0,0,1)
        _OutlineFactor("OutlineFactor", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 pos : SV_POSITION;

                float3 worldNormal : TEXCOORD1;
                //float3 worldViewDir : TEXCOORD2;
                float3 worldLightDir : TEXCOORD2;
                float3 worldPos:TEXCOORD3;
                //float2 matcapUV : TEXCOORD4;
            };

            sampler2D _MainTex,_Mask,_MatCap;
            float4 _MainTex_ST;
            fixed4 _RimColor;
            float _RimPower;
            fixed4 _Specular;
            float _Gloss;


            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //o.worldViewDir = _WorldSpaceCameraPos.xyz - o.worldPos;

                o.worldLightDir = WorldSpaceLightDir(v.vertex);

                o.uv.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
                o.uv.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
                o.uv.zw = o.uv.zw * 0.5 + 0.5;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed3 matcap = tex2D(_MatCap, i.uv.zw).rgb;
                fixed mask = tex2D(_Mask, i.uv.xy).r;
                
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(i.worldLightDir);

                fixed3 diffuse = (dot(worldNormal, worldLightDir) * 0.5 + 0.5) * _LightColor0;
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                // 计算 RimLight
                //float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                float rim = 1 - max(0, dot(viewDir, worldNormal));
                fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);

                fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal, halfDir)), _Gloss);


                col.rgb = lerp(col.rgb,col.rgb * diffuse ,0.2);
                col.rgb += rimColor;
                col.rgb = lerp(col.rgb + specular, col.rgb , mask);
                col.rgb *= matcap;
                return col;
            }
            ENDCG
        }


        Pass
        {
            Cull Front
            Offset 1,1
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
  
            fixed4 _OutlineCol;
            float _OutlineFactor,_IsOutLine;
  
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                if(_IsOutLine)
                {
                    float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                    float2 offset = TransformViewToProjection(vnormal.xy);
                    o.pos.xy += offset * _OutlineFactor * 0.1f;
                }
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(_IsOutLine);
                return _OutlineCol;
            }
            ENDCG
        }
    }
}
