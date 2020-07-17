Shader "Custom/CircleMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_CenPosX("Center Pos X", Float) = 0
		_CenPosY("Center Pos Y", Float) = 0
		_Rad("Radius", Float) = 1
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _CenPosX, _CenPosY, _Rad;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col *= _Color;

				//float4 cenPos = UnityObjectToClipPos(float4(0, 0, 0, 0));
				float2 cenPos = float2(_CenPosX, _CenPosY);
				float2 deltaPos = (cenPos - i.vertex.xy);
				float dist = deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y;
				if(dist < _Rad * _Rad * 0.98)
					col.a = 0;
                return col;
            }
            ENDCG
        }
    }
}
