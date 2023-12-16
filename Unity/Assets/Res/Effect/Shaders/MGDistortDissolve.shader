
Shader "VGame/Effects/MG Distort Dissolve" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Intensity("Intensity", Float) = 1
		_MainTex("Main Texture", 2D) = "white" {}
		_U ("Base U", Float) =0
		_V ("Base V", Float) =0
		[MaterialEnum(Additive, 1,  Alpha Blend, 10)]DstMode("Blend Mode", Int) = 1
		[MaterialEnum(Off, 0, Back, 2)]CullMode("CullMode", int) = 0

		_DissolveTex("Dissolve (RGB)", 2D) = "white" {}
		_DissolveU("Dissolve U", Float) = 0
		_DissolveV("Dissolve V", Float) = 0
		_Cutoff ("_Cutoff", Range(-0.1,1)) = 0
		[HDR]_BorderColor ("Border Color", Color) = (1,1,1,1)
		_CutoutThickness ("Cutout Thickness", Range(0,1)) = 0
		_NoiseTex ("Distort Texture (RG)", 2D) = "white" {}
		_HeatTime  ("Heat Time", range (-1,1)) = 0
		_ForceX  ("Strength X", range (0,1)) = 0.1
		_ForceY  ("Strength Y", range (0,1)) = 0.1
		_Offset("Offset", int) = 0
	}

	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend One [DstMode], One One
		Cull [CullMode] Lighting Off ZWrite Off
		Offset [_Offset], [_Offset]

		SubShader{
			Pass{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ _DISTORT_ON
			
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Intensity;
				float _U,_V;
				half4 _TintColor;
				half DstMode;

				sampler2D _DissolveTex;
				float4 _DissolveTex_ST;
				float _DissolveU, _DissolveV;
				half4 _BorderColor;
				half _CutoutThickness;
				half _Cutoff;

				#if _DISTORT_ON
				fixed _ForceX;
				fixed _ForceY;
				fixed _HeatTime;
				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;
				#endif
			
				struct appdata_t {
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2: TEXCOORD1;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half4 color : COLOR;
					float2 uvMain : TEXCOORD0;
				#if _DISTORT_ON
					float2 uvDistort : TEXCOORD1;
				#endif
					float2 uvDissolve : TEXCOORD2;
				};

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					o.color = v.color;
					float2 worlPos = mul(unity_ObjectToWorld, v.vertex);
					o.uvMain.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				#if _DISTORT_ON
					o.uvDistort = TRANSFORM_TEX(worlPos, _NoiseTex);
				#endif
					o.uvDissolve.xy = TRANSFORM_TEX(v.texcoord2, _DissolveTex);
					return o;
				}
			
				half4 frag (v2f i) : SV_Target
				{
					#if _DISTORT_ON
						fixed4 offsetColor1 = tex2D(_NoiseTex, i.uvDistort.xy + _Time.xz*_HeatTime);
						fixed4 offsetColor2 = tex2D(_NoiseTex, i.uvDistort.xy + _Time.yx*_HeatTime);
						i.uvMain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceX;
						i.uvMain.y += ((offsetColor1.r + offsetColor2.r) - 1) * _ForceY;
					#endif
					half4 tex = tex2D(_MainTex, i.uvMain + float2(_U, _V) * _Time.z);
					half4 col = _Intensity * i.color * _TintColor * tex;

					fixed4 mask = tex2D(_DissolveTex, i.uvDissolve + float2(_DissolveU, _DissolveV) * _Time.z);

					_Cutoff += 1 - mask.r;
					clip(i.color.a - _Cutoff);
					if (i.color.a < _Cutoff + _CutoutThickness) col.rgb += _BorderColor.rgb;

					col.rgb *= col.a;
					col.a = DstMode > 2 ? col.a : 0;
					return col;
				}
				ENDCG 
			}
		}
	}
	CustomEditor "DistortDissolveMaterialInspector"
}
