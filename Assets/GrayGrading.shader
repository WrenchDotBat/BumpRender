Shader "Custom/GrayGrading"
{
	Properties{
		_MeshDepth("Mesh Depth", Float) = 1.0
	}
		SubShader{
			Tags{
			"RenderType" = "Always"
		}
			Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			float _MeshDepth;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				//o.color.xyz = (mul(unity_ObjectToWorld, v.vertex).z + 1.0f / 2.0f) / _MeshDepth;

				o.color.xyz = (mul(unity_ObjectToWorld, v.vertex).z / _MeshDepth) + 0.5f;// * 2.0f;
				o.color.w = 1.0;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target { return i.color; }
			ENDCG

	}
	}
}
