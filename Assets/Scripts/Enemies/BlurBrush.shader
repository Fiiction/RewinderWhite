Shader "Custom/BlurBrush"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_CC("Color Cover", Color) = (1,1,1,0)
		_Alpha("Alpha", float) = 0
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
			float4 _CC;
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

			float3 grab()
			{
				
				float3 ret = float3(0, 0, 0);
				ret += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(float4(0, 0, 0, 0)))).rgb;
				ret += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(float4(0.3, 0, 0, 0)))).rgb;
				ret += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(float4(0, 0.2, 0, 0)))).rgb;
				ret += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(float4(-0.3, 0, 0, 0)))).rgb;
				ret += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(calcPos(float4(0, -0.2, 0, 0)))).rgb;

				ret /= 5;
				return ret;
				

			}
			
			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
				
				col.a *= _Alpha;
				if (col.a <= 0.01)
					return col;
				float3 tmp = grab();
				tmp = lerp(tmp, _CC.rgb, _CC.a);
				col.rgb = tmp;
				
				//col.rgb = fixed3(0, 1, 0);
				return col;
			}
			ENDCG
		}
	}
}
