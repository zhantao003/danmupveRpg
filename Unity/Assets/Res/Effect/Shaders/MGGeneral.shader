// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "VGame/Effects/MG General" {
	Properties{
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Intensity("Intensity", Float) = 1
		_MainTex("Base Texture", 2D) = "white" {}
		[HideInInspector]SrcMode ("SrcMode", int) = 1
		[HideInInspector]DstMode ("DstMode", int) = 1
		[HideInInspector]CullMode ("CullMode", int) = 0

		[HideInInspector]_Offset ("Offset", int) = 0
		[HideInInspector]_ZTestFactor ("ZTestFactor", int) = 4

		[HideInInspector]_MaskTex ("Mask (RGB)", 2D) = "white" {}
		[HideInInspector]_Cutout ("Cutout Step", Float) = 0.5
	}

	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend One [DstMode], One One
		Cull [CullMode] Lighting Off ZWrite Off
		//Offset [_Offset], [_Offset]
		ZTest [_ZTestFactor]

		SubShader{
			Pass{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag 
				#pragma multi_compile __ _MASK_ON
				#pragma multi_compile __ Clip_ON
				#pragma multi_compile __ USE_LINEAR_INPUT
				#pragma multi_compile __ USE_RGB_ALPHA

				#include "UnityCG.cginc"
			 //   #include "/Commond.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _TintColor;
				float _Intensity;
				half DstMode;

				#if Clip_ON
				float _Cutout;
				#endif

				#if _MASK_ON
				sampler2D _MaskTex;
				float4 _MaskTex_ST;
				#endif

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					#if _MASK_ON
					float2 texcoord2: TEXCOORD1;
					#endif
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 uvmain : TEXCOORD0;
					#if _MASK_ON
					float2 uvmask : TEXCOORD1;
					#endif
				};


				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uvmain = TRANSFORM_TEX(v.texcoord,_MainTex);

					#if _MASK_ON
						o.uvmask = TRANSFORM_TEX(v.texcoord2,_MaskTex);
					#endif
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					#if _MASK_ON
						fixed4 mask = tex2D (_MaskTex, i.uvmask);
					#endif

					#if !USE_LINEAR_INPUT
						fixed4 tex = tex2D(_MainTex, i.uvmain);
					#else
						fixed4 tex = tex2D(_MainTex, i.uvmain);
						_TintColor = _TintColor;
					#endif

					#if USE_RGB_ALPHA
						tex.a = any(tex.rgb) ? 1 : 0;
					#endif

					fixed4 col = _Intensity * i.color * _TintColor * tex;

					#if Clip_ON
						col.a = step(_Cutout, tex.a) * col.a;
					#endif

					#if _MASK_ON
						col.a = col.a * mask.r;
					#endif

#if USE_LINEAR_INPUT
					col.rgb = col.rgb * 0.6;
					col.rgb = (col.rgb / ((col.rgb * 0.9661836) + 0.180676)) * 1.2;
#endif
					col.rgb *= col.a;
					col.a = DstMode > 2 ? col.a : 0;
					return col;
				}
				ENDCG
			}
		}
	}
	CustomEditor "ParticleMaterialInspector"
}
