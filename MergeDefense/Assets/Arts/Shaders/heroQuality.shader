Shader "Kein/Hero/heroQuality"
{
    Properties
    {
        [IntRange]_ColorID("ColorID",Range(1,4))=1
        
        _MainTex ("Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Color1_1("Color1_1",color) = (1,1,1,1)
        _Color1_2("Color1_2",color) = (1,1,1,1)
        _Color1_3("Color1_3",color) = (1,1,1,1)
        _Color2_1("Color1_1",color) = (1,1,1,1)
        _Color2_2("Color1_2",color) = (1,1,1,1)
        _Color2_3("Color1_3",color) = (1,1,1,1)
        _Color3_1("Color1_1",color) = (1,1,1,1)
        _Color3_2("Color1_2",color) = (1,1,1,1)
        _Color3_3("Color1_3",color) = (1,1,1,1)
        _Color4_1("Color1_1",color) = (1,1,1,1)
        _Color4_2("Color1_2",color) = (1,1,1,1)
        _Color4_3("Color1_3",color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType " = "Transparent"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            //Cull Off
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
                float4 vertex : SV_POSITION;
            };
            float _ColorID;
            sampler2D _MainTex,_Mask;
            float4 _MainTex_ST,_Mask_ST;
            fixed4 _Color1_1,_Color1_2,_Color1_3;
            fixed4 _Color2_1,_Color2_2,_Color2_3;
            fixed4 _Color3_1,_Color3_2,_Color3_3;
            fixed4 _Color4_1,_Color4_2,_Color4_3;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed r = tex2D(_Mask, i.uv).r;

                // 颜色1
                if(_ColorID > 0.5 && _ColorID < 1.5)
                {
                    if(r < 0.1 )
                    {
                        col *= _Color1_1;
                    }
                    else if(r < 0.9 && r > 0.1)
                    {
                        col *= _Color1_2;
                    }
                    else
                    {
                         col *= _Color1_3;
                    }
                }
                else if(_ColorID > 1.5 && _ColorID < 2.5)
                {
                    if(r < 0.1 )
                    {
                        col *= _Color2_1;
                    }
                    else if(r < 0.9 && r > 0.1)
                    {
                        col *= _Color2_2;
                    }
                    else
                    {
                         col *= _Color2_3;
                    }

                }
                else if(_ColorID > 2.5 && _ColorID < 3.5)
                {
                    if(r < 0.1 )
                    {
                        col *= _Color3_1;
                    }
                    else if(r < 0.9 && r > 0.1)
                    {
                        col *= _Color3_2;
                    }
                    else
                    {
                         col *= _Color3_3;
                    }

                }
                else
                {
                    if(r < 0.1 )
                    {
                        col *= _Color4_1;
                    }
                    else if(r < 0.9 && r > 0.1)
                    {
                        col *= _Color4_2;
                    }
                    else
                    {
                         col *= _Color4_3;
                    }
                }
                return col;
            }
            ENDCG
        }
    }
}
