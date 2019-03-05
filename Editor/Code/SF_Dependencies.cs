using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace ShaderForge {

	



	public class SF_Dependencies {

		private int vert_out_texcoordNumber = 0;

		public bool uv0 = false;
		public bool uv0_frag = false;
		public bool uv1 = false;
		public bool uv1_frag = false;
		public bool uv2 = false;
		public bool uv2_frag = false;
		public bool uv3 = false;
		public bool uv3_frag = false;
		public bool uv0_float4 = false;
		public bool uv1_float4 = false;
		public bool uv2_float4 = false;
		public bool uv3_float4 = false;

		public bool vertexColor = false;
		public bool lightColor = false;
		public bool time = false;
		public bool grabPass = false;
		public bool scene_uvs = false;
		public bool tessellation = false;
		public bool displacement = false;

		public bool frag_facing = false;

		public bool vert_out_worldPos = false;
		public bool vert_out_screenPos = false;
		public bool vert_out_normals = false;
		public bool vert_out_tangents = false;
		public bool vert_out_bitangents = false;
		public bool vert_in_normals = false;
		public bool vert_in_tangents = false;
		public bool vert_in_vertexColor = false;
		public bool vert_out_vertexColor = false;
		public bool frag_viewReflection = false;
		public bool frag_viewDirection = false;
		public bool frag_normalDirection = false;
		public bool frag_lightDirection = false;
		public bool frag_lightColor = false;
		public bool frag_halfDirection = false;
		public bool frag_attenuation = false;
		public bool frag_tangentTransform = false;
		public bool frag_screenPos = false;
		public bool frag_pixelDepth = false;
		public bool vert_screenPos = false;

		public bool reflection_probes = false;
		public bool fog_color = false;


		public bool vert_viewReflection = false;
		public bool vert_viewDirection = false;
		public bool vert_normalDirection = false;
		public bool vert_lightDirection = false;
		public bool vert_halfDirection = false;
		public bool vert_tangentTransform = false;

		public bool frag_objectPos = false;
		public bool vert_objectPos = false;

		public bool objectScale = false;
		public bool objectScaleReciprocal = false;

		public bool frag_sceneDepth = false;
		public bool depthTexture = false;
		//public bool frag_pixelDepth = false;

		public bool frag_projPos = false;
		

		public bool const_pi = false;
		public bool const_tau = false;
		public bool const_root2 = false;
		public bool const_e = false;
		public bool const_phi = false;

		public bool pragmaGlsl = false;


		int shaderTarget = 3; // Shader target: #pragma target 3.0
		public List<RenderPlatform> includeRenderers;

		public SF_Dependencies(SF_PassSettings ps) {
			includeRenderers = new List<RenderPlatform>();
			for( int i = 0; i < ps.catMeta.usedRenderers.Length; i++ ) {
				if( ps.catMeta.usedRenderers[i] ) {
					includeRenderers.Add( (RenderPlatform)i );
				}
			}
			


			//excludeRenderers.Add( RenderPlatform.flash );
			//excludeRenderers.Add( RenderPlatform.gles );
			//excludeRenderers.Add( RenderPlatform.xbox360 );
			//excludeRenderers.Add( RenderPlatform.ps3 );
		}

		public void NeedSceneAndFragDepth(){
			frag_pixelDepth = true;
			NeedSceneDepth();
		}

		public void NeedSceneDepth(){
			frag_projPos = true;
			frag_sceneDepth = true;
			depthTexture = true;
		}

		public void IncrementTexCoord( int num ) {
			vert_out_texcoordNumber += num;
		}

		public bool UsesLightNodes() {
			return frag_attenuation || frag_lightDirection || frag_halfDirection || lightColor;
		}

		public void NeedFragVertexColor() {
			vert_in_vertexColor = true;
			vert_out_vertexColor = true;
		}

		public void NeedFragObjPos() {
			frag_objectPos = true;
		}
		public void NeedVertObjPos(){
			vert_objectPos = true;
		}

		public void NeedVertScreenPos() {
			vert_screenPos = true;
		}

		public void NeedLightColor() {
			lightColor = true;
			frag_lightColor = true;
		}

		public void NeedFragAttenuation(){
			frag_attenuation = true;
		}

		public void NeedRefraction() {
			NeedGrabPass();
			NeedSceneUVs();
		}

		public void NeedSceneUVs(){
			frag_projPos = true;
			scene_uvs = true;
		}

		public void NeedFragPixelDepth(){
			NeedFragWorldPos();
			frag_projPos = true;
			frag_pixelDepth = true;
		}

		public void NeedGrabPass() {
			NeedSceneUVs(); // TODO: Really?
			grabPass = true;
		}

		public void NeedTessellation(){
			shaderTarget = Mathf.Max( shaderTarget, 5);
			vert_in_tangents = true;
			vert_in_normals = true;
			tessellation = true;
		}


		public void NeedDisplacement() {
			displacement = true;
		}

		public void NeedFragWorldPos() {
			vert_out_worldPos = true;
		}
		public void NeedVertWorldPos() {
			vert_out_worldPos = true; // TODO ?
		}

		public void NeedFragHalfDir() {
			frag_halfDirection = true;
			NeedFragLightDir();
			NeedFragViewDirection();
		}
		public void NeedVertHalfDir() {
			vert_halfDirection = true;
			NeedVertLightDir();
			NeedVertViewDirection();
		}

		public void NeedFragLightDir() {
			frag_lightDirection = true;
			NeedFragWorldPos();
		}
		public void NeedVertLightDir() {
			vert_lightDirection = true;
			NeedVertWorldPos();
		}

		public void NeedFragViewDirection() {
			frag_viewDirection = true;
			NeedFragWorldPos();
		}
		public void NeedVertViewDirection() {
			vert_viewDirection = true;
			NeedVertWorldPos();
		}


	

		public void NeedFragViewReflection() {
			NeedFragViewDirection();
			NeedFragNormals();
			frag_viewReflection = true;
		}

		public void NeedFragNormals() {
			vert_in_normals = true;
			vert_out_normals = true;
			frag_normalDirection = true;
		}

		public void NeedFragTangents() {
			vert_in_tangents = true;
			vert_out_tangents = true;
		}
		public void NeedFragBitangents() {
			vert_in_normals = true;
			vert_out_normals = true;
			vert_in_tangents = true;
			vert_out_tangents = true;
			vert_out_bitangents = true;
		}

		public void NeedFragTangentTransform() {
			frag_tangentTransform = true;
			frag_normalDirection = true;
			vert_in_normals = true;
			vert_out_normals = true;
			vert_in_tangents = true;
			vert_out_tangents = true;
			vert_out_bitangents = true;
		}



		public void IncludeRenderPlatform( RenderPlatform plat ) {
			if( includeRenderers.Contains( plat ) == false ) {
				includeRenderers.Add( plat );
			}
		}

		public bool DoesIncludePlatforms() {
			return includeRenderers.Count > 0;
		}

		public bool IsTargetingAboveDefault() {
			return ( shaderTarget > 2 );
		}

		public string GetIncludedPlatforms() {
			string s = "";
			foreach( RenderPlatform plat in includeRenderers )
			{
				if(plat.ToString()=="nswitch")
				{
					s += "switch ";
				}
				else
				{
					s += plat.ToString() + " ";
				}
			}
			//Debug.Log("Exclude Str: " + s);
			return s;
		}

		public void SetMinimumShaderTarget( int x ) {
			if( x > shaderTarget )
				shaderTarget = x;
		}
		public string GetShaderTarget() {
			return ( shaderTarget + ".0" );
		}

		public string GetVertOutTexcoord() {
			string s = vert_out_texcoordNumber.ToString();
			vert_out_texcoordNumber++;
			return s;
		}

		public void ResetTexcoordNumbers() {
			//vert_in_texcoordNumber = 0;
			vert_out_texcoordNumber = 0;
		}

	}
}