Shader "Tessellation Sample" {
		Properties {
			_Tess ("Tessellation", Range(1,32)) = 4
			_MainTex ("Base (RGB)", 2D) = "white" {}
			_DispTex ("Disp Texture", 2D) = "gray" {}
			_NormalMap ("Normalmap", 2D) = "bump" {}
			_Displacement ("Displacement", Range(0, 1.0)) = 0.3
			_Color ("Color", color) = (1,1,1,0)
			_SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
		}
		SubShader {
			Tags { "RenderType"="Opaque" }
			LOD 300

			CGPROGRAM
			#pragma surface surf Lambert vertex:disp tessellate:tessFixed nolightmap
			#pragma target 5.0
			#pragma debug

			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			float _Tess;

			float4 tessFixed()
			{
				return _Tess;
			}

			sampler2D _DispTex;
			float _Displacement;

			void disp (inout appdata v)
			{
				float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement;
				v.vertex.xyz += v.normal * d;
			}

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			sampler2D _NormalMap;
			fixed4 _Color;

			void surf (Input IN, inout SurfaceOutput o) {
				half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Specular = 0.0;
				o.Gloss = 0.0;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			}
			ENDCG
		}
		FallBack "Diffuse"
	}