Shader "Custom/Gravite"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_G("G", float) = 1
		_Alpha("Alpha", float) = 1
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

		GrabPass
		{
			"_GrabTexture"
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

			#include "UnityCG.cginc"

			sampler2D _GrabTexture;
			sampler2D _MainTex;
			float _G;
			float _Alpha;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//float4 uvgrab : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 calcPos(float4 pos)
			{
				float4 v = UnityObjectToClipPos(pos);
				//v = ComputeGrabScreenPos(pos);
				float4 ret;
				#if UNITY_UV_STARTS_AT_TOP  
				float SCALE = -1.0;
				#else  
				float SCALE = 1.0;
				#endif  
				ret.xy = (float2(v.x, v.y*SCALE) + v.w) * 0.5;
				ret.zw = v.zw;
				return ret;
			}

			
			fixed4 frag(v2f i) : SV_Target
			{
				
				float2 pos = i.uv - float2(0.5,0.5);
				float dist = sqrt(pos.x * pos.x + pos.y * pos.y),rDist;
				float2 vec = pos / dist;


				if (dist > 0.5)
					return fixed4(0, 0, 0, 0);

				/*
				if (dist < 0.5/(_G+1.0))
					rDist = _G * dist;
				else
					rDist = 0.5 - (0.5 - dist) / _G;
					*/
				rDist = 0.5 - pow((0.5 - dist) / 0.5, _G) * 0.5;
				float4 grabPos = float4(0,0,0,0);
				grabPos.xy = rDist * vec;
				fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(grabPos)));
				col.a = _Alpha;
				return col;
				
			}
			ENDCG
		}
	}
}
