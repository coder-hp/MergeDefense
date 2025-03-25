Shader "Kein/Scene/Boat_1"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _WaterDown("WaterDown",Range(-1,2)) = 0
        _WaveSpeed("WaveSpeed",Range(0.1,3)) = 1
        _BoatShedSpeed("BoatShedSpeed",Range(0,1)) = 1
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

            

            float4x4 Kein_Rotation4x4(float3 _rotation)
            {
                float radX = radians(_rotation.x);
                float radY = radians(_rotation.y);
                float radZ = radians(_rotation.z);

                float sinX = sin(radX);
                float cosX = cos(radX);
                float sinY = sin(radY);
                float cosY = cos(radY);
                float sinZ = sin(radZ);
                float cosZ = cos(radZ);

                return float4x4(
                cosY * cosZ, -cosY * sinZ, sinY, 0.0,
                cosX * sinZ + sinX * sinY * cosZ, cosX * cosZ - sinX * sinY * sinZ, -sinX * cosY, 0.0,
                sinX * sinZ - cosX * sinY * cosZ, sinX * cosZ + cosX * sinY * sinZ, cosX * cosY, 0.0,
                0.0, 0.0, 0.0, 1.0
                );
            }
            float4 Kein_Rotation(float3 _rotation, float4 _vertex)
            {
                return float4(mul(Kein_Rotation4x4(_rotation), _vertex));
            }
            inline float RandomValue(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 76.154))) * 45359.6543);
            }

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
                //fixed4 color : COLOR;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaterDown,_WaveSpeed,_BoatShedSpeed;
            v2f vert (appdata v)
            {
                v2f o;
                if(v.color.g < 0.5)
                {
                    v.vertex.z += 0.626;
                    v.vertex.y -= 0.141;
                    v.vertex = Kein_Rotation(float3(-_Time.y * 1000 * _BoatShedSpeed,0,0),v.vertex);
                    v.vertex.z -= 0.626;
                    v.vertex.y += 0.141;
                }
                float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
                if(v.color.r < 0.5)
                {
                    float time = _Time.y * worldPos.x * worldPos.y * _WaveSpeed * 0.5;
                    worldPos.x += sin(time) * 0.05;
                    worldPos.y += sin(time) * 0.05;
                }
                else
                {
                     worldPos.x += -sin(_Time.y * 2) * 0.03;
                     worldPos.y += sin(_Time.y * 2) * 0.1;
                }
                worldPos.y -= v.vertex.z * 0.8;

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
                //col.rgb *= i.color;
                return col;
            }


            
            ENDCG
        }
    }
}
