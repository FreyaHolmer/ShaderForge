using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using UnityEditor.VersionControl;
using System.Linq;

namespace ShaderForge {


	public enum PassType {
		FwdBase, FwdAdd, ShadCast,
		Outline,
		Deferred,
		Meta
	};
	public enum ShaderProgram { Any, Vert, Frag, Tess };

	[System.Serializable]
	public class SF_Evaluator {

		public SF_Editor editor;
		public List<SF_Node> cNodes;
		public List<SF_Node> ghostNodes;
		public int scope = 0;
		public string shaderString = "";

		public SF_PassSettings ps;
		public SF_Dependencies dependencies;
		public SF_ShaderProperty properties;

		public SFN_Final mOut;

		const bool DEBUG = true;





		public static PassType currentPass = PassType.FwdBase;
		public static ShaderProgram currentProgram = ShaderProgram.Vert;

		public static bool inFrag => SF_Evaluator.currentProgram == ShaderProgram.Frag;
		public static bool inVert => SF_Evaluator.currentProgram == ShaderProgram.Vert;
		public static bool inTess => SF_Evaluator.currentProgram == ShaderProgram.Tess;

		public static string WithProgramPrefix( string s ) {
			if( SF_Evaluator.inFrag )
				return "i." + s;
			if( SF_Evaluator.inVert )
				return "o." + s;
			if( SF_Evaluator.inTess )
				return "v." + s;
			Debug.Log( "Invalid program" );
			return null;
		}



		// TODO: SHADER MODEL
		public SF_Evaluator() {


		}

		public SF_Evaluator( SF_Editor editor ) {
			this.editor = editor;
			this.ps = editor.ps;
		}


		public void PrepareEvaluation() {
			ps.UpdateAutoSettings();

			mOut = editor.mainNode;
		}

		public void RemoveGhostNodes() {
			if( ghostNodes == null )
				return;

			if( SF_Debug.ghostNodes )
				Debug.Log( "Removing ghost nodes. Count: " + ghostNodes.Count );
			for( int i = ghostNodes.Count - 1; i >= 0; i-- ) {
				editor.nodes.Remove( ghostNodes[i] );
				ghostNodes[i].DeleteGhost();
				ghostNodes.Remove( ghostNodes[i] );
			}
			//Debug.Log( "Done removing ghost nodes. Count: " + ghostNodes.Count );
		}

		bool LightmappedAndLit() {
			return ps.catLighting.bakedLight && ( ps.HasSpecular() || ps.HasDiffuse() ) && ps.catLighting.lightMode != SFPSC_Lighting.LightMode.Unlit;
		}

		bool IsReflectionProbed() {
			return ( ps.HasSpecular() && ps.catLighting.lightMode != SFPSC_Lighting.LightMode.Unlit ) && ps.catLighting.reflectprobed && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred );
		}

		public void UpdateDependencies() {

			dependencies = new SF_Dependencies( editor.ps );

			if( SF_Debug.evalFlow )
				Debug.Log( "UPDATING DEPENDENCIES: Pass = " + currentPass + " Prog = " + currentProgram );
			cNodes = editor.nodeView.treeStatus.GetListOfConnectedNodesWithGhosts( out ghostNodes, passDependent: true );
			if( SF_Debug.evalFlow )
				Debug.Log( "Found " + cNodes.Count + " nodes" );


			for( int i = 0; i < cNodes.Count; i++ ) {
				cNodes[i].PrepareEvaluation();
			}

			if( currentPass == PassType.Meta ) {
				dependencies.uv1 = true;
				dependencies.uv2 = true;
			}

			// Dependencies
			if( ps.catLighting.IsLit() && !IsShadowOrOutlineOrMetaPass() && currentPass != PassType.Deferred ) {
				dependencies.NeedLightColor();
				dependencies.NeedFragNormals();
				dependencies.NeedFragLightDir();

				if( ( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.BlinnPhong || ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) && ps.mOut.specular.IsConnectedEnabledAndAvailableInThisPass( currentPass ) ) {
					dependencies.NeedFragHalfDir();
				}

				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL && ps.mOut.diffuse.IsConnectedEnabledAndAvailableInThisPass(currentPass)){
					dependencies.NeedFragHalfDir();
				}


			}

			if( editor.nodeView.treeStatus.viewDirectionInVertOffset ) {
				dependencies.vert_viewDirection = true;
			}

			if( currentPass == PassType.Deferred ) {
				dependencies.NeedFragNormals();
			}

