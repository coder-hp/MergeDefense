Shader "Kein/UI/weaponShopPanelLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskColor("MaskColor",color)=(1,1,1,1)
        _FlowColor("FlowColor",color)=(1,1,1,1)
        _Flowing("Flowing",Range(0,1))=0
        _FlowTex ("FlowTexture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType " = "Transparent"}

        Pass
        {
            Stencil {
                Ref 1
                Comp equal
                Pass keep
                ZFail decrWrap
            }


            Blend SrcAlpha OneMinusSrcAlpha
			//ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			//ZTest LEqual


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
            fixed4 _MaskColor,_FlowColor;
            float _Flowing;
            sampler2D _MainTex,_FlowTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.y += _Flowing * 550;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //i.uv.y -= _Flowing;
                fixed4 col = tex2D(_MainTex, i.uv) * _MaskColor;
                
                fixed4 flowCol = tex2D(_FlowTex, i.uv) * _FlowColor;
                col += flowCol;
                col.rgb = lerp(col.rgb,col.rgb * 0.1,i.uv.y);
                return col;
            }
            ENDCG
        }
        
    }
}
