// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "NPR Cartoon Effect/Cartoon" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_RampTex ("Ramp", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SpecularScale ("Specular Scale", Range(0, 0.05)) = 0.01
		_SpecularTranslationX ("Specular Translation X", Range(-1, 1)) = 0
		_SpecularTranslationY ("Specular Translation Y", Range(-1, 1)) = 0
		_SpecularRotationX ("Specular Rotation X", Range(-180, 180)) = 0
		_SpecularRotationY ("Specular Rotation Y", Range(-180, 180)) = 0
		_SpecularRotationZ ("Specular Rotation Z", Range(-180, 180)) = 0
		_SpecularScaleX ("Specular Scale X", Range(-1, 1)) = 0
		_SpecularScaleY ("Specular Scale Y", Range(-1, 1)) = 0
		_SpecularSplitX ("Specular Split X", Range(0, 1)) = 0
		_SpecularSplitY ("Specular Split Y", Range(0, 1)) = 0
		_StylizedTex ("Stylized", 2D) = "white" {}
		_StylizedTexStart ("Stylized Tex Start", Float) = 0.5
		_StylizedTexEnd ("Stylized Tex End", Float) = 0.6
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 0)
		_OutlineWidth ("Outline Width", Float) = 0.1
		_ExpandFactor ("Outline Factor", Range(0, 1)) = 1
	}
	SubShader {
		Pass {
			Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			Cull Back
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase
			#pragma multi_compile _ NCE_STYLIZED
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex, _RampTex, _StylizedTex;
			float4 _MainTex_ST, _StylizedTex_ST;
			float4 _DiffuseColor, _SpecularColor;
			float _SpecularScale;
			float _SpecularTranslationX, _SpecularTranslationY;
			float _SpecularRotationX, _SpecularRotationY, _SpecularRotationZ;
			float _SpecularScaleX, _SpecularScaleY;
			float _SpecularSplitX, _SpecularSplitY;
			float _StylizedTexStart, _StylizedTexEnd;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float3 tgsnor : TEXCOORD1;    // tangent space normal
				float3 tgslit : TEXCOORD2;    // tangent space light
				float3 tgsview : TEXCOORD3;   // tangent space view
				LIGHTING_COORDS(4, 5)
			};
			v2f vert (appdata_tan v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.tex.zw = TRANSFORM_TEX(v.texcoord, _StylizedTex);
				TANGENT_SPACE_ROTATION;
				o.tgsnor = mul(rotation, v.normal);
				o.tgslit = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.tgsview = mul(rotation, ObjSpaceViewDir(v.vertex));
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				float3 N = normalize(i.tgsnor);
				float3 L = normalize(i.tgslit);
				float3 V = normalize(i.tgsview);
				float3 H = normalize(V + L);
				
				// specular highlights scale
				H = H - _SpecularScaleX * H.x * float3(1, 0, 0);
				H = normalize(H);
				H = H - _SpecularScaleY * H.y * float3(0, 1, 0);
				H = normalize(H);
				
				// specular highlights rotation
				#define DegreeToRadian 0.0174533
				float radX = _SpecularRotationX * DegreeToRadian;
				float3x3 rotMatX = float3x3(
					1,	0, 		 	0,
					0,	cos(radX),	sin(radX),
					0,	-sin(radX),	cos(radX));
				float radY = _SpecularRotationY * DegreeToRadian;
				float3x3 rotMatY = float3x3(
					cos(radY), 	0, 		-sin(radY),
					0,			1,		0,
					sin(radY), 	0, 		cos(radY));
				float radZ = _SpecularRotationZ * DegreeToRadian;
				float3x3 rotMatZ = float3x3(
					cos(radZ), 	sin(radZ), 	0,
					-sin(radZ), cos(radZ), 	0,
					0, 			0,			1);
				H = mul(rotMatZ, mul(rotMatY, mul(rotMatX, H)));
				H = normalize(H);
				
				// specular highlights translation
				H = H + float3(_SpecularTranslationX, _SpecularTranslationY, 0);
				H = normalize(H);
				
				// specular highlights split
				float signX = 1;
				if (H.x < 0)
					signX = -1;

				float signY = 1;
				if (H.y < 0)
					signY = -1;

				H = H - _SpecularSplitX * signX * float3(1, 0, 0) - _SpecularSplitY * signY * float3(0, 1, 0);
				H = normalize(H);
				
				//
				// cartoon light model
				//
				
				// ambient light from Unity render setting
				float3 ambientColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

				// diffuse cartoon light
				float diff = saturate(dot(N, L)) * LIGHT_ATTENUATION(i);
				float4 albedo = tex2D(_MainTex, i.tex.xy);
#if NCE_STYLIZED   // stylized dark part shading
				float f = 1.0 - smoothstep(_StylizedTexStart, _StylizedTexEnd, diff);
				float4 stylizedColor = tex2D(_StylizedTex, i.tex.zw);
				float4 diffuseColor = lerp(albedo, stylizedColor * albedo, f) * _DiffuseColor;
#else
				float4 ramp = tex2D(_RampTex, float2(diff, 0.5));
				float4 diffuseColor = albedo * _DiffuseColor * ramp;
#endif				
				// stylized specular light
				float spec = dot(N, H);
				float w = fwidth(spec);
				float3 specularColor = lerp(float3(0, 0, 0), _SpecularColor.rgb, smoothstep(-w, w, spec + _SpecularScale - 1.0));
				
				return float4(ambientColor + diffuseColor.rgb + specularColor, 1.0) * _LightColor0;
            }
			ENDCG
		}
		Pass {
			Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			Cull Front

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			float4 _OutlineColor;
			float _OutlineWidth, _ExpandFactor;
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			v2f vert (appdata_base v)
			{
				float3 dir1 = normalize(v.vertex.xyz);
				float3 dir2 = v.normal;
				float3 dir = lerp(dir1, dir2, _ExpandFactor);
				dir = mul((float3x3)UNITY_MATRIX_IT_MV, dir);
				float2 offset = TransformViewToProjection(dir.xy);
				offset = normalize(offset);
				float dist = distance(mul(unity_ObjectToWorld, v.vertex), _WorldSpaceCameraPos);
			
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pos.xy += offset * o.pos.z * _OutlineWidth / dist;
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				return _OutlineColor;
            }
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
