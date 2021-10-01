Shader "Game/Terrain"
{
	Properties
	{
		// grass
		[Header(Plain)]
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_PlainSplatmap ( "Emission Texture", 2D) = "black" {}
		_CheckboardPattern ("Checkboard", float) = 0

		// mountain
		[Header(Mountain)]
		_MountainTex ( "Mountain Texture", 2D) = "black" {}
		_MountainNormals ( "Mountain Normals", 2D) = "black" {}
		_MountainColor ( "Mountain Color", Color) = (1, 1, 1, 1)
		_FresnelPower ("Fresnel Power", float) = 5
		_FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)

		// shore
		[Header(Shore)]
		_ShoreTex ( "Shore texture", 2D) = "white" {}
		_ShoreColor ( "Shore Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		pass
		{
			Tags { "LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			// Common
			float4 _LightColor0;

			// Plain
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _PlainSplatmap;
			float4 _PlainSplatmap_ST;
			float _CheckboardPattern;

			// Mountain
			sampler2D _MountainNormals;
			sampler2D _MountainTex;
			float4 _MountainTex_ST;
			float4 _MountainColor;
			// Mountain - fresnel
			float _FresnelPower;
			float4 _FresnelColor;

			// Shore
			float4 _ShoreColor;
			sampler2D _ShoreTex;
			float4 _ShoreTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 splatMap : TEXCOORD0;
				float3 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 localPos : TEXCOORD2;
				float3 worldPos : TEXCOORD1;
				float3 lightDir : TEXCOORD0;
				float3 normal : NORMAL;
				float4 splatMap : TEXCOORD3;
				float3 tangent : TANGENT;
				LIGHTING_COORDS(3, 4)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.localPos = v.vertex;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.lightDir = ObjSpaceLightDir(v.vertex);
				o.normal = v.normal;
				o.splatMap = v.splatMap;
				o.tangent = v.tangent;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			// --------------------------------------------------------------
			float3 diffuseGrass(v2f i, float3 N, float3 L, float attenuation)
			{
				float2 uv = _MainTex_ST.zw + i.localPos.xz / _MainTex_ST.xy;
				float2 uv2 = _PlainSplatmap_ST.zw + i.localPos.xz / _PlainSplatmap_ST.xy;
				float4 emission = tex2D(_PlainSplatmap, uv2);
				float3 color = tex2D(_MainTex, uv).rgb;
				color = lerp(color, emission.rgb, emission.a);
				color *= 1 - i.splatMap.w * _CheckboardPattern;

				float NdotL = saturate(dot(N, L));
				float3 diffuseTerm = _LightColor0.rgb * NdotL;
				float3 final = (diffuseTerm * attenuation + UNITY_LIGHTMODEL_AMBIENT) * color;
				return final;
			}
			// --------------------------------------------------------------

			// --------------------------------------------------------------
			float3 diffuseMountain2(v2f i, float3 N, float3 L, float attenuation, float2 proj)
			{
				float2 uv = _MountainTex_ST.zw + proj / _MountainTex_ST.xy;
				float3 color = lerp(tex2D(_MountainTex, uv).rgb, _MountainColor.rgb, _MountainColor.a);
				float3 normalMap = UnpackNormalWithScale(tex2D(_MountainNormals, uv), 1.5);
				float3 tangent = normalize(i.tangent);
				float3 B = cross(N, tangent);

				N = normalize(
				(
					normalMap.r * tangent +
					normalMap.g * B +
					normalMap.b * N
				));

				float NdotL = saturate(dot(N, L));
				float3 diffuseTerm = _LightColor0.rgb * NdotL * attenuation;

				// add fresnel
				float3 I = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
				float R = pow(1.0 + dot(I, N), _FresnelPower);
				float3 fresnelTerm = _FresnelColor.rgb * R;

				// finalize
				float3 final = (diffuseTerm + fresnelTerm + UNITY_LIGHTMODEL_AMBIENT) * color;
				return final;
			}

			float3 diffuseMountain(v2f i, float3 N, float3 L, float attenuation)
			{
				float3 xAxis = float3(-0.7071067811865475, 0, 0.7071067811865475);
				float3 yAxis = float3(0, 1, 0);
				float3 zAxis = float3(0.7071067811865475, 0, 0.7071067811865475);

				float3 x = diffuseMountain2(i, N, L, attenuation, float2(dot(i.worldPos, zAxis), dot(i.worldPos, yAxis)));
				float3 y = diffuseMountain2(i, N, L, attenuation, float2(dot(i.worldPos, xAxis), dot(i.worldPos, zAxis)));
				float3 z = diffuseMountain2(i, N, L, attenuation, float2(dot(i.worldPos, xAxis), dot(i.worldPos, yAxis)));

				float3 Nprojection = abs(float3(dot(N, xAxis), N.y, dot(N, zAxis)));
				float3 ratio = saturate(pow(Nprojection, 5));
				float totalRatio = ratio.x + ratio.y + ratio.z;
				return (x * ratio.x + y * ratio.y + z * ratio.z) / totalRatio;
			}
			// --------------------------------------------------------------

			// Shore sub-shader
			// --------------------------------------------------------------
			float3 diffuseShore(v2f i, float3 N, float3 L, float attenuation)
			{
				float2 uv = _ShoreTex_ST.zw + i.localPos.xz / _ShoreTex_ST.xy;
				float3 color = tex2D(_ShoreTex, uv).rgb;
				color = lerp(color, _ShoreColor.rgb, _ShoreColor.a);

				float NdotL = saturate(dot(N, L));
				float3 diffuseTerm = _LightColor0.rgb * NdotL;
				float3 final = (diffuseTerm * attenuation + UNITY_LIGHTMODEL_AMBIENT) * color;
				return final;
			}
			// --------------------------------------------------------------

			// Fragment shader
			// --------------------------------------------------------------
			float4 frag(v2f i) : COLOR
			{
				// get commonalities
				float3 L = normalize(i.lightDir);
				float3 N = normalize(i.normal);
				float attenuation = LIGHT_ATTENUATION(i);

				// execute sub-shader to be blended.
				float3 grass = diffuseGrass(i, N, L, attenuation) * i.splatMap.x;
				float3 mountain = diffuseMountain(i, N, L, attenuation) * i.splatMap.y;
				float3 shore = diffuseShore(i, N, L, attenuation) * i.splatMap.z;
				float3 color = grass + mountain + shore;

				return float4(color, 1);
			}
			// --------------------------------------------------------------

			ENDCG
		}
	}

	// used for shadow casting pass
	FallBack "Diffuse"
}