			if( IsReflectionProbed() && ps.HasSpecular() && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred ) ) {
				dependencies.NeedFragViewReflection();
				dependencies.reflection_probes = true;
			}

			if( ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred ) && ( LightmappedAndLit() || IsReflectionProbed() || ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) ) {
				dependencies.NeedFragViewReflection();
			}


			if( LightmappedAndLit() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.vert_in_normals = true;
				if( ps.catLighting.highQualityLightProbes )
					dependencies.NeedFragNormals();
			}

			if( ps.IsOutlined() && currentPass == PassType.Outline ) {
				dependencies.vert_in_normals = true;
				if( ps.catGeometry.outlineMode == SFPSC_Geometry.OutlineMode.VertexColors ) {
					dependencies.vert_in_vertexColor = true;
				}
			}

			if( ps.catLighting.IsVertexLit() && ps.catLighting.IsLit() && !IsShadowOrOutlineOrMetaPass() ) {
				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.BlinnPhong || ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL )
					dependencies.NeedVertHalfDir();
				dependencies.NeedVertLightDir();
			}


			if( LightmappedAndLit() ) {
				dependencies.NeedFragWorldPos();
				dependencies.NeedFragViewDirection();
				dependencies.uv1 = true;
				dependencies.uv1_frag = true;
				dependencies.uv2 = true;
				dependencies.uv2_frag = true;
			}


			if( LightmappedAndLit() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.NeedFragTangentTransform(); // Directional LMs
				dependencies.uv1 = true; // Lightmap UVs
			}

			//if( ps.HasAnisotropicLight() && !IsShadowPass() ) {
			//	dependencies.NeedFragTangents();
			//	dependencies.NeedFragBinormals();
			//}



			if( ps.catLighting.IsFragmentLit() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.vert_in_normals = true;
				dependencies.vert_out_normals = true;
				dependencies.vert_out_worldPos = true;
				dependencies.frag_normalDirection = true;
				if( ps.HasNormalMap() || ps.catLighting.HasSpecular() )
					dependencies.NeedFragViewDirection();
			}

			if( ps.HasTangentSpaceNormalMap() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.frag_normalDirection = true;
				dependencies.NeedFragTangentTransform();
			}

			if( ps.HasObjectSpaceNormalMap() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.objectScaleReciprocal = true;
			}

			if( ps.HasRefraction() && !IsShadowOrOutlineOrMetaPass() ) {
				dependencies.NeedRefraction();
			}

			if( ps.HasTessellation() ) {
				dependencies.NeedTessellation();
			}

			if( ps.HasDisplacement() ) {
				dependencies.NeedDisplacement();
			}


			if( ps.catBlending.dithering != Dithering.Off && editor.mainNode.alphaClip.IsConnectedEnabledAndAvailable() ) {
				dependencies.NeedSceneUVs();
			}





			foreach( SF_Node n in cNodes ) {

				if( n is SFN_Time ) {
					dependencies.time = true;
				}

				if( n is SFN_SceneColor ) {
					if( ( n as SFN_SceneColor ).AutoUV() )
						dependencies.NeedSceneUVs();
					dependencies.NeedGrabPass();
				}

				if( n is SFN_ObjectPosition ) {
					if( currentProgram == ShaderProgram.Frag )
						dependencies.NeedFragObjPos();
					else
						dependencies.NeedVertObjPos();
				}

				if( n is SFN_Fresnel ) {
					dependencies.NeedFragViewDirection();
					if( !n.GetInputIsConnected( "NRM" ) ) // Normal. If it's not connected, make sure we have the dependency for normals
						dependencies.NeedFragNormals();
				}

				if( n is SFN_FragmentPosition ) {
					dependencies.NeedFragWorldPos();
				}

				if( n is SFN_SceneDepth ) {
					dependencies.depthTexture = true;
					if(n.GetInputIsConnected("UV") == false)
						dependencies.NeedSceneUVs();
				}

				if( n is SFN_DepthBlend ) {
					dependencies.NeedSceneDepth();
					dependencies.frag_pixelDepth = true;
				}

				if( n is SFN_Depth ) {
					// (mul( UNITY_MATRIX_V, float4((_WorldSpaceCameraPos.rgb-i.posWorld.rgb),0) ).b - _ProjectionParams.g)
					dependencies.NeedFragPixelDepth();
				}

				if( n is SFN_ObjectScale ) {
					if( ( n as SFN_ObjectScale ).reciprocal )
						dependencies.objectScaleReciprocal = true;
					else
						dependencies.objectScale = true;
				}

				/*
				if( n is SFN_Rotator ) {
					if(!n.GetInputIsConnected("ANG"))
						dependencies.time = true;
				}*/

				/*
				if( n is SFN_Panner ) {
					if( !n.GetInputIsConnected( "DIST" ) )
						dependencies.time = true;
				}
				*/

				if( n is SFN_ScreenPos ) {
					dependencies.NeedSceneUVs();
				}

				if( n is SFN_Tex2d ) {
					if( n.GetInputIsConnected( "MIP" ) ) { // MIP connection
						//dependencies.ExcludeRenderPlatform( RenderPlatform.opengl ); // TODO: Find workaround!
						dependencies.SetMinimumShaderTarget( 3 );
					}
				}

				if( n is SFN_Cubemap ) {
					if( n.GetInputIsConnected( "MIP" ) ) { // MIP connection
						//dependencies.ExcludeRenderPlatform( RenderPlatform.opengl ); // TODO: Find workaround!
						dependencies.SetMinimumShaderTarget( 3 );
					}
				}

				/*
				if( n is SFN_Tex2d ) {
					if( !n.GetInputIsConnected( "UVIN" ) ) { // Unconnected UV input
						dependencies.uv0 = true;
						dependencies.uv0_frag = true;
					}
				}*/

				if( n is SFN_VertexColor ) {
					dependencies.NeedFragVertexColor(); // TODO: Check if it really needs to be frag
				}

				if( n is SFN_DDX || n is SFN_DDY ) {
					dependencies.pragmaGlsl = true;
				}

				if( n is SFN_TexCoord ) {
					SFN_TexCoord nTC = (SFN_TexCoord)n;
					switch( nTC.currentUV ) {
						case SFN_TexCoord.UV.uv0:
							dependencies.uv0 = true;
							dependencies.uv0_frag = true;
							if( nTC.useAsFloat4 ) dependencies.uv0_float4 = true;
							break;
						case SFN_TexCoord.UV.uv1:
							dependencies.uv1 = true;
							dependencies.uv1_frag = true;
							if( nTC.useAsFloat4 ) dependencies.uv1_float4 = true;
							break;
						case SFN_TexCoord.UV.uv2:
							dependencies.uv2 = true;
							dependencies.uv2_frag = true;
							if( nTC.useAsFloat4 ) dependencies.uv2_float4 = true;
							break;
						case SFN_TexCoord.UV.uv3:
							dependencies.uv3 = true;
							dependencies.uv3_frag = true;
							if( nTC.useAsFloat4 ) dependencies.uv3_float4 = true;
							break;
					}
				}
				if( n is SFN_Pi ) {
					dependencies.const_pi = true;
				}
				if( n is SFN_Phi ) {
					dependencies.const_phi = true;
				}
				if( n is SFN_E ) {
					dependencies.const_e = true;
				}
				if( n is SFN_Root2 ) {
					dependencies.const_root2 = true;
				}
				if( n is SFN_Tau ) {
					dependencies.const_tau = true;
				}

				if( n is SFN_HalfVector ) {
					dependencies.NeedFragHalfDir();
				}
				if( n is SFN_LightColor ) {
					dependencies.NeedLightColor();
				}

				if( n.IsProperty() && n.property.IsInstancedType() )
					dependencies.hasPropertiesWithInstancing = true;

				if( n is SFN_Parallax ) {
					dependencies.NeedFragViewDirection();
					dependencies.NeedFragTangentTransform();
					if( !( n as SFN_Parallax ).GetInputIsConnected( "UVIN" ) ) {
						dependencies.uv0 = true;
					}
				}

				if( n is SFN_Cubemap ) {
					if( !n.GetInputIsConnected( "DIR" ) ) { // DIR connection, if not connected, we need default reflection vector
						dependencies.NeedFragViewReflection();
					}
				}



				if( SF_Editor.NodeExistsAndIs( n, "SFN_SkyshopSpec" ) ) {
					if( !n.GetInputIsConnected( "REFL" ) ) { // Reflection connection, if not connected, we need default reflection vector
						dependencies.NeedFragViewReflection();
					}
				}

				if( n is SFN_LightAttenuation ) {
					dependencies.NeedFragAttenuation();
				}

				if( n is SFN_ViewReflectionVector ) {
					dependencies.NeedFragViewReflection();
				}

				if( n is SFN_LightVector ) {
					dependencies.NeedFragLightDir();
				}

				if( n is SFN_ViewVector ) {
					dependencies.NeedFragViewDirection();
				}

				if( n is SFN_Tangent ) {
					dependencies.NeedFragTangents();
				}
				if( n is SFN_Bitangent ) {
					dependencies.NeedFragBitangents();
				}
				if( n is SFN_NormalVector ) {
					dependencies.NeedFragNormals();
				}



				if( n is SFN_Transform ) {
					if( ( n as SFN_Transform ).spaceSelFrom == SFN_Transform.Space.Tangent || ( n as SFN_Transform ).spaceSelTo == SFN_Transform.Space.Tangent ) {
						dependencies.NeedFragTangentTransform();
					}
				}

				if( n is SFN_FaceSign ) {
					dependencies.frag_facing = true;
				}

				if( ps.catGeometry.IsDoubleSided() ) {
					dependencies.frag_facing = true;
				}


				// This has to be done afterwards
				if( dependencies.frag_normalDirection && ps.catGeometry.IsDoubleSided() ) {
					dependencies.NeedFragViewDirection();
				}


			}

			//RemoveGhostNodes(); // TODO: Maybe not here?

			if( SF_Debug.evalFlow )
				Debug.Log( "DONE UPDATING DEPENDENCIES" );
		}





		void BeginShader() {
			App( "Shader \"" + editor.currentShaderPath + "\" {" );
			scope++;
		}
		void BeginProperties() {
			App( "Properties {" );
			scope++;
		}

		void PropertiesShaderLab() {

			BeginProperties();

			//Debug.Log("Printing properties, count = " + editor.nodeView.treeStatus.propertyList.Count);

			for( int i = 0; i < editor.nodeView.treeStatus.propertyList.Count; i++ ) {
				SF_Node propertyNode = editor.nodeView.treeStatus.propertyList[i];
				if( propertyNode == null ) {
					editor.nodeView.treeStatus.propertyList.RemoveAt( i );
					i = -1; // restart
				}
				if( propertyNode.IsProperty() ) {
					string line = propertyNode.property.GetInitializationLine();
					App( line );
				}
			}

			bool transparency = ps.mOut.alphaClip.IsConnectedEnabledAndAvailable() || ps.HasAlpha();

			if( transparency )
				App( "[HideInInspector]_Cutoff (\"Alpha cutoff\", Range(0,1)) = 0.5" ); // Hack, but, required for transparency to play along with depth etc

			if( ps.catGeometry.showPixelSnap )
				App("[MaterialToggle] PixelSnap (\"Pixel snap\", Float) = 0");

			if( ps.catBlending.allowStencilWriteThroughProperties ) {
				App( "_Stencil (\"Stencil ID\", Float) = 0" );
				App( "_StencilReadMask (\"Stencil Read Mask\", Float) = 255" );
				App( "_StencilWriteMask (\"Stencil Write Mask\", Float) = 255" );
				App( "_StencilComp (\"Stencil Comparison\", Float) = 8" );
				App( "_StencilOp (\"Stencil Operation\", Float) = 0" );
				App( "_StencilOpFail (\"Stencil Fail Operation\", Float) = 0" );
				App( "_StencilOpZFail (\"Stencil Z-Fail Operation\", Float) = 0" );
			}



			End();

		}




		void PropertiesCG() {


			bool ValidProperty( SF_Node n ) {  // SpecColor already defined in Lighting.cginc
				return !( ( IncludeLightingCginc() || IncludeUnity5BRDF() ) && n.property.nameInternal == "_SpecColor" );
			}

			// Non-instanced properties
			for( int i = 0; i < cNodes.Count; i++ ) {
				AppIfNonEmpty( cNodes[i].GetPrepareUniformsAndFunctions() );
				if( cNodes[i].IsProperty() && cNodes[i].property.IsInstancedType() == false ) {
					if( ValidProperty( cNodes[i] ) )
						App( cNodes[i].property.GetVariableLine() );
				}
			}

			// Instanced properties
			if( dependencies.hasPropertiesWithInstancing ) {
				App( "UNITY_INSTANCING_BUFFER_START( Props )" );
				scope++;
				foreach( SF_Node node in cNodes.Where(n => n.IsProperty() && n.property.IsInstancedType() && ValidProperty(n) ) )
					App( $"UNITY_DEFINE_INSTANCED_PROP( {node.property.GetCGType()}, {node.property.nameInternal})" );
				scope--;
				App( "UNITY_INSTANCING_BUFFER_END( Props )" );
			}

		}
		void BeginSubShader() {
			App( "SubShader {" );
			scope++;
		}
		void BeginTags() {
			App( "Tags {" );
			scope++;
		}
		void BeginCG() {
			App( "CGPROGRAM" );

			if( dependencies.tessellation ) {
				App( "#pragma hull hull" );
				App( "#pragma domain domain" );
				App( "#pragma vertex tessvert" );
			} else {
				App( "#pragma vertex vert" );
			}
			App( "#pragma fragment frag" );



			switch( currentPass ) {
				/*
				case PassType.FwdBase:
					App( "#define UNITY_PASS_FORWARDBASE" ); // This seems to be auto-added somewhere else these days (??)
					break;
				case PassType.FwdAdd:
					App( "#define UNITY_PASS_FORWARDADD" );
					break;*/
				case PassType.Deferred:
					App( "#define UNITY_PASS_DEFERRED" );
					break;
				/*case PassType.ShadCast:
					App( "#define UNITY_PASS_SHADOWCASTER" );
					break;*/
				case PassType.Meta:
					App( "#define UNITY_PASS_META 1" );
					break;
			}


			if( LightmappedAndLit() ) {
				App( "#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )" );

			}
			
			if(ps.catLighting.reflectprobed ){
				App ("#define _GLOSSYENV 1");
			}

			if( dependencies.hasPropertiesWithInstancing )
				App( "#pragma multi_compile_instancing" );

			if( ps.catGeometry.showPixelSnap )
				App( "#pragma multi_compile _ PIXELSNAP_ON" );


			App( "#include \"UnityCG.cginc\"" );

			if( ShouldUseLightMacros() )
				App( "#include \"AutoLight.cginc\"" );
			if( IncludeLightingCginc() )
				App( "#include \"Lighting.cginc\"" );
			if( dependencies.tessellation )
				App( "#include \"Tessellation.cginc\"" );
			if( IncludeUnity5BRDF() ){
				App( "#include \"UnityPBSLighting.cginc\"" );
				App( "#include \"UnityStandardBRDF.cginc\"" );
			}
			if( currentPass == PassType.Meta ) {
				App("#include \"UnityMetaPass.cginc\"");
			}
			if( ps.catMeta.cgIncludes.Count > 0 ) { // Custom CG includes
				for (int i = 0; i < ps.catMeta.cgIncludes.Count; i++){
					string incStr = ps.catMeta.cgIncludes[i];
					if( incStr == string.Empty )
						continue;
					App( "#include \"" + incStr + ".cginc\"" );
				}
			}

			if( currentPass == PassType.FwdBase ) {
				App( "#pragma multi_compile_fwdbase" + ps.catBlending.GetShadowPragmaIfUsed() );
			} else if( currentPass == PassType.FwdAdd ) {
				App( "#pragma multi_compile_fwdadd" + ps.catBlending.GetShadowPragmaIfUsed() );
			} else {
				App( "#pragma fragmentoption ARB_precision_hint_fastest" );
				if(!ps.catExperimental.forceNoShadowPass)
					App( "#pragma multi_compile_shadowcaster" );
			}

			if( currentPass == PassType.Deferred ) {
				App( "#pragma multi_compile ___ UNITY_HDR_ON" );
			}

			if( LightmappedAndLit() ) {
				App( "#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON" );
				App( "#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE" );
				App( "#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON" );
			}

			if( UseUnity5Fog() )
				App( "#pragma multi_compile_fog" );

			


			List<int> groups = new List<int>();
			foreach( SF_Node n in cNodes ) {
				int group;
				string[] mcPrags = n.TryGetMultiCompilePragmas( out group );
				if( !groups.Contains( group ) && mcPrags != null ) {
					groups.Add( group );
					for( int i = 0; i < mcPrags.Length; i++ ) {
						App( "#pragma multi_compile " + mcPrags[i] );
					}
				}
				// Old branching tests
				//if(n.IsProperty() && n.property is SFP_Branch){
				//	App(n.property.GetMulticompilePragma ());
				//}
			}



			if( dependencies.DoesIncludePlatforms() )
				App( "#pragma only_renderers " + dependencies.GetIncludedPlatforms() );
			if( dependencies.IsTargetingAboveDefault() ) {
				if( ps.catExperimental.force2point0 )
					App( "#pragma target 2.0" );
				else
					App( "#pragma target " + dependencies.GetShaderTarget() );
			}

		}
		void EndCG() {
			App( "ENDCG" );
		}

		public bool IncludeUnity5BRDF() {
			return LightmappedAndLit() || ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL || ps.catLighting.reflectprobed;
		}

		bool UseUnity5Fog() {
			return ps.catBlending.useFog;
		}

		bool UseUnity5FogInThisPass() {
			return ps.catBlending.useFog && ( currentPass == PassType.FwdAdd || currentPass == PassType.FwdBase || currentPass == PassType.Outline );
		}



		void AppTag( string k, string v ) {
			App( "\"" + k + "\"=\"" + v + "\"" );
		}

		void PassTags() {
			BeginTags();
			if( currentPass == PassType.FwdBase )
				AppTag( "LightMode", "ForwardBase" );
			else if( currentPass == PassType.FwdAdd )
				AppTag( "LightMode", "ForwardAdd" );
			else if( currentPass == PassType.ShadCast )
				AppTag( "LightMode", "ShadowCaster" );
			else if( currentPass == PassType.Deferred )
				AppTag( "LightMode", "Deferred" );
			else if( currentPass == PassType.Meta )
				AppTag( "LightMode", "Meta" );
			End();
		}


		void SubShaderTags() {

			bool ip = ps.catBlending.ignoreProjector;
			bool doesOffset = ps.catBlending.queuePreset != Queue.Geometry || ps.catBlending.queueOffset != 0;
			bool hasRenderType = ps.catBlending.renderType != RenderType.None;
			bool hasBatchConfig = ps.catMeta.batchingMode != SFPSC_Meta.BatchingMode.Enabled;
			bool hasAtlasConfig = ps.catMeta.canUseSpriteAtlas;
			bool hasPreviewType = ps.catMeta.previewType != SFPSC_Meta.Inspector3DPreviewType.Sphere;

			if( !ip && !doesOffset && !hasRenderType && !hasBatchConfig && !hasAtlasConfig && !hasPreviewType )
				return; // No tags!

			BeginTags();
			if( ip )
				AppTag( "IgnoreProjector", "True" );
			if( doesOffset ) {
				string bse = ps.catBlending.queuePreset.ToString();
				string ofs = "";
				if( ps.catBlending.queueOffset != 0 )
					ofs = ps.catBlending.queueOffset > 0 ? ( "+" + ps.catBlending.queueOffset ) : ( ps.catBlending.queueOffset.ToString() );
				AppTag( "Queue", ( bse + ofs ).ToString() );
			}
			if( hasRenderType )
				AppTag( "RenderType", ps.catBlending.renderType.ToString() );
			if( hasBatchConfig ) {
				if(ps.catMeta.batchingMode == SFPSC_Meta.BatchingMode.Disabled)
					AppTag( "DisableBatching", "True" );
				if(ps.catMeta.batchingMode == SFPSC_Meta.BatchingMode.DisableDuringLODFade)
					AppTag( "DisableBatching", "LODFading" );
			}
			if( hasAtlasConfig ) {
				AppTag( "CanUseSpriteAtlas", "True" );
			}
			if( hasPreviewType ) {
				if( ps.catMeta.previewType == SFPSC_Meta.Inspector3DPreviewType.Plane)
					AppTag( "PreviewType", "Plane" );
				if( ps.catMeta.previewType == SFPSC_Meta.Inspector3DPreviewType.Skybox )
					AppTag( "PreviewType", "Skybox" );
			}



			End();
		}

		void RenderSetup() {

			if( currentPass == PassType.FwdAdd )
				App( "Blend One One" );
			else if( currentPass == PassType.FwdBase && ps.catBlending.UseBlending() ) // Shadow passes and outlines use default blending
				App( ps.catBlending.GetBlendString() );

			if( currentPass == PassType.Meta ) {
				App( "Cull Off" );
			} else if( currentPass == PassType.ShadCast ) {
				App( "Offset 1, 1" );
				App( ps.catGeometry.GetCullString() );
			} else if( currentPass == PassType.Outline ) {
				App( "Cull Front" );
			} else if( ps.catGeometry.UseCulling() )
				App( ps.catGeometry.GetCullString() );

			if( ps.catBlending.UseDepthTest() && !IsShadowOrOutlineOrMetaPass() ) // Shadow passes and outlines use default
				App( ps.catBlending.GetDepthTestString() );

			if( !IsShadowOrOutlineOrMetaPass() ) {
				App( ps.catBlending.GetZWriteString() );
			}

			if( ps.catBlending.colorMask != 15 ) { // 15 means RGBA, which is default
				App("ColorMask " + ps.catBlending.colorMask.ToColorMaskString());
			}

			App( ps.catBlending.GetOffsetString() );

			if(ps.catBlending.useStencilBuffer && currentPass == PassType.FwdBase){
				App("Stencil {");
				scope++;
				App( ps.catBlending.GetStencilContent() );
				scope--;
				App("}");
			}

			if( currentPass == PassType.FwdBase && ps.catBlending.alphaToCoverage ) {
				App("AlphaToMask On");
			}


		}

		void CGvars() {

			if( editor.mainNode.alphaClip.IsConnectedEnabledAndAvailable() ) {
				if( ps.catBlending.dithering == Dithering.Dither2x2 ) {
					App( "// Dithering function, to use with scene UVs (screen pixel coords)" );
					App( "// 2x2 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering" );
					App( "float BinaryDither2x2( float value, float2 sceneUVs ) {" );
					scope++;
					App( "float2x2 mtx = float2x2(" );
					scope++;
					App( "float2( 1, 3 )/5.0," );
					App( "float2( 4, 2 )/5.0" );
					scope--;
					App( ");" );
					App( "float2 px = floor(_ScreenParams.xy * sceneUVs);" );
					App( "int xSmp = fmod(px.x,2);" );
					App( "int ySmp = fmod(px.y,2);" );
					App( "float2 xVec = 1-saturate(abs(float2(0,1) - xSmp));" );
					App( "float2 yVec = 1-saturate(abs(float2(0,1) - ySmp));" );
					App( "float2 pxMult = float2( dot(mtx[0],yVec), dot(mtx[1],yVec) );" );
					App( "return round(value + dot(pxMult, xVec));" );
					scope--;
					App( "}" );
				} else if( ps.catBlending.dithering == Dithering.Dither3x3 ) {
					App( "// Dithering function, to use with scene UVs (screen pixel coords)" );
					App( "// 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering" );
					App( "float BinaryDither3x3( float value, float2 sceneUVs ) {" );
					scope++;
					App( "float3x3 mtx = float3x3(" );
					scope++;
					App( "float3( 3,  7,  4 )/10.0," );
					App( "float3( 6,  1,  9 )/10.0," );
					App( "float3( 2,  8,  5 )/10.0" );
					scope--;
					App( ");" );
					App( "float2 px = floor(_ScreenParams.xy * sceneUVs);" );
					App( "int xSmp = fmod(px.x,3);" );
					App( "int ySmp = fmod(px.y,3);" );
					App( "float3 xVec = 1-saturate(abs(float3(0,1,2) - xSmp));" );
					App( "float3 yVec = 1-saturate(abs(float3(0,1,2) - ySmp));" );
					App( "float3 pxMult = float3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );" );
					App( "return round(value + dot(pxMult, xVec));" );
					scope--;
					App( "}" );
				} else if( ps.catBlending.dithering == Dithering.Dither4x4 ) {
					App( "// Dithering function, to use with scene UVs (screen pixel coords)" );
					App( "// 4x4 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering" );
					App( "float BinaryDither4x4( float value, float2 sceneUVs ) {" );
					scope++;
					App( "float4x4 mtx = float4x4(" );
					scope++;
					App( "float4( 1,  9,  3, 11 )/17.0," );
					App( "float4( 13, 5, 15,  7 )/17.0," );
					App( "float4( 4, 12,  2, 10 )/17.0," );
					App( "float4( 16, 8, 14,  6 )/17.0" );
					scope--;
					App( ");" );
					App( "float2 px = floor(_ScreenParams.xy * sceneUVs);" );
					App( "int xSmp = fmod(px.x,4);" );
					App( "int ySmp = fmod(px.y,4);" );
					App( "float4 xVec = 1-saturate(abs(float4(0,1,2,3) - xSmp));" );
					App( "float4 yVec = 1-saturate(abs(float4(0,1,2,3) - ySmp));" );
					App( "float4 pxMult = float4( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec), dot(mtx[3],yVec) );" );
					App( "return round(value + dot(pxMult, xVec));" );
					scope--;
					App( "}" );
				}
			}


			if( dependencies.lightColor && !IncludeLightingCginc() && !IncludeUnity5BRDF() ) // Lightmap and shadows include Lighting.cginc, which already has this. Don't include when making Unity 5 shaders
				App( "uniform float4 _LightColor0;" );


			if( dependencies.grabPass ) {
				App( "uniform sampler2D " + ps.catBlending.GetGrabTextureName() + ";" );
			}
				
			if( dependencies.depthTexture )
				App( "uniform sampler2D _CameraDepthTexture;" );

			if( dependencies.fog_color ) {
				App( "uniform float4 unity_FogColor;" );
			}


			 
			PropertiesCG();

		}

		void InitViewDirVert() {
			if( dependencies.vert_viewDirection )
				App( "float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);" );
		}
		void InitViewDirFrag() {
			if( dependencies.frag_viewDirection )
				App( "float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);" );
		}
		void InitTangentTransformFrag() {
			if( ( dependencies.frag_tangentTransform && currentProgram == ShaderProgram.Frag ) || ( dependencies.vert_tangentTransform && currentProgram == ShaderProgram.Vert ) )
				App( "float3x3 tangentTransform = float3x3( " + WithProgramPrefix( "tangentDir" ) + ", " + WithProgramPrefix( "bitangentDir" ) + ", " + WithProgramPrefix( "normalDir" ) + ");" );
		}




		string LightmapNormalDir() {
			if( editor.mainNode.normal.IsConnectedAndEnabled() ) {
				return "normalLocal";
			}
			return "float3(0,0,1)";
		}

		void PrepareLightmapVars() {
			if( !LightmapThisPass() )
				return;

		
			// TODO U5 LMs


		}

		void InitLightDir() {

			if( IsShadowPass() )
				return;

			if( ( currentProgram == ShaderProgram.Frag && !dependencies.frag_lightDirection ) || ( currentProgram == ShaderProgram.Vert && !dependencies.vert_lightDirection ) )
				return;

			if( currentPass == PassType.FwdBase ) {

				App( "float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);" );

				return;
			}

			// Point vs directional
			App( "float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - " + WithProgramPrefix( "posWorld.xyz" ) + ",_WorldSpaceLightPos0.w));" );

		}

		void InitLightColor(){
			App("float3 lightColor = _LightColor0.rgb;");
		}


		void InitHalfVector() {
			if( ( !dependencies.frag_halfDirection && currentProgram == ShaderProgram.Frag ) || ( !dependencies.vert_halfDirection && currentProgram == ShaderProgram.Vert ) )
				return;
			App( "float3 halfDirection = normalize(viewDirection+lightDirection);" );
		}

		void InitAttenuation() {

			if( SF_Evaluator.inVert && ps.catLighting.IsVertexLit() && ShouldUseLightMacros() )
				App( "TRANSFER_VERTEX_TO_FRAGMENT(o)" );

			string atten = "LIGHT_ATTENUATION(" + ( ( currentProgram == ShaderProgram.Frag ) ? "i" : "o" ) + ")";

			string inner = ( ShouldUseLightMacros() ? atten : "1" );
			App( "float attenuation = " + inner + ";" );
			if( ps.catLighting.lightMode != SFPSC_Lighting.LightMode.Unlit )
				App( "float3 attenColor = attenuation * _LightColor0.xyz;" );
		}


		string GetWithDiffPow( string s ) {
			if( ps.HasDiffusePower() ) {
				return "pow(" + s + ", " + ps.n_diffusePower + ")";
			}
			return s;
		}



		void CalcDiffuse() {

			//App( "float atten = 1.0;" );
			AppDebug( "Diffuse" );



			//InitAttenuation();


			string lmbStr = "";







			if( !InDeferredPass() ) {
				bool definedNdotL = ps.HasSpecular();
				bool definedNdotLwrap = false;

				if( ps.HasTransmission() || ps.HasLightWrapping() ) {

					

					if( !InDeferredPass() ) {
						if( !definedNdotL ) {
							App( "float NdotL = dot( " + VarNormalDir() + ", lightDirection );" );
							definedNdotL = true;
						} else {
							App( "NdotL = dot( " + VarNormalDir() + ", lightDirection );" );
						}
						definedNdotL = true;
					}

					string fwdLight = "float3 forwardLight = "; // TODO
					string backLight = "float3 backLight = "; // TODO


					if( ps.HasLightWrapping() ) {
						App( "float3 w = " + ps.n_lightWrap + "*0.5; // Light wrapping" );
						if( !definedNdotLwrap ) {
							App( "float3 NdotLWrap = NdotL * ( 1.0 - w );" );
							definedNdotLwrap = true;
						}
							
						App( fwdLight + GetWithDiffPow( "max(float3(0.0,0.0,0.0), NdotLWrap + w )" ) + ";" );
						if( ps.HasTransmission() ) {
							App( backLight + GetWithDiffPow( "max(float3(0.0,0.0,0.0), -NdotLWrap + w )" ) + " * " + ps.n_transmission + ";" );
						}

					} else {
						App( fwdLight + GetWithDiffPow( "max(0.0, NdotL )" ) + ";" );
						if( ps.HasTransmission() ) {
							App( backLight + GetWithDiffPow( "max(0.0, -NdotL )" ) + " * " + ps.n_transmission + ";" );
						}
					}

					lmbStr = "forwardLight";

					if( ps.HasTransmission() ) {
						lmbStr += "+backLight";
						lmbStr = "(" + lmbStr + ")";
					}

				}// else {


				bool noSpec = !ps.HasSpecular();
				bool unity5pblDiffuse = ps.HasDiffuse() && ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL;

				bool needsToDefineNdotV = noSpec && unity5pblDiffuse;

				if( needsToDefineNdotV ) {
					App( "float NdotV = max(0.0,dot( " + VarNormalDir() + ", viewDirection ));" );
				}



				if( !definedNdotL ) {
					App( "float NdotL = max(0.0,dot( " + VarNormalDir() + ", lightDirection ));" );
				} else {
					App( "NdotL = max(0.0,dot( " + VarNormalDir() + ", lightDirection ));" );
				}

				if( Unity5PBL() ) {
					//if( ps.HasTransmission() || ps.HasLightWrapping() )
						//App( "NdotL = max(0.0,NdotL);" );
					App( "half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);" );
					if( ps.HasTransmission() || ps.HasLightWrapping() ) {
						if( !definedNdotLwrap )
							App( "float3 NdotLWrap = max(0,NdotL);" );
						App( "float nlPow5 = Pow5(1-NdotLWrap);" );
					} else {
						App( "float nlPow5 = Pow5(1-NdotL);" );
					}
					App( "float nvPow5 = Pow5(1-NdotV);" );


					string pbrStr = "((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)";

					if( ps.HasTransmission() || ps.HasLightWrapping() ) {
						lmbStr = "(" + lmbStr + " + " + pbrStr + ")";
					} else {
						lmbStr = pbrStr;
					}


				} else if( !( ps.HasTransmission() || ps.HasLightWrapping() ) ) {
					lmbStr = GetWithDiffPow( "max( 0.0, NdotL)" );
				}



				//}

				if( ps.catLighting.IsEnergyConserving() && !Unity5PBL() ) {
					if( ps.HasLightWrapping() ) {
						lmbStr += "*(0.5-max(w.r,max(w.g,w.b))*0.5)";
					}
				}

				lmbStr = "float3 directDiffuse = " + lmbStr + " * attenColor";
				lmbStr += ";";
				App( lmbStr );
			}
			


			bool ambDiff = ps.mOut.ambientDiffuse.IsConnectedEnabledAndAvailableInThisPass( currentPass );
			bool shLight = DoPassSphericalHarmonics();
			bool diffAO = ps.mOut.diffuseOcclusion.IsConnectedEnabledAndAvailableInThisPass( currentPass );
			bool ambLight = ps.catLighting.useAmbient && ( currentPass == PassType.FwdBase ) && !LightmappedAndLit(); // Ambient is already in light probe data

			bool hasIndirectLight = ambDiff || shLight || ambLight; // TODO: Missing lightmaps


			

			if( hasIndirectLight ) {
				App( "float3 indirectDiffuse = float3(0,0,0);" );
			}



			
			
			



			// Direct light done, now let's do indirect light

			//if( !InDeferredPass() ) {
				if( hasIndirectLight ) {
					//App( " indirectDiffuse = float3(0,0,0);" );

					if( ambLight )
						App( "indirectDiffuse += " + GetAmbientStr() + "; // Ambient Light" );
					if( ambDiff )
						App( "indirectDiffuse += " + ps.n_ambientDiffuse + "; // Diffuse Ambient Light" );


					if( LightmappedAndLit() ) {


						App( "indirectDiffuse += gi.indirect.diffuse;" );


					}

					// Diffuse AO
					if( diffAO ) {
						App( "indirectDiffuse *= " + ps.n_diffuseOcclusion + "; // Diffuse AO" );
					}


				}
			//}


			//if( LightmapThisPass() ) {
			//	scope--;
			//App( "#endif" );
			//	}

			// This has been defined before specular, in the case of PBL
			if( !Unity5PBL() ) {
				App( "float3 diffuseColor = " + ps.n_diffuse + ";" );
			}

			// To make diffuse/spec tradeoff better
			if( DoPassDiffuse() && DoPassSpecular() ) {
				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) {
					if(ps.catLighting.specularMode != SFPSC_Lighting.SpecularMode.Metallic) // Metallic has already done this by now
						App( "diffuseColor *= 1-specularMonochrome;" );
				} else if( ps.catLighting.energyConserving ) {
					App( "diffuseColor *= 1-specularMonochrome;" );
				}
			}

			if( !InDeferredPass() ) {
				if( hasIndirectLight ) {
					App( "float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;" );
				} else {
					App( "float3 diffuse = directDiffuse * diffuseColor;" );
				}

			}
			
			//if( SF_Tools.UsingUnity5plus && ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) {
			//	App( "diffuse *= 0.75;" );
			//}


			


		}

		bool LightmapThisPass() {
			return LightmappedAndLit() && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred );
		}

		void InitNormalDirVert() {
			if( dependencies.vert_out_normals ) {
				App( "o.normalDir = UnityObjectToWorldNormal(" + ps.catGeometry.GetNormalSign() + "v.normal);" );
			}
		}

		void InitTangentDirVert() {
			App( "o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );" );
		}

		void InitBitangentDirVert() {
			App( "o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);" );
		}

		void InitObjectPos() {
			if( dependencies.frag_objectPos || dependencies.vert_objectPos )
				App( "float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );" );
		}
		void InitObjectScale() {
			if( dependencies.objectScaleReciprocal || dependencies.objectScale )
				App( "float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );" );
			if( dependencies.objectScale )
				App( "float3 objScale = 1.0/recipObjScale;" );
		}

		void InitNormalDirFrag() {

			if( ( !dependencies.frag_normalDirection && currentProgram == ShaderProgram.Frag ) )
				return;




			//if(ps.normalQuality == SF_PassSettings.NormalQuality.Normalized){
			//	App ("i.normalDir = normalize(i.normalDir);");
			//}



			if( currentPass == PassType.ShadCast || currentPass == PassType.Meta ) {
				App( "float3 normalDirection = i.normalDir;" );
			} else {
				if( ps.HasTangentSpaceNormalMap() ) {
					App( "float3 normalLocal = " + ps.n_normals + ";" );
					App( "float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals" );
				} else if( ps.HasObjectSpaceNormalMap() ) {
					App( "float3 normalLocal = " + ps.n_normals + ";" );
					App( "float3 normalDirection = mul( unity_WorldToObject, float4(normalLocal,0)) / recipObjScale;" );
				} else if( ps.HasWorldSpaceNormalMap() ) {
					App( "float3 normalDirection = " + ps.n_normals + ";" );
				} else {
					App( "float3 normalDirection = i.normalDir;" );
				}
			}



		}


		void CalcGloss() {
			AppDebug( "Gloss" );
			if( ps.catLighting.glossRoughMode == SFPSC_Lighting.GlossRoughMode.Roughness ){
				App( "float gloss = 1.0 - " + ps.n_gloss + "; // Convert roughness to gloss" );
				if( Unity5PBL() )
					App( "float perceptualRoughness = " + ps.n_gloss + ";" );
			} else {
				App( "float gloss = " + ps.n_gloss + ";" );
				if( Unity5PBL() )
					App( "float perceptualRoughness = 1.0 - " + ps.n_gloss + ";" );
			}

			if( Unity5PBL() ) {
				if( ps.catLighting.geometricAntiAliasing ) {
					App( "float3 spaaDx = ddx(i.normalDir);" );
					App( "float3 spaaDy = ddy(i.normalDir);" );
					App( "float geoRoughFactor = pow(saturate(max(dot(spaaDx,spaaDx),dot(spaaDy,spaaDy))),0.333);" );
					App( "perceptualRoughness = max(perceptualRoughness, geoRoughFactor);" );
				}
				App( "float roughness = perceptualRoughness * perceptualRoughness;" );
			}
			
			if( !InDeferredPass() ) {
				if( ps.catLighting.remapGlossExponentially ) {
					App( "float specPow = exp2( gloss * 10.0 + 1.0 );" );
				} else {
					App( "float specPow = gloss;" );
				}
			}
			
			
			
		}

		bool DoAmbientSpecThisPass() {
			return ( mOut.ambientSpecular.IsConnectedEnabledAndAvailable() && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred ) );
		}


		void CalcSpecular() {



			AppDebug( "Specular" );

			

			if( currentPass != PassType.Deferred ) {
				App( "float NdotL = saturate(dot( " + VarNormalDir() + ", lightDirection ));" );
			}


			//if(DoAmbientSpecThisPass() && ps.IsPBL())
			//App ("float NdotR = max(0, dot(viewReflectDirection, normalDirection));"); // WIP

			string directSpecular = "float3 directSpecular = ";

			string attColStr;
			//if( ps.catLighting.maskedSpec && currentPass == PassType.FwdBase && ps.catLighting.lightMode != SFPSC_Lighting.LightMode.PBL ) {
			//	attColStr = "(floor(attenuation) * _LightColor0.xyz)";
			//} else {
			attColStr = "attenColor";
			//}








			/*
			 * float3 specular = pow(max(0.0,dot(halfDirection, normalDirection)),specPow) * specularColor;
							#ifndef LIGHTMAP_OFF
								#ifndef DIRLIGHTMAP_OFF
									specular *= lightmap;
								#else
									specular *= floor(attenuation) * _LightColor0.xyz;
								#endif
							#else
								specular = floor(attenuation) * _LightColor0.xyz;
							#endif
			 * */

			directSpecular += attColStr; /* * " + ps.n_specular;*/ // TODO: Doesn't this double the spec? Removed for now. Shouldn't evaluate spec twice when using PBL




			//if( mOut.ambientSpecular.IsConnectedEnabledAndAvailable() && currentPass == PassType.FwdBase){
			//	s += "(attenColor + " + ps.n_ambientSpecular + ")";
			//} else {
			//	s += "attenColor";
			//}



			bool occluded = ps.mOut.specularOcclusion.IsConnectedEnabledAndAvailableInThisPass( currentPass ) && !InDeferredPass();
			bool ambSpec = DoAmbientSpecThisPass();
			bool reflProbed = dependencies.reflection_probes;
			bool hasIndirectSpecular = ambSpec || ( reflProbed && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred ) );
			string indirectSpecular = "";

			if( hasIndirectSpecular ) {

				if( occluded ) {
					App( "float3 specularAO = " + ps.n_specularOcclusion + ";" );
				}

				indirectSpecular = "float3 indirectSpecular = ";





				if( reflProbed ) {
					indirectSpecular += "(gi.indirect.specular";
				} else {
					indirectSpecular += "(0";
				}
				

				if( ambSpec ) {
					indirectSpecular += " + " + ps.n_ambientSpecular + ")";
				} else {
					indirectSpecular += ")";
				}

				if( occluded ) {
					indirectSpecular += " * specularAO";
				}


			}








			if( ps.catLighting.IsPBL() && !InDeferredPass() ) {

				App( "float LdotH = saturate(dot(lightDirection, halfDirection));" );

				

				//s += "*NdotL"; // TODO: Really? Is this the cosine part?

				//if(DoAmbientSpecThisPass())
				//sAmb += " * NdotR";

			}

			if( !InDeferredPass() && !Unity5PBL() ) {
				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.Phong )
					directSpecular += " * pow(max(0,dot(reflect(-lightDirection, " + VarNormalDir() + "),viewDirection))";
				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.BlinnPhong ) {
					directSpecular += " * pow(max(0,dot(halfDirection," + VarNormalDir() + "))";
				}
				directSpecular += ",specPow)";
			}

			bool initialized_NdotV = false;
			bool initialized_NdotH = false;
			bool initialized_VdotH = false;


			App( "float3 specularColor = " + ps.n_specular + ";" );
			if( Unity5PBL() ) {
				App( "float specularMonochrome;" );
				App( "float3 diffuseColor = " + ps.n_diffuse + "; // Need this for specular when using metallic" );
				if( MetallicPBL() ) {
					App( "diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );" );
				} else {
					App( "diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);" );
				}
				App( "specularMonochrome = 1.0-specularMonochrome;" );
			} else if( ps.catLighting.energyConserving && DoPassDiffuse() && DoPassSpecular() ){
				App( "float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);" );
			}
			

			
			
			

			
			string specularPBL = "";

			// PBL SHADING, normalization term comes after this
			if( ps.catLighting.IsPBL() && !InDeferredPass() ) {



				// FRESNEL TERM
				//App( "float3 specularColor = " + ps.n_specular + ";" );
				

				
				//specularPBL += "*NdotL";



				// VISIBILITY TERM / GEOMETRIC TERM?

				if( !initialized_NdotV ) {
					App( "float NdotV = abs(dot( " + VarNormalDir() + ", viewDirection ));" );
					initialized_NdotV = true;
				}

				
				if( !initialized_NdotH ) {
					App( "float NdotH = saturate(dot( " + VarNormalDir() + ", halfDirection ));" );
					initialized_NdotH = true;
				}
				if( !initialized_VdotH ) {
					App( "float VdotH = saturate(dot( viewDirection, halfDirection ));" );
					initialized_VdotH = true;
				}

				App( "float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );" );

				specularPBL += "*visTerm";

				
				
				


			} else {
				//sAmb += " * specularColor";
				//directSpecular += " * specularColor";
			}



			if( ps.catLighting.IsEnergyConserving() && !InDeferredPass() ) {
				// NORMALIZATION TERM
				if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.Phong ) {
					App( "float normTerm = (specPow + 2.0 ) / (2.0 * Pi);" );
					directSpecular += "*normTerm";
				} else if( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.BlinnPhong || ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) {
					if( Unity5PBL() ) {

						if( !initialized_NdotH ) {
							App( "float NdotH = saturate(dot( " + VarNormalDir() + ", halfDirection ));" );
							initialized_NdotH = true;
						}

						App( "float normTerm = GGXTerm(NdotH, roughness);" );
						specularPBL += "*normTerm";
						
					} else {
						App( "float normTerm = (specPow + 8.0 ) / (8.0 * Pi);" );
						directSpecular += "*normTerm";
					}

				}

				if( DoAmbientSpecThisPass() ) {
					//sAmb += " * normTerm";
				}




			}





			if( !InDeferredPass() ) {
				if( Unity5PBL() ) {

					if( !initialized_NdotV ) {
						App( "float NdotV = max(0.0,dot( " + VarNormalDir() + ", viewDirection ));" );
						initialized_NdotV = true;
					}
				


					specularPBL = specularPBL.Substring( 1 ); // Remove first * symbol
					specularPBL = "float specularPBL = (" + specularPBL + ") * UNITY_PI;";
				
					App( specularPBL );

					App( "#ifdef UNITY_COLORSPACE_GAMMA" );
					scope++;
					App( "specularPBL = sqrt(max(1e-4h, specularPBL));" );
					scope--;
					App( "#endif" );
					App( "specularPBL = max(0, specularPBL * NdotL);" );
					App( "#if defined(_SPECULARHIGHLIGHTS_OFF)" );
					scope++;
					App( "specularPBL = 0.0;" );
					scope--;
					App( "#endif" );

					// Surface reduction
					if( hasIndirectSpecular ) {
						App( "half surfaceReduction;" );
						App( "#ifdef UNITY_COLORSPACE_GAMMA" );
						scope++;
						App( "surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;" );
						scope--;
						App( "#else" );
						scope++;
						App( "surfaceReduction = 1.0/(roughness*roughness + 1.0);" );
						scope--;
						App( "#endif" );
					}
					

					// Kill spec if color = 0
					App( "specularPBL *= any(specularColor) ? 1.0 : 0.0;" );


					directSpecular += "*specularPBL*FresnelTerm(specularColor, LdotH)";
				} else {
					directSpecular += "*specularColor";
				}

				directSpecular += ";";

				App( directSpecular );
			} else {
				// If we're in deferred, we still need NdotV for lightmapping
				if( !initialized_NdotV ) {
					App( "float NdotV = max(0.0,dot( " + VarNormalDir() + ", viewDirection ));" );
					initialized_NdotV = true;
				}
			}


			 


			string specular = "";


			if( hasIndirectSpecular ) {

				if( Unity5PBL() ) {
					App( "half grazingTerm = saturate( gloss + specularMonochrome );" );
				} else {
					indirectSpecular += "*specularColor";
				}

				App( indirectSpecular + ";" );

				if( Unity5PBL() ) {
					if( ps.HasSpecular() ) {
						App( "indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);" );
						if(!InDeferredPass())
							App( "indirectSpecular *= surfaceReduction;" );
					} else {
						//App( "float3 indirectFresnelPBL = FresnelLerp (specularColor, grazingTerm, NdotV);" );
					}
				}
				if( !InDeferredPass() ) {
					specular = "float3 specular = (directSpecular + indirectSpecular);";
				}
					
			} else if(!InDeferredPass()){
				specular = "float3 specular = directSpecular;";
			}

			if( !InDeferredPass() )
				App( specular ); // Specular

			








		}

		// Spec & emissive
		/*
		void CalcAddedLight() {


			// No added light unless we're using spec or emissive
			if( !ps.HasSpecular() && !ps.HasEmissive() )
				return;

			AppDebug("CalcAddedLight()");

			string s = "";
			//if( ps.HasSpecular() || ps.HasEmissive() && currentPass == PassType.FwdBase )

			if( ps.HasSpecular() ) {

				CalcGloss();
				CalcSpecular();

				if( ps.HasEmissive() && currentPass == PassType.FwdBase )
					s += " + " + ps.n_emissive;

				s += ";";

			} else if( ps.HasEmissive() && currentPass == PassType.FwdBase ) {
				s = "float3 addLight = ";
				s += ps.n_emissive + ";";
			}

			App( s );
		}
		*/


		public bool MetallicPBL() {
			return ps.catLighting.IsPBL() && ps.catLighting.specularMode == SFPSC_Lighting.SpecularMode.Metallic;
		}

		bool Unity5PBL() {
			return ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL;
		}


		void CalcEmissive() {
			AppDebug( "Emissive" );
			App( "float3 emissive = " + ps.n_emissive + ";" );
		}

		bool DoPassDiffuse() {
			return ps.HasDiffuse() && ( currentPass == PassType.FwdBase || currentPass == PassType.FwdAdd || currentPass == PassType.Deferred );
		}
		bool DoPassEmissive() { // Emissive should always be in the base pass
			return ps.HasEmissive() && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred );
		}
		bool DoPassSpecular() { // Spec only in base and add passes
			return ps.catLighting.HasSpecular() && ( currentPass == PassType.FwdBase || currentPass == PassType.FwdAdd || currentPass == PassType.Deferred );
		}



		void CalcFinalLight() {
			//bool addedOnce = false;
			string finalLightStr = "float3 lightFinal = ";
			if( ps.catLighting.IsLit() ) {
				finalLightStr += "diffuse";
				if( ps.catLighting.useAmbient && currentPass == PassType.FwdBase ) {
					finalLightStr += " + UNITY_LIGHTMODEL_AMBIENT.xyz";
				}
			}

			finalLightStr += ";";
			App( finalLightStr );

		}




		void AppFinalOutput( string color, string alpha ) {

			string rgbaValue;
			if( ps.HasRefraction() && currentPass == PassType.FwdBase ) {
				rgbaValue = "fixed4(lerp(sceneColor.rgb, " + color + "," + alpha + "),1)";
			} else {
				rgbaValue = "fixed4(" + color + "," + alpha + ")";
			}

			if( UseUnity5FogInThisPass() ) {
				App( "fixed4 finalRGBA = " + rgbaValue + ";" );
				if( ps.catBlending.fogOverrideColor ) {
					App( "UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, " + GetFogColorAsFixed3Value() + ");" );
				} else {
					App( "UNITY_APPLY_FOG(i.fogCoord, finalRGBA);" );
				}
				App( "return finalRGBA;" );
			} else {
				App( "return " + rgbaValue + ";" );
			}

		}

		string GetFogColorAsFixed3Value() {
			Color c = ps.catBlending.fogColor;
			return "fixed4(" + c.r + "," + c.g + "," + c.b + "," + c.a + ")";
		}


		string GetAmbientStr() {
			string s;
			if( InDeferredPass() )
				s = "unity_Ambient.rgb";
			else
				s = "UNITY_LIGHTMODEL_AMBIENT.rgb";


			if( InDeferredPass() ) {
				s += "*0.5"; // TODO: Maybe not?
			}





			return s;

		}


		bool DoPassSphericalHarmonics() {
			return DoPassDiffuse() && LightmappedAndLit() && ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred );
		}

		bool InDeferredPass() {
			return currentPass == PassType.Deferred;
		}


		void Lighting() {

			if( IsShadowOrOutlineOrMetaPass() )
				return;
			AppDebug( "Lighting" );

			/*
			if( ps.IsVertexLit() && SF_Evaluator.inFrag ) {
				string finalLightStr = "float3 lightFinal = i.vtxLight";

				if(DoPassDiffuse())
					finalLightStr += " * " + ps.n_diffuse; // TODO: Not ideal, affects both spec and diffuse

				finalLightStr += ";";
				App( finalLightStr ); // TODO: Emissive and other frag effects? TODO: Separate vtx spec and vtx diffuse
				return;
			}
			*/

			bool attenBuiltin = ps.catLighting.IsLit() && ( ps.HasDiffuse() || ps.catLighting.HasSpecular() ) && currentPass != PassType.Deferred;

			if( attenBuiltin || ( dependencies.frag_attenuation && SF_Evaluator.inFrag ) )
				InitAttenuation();

			if( !ps.catLighting.IsLit() && SF_Evaluator.inFrag ) {


				string s = "float3 finalColor = ";



				//bool doAmbient = (currentPass == ShaderForge.PassType.FwdBase && ps.useAmbient);
				bool doEmissive = DoPassEmissive();
				bool doCustomLight = mOut.customLighting.IsConnectedEnabledAndAvailable();

				bool didAddLight = /*doAmbient || */doEmissive || doCustomLight;

				bool somethingAdded = false;
				//if( doAmbient ){
				//	s += somethingAdded ? " + ":"";
				//	s += GetAmbientStr();
				//	somethingAdded = true;
				//}
				if( doEmissive ) {
					CalcEmissive();
					s += somethingAdded ? " + " : "";
					s += "emissive";
					somethingAdded = true;
				}
				if( doCustomLight ) {
					s += somethingAdded ? " + " : "";
					s += ps.n_customLighting;
					somethingAdded = true;
				}



				if( !didAddLight )
					s += "0"; // TODO: Don't do lighting at all if this is the case


				s += ";";

				App( s );

				//if( ps.useAmbient && currentPass == PassType.FwdBase )
				//	App( "float3 lightFinal = " + ps.n_emissive + "+UNITY_LIGHTMODEL_AMBIENT.xyz;"); // TODO; THIS IS SUPER WEIRD
				//else
				//	App( "float3 lightFinal = " + ps.n_emissive + ";"); // Kinda weird, but emissive = light when unlit is on, so it's needed in additional passes too
				return;

			}



			// Else if frag light...

			//InitLightDir();

			//if(SF_Evaluator.inFrag)


			if( DoPassDiffuse() || DoPassSpecular() ) {
				if( ps.catLighting.IsEnergyConserving() ) {
					App( "float Pi = 3.141592654;" );
					App( "float InvPi = 0.31830988618;" );
				}
			}


			bool unity5pblDiffusePlugged = ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL && ps.mOut.diffuse.IsConnectedEnabledAndAvailableInThisPass(currentPass);


			if( DoPassSpecular() || unity5pblDiffusePlugged ) { // Specular
				CalcGloss();
			}


			CalcGIdata();




			if( DoPassSpecular() ) { // Specular
				//if( !InDeferredPass() )
					//CalcGloss();
				CalcSpecular();
				//AppDebug("Spec done"); 
			}

			if( DoPassDiffuse() ) // Diffuse + texture (If not vertex lit)
				CalcDiffuse();

			if( DoPassEmissive() ) // Emissive
				CalcEmissive();

			/*if(!ps.IsLit() && ps.mOut.customLighting.IsConnectedEnabledAndAvailable() ){

				App("float3 lightFinal = " + ps.n_customLighting );

			}*/
			if( /*!ps.IsVertexLit() &&*/ currentProgram == ShaderProgram.Frag ) {

				AppDebug( "Final Color" );

				/*
				bool fresnelIndirectPBL =
					Unity5PBL() &&
					( ps.catLighting.reflectprobed || ps.HasAmbientSpecular() ) && 
					(currentPass == PassType.FwdBase || currentPass == PassType.PrePassFinal)
				;*/

				

				if(!InDeferredPass()){


					string diffuse = ps.HasAlpha() && ps.catLighting.transparencyMode == SFPSC_Lighting.TransparencyMode.Reflective ? "diffuse * " + ps.n_alpha : "diffuse" ;

					string s = SumString(
						new bool[] { DoPassDiffuse(), DoPassSpecular(), DoPassEmissive() },
						new string[] { diffuse, "specular", "emissive" },
						"0"
					);
				
					App( "float3 finalColor = " + s + ";" );
				}
			
				
			}

		}


		void CalcGIdata(){


			if( ( currentPass == PassType.FwdBase || currentPass == PassType.Deferred ) && ( ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL || ps.catLighting.reflectprobed || LightmappedAndLit() ) ) {


				AppDebug("GI Data");


				if( InDeferredPass() ) {
					App( "UnityLight light; // Dummy light" );
					App( "light.color = 0;" );
					App( "light.dir = half3(0,1,0);" );
					App( "light.ndotl = max(0,dot(normalDirection,light.dir));" );
				} else {
					App( "UnityLight light;" );
					App( "#ifdef LIGHTMAP_OFF" );
					scope++;
					App( "light.color = lightColor;" );
					App( "light.dir = lightDirection;" );
					App( "light.ndotl = LambertTerm (normalDirection, light.dir);" );
					scope--;
					App( "#else" );
					scope++;
					App( "light.color = half3(0.f, 0.f, 0.f);" );
					App( "light.ndotl = 0.0f;" );
					App( "light.dir = half3(0.f, 0.f, 0.f);" );
					scope--;
					App( "#endif" );
				}
				
				
				
				App("UnityGIInput d;");
				App("d.light = light;");
				App("d.worldPos = i.posWorld.xyz;");
				App("d.worldViewDir = viewDirection;");
				if( InDeferredPass() )
					App( "d.atten = 1;" );
				else
					App("d.atten = attenuation;");

				if( LightmappedAndLit() ) {
					App( "#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)" );
					scope++;
					App( "d.ambient = 0;" );
					App( "d.lightmapUV = i.ambientOrLightmapUV;" );
					scope--;
					App( "#else" );
					scope++;
					App( "d.ambient = i.ambientOrLightmapUV;" );
					scope--;
					App( "#endif" );
				}
				
				


				if(DoPassSpecular() && ps.catLighting.reflectprobed){
					App( "#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION" );
					scope++;
					App("d.boxMin[0] = unity_SpecCube0_BoxMin;");
					App("d.boxMin[1] = unity_SpecCube1_BoxMin;");
					scope--;
					App( "#endif" );

					App( "#if UNITY_SPECCUBE_BOX_PROJECTION" );
					scope++;
					App("d.boxMax[0] = unity_SpecCube0_BoxMax;");
					App("d.boxMax[1] = unity_SpecCube1_BoxMax;");
					App("d.probePosition[0] = unity_SpecCube0_ProbePosition;");
					App("d.probePosition[1] = unity_SpecCube1_ProbePosition;");
					scope--;
					App( "#endif" );

					App("d.probeHDR[0] = unity_SpecCube0_HDR;");
					App("d.probeHDR[1] = unity_SpecCube1_HDR;");

				}

				
				string glossStr = DoPassSpecular() ? "gloss" : "0";

				App( "Unity_GlossyEnvironmentData ugls_en_data;" );
				App( "ugls_en_data.roughness = 1.0 - " + glossStr + ";" );
				App( "ugls_en_data.reflUVW = viewReflectDirection;" );

				App( "UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );" );


				if( !InDeferredPass() ) {
					App ("lightDirection = gi.light.dir;");
					App ("lightColor = gi.light.color;");
				}
				
				
			}





		}




		string SumString( bool[] bools, string[] strings, string defStr ) {

			int validCount = 0;
			for( int i = 0; i < bools.Length; i++ ) {
				if( bools[i] )
					validCount++;
			}

			if( validCount == 0 )
				return defStr;

			string s = "";
			int added = 0;
			for( int i = 0; i < strings.Length; i++ ) {
				if( bools[i] ) {
					s += strings[i];
					added++;
					if( added < validCount )
						s += " + ";
				}
			}
			return s;
		}

		void InitReflectionDir() {
			if( ( !dependencies.frag_viewReflection && currentProgram == ShaderProgram.Frag ) || ( !dependencies.vert_viewReflection && currentProgram == ShaderProgram.Vert ) )
				return;
			App( "float3 viewReflectDirection = reflect( -" + VarViewDir() + ", " + VarNormalDir() + " );" );
		}

		void InitSceneColorAndDepth() {

			if( dependencies.frag_sceneDepth ) {
				App( "float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);" );
			}
			if( dependencies.frag_pixelDepth ) {
				App( "float partZ = max(0,i.projPos.z - _ProjectionParams.g);" );
			}


			if( dependencies.scene_uvs ) {
				string sUv = "float2 sceneUVs = ";


				if( ps.HasRefraction() ) {
					sUv += "(i.projPos.xy / i.projPos.w) + " + ps.n_distortion + ";";
				} else {
					sUv += "(i.projPos.xy / i.projPos.w);";
				}

				App( sUv );
			}


			if( dependencies.grabPass ) {

				string s = "float4 sceneColor = ";
				s += "tex2D(" + ps.catBlending.GetGrabTextureName() + ", sceneUVs);";
				App( s );
			}





		}


		string VarNormalDir() {
			if( currentProgram == ShaderProgram.Vert )
				return "o.normalDir";
			return "normalDirection";
		}

		string VarViewDir() { // TODO: Define view variable, dependency etc
			if( currentProgram == ShaderProgram.Vert )
				return "normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz)";
			return "viewDirection";
		}







		public string GetUvCompCountString( int channel ) {
			bool useFloat4 = false;
			if( channel == 0 && dependencies.uv0_float4 ) {
				useFloat4 = true;
			} else if( channel == 1 && dependencies.uv1_float4 ) {
				useFloat4 = true;
			} else if( channel == 2 && dependencies.uv2_float4 ) {
				useFloat4 = true;
			} else if( channel == 3 && dependencies.uv3_float4 ) {
				useFloat4 = true;
			}
			return useFloat4 ? "float4" : "float2";
		}






		void VertexInputStruct() {
			App( "struct VertexInput {" );
			scope++;
			App( "float4 vertex : POSITION;" );
			CommonVertexData();
			scope--;
			App( "};" );
		}

		void CommonVertexData() {
			if( dependencies.vert_in_normals )
				App( "float3 normal : NORMAL;" );
			if( dependencies.vert_in_tangents )
				App( "float4 tangent : TANGENT;" );
			if( dependencies.uv0 )
				App( GetUvCompCountString( 0 ) + " texcoord0 : TEXCOORD0;" );
			if( dependencies.uv1 )
				App( GetUvCompCountString( 1 ) + " texcoord1 : TEXCOORD1;" );
			if( dependencies.uv2 )
				App( GetUvCompCountString( 2 ) + " texcoord2 : TEXCOORD2;" );
			if( dependencies.uv3 )
				App( GetUvCompCountString( 3 ) + " texcoord3 : TEXCOORD3;" );
			if( dependencies.vert_in_vertexColor )
				App( "float4 vertexColor : COLOR;" );
		}

		void TransferCommonData() {
			App( "o.vertex = v.vertex;" );
			if( dependencies.vert_in_normals )
				App( "o.normal = v.normal;" );
			if( dependencies.vert_in_tangents )
				App( "o.tangent = v.tangent;" );
			if( inTess ) {
				if( dependencies.uv0 )
					App( "o.texcoord0 = v.texcoord0;" );
				if( dependencies.uv1 )
					App( "o.texcoord1 = v.texcoord1;" );
				if( dependencies.uv2 )
					App( "o.texcoord2 = v.texcoord2;" );
				if( dependencies.uv3 )
					App( "o.texcoord3 = v.texcoord3;" );
			} else {
				if( dependencies.uv0 )
					App( "o.uv0 = v.texcoord0;" );
				if( dependencies.uv1 )
					App( "o.uv1 = v.texcoord1;" );
				if( dependencies.uv2 )
					App( "o.uv2 = v.texcoord2;" );
				if( dependencies.uv3 )
					App( "o.uv3 = v.texcoord3;" );
			}

			if( dependencies.vert_in_vertexColor )
				App( "o.vertexColor = v.vertexColor;" );
		}


		public string GetVertOutTexcoord( bool numberOnly = false ) {
			if( numberOnly )
				return dependencies.GetVertOutTexcoord();
			return ( " : TEXCOORD" + dependencies.GetVertOutTexcoord() + ";" );
		}

		void VertexOutputStruct() {
			App( "struct VertexOutput {" );
			scope++;
			{
				if( currentPass == PassType.ShadCast ) {
					App( "V2F_SHADOW_CASTER;" );
					dependencies.IncrementTexCoord( 1 );
				} else {
					App( "float4 pos : SV_POSITION;" ); // Already included in shadow passes
				}

				if( dependencies.hasPropertiesWithInstancing )
					App( "UNITY_VERTEX_INPUT_INSTANCE_ID" );

				if( ps.catLighting.IsVertexLit() )
					App( "float3 vtxLight : COLOR;" );
				//if( DoPassSphericalHarmonics() && !ps.highQualityLightProbes )
				//	App ("float3 shLight" + GetVertOutTexcoord() );
				if( dependencies.uv0_frag )
					App( GetUvCompCountString( 0 ) + " uv0" + GetVertOutTexcoord() );
				if( dependencies.uv1_frag )
					App( GetUvCompCountString( 1 ) + " uv1" + GetVertOutTexcoord() );
				if( dependencies.uv2_frag )
					App( GetUvCompCountString( 2 ) + " uv2" + GetVertOutTexcoord() );
				if( dependencies.uv3_frag )
					App( GetUvCompCountString( 3 ) + " uv3" + GetVertOutTexcoord() );
				if( dependencies.vert_out_worldPos )
					App( "float4 posWorld" + GetVertOutTexcoord() );
				if( dependencies.vert_out_normals )
					App( "float3 normalDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_tangents )
					App( "float3 tangentDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_bitangents )
					App( "float3 bitangentDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_screenPos )
					App( "float4 screenPos" + GetVertOutTexcoord() );
				if( dependencies.vert_in_vertexColor )
					App( "float4 vertexColor : COLOR;" );
				if( dependencies.frag_projPos )
					App( "float4 projPos" + GetVertOutTexcoord() );
				if( ShouldUseLightMacros() )
					App( "LIGHTING_COORDS(" + GetVertOutTexcoord( true ) + "," + GetVertOutTexcoord( true ) + ")" );
				if( UseUnity5FogInThisPass() )
					App( "UNITY_FOG_COORDS(" + GetVertOutTexcoord( true ) + ")" ); // New in Unity 5

				bool sh = DoPassSphericalHarmonics() && !ps.catLighting.highQualityLightProbes;
				bool lm = LightmapThisPass();


				string shlmTexCoord = GetVertOutTexcoord();


				// Unity 5 LMs
				if( sh || lm ) {
					App( "#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)" );
					scope++;
					App( "float4 ambientOrLightmapUV" + shlmTexCoord );
					scope--;
					App( "#endif" );
				}

			}
			scope--;
			App( "};" );
		}



		public bool ShouldUseLightMacros() {
			return ( ( currentPass == PassType.FwdAdd || ( currentPass == PassType.FwdBase && !ps.catBlending.ignoreProjector ) ) && ( dependencies.UsesLightNodes() || ps.catLighting.IsLit() ) );
		}

		public bool IsShadowPass() {
			return currentPass == PassType.ShadCast;
		}

		public bool IsShadowOrOutlineOrMetaPass() {
			return currentPass == PassType.Outline || currentPass == PassType.Meta || IsShadowPass();
		}

		public bool IncludeLightingCginc() {
			return LightmappedAndLit() || IsShadowPass() || ( cNodes.Where( x => x is SFN_LightAttenuation ).Count() > 0 );
		}


		void Vertex() {
			currentProgram = ShaderProgram.Vert;
			App( "VertexOutput vert (VertexInput v) {" );
			scope++;
			App( "VertexOutput o = (VertexOutput)0;" );

			if( dependencies.hasPropertiesWithInstancing ) {
				App( "UNITY_SETUP_INSTANCE_ID( v );" );
				App( "UNITY_TRANSFER_INSTANCE_ID( v, o );" );
			}

			if( dependencies.uv0_frag )
				App( "o.uv0 = v.texcoord0;" );
			if( dependencies.uv1_frag )
				App( "o.uv1 = v.texcoord1;" );
			if( dependencies.uv2_frag )
				App( "o.uv2 = v.texcoord2;" );
			if( dependencies.uv3_frag )
				App( "o.uv3 = v.texcoord3;" );
			if( dependencies.vert_out_vertexColor )
				App( "o.vertexColor = v.vertexColor;" );


			bool lm = LightmapThisPass();
			bool sh = DoPassSphericalHarmonics() && !ps.catLighting.highQualityLightProbes;

			if( lm ){
				App("#ifdef LIGHTMAP_ON");
				scope++;
				App( "o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;" );
				App( "o.ambientOrLightmapUV.zw = 0;" );

				scope--;
				if(sh){
					App("#elif UNITY_SHOULD_SAMPLE_SH");
					scope++;
				} else {
					App("#endif");
				}
			}

			if( sh ) {

				if( !lm ) {
					App( "#if SHOULD_SAMPLE_SH" );
					scope++;
				}
				//App( "o.ambientOrLightmapUV.rgb = 0.01*ShadeSH9(float4(UnityObjectToWorldNormal(v.normal),1));" );
				//if( !lm ) {
					scope--;
					App( "#endif" );
				//}

			}

			if( lm ) {
				App( "#ifdef DYNAMICLIGHTMAP_ON" );
				scope++;
				App( "o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;" );
				scope--;
				App("#endif");
			}






			if( dependencies.vert_out_normals )
				InitNormalDirVert();
			if( dependencies.vert_out_tangents )
				InitTangentDirVert();
			if( dependencies.vert_out_bitangents )
				InitBitangentDirVert();

			InitObjectPos();

			if( editor.nodeView.treeStatus.viewDirectionInVertOffset ) {
				InitViewDirVert();
			}

			InitObjectScale();

			if( editor.mainNode.vertexOffset.IsConnectedAndEnabled() ) {
				
				if( ps.catGeometry.vertexOffsetMode == SFPSC_Geometry.VertexOffsetMode.Relative  )
					App( "v.vertex.xyz += " + ps.n_vertexOffset + ";" );
				else
					App( "v.vertex.xyz = " + ps.n_vertexOffset + ";" );
			}

			if( dependencies.vert_out_worldPos )
				App( "o.posWorld = mul(unity_ObjectToWorld, v.vertex);" );




			InitTangentTransformFrag();

			if( !editor.nodeView.treeStatus.viewDirectionInVertOffset ) {
				InitViewDirVert();
			}
			
			InitReflectionDir();
			if( dependencies.frag_lightDirection ) {
				InitLightDir();
			}
			if(dependencies.frag_lightColor)
				InitLightColor();
			InitHalfVector();

			string positioningPrefix;
			if( ps.catExperimental.forceSkipModelProjection ){
				positioningPrefix = "mul(UNITY_MATRIX_VP, "; // Local space. Broken for shadows due to TRANSFER_SHADOW_CASTER assuming model projections
			} else {
				positioningPrefix = "UnityObjectToClipPos( "; // World space 
			}
			
			string positioningSuffix = " );";
			if( ps.catGeometry.vertexPositioning == SFPSC_Geometry.VertexPositioning.ClipSpace ) {
				positioningPrefix = "";
				positioningSuffix = ";";
			}
			if( ps.catGeometry.vertexPositioning == SFPSC_Geometry.VertexPositioning.Billboard ) {


				App("float4x4 bbmv = UNITY_MATRIX_MV;");
				App( "bbmv._m00 = -1.0/length(unity_WorldToObject[0].xyz);" );
				App( "bbmv._m10 = 0.0f;" );
				App( "bbmv._m20 = 0.0f;" );
				App( "bbmv._m01 = 0.0f;" );
				App( "bbmv._m11 = -1.0/length(unity_WorldToObject[1].xyz);" );
				App( "bbmv._m21 = 0.0f;" );
				App( "bbmv._m02 = 0.0f;" );
				App( "bbmv._m12 = 0.0f;" );
				App( "bbmv._m22 = -1.0/length(unity_WorldToObject[2].xyz);" );


				positioningPrefix = "mul( UNITY_MATRIX_P, mul( bbmv, ";
				positioningSuffix = " ));";
			}


			if( currentPass == PassType.Outline ) {
				string dir = "";
				if( ps.catGeometry.outlineMode == SFPSC_Geometry.OutlineMode.VertexNormals ) {
					dir = "v.normal";
				} else if( ps.catGeometry.outlineMode == SFPSC_Geometry.OutlineMode.VertexColors ) {
					dir = "v.vertexColor";
				} else if( ps.catGeometry.outlineMode == SFPSC_Geometry.OutlineMode.FromOrigin ) {
					dir = "normalize(v.vertex)";
				}
				App( "o.pos = "+ positioningPrefix +"float4(v.vertex.xyz + "+dir+"*" + ps.n_outlineWidth + ",1)" + positioningSuffix );

			} else if(currentPass == PassType.Meta ){
				App( "o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );" );
			} else {
				App( "o.pos = " + positioningPrefix + "v.vertex" + positioningSuffix );
			}

			if( ps.catGeometry.showPixelSnap ) {
				App( "#ifdef PIXELSNAP_ON" );
				scope++;
				App( "o.pos = UnityPixelSnap(o.pos);" );
				scope--;
				App( "#endif" );
			}
			

			// New in Unity 5
			if( UseUnity5FogInThisPass() ) {
				App( "UNITY_TRANSFER_FOG(o,o.pos);" );
			}


			if( dependencies.frag_projPos ) {
				App( "o.projPos = ComputeScreenPos (o.pos);" );
				App( "COMPUTE_EYEDEPTH(o.projPos.z);" );
			}


			if( dependencies.vert_out_screenPos ) { // TODO: Select screen pos accuracy etc

				if( ps.catGeometry.highQualityScreenCoords ) {
					App( "o.screenPos = o.pos;" ); // Unpacked per-pixel
				} else {
					App( "o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );" );
					App( "o.screenPos.y *= _ProjectionParams.x;" );
				}
			}



			if( LightmapThisPass() ){

				// TODO, I think

			}

			/* MOVE THIS:
			App( "float4 unity_LightmapST;");
			App( "#ifdef DYNAMICLIGHTMAP_ON");
			scope++;
			App( "float4 unity_DynamicLightmapST;");
			scope--;
			App( "#endif");
*/




			if( currentPass == PassType.ShadCast ) {
				App( "TRANSFER_SHADOW_CASTER(o)" );
			} else {
				if( ps.catLighting.IsVertexLit() )
					Lighting();
				else if( ShouldUseLightMacros() )
					App( "TRANSFER_VERTEX_TO_FRAGMENT(o)" );
			}

			App( "return o;" );

			ResetDefinedState();
			End();
		}


		void Fragment() {
			currentProgram = ShaderProgram.Frag;

			if( currentPass == PassType.Meta ) {
				string vface = "";
				if( dependencies.frag_facing ) {
					vface = ", float facing : VFACE";
				}
				App( "float4 frag(VertexOutput i" + vface + ") : SV_Target {" );
			} else if(currentPass == PassType.Deferred) {
				App( "void frag(" );
				scope++;
				App( "VertexOutput i," );
				App( "out half4 outDiffuse : SV_Target0," );
				App( "out half4 outSpecSmoothness : SV_Target1," );
				App( "out half4 outNormal : SV_Target2," );
				if( dependencies.frag_facing ) {
					App( "out half4 outEmission : SV_Target3," );
					App( "float facing : VFACE )" );
				} else {
					App( "out half4 outEmission : SV_Target3 )" );
				}
				scope--;
				App( "{" );
			} else {
				string vface = "";
				if( dependencies.frag_facing ) {
					vface = ", float facing : VFACE";
				}
				App( "float4 frag(VertexOutput i" + vface + ") : COLOR {" );
			}
			
			scope++;

			if( dependencies.hasPropertiesWithInstancing ) {
				App( "UNITY_SETUP_INSTANCE_ID( i );" );
			}

			if( dependencies.frag_facing ) {
				App( "float isFrontFace = ( facing >= 0 ? 1 : 0 );" );
				App( "float faceSign = ( facing >= 0 ? 1 : -1 );" );
			}

			InitObjectPos();
			InitObjectScale();

			if( ps.catGeometry.normalQuality == SFPSC_Geometry.NormalQuality.Normalized && dependencies.frag_normalDirection ) {
				App( "i.normalDir = normalize(i.normalDir);" );
				if( dependencies.frag_facing ) {
					App( "i.normalDir *= faceSign;" );
				}
			}

			if( dependencies.vert_out_screenPos && ps.catGeometry.highQualityScreenCoords ) {
				App( "i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );" );
				App( "i.screenPos.y *= _ProjectionParams.x;" );
			}

			InitTangentTransformFrag();
			InitViewDirFrag();
			InitNormalDirFrag();
			InitReflectionDir();

			InitSceneColorAndDepth();

			CheckClip();

			PrepareLightmapVars();


			if( dependencies.frag_lightDirection )
				InitLightDir();
			if(dependencies.frag_lightColor)
				InitLightColor();
			InitHalfVector();




			
			Lighting(); // This is ignored in shadow passes
			

			if( currentPass == PassType.Meta ) {
				LightmapMetaPassFrag();
			} else if( currentPass == PassType.Deferred ) {
				DeferredFragReturn();
			} else if( currentPass == PassType.ShadCast ) {
				App( "SHADOW_CASTER_FRAGMENT(i)" );
			} else if( currentPass == PassType.Outline ) {
				App( "return fixed4(" + ps.n_outlineColor + ",0);" );
			} else {

				//if(ps.mOut.diffuse.IsConnectedEnabledAndAvailable()){
				//	AppFinalOutput("lightFinal + " + "diffuse", ps.n_alpha); // This is really weird, it should already be included in the light calcs. Do more research // TODO
				//}else
				if( currentPass == PassType.FwdAdd ) {
					if(ps.catLighting.transparencyMode == SFPSC_Lighting.TransparencyMode.Fade)
						AppFinalOutput( "finalColor * " + ps.n_alpha, "0" );
					else
						AppFinalOutput( "finalColor", "0" );
				} else {

					if( ps.catBlending.alphaToCoverage && currentPass == PassType.FwdBase && ps.HasAlpha() == false && ps.HasAlphaClip() ) {
						AppFinalOutput( "finalColor", "(" + ps.n_alphaClip + ") * 2.0 - 1.0" );
					} else {
						AppFinalOutput( "finalColor", ps.n_alpha );
					}
				}


			}

			End();
		}

		void DeferredFragReturn() {


			// DIFFUSE
			if( ps.HasDiffuse() ) {
				if( ps.mOut.diffuseOcclusion.IsConnectedEnabledAndAvailable() ) {
					App( "outDiffuse = half4( diffuseColor, " + ps.n_diffuseOcclusion + " );" );
				} else {
					App( "outDiffuse = half4( diffuseColor, 1 );" );
				}
			} else {
				App( "outDiffuse = half4( 0, 0, 0, 1 );" );
			}

			// SPEC & GLOSS
			if( ps.HasSpecular() ) {
				if( ps.HasGloss() ) {
					App( "outSpecSmoothness = half4( specularColor, gloss );" );
				} else {
					App( "outSpecSmoothness = half4( specularColor, 0.5 );" );
				}
			} else {
				App( "outSpecSmoothness = half4(0,0,0,0);" );
			}
			
			// NORMALS
			App( "outNormal = half4( normalDirection * 0.5 + 0.5, 1 );" );

			// EMISSION
			if( ps.HasEmissive() ) {
				App( "outEmission = half4( "+ps.n_emissive+", 1 );" );
			} else {
				App( "outEmission = half4(0,0,0,1);" );
			}


			bool specAmb = LightmappedAndLit() && ps.HasSpecular() || ps.mOut.ambientSpecular.IsConnectedEnabledAndAvailable();
			bool diffAmb = LightmappedAndLit() && ps.HasDiffuse() || ps.mOut.ambientDiffuse.IsConnectedEnabledAndAvailable();

			if( specAmb ) {
				if( ps.mOut.ambientSpecular.IsConnectedEnabledAndAvailable() ) {
					App( "outEmission.rgb += indirectSpecular;" );
				} else {
					App( "outEmission.rgb += indirectSpecular * "+ps.n_specularOcclusion+";" );
				}
			}
			if( diffAmb ) {
				App( "outEmission.rgb += indirectDiffuse * diffuseColor;" ); // No need for diffuse AO, since that's covered already
			}

		
			App( "#ifndef UNITY_HDR_ON" );
			scope++;
			App( "outEmission.rgb = exp2(-outEmission.rgb);" );
			scope--;
			App( "#endif" );
			


		}



		void LightmapMetaPassFrag() {


			bool hasDiffuse = ps.mOut.diffuse.IsConnectedEnabledAndAvailable();
			bool hasSpec = ps.mOut.specular.IsConnectedEnabledAndAvailable();
			bool hasGloss = ps.mOut.gloss.IsConnectedEnabledAndAvailable();
			

			App( "UnityMetaInput o;" );
			App( "UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );" );
			App( "" );
			if( ps.mOut.emissive.IsConnectedEnabledAndAvailable() ) {
				App( "o.Emission = " + ps.n_emissive + ";" );
			} else {
				App( "o.Emission = 0;" );
			}
			App( "" );

			
			if(hasDiffuse)
				App( "float3 diffColor = " + ps.n_diffuse + ";" );
			else
				App( "float3 diffColor = float3(0,0,0);" );

			// Handle metallic properly
			if( MetallicPBL() ) {
				App( "float specularMonochrome;" );
				App( "float3 specColor;" );
				if( hasSpec )
					App( "diffColor = DiffuseAndSpecularFromMetallic( diffColor, " + ps.n_specular + ", specColor, specularMonochrome );" );
				else
					App( "diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0, specColor, specularMonochrome );" );
			} else {
				if( hasSpec ) {
					App( "float3 specColor = " + ps.n_specular + ";" );
					if( Unity5PBL() ) {
						App( "float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);" );
						App( "diffColor *= (1.0-specularMonochrome);" );
					}
				}
			}

			if( hasGloss ) {

				if( hasSpec ) {
					if( ps.catLighting.glossRoughMode == SFPSC_Lighting.GlossRoughMode.Roughness ) {
						App( "float roughness = " + ps.n_gloss + ";" );
					} else {
						App( "float roughness = 1.0 - " + ps.n_gloss + ";" );
					}
				}
			
				if( hasSpec )
					App( "o.Albedo = diffColor + specColor * roughness * roughness * 0.5;" );
				else
					App( "o.Albedo = diffColor;" );

			} else {
				if( hasSpec )
					App( "o.Albedo = diffColor + specColor * 0.125; // No gloss connected. Assume it's 0.5" );
				else
					App( "o.Albedo = diffColor;" );
			}
			
			
			

			App( "" );
			//App( "o.Albedo = float3(0,1,0);" );	// Debug
           	//App( "o.Emission = float3(0,1,0);");
			//App( "" );
			App( "return UnityMetaFragment( o );" );

		}


		string GetMaxUvCompCountString() {
			return ( dependencies.uv0_float4 || dependencies.uv1_float4 || dependencies.uv2_float4 || dependencies.uv3_float4 ) ? "float4" : "float2";
		}



		void TessellationVertexStruct() {
			App( "struct TessVertex {" );
			scope++;
			App( "float4 vertex : INTERNALTESSPOS;" );
			CommonVertexData();
			scope--;
			App( "};" );
		}

		void TessellationPatchConstant() {
			App( "struct OutputPatchConstant {" );
			scope++;
			App( "float edge[3]         : SV_TessFactor;" );
			App( "float inside          : SV_InsideTessFactor;" );
			App( "float3 vTangent[4]    : TANGENT;" );
			App( GetMaxUvCompCountString() + " vUV[4]         : TEXCOORD;" );
			App( "float3 vTanUCorner[4] : TANUCORNER;" );
			App( "float3 vTanVCorner[4] : TANVCORNER;" );
			App( "float4 vCWts          : TANWEIGHTS;" );
			scope--;
			App( "};" );
		}

		void TessellationVertexTransfer() {
			App( "TessVertex tessvert (VertexInput v) {" );
			scope++;
			App( "TessVertex o;" );
			TransferCommonData();
			App( "return o;" );
			scope--;
			App( "}" );
		}

		void TessellationHullConstant() {
			App( "OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {" );
			scope++;
			App( "OutputPatchConstant o = (OutputPatchConstant)0;" );
			App( "float4 ts = Tessellation( v[0], v[1], v[2] );" );
			App( "o.edge[0] = ts.x;" );
			App( "o.edge[1] = ts.y;" );
			App( "o.edge[2] = ts.z;" );
			App( "o.inside = ts.w;" );
			App( "return o;" );
			scope--;
			App( "}" );
		}

		void TessellationHull() {
			App( "[domain(\"tri\")]" );
			App( "[partitioning(\"fractional_odd\")]" );
			App( "[outputtopology(\"triangle_cw\")]" );
			App( "[patchconstantfunc(\"hullconst\")]" );
			App( "[outputcontrolpoints(3)]" );
			App( "TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {" );
			scope++;
			App( "return v[id];" );
			scope--;
			App( "}" );
		}


		void TessellationDomain() {

			App( "[domain(\"tri\")]" );
			App( "VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {" );
			scope++;
			App( "VertexInput v = (VertexInput)0;" );

			TransferBarycentric( "vertex" );
			if( dependencies.vert_in_normals )
				TransferBarycentric( "normal" );
			if( dependencies.vert_in_tangents )
				TransferBarycentric( "tangent" );
			if( dependencies.uv0 )
				TransferBarycentric( "texcoord0" );
			if( dependencies.uv1 )
				TransferBarycentric( "texcoord1" );
			if( dependencies.vert_in_vertexColor )
				TransferBarycentric( "vertexColor" );
			if( dependencies.displacement )
				App( "displacement(v);" );
			App( "VertexOutput o = vert(v);" );
			App( "return o;" );
			scope--;
			App( "}" );

		}

		void TransferBarycentric( string s ) {
			App( "v." + s + " = vi[0]." + s + "*bary.x + vi[1]." + s + "*bary.y + vi[2]." + s + "*bary.z;" );
		}


		void FuncTessellation() {
			
			switch( ps.catGeometry.tessellationMode ) {
				case SFPSC_Geometry.TessellationMode.Regular:

					App("float Tessellation(TessVertex v){");// First, we need a per-vertex evaluation of the tess factor
					scope++;
					App( "return " + ps.n_tessellation + ";");
					scope--;
					App("}");

					App( "float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){" );
					scope++;
					App( "float tv = Tessellation(v);" );
					App( "float tv1 = Tessellation(v1);" );
					App( "float tv2 = Tessellation(v2);" );
					App( "return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);" );
					scope--;
					App( "}" );
					break;

				case SFPSC_Geometry.TessellationMode.EdgeLength:
					App( "float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){" );
					scope++;
					App( "return UnityEdgeLengthBasedTess(v.vertex, v1.vertex, v2.vertex, " + ps.n_tessellation + ");" );
					scope--;
					App( "}" );
					break;
			}
			
		}

		void FuncDisplacement() {
			if( !dependencies.displacement )
				return;
			App( "void displacement (inout VertexInput v){" );
			scope++;
			App( "v.vertex.xyz += " + ps.n_displacement + ";" );
			scope--;
			App( "}" );
		}






		void Tessellation() {
			if( !dependencies.tessellation )
				return;
			currentProgram = ShaderProgram.Tess; // Not really, but almost

			App( "#ifdef UNITY_CAN_COMPILE_TESSELLATION" );
			scope++;
			//------------------------------------------------------------
			TessellationVertexStruct();
			TessellationPatchConstant();
			TessellationVertexTransfer();
			ResetDefinedState(); // A bit of a hack
			FuncDisplacement();
			ResetDefinedState(); // A bit of a hack
			FuncTessellation();
			TessellationHullConstant();
			TessellationHull();
			TessellationDomain();
			//------------------------------------------------------------
			scope--;
			App( "#endif" );

			ResetDefinedState();
		}








		// Todo: threshold
		void CheckClip() {
			if( !ps.UseClipping() || currentPass == PassType.Meta )
				return;
			if( ps.catBlending.dithering == Dithering.Off ) {
				App( "clip(" + ps.n_alphaClip + " - 0.5);" );
			} else {
				string ditherStr = SFPSC_Blending.strDithering[(int)ps.catBlending.dithering].ToString().Split( ' ' )[0];
				App( "clip( BinaryDither" + ditherStr + "(" + ps.n_alphaClip + " - 1.5, sceneUVs) );" );
			}
			
		}



		void Fallback() {
			if( ps.catExperimental.forceNoFallback )
				return;
			if( !string.IsNullOrEmpty( ps.catMeta.fallback ) )
				App( "FallBack \"" + ps.catMeta.fallback + "\"" );
			else
				App( "FallBack \"Diffuse\"" ); // Needed for shadows!
		}

		void WriteCustomEditor() {
			App( "CustomEditor \"ShaderForgeMaterialInspector\"" );
		}


		public void GrabPass() {
			if( !dependencies.grabPass )
				return;
			if(ps.catBlending.perObjectRefraction)
				App( "GrabPass{ }" );
			else
				App( "GrabPass{ \"" + ps.catBlending.GetGrabTextureName() + "\" }" );
				
		}

		//////////////////////////////////////////////////////////////// DEFERRED

		void DeferredPass() {
			currentPass = PassType.Deferred;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"DEFERRED\"" ); // TODO this name is a guess
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}


		////////////////////////////////////////////////////////////////




		void ForwardBasePass() {
			currentPass = PassType.FwdBase;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"FORWARD\"" );
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}

		public void ForwardLightPass() {

			// TODO: FIX
			// Only when real-time light things are connected. These are:
			// Diffuse
			// Specular
			// Although could be any D:

			bool customLit = dependencies.UsesLightNodes();
			bool builtinLit = ps.catLighting.IsLit() && ( ps.HasDiffuse() || ps.catLighting.HasSpecular() );

			bool needsLightPass = ( builtinLit || customLit ) && ps.catLighting.UseMultipleLights();

			if( !needsLightPass )
				return;



			currentPass = PassType.FwdAdd;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"FORWARD_DELTA\"" );
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}



		// Only needed when using alpha clip and/or vertex offset (May be needed with Tessellation as well)
		public void ShadowCasterPass() {
			bool shouldUse = ps.UseClipping() || mOut.vertexOffset.IsConnectedAndEnabled() || mOut.displacement.IsConnectedAndEnabled() || ps.catGeometry.cullMode != SFPSC_Geometry.CullMode.BackfaceCulling;
			if( !shouldUse || ps.catExperimental.forceNoShadowPass )
				return;
			currentPass = PassType.ShadCast;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();

			App( "Pass {" );
			scope++;
			{
				App( "Name \"ShadowCaster\"" );
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}


		public void OutlinePass() {
			if( !mOut.outlineWidth.IsConnectedAndEnabled() )
				return;
			currentPass = PassType.Outline;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"Outline\"" );
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}

		public void MetaPass() {
			if( ps.catLighting.includeMetaPass == false )
				return;
			if( !ps.catLighting.bakedLight || ( !mOut.diffuse.IsConnectedEnabledAndAvailable() && !mOut.emissive.IsConnectedAndEnabled() ) )
				return;
			currentPass = PassType.Meta;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"Meta\"" );
				PassTags();
				RenderSetup();
				BeginCG();
				{
					CGvars();
					VertexInputStruct();
					VertexOutputStruct();
					Vertex();
					Tessellation();
					Fragment();
				}
				EndCG();
			}
			End();
			RemoveGhostNodes();
		}





		public void ResetDefinedState() {
			for( int i = 0; i < cNodes.Count; i++ ) {
				cNodes[i].varDefined = false;
				cNodes[i].varPreDefined = false;
			}
		}



		public void Evaluate() {

			if( SF_Debug.evalFlow )
				Debug.Log( "SHADER EVALUATING" );

			editor.ps.fChecker.UpdateAvailability();
			if( !editor.nodeView.treeStatus.CheckCanCompile() ) {
				return;
			}
			ps.UpdateAutoSettings();
			currentPass = PassType.FwdBase;
			PrepareEvaluation();
			UpdateDependencies();
			shaderString = "";
			scope = 0;

			//EditorUtility.UnloadUnusedAssets();
			GC.Collect();


			BeginShader();
			{
				PropertiesShaderLab();
				BeginSubShader();
				{
					SubShaderTags();
					if( ps.catMeta.LOD > 0 )
						App( "LOD " + ps.catMeta.LOD );

					GrabPass();
					OutlinePass();
					if( ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Deferred ) {
						DeferredPass();
					}
					ForwardBasePass();
					ForwardLightPass();
					ShadowCasterPass();
					MetaPass();

				}
				End();
				Fallback();
				WriteCustomEditor();
			}
			End();

			SaveShaderAsset();
			ApplyPropertiesToMaterial();
			editor.ShaderOutdated = UpToDateState.UpToDate;

		}


		//string GetEditorVersionOfShader() {
		//return shaderString.Replace( "_Time", "_EditorTime" );
		//		shaderString.Replace( "_SinTime", "_SinTimeEditor" );
		//		shaderString.Replace( "_CosTime", "_CosTimeEditor" );
		//		shaderString.Replace( "_SinTime", "_SinTimeEditor" );
		//return shaderString;
		//}




		public void SaveShaderAsset() {

			//Debug.Log("SaveShaderAsset()");
			string fileContent = editor.nodeView.GetNodeDataSerialized() + "\n\n" + shaderString;


			// Version control unlocking
			Asset shaderAsset = UnityEditor.VersionControl.Provider.GetAssetByPath( editor.GetShaderFilePath() );
			if( shaderAsset.locked || shaderAsset.readOnly ) {
				UnityEditor.VersionControl.Provider.Lock( shaderAsset, false );
				UnityEditor.VersionControl.Provider.Checkout( shaderAsset, CheckoutMode.Both );
			}

			string path = editor.GetShaderFilePath();
			StreamWriter sw = new StreamWriter( path );
			sw.Write( fileContent );
			sw.Flush();
			sw.Close();

			// Shader written, set default textures in import settings
			List<string> texNames = new List<string>();
			List<Texture> textures = new List<Texture>();

			// Collect all texture names and references
			for( int i = 0; i < editor.nodes.Count; i++ ) {
				if( editor.nodes[i] is SFN_Tex2d ) {
					SFN_Tex2d t2d = editor.nodes[i] as SFN_Tex2d;
					if( !t2d.TexAssetConnected() && t2d.textureAsset != null) {
						texNames.Add( t2d.property.nameInternal );
						textures.Add( t2d.textureAsset );
					}
				} else if( editor.nodes[i] is SFN_Tex2dAsset ) {
					SFN_Tex2dAsset t2dAsset = editor.nodes[i] as SFN_Tex2dAsset;
					if( t2dAsset.textureAsset != null ) {
						texNames.Add( t2dAsset.property.nameInternal );
						textures.Add( t2dAsset.textureAsset );
					}
				}
			}

			// Apply default textures to the shader importer
			ShaderImporter sImporter = ShaderImporter.GetAtPath( path ) as ShaderImporter;
			sImporter.SetDefaultTextures( texNames.ToArray(), textures.ToArray() );


			try {
				AssetDatabase.Refresh( ImportAssetOptions.DontDownloadFromCacheServer );
			} catch( Exception e ) {
				e.ToString();
			}

			editor.OnShaderEvaluated();

		}





		public void ApplyPropertiesToMaterial() {
			for( int i = 0; i < cNodes.Count; i++ ) {
				if( !cNodes[i].IsProperty() )
					continue;
				ApplyProperty( cNodes[i] );
			}
		}

		public void ApplyProperty( SF_Node node ) {

			if( !node.IsProperty() )
				return;

			Material m = SF_Editor.instance.preview.InternalMaterial;
			switch( node.GetType().ToString() ) {
				case ( "ShaderForge.SFN_Tex2d" ):
					SFN_Tex2d texNode = (SFN_Tex2d)node;
					m.SetTexture( texNode.property.GetVariable(), texNode.TextureAsset );
					break;
				case ( "ShaderForge.SFN_Tex2dAsset" ):
					SFN_Tex2dAsset texAssetNode = (SFN_Tex2dAsset)node;
					m.SetTexture( texAssetNode.property.GetVariable(), texAssetNode.textureAsset );
					break;
				case ( "ShaderForge.SFN_Cubemap" ):
					SFN_Cubemap cubeNode = (SFN_Cubemap)node;
					m.SetTexture( cubeNode.property.GetVariable(), cubeNode.cubemapAsset );
					break;
				case ( "ShaderForge.SFN_Slider" ):
					SFN_Slider sliderNode = (SFN_Slider)node;
					m.SetFloat( sliderNode.property.GetVariable(), sliderNode.current );
					break;
				case ( "ShaderForge.SFN_Color" ):
					SFN_Color colorNode = (SFN_Color)node;
					m.SetColor( colorNode.property.GetVariable(), colorNode.GetColor() );
					break;
				case ( "ShaderForge.SFN_ValueProperty" ):
					SFN_ValueProperty valueNode = (SFN_ValueProperty)node;
					m.SetFloat( valueNode.property.GetVariable(), valueNode.texture.dataUniform[0] );
					break;
				case ( "ShaderForge.SFN_ToggleProperty" ):
					SFN_ToggleProperty toggleNode = (SFN_ToggleProperty)node;
					m.SetFloat( toggleNode.property.GetVariable(), toggleNode.texture.dataUniform[0] );
					break;
				case ( "ShaderForge.SFN_SwitchProperty" ):
					SFN_SwitchProperty switchNode = (SFN_SwitchProperty)node;
					m.SetFloat( switchNode.property.GetVariable(), switchNode.on ? 1f : 0f );
					break;
				case ( "ShaderForge.SFN_Vector4Property" ):
					SFN_Vector4Property vector4Node = (SFN_Vector4Property)node;
					m.SetVector( vector4Node.property.GetVariable(), vector4Node.texture.dataUniform );
					break;
				case ( "ShaderForge.SFN_StaticBranch" ):
					SFN_StaticBranch sbNode = (SFN_StaticBranch)node;

					if( sbNode.on ) {
						//Debug.Log("Enabling keyword");
						//m.EnableKeyword(sbNode.property.nameInternal);
					} else {
						//Debug.Log("Disabling keyword");
						//m.DisableKeyword(sbNode.property.nameInternal);
					}

					break;
			}
		}




		void End() {
			scope--;
			App( "}" );
		}
		public void AppIfNonEmpty( string s ) {
			if( !string.IsNullOrEmpty( s ) )
				App( s );
		}
		public void AppFormat( string s, params object[] args ) {
			App( string.Format( s, args ) );
		}
		public void App( string s ) {
			if( s.Contains( "\n" ) ) {
				string[] split = s.Split( '\n' );
				for( int i = 0; i < split.Length; i++ ) {
					App( split[i] );
				}
			} else {
				shaderString += GetScopeTabs() + s + "\n";
			}
		}
		public void AppDebug( string s ) {
			//if(DEBUG)

			string scopeSlashes = GetScopeTabs().Replace( ' ', '/' );

			if( scopeSlashes.Length < 2 )
				scopeSlashes = "//";


			shaderString += scopeSlashes.Substring( Mathf.Min( s.Length + 2, scopeSlashes.Length - 2 ) ) + " " + s + ":\n";
		}
		string GetScopeTabs() {
			string s = "";
			for( int i = 0; i < scope; i++ ) {
				s += "    ";
			}
			return s;
		}
		void NewLine() {
			shaderString += "\n";
		}

		//	shaderEvaluator.previewBackgroundColor


	}
}
