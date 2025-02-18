Shader "Kein/Hero/hero_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPower", Range(0.000001, 3.0)) = 0.1

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
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;

                float3 worldNormal : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _RimColor;
            float _RimPower;

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);


                fixed3 worldNormal = normalize(i.worldNormal);
                // 计算 RimLight
                float3 worldViewDir = normalize(i.worldViewDir);
                float rim = 1 - max(0, dot(worldViewDir, worldNormal));
                fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);

                col.rgb += rimColor;
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
