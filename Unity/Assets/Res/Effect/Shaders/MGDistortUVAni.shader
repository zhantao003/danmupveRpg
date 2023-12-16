// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "VGame/Effects/MG Distort UV Ani" {
	Properties{
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Intensity("Intensity", Float) = 1
		_MainTex("Base Texture", 2D) = "white" {}
		_U ("Base U", Float) =0
		_V ("Base V", Float) =0
		[HideInInspector]SrcMode ("SrcMode", int) = 1
		[HideInInspector]DstMode ("DstMode", int) = 1
		[HideInInspector]CullMode ("CullMode", int) = 0

		[HideInInspector]_MaskTex ("Mask (RGB)", 2D) = "white" {}
		[HideInInspector]_MaskU ("Mask U", Float) =0
		[HideInInspector]_MaskV ("Mask V", Float) =0
		[HideInInspector]_NoiseTex ("Distort Texture (RG)", 2D) = "white" {}
		[HideInInspector]_HeatTime  ("Heat Time", range (-1,1)) = 0
		[HideInInspector]_ForceX  ("Strength X", range (0,1)) = 0.1
		[HideInInspector]_ForceY  ("Strength Y", range (0,1)) = 0.1

		[HideInInspector]_Offset ("Offset", int) = 0
		[HideInInspector]_ZTestFactor ("ZTestFactor", int) = 4
		[HideInInspector]_Cutout ("_Cutout", Float) = 0.5

		//MASK SUPPORT ADD
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
		//MASK SUPPORT END

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
				#pragma multi_compile __ _DISTORT_ON
				#pragma multi_compile __ Clip_ON
				#pragma multi_compile __ USE_LINEAR_INPUT
				#pragma multi_compile __ USE_RGB_ALPHA

				#include "UnityCG.cginc"
				//#include "../QSLighting.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _TintColor;
				float _Intensity;
				float _U,_V;
				half DstMode;

				#if Clip_ON
				float _Cutout;
				#endif
			
				#if _MASK_ON
				sampler2D _MaskTex;
				float4 _MaskTex_ST;
				float _MaskU,_MaskV;
				#endif

				#if _DISTORT_ON
				fixed _ForceX;
				fixed _ForceY;
				fixed _HeatTime;
				sampler2D _NoiseTex;
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

				////MASK SUPPORT ADD
				//Stencil
				//{
				//	Ref[_Stencil]
				//	Comp[_StencilComp]
				//	Pass[_StencilOp]
				//	ReadMask[_StencilReadMask]
				//	WriteMask[_StencilWriteMask]
				//}
				//ColorMask[_ColorMask]
				////MASK SUPPORT END

				fixed4 frag(v2f i) : SV_Target
				{
					#if _MASK_ON
						fixed4 mask = tex2D (_MaskTex, i.uvmask + float2(_MaskU, _MaskV) * _Time.z);
					#endif

					#if _DISTORT_ON
						fixed4 offsetColor1 = tex2D(_NoiseTex, i.uvmain + _Time.xz*_HeatTime);
						fixed4 offsetColor2 = tex2D(_NoiseTex, i.uvmain + _Time.yx*_HeatTime);
						i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceX;
						i.uvmain.y += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceY;
					#endif
					
					#if !USE_LINEAR_INPUT
						fixed4 tex = tex2D(_MainTex, i.uvmain + float2(_U, _V) * _Time.z);
						half4 col = _Intensity * i.color * _TintColor * tex;
					#else
						fixed4 tex = tex2D(_MainTex, i.uvmain + float2(_U, _V) * _Time.z);
						half4 col = _Intensity * i.color * _TintColor * tex;
					#endif

					#if USE_RGB_ALPHA
						tex.a = any(tex.rgb) ? 1 : 0;
					#endif

					#if Clip_ON
						col.a = step(_Cutout, tex.a) * col.a;
					#endif

					#if _MASK_ON
						col.a = col.a * mask.r;
					#endif

					col.rgb *= col.a;
					col.a = DstMode > 2 ? col.a : 0;

					return col;
				}
				ENDCG
			}
		}
	}
	CustomEditor "UVAnimDistortMaterialInspector"
}
