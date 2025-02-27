Shader "Kein/Scene/attackRange"
{
    Properties
    {
        _Color("Color",color)=(1,1,1,1)
        _OutLineCol("OutLineCol",color)=(1,1,1,1)
        [HideInInspector]_OutLineRange("OutLineRange",float) = 1
        _SetOutLine("SetOutLine",Range(0.01,1))= 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType " = "Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
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
            fixed4 _Color,_OutLineCol;
            float _OutLineRange,_SetOutLine;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = _Color;

                float2 center = float2(0.5,0.5);
                float dis = distance(i.uv,center);
                if(dis>0.5)
                discard;

                if(dis > (0.5 - _OutLineRange * 0.25 * _SetOutLine))
                col = _OutLineCol;
                
                return col;
            }
            ENDCG
        }
    }
}
