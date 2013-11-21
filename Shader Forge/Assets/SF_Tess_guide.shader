Shader "Custom/SF_Tess_guide" {
	Properties {
		_Tess ("Tesselation", Range(1, 5)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
			Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex tessvert // The part after tess determines the regular name of vert
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap
			#include "HLSLSupport.cginc"
			#include "Tessellation.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal

			//#pragma surface surf Lambert vertex:disp tessellate:tessFixed nolightmap
			#pragma target 5.0
			#pragma debug

			struct VertexInput {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 uv0 : TEXCOORD0;
			};

			float _Tess;


			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
			};
			

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv0 = v.uv0;
				return o;
			}



			#ifdef UNITY_CAN_COMPILE_TESSELLATION

				struct TessVertex {
					float4 vertex : INTERNALTESSPOS;
					float4 tangent : TANGENT;
					float3 normal : NORMAL;
					float2 uv0 : TEXCOORD0;
				};
				struct OutputPatchConstant{
					float edge[3]         : SV_TessFactor;
					float inside          : SV_InsideTessFactor;
					float3 vTangent[4]    : TANGENT;
					float2 vUV[4]         : TEXCOORD;
					float3 vTanUCorner[4] : TANUCORNER;
					float3 vTanVCorner[4] : TANVCORNER;
					float4 vCWts          : TANWEIGHTS;
				};
				TessVertex tessvert (VertexInput v) {
					TessVertex o;
					o.vertex = v.vertex;
					o.tangent = v.tangent;
					o.normal = v.normal;
					o.uv0 = v.uv0;
					return o;
				}



				float tesselation(TessVertex v){
					return _Tess;
				}
				void displacement (inout VertexInput v){
					v.vertex.xyz += float3(0,0,0);
				}


				OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
					OutputPatchConstant o;
					float ts = tesselation(v[0]);
					o.edge[0] = ts;
					o.edge[1] = ts;
					o.edge[2] = ts;
					o.inside = ts;
					return o;
				}

				[domain("tri")]
				[partitioning("fractional_odd")]
				[outputtopology("triangle_cw")]
				[patchconstantfunc("hullconst")]
				[outputcontrolpoints(3)]
				TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
					return v[id];
				}

				[domain("tri")]
				VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
					VertexInput v;
					v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
					v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
					v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
					v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
					displacement (v);
					VertexOutput o = vert (v);
					return o;
				}
			#endif // TESSELLATION

			fixed4 frag (VertexOutput o) : COLOR {
				return fixed4(o.uv0.xy,0,0);
			}


		ENDCG
		}
	} 
	FallBack "Diffuse"
}
