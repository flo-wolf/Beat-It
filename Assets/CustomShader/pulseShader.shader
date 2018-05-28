Shader "Unlit/pulseShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaTex ("Alpha" , 2D) = "white" {}
		_PulseAmount("Let it pulse b8b", float) = 0.3
	}
	SubShader
	{
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  "DisableBatching" = "True" }		
     LOD 100

     ZWrite off
     Blend SrcAlpha OneMinusSrcAlpha 
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			float _PulseAmount;

			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz *= 0.25 * sin(_PulseAmount * _Time.y) + 2;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_AlphaTex , i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
