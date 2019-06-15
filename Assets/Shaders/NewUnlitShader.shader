Shader "Custom/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_MaskTex("Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float2 muv : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 muv : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
            sampler2D _MainTex,_MaskTex;
            float4 _MainTex_ST,_MaskTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.muv = TRANSFORM_TEX(v.uv, _MaskTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col *= _Color;
				fixed4 mcol = tex2D(_MaskTex, i.muv);
				if(mcol.a>0.5F)
					col.a = 0;
                return col;
            }
            ENDCG
        }
    }
}
