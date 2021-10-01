Shader "Game/Diffuse"
{
	Properties
	{
		[MaterialToggle] USE_PROJECT_UV("Project UV", Float) = 0
		[MaterialToggle] USE_VERTEX_COLOR("Use Vertex Color", Float) = 0
		_MainTex ("Diffuse Texture", 2D) = "white" {}

		[Space]
		[MaterialToggle] USE_NORMAL_MAP("Use Normal Map", Float) = 0
		_NormalTex ("Normal Map", 2D) = "white" {}

		[Space]
		_MainColor ( "Main Color", Color) = (1, 1, 1, 1)
		_MainColorMult ( "Main Color Mult", float) = 1

		[Space]
		[MaterialToggle] USE_FRESNEL("Use Fresnel", Float) = 0
		_FresnelPower ("Fresnel Power", float) = 5
		_FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Tags { "LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma shader_feature USE_NORMAL_MAP_ON
			#pragma shader_feature USE_FRESNEL_ON
			#pragma shader_feature USE_PROJECT_UV_ON
			#pragma shader_feature USE_VERTEX_COLOR_ON

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			sampler2D _AmbientOcclusionMap;
			float4 _MainTex_ST;

			sampler2D _NormalTex;
			float4 _NormalTex_ST;

			float4 _FresnelColor;
			float _FresnelPower;
			float _MainColorMult;

			float4 _MainColor;

			float4 _LightColor0;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float3 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float3 lightDir : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 tangent : TANGENT;
				float2 uv : TEXCOORD2;
				float3 normal : NORMAL;
				LIGHTING_COORDS(3, 4)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

#if defined(USE_PROJECT_UV_ON)
				o.uv = TRANSFORM_TEX(o.worldPos.xz, _MainTex);
#else
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#endif
				o.color = v.color;
				o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
				o.normal = v.normal;
				o.tangent = v.tangent;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			float3 GetNormal(v2f i)
			{
#if defined(USE_NORMAL_MAP_ON)
				float3 N = normalize(i.normal);
				float3 normalMap = UnpackNormalWithScale(tex2D(_NormalTex, i.uv), 1);
				float3 tangent = normalize(i.tangent);
				float3 B = cross(N, tangent);
				return normalize(
				(
					normalMap.r * tangent +
					normalMap.g * B +
					normalMap.b * N
				));
#else
				return normalize(i.normal);
#endif
			}

			float4 frag(v2f i) : COLOR
			{
				float3 N = GetNormal(i);
				float3 L = normalize(i.lightDir);
				float attenuation = LIGHT_ATTENUATION(i);
				float NdotL = saturate(dot(N, L));
				float diffuseTerm = _LightColor0 * NdotL * attenuation;
				diffuseTerm += UNITY_LIGHTMODEL_AMBIENT;

				float4 color = tex2D(_MainTex, i.uv) * _MainColor * _MainColorMult;
#if defined(USE_VERTEX_COLOR_ON)
				color.rgb *= i.color;
#endif
				color.rgb *= diffuseTerm;

#if defined(USE_FRESNEL_ON)
				float3 I = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
				float R = pow(saturate(1 - abs(dot(I, N))), _FresnelPower);
				color.rgb += _FresnelColor.rgb * _FresnelColor.a * R;
#endif

				return color;
			}

			ENDCG
		}
	}

	// used for shadow casting pass
	FallBack "Diffuse"
}