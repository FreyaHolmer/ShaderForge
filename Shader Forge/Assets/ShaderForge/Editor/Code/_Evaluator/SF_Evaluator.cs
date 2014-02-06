using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using UnityEditor.VersionControl;

namespace ShaderForge {


	public enum PassType { FwdBase, FwdAdd, ShadColl, ShadCast, Outline };
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

		public static bool inFrag {
			get {
				return SF_Evaluator.currentProgram == ShaderProgram.Frag;
			}
		}
		public static bool inVert {
			get {
				return SF_Evaluator.currentProgram == ShaderProgram.Vert;
			}
		}
		public static bool inTess {
			get {
				return SF_Evaluator.currentProgram == ShaderProgram.Tess;
			}
		}

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

			mOut = editor.materialOutput;
		}

		public void RemoveGhostNodes() {
			if(ghostNodes == null)
				return;

			if(SF_Debug.ghostNodes)
				Debug.Log( "Removing ghost nodes. Count: " + ghostNodes.Count );
			for( int i = ghostNodes.Count - 1; i >= 0; i-- ) {
				editor.nodes.Remove( ghostNodes[i] );
				ghostNodes[i].Delete();
				ghostNodes.Remove( ghostNodes[i] );
			}
			//Debug.Log( "Done removing ghost nodes. Count: " + ghostNodes.Count );
		}


		public void UpdateDependencies() {

			dependencies = new SF_Dependencies( editor.ps );

			if(SF_Debug.evalFlow)
				Debug.Log("UPDATING DEPENDENCIES: Pass = " + currentPass + " Prog = " + currentProgram);
			cNodes = editor.nodeView.treeStatus.GetListOfConnectedNodesWithGhosts( out ghostNodes, passDependent:true );
			if(SF_Debug.evalFlow)
				Debug.Log("Found " + cNodes.Count + " nodes");


			for( int i = 0; i < cNodes.Count; i++ ) {
				cNodes[i].PrepareEvaluation();
			}

		
			// Dependencies
			if( ps.IsLit() && !IsShadowOrOutlinePass() ) {
				dependencies.NeedLightColor();
				dependencies.NeedFragNormals();
				dependencies.NeedFragLightDir();
				
				if( ( ps.lightMode == SF_PassSettings.LightMode.BlinnPhong || ps.lightMode == SF_PassSettings.LightMode.PBL) && ps.mOut.specular.IsConnectedEnabledAndAvailableInThisPass(currentPass) ) {
					dependencies.NeedFragHalfDir();
				}
			}

			if(ps.lightprobed  && !IsShadowOrOutlinePass()){
				dependencies.vert_in_normals = true;
				if(ps.highQualityLightProbes)
					dependencies.NeedFragNormals();
			}

			if( ps.IsOutlined() && currentPass == PassType.Outline ){
				dependencies.vert_in_normals = true;
			}

			if(ps.IsVertexLit() && ps.IsLit() && !IsShadowOrOutlinePass() ){
				if(ps.lightMode == SF_PassSettings.LightMode.BlinnPhong || ps.lightMode == SF_PassSettings.LightMode.PBL)
					dependencies.NeedVertHalfDir();
				dependencies.NeedVertLightDir();
			}


			if( ps.lightmapped && !IsShadowOrOutlinePass() ) {
				dependencies.NeedFragTangentTransform(); // Directional LMs
				dependencies.uv1 = true; // Lightmap UVs
			}

			//if( ps.HasAnisotropicLight() && !IsShadowPass() ) {
			//	dependencies.NeedFragTangents();
			//	dependencies.NeedFragBinormals();
			//}

			if( ps.IsDoubleSided() && !IsShadowOrOutlinePass() ) {
				dependencies.NeedFragViewDirection();
				dependencies.NeedFragNormals();
			}

			if( ps.IsFragmentLit() && !IsShadowOrOutlinePass() ) {
				dependencies.vert_in_normals = true;
				dependencies.vert_out_normals = true;
				dependencies.vert_out_worldPos = true;
				dependencies.frag_normalDirection = true;
				if( ps.HasNormalMap() || ps.HasSpecular() )
					dependencies.NeedFragViewDirection();
			}

			if( ps.HasNormalMap() && !IsShadowOrOutlinePass() ) {
				dependencies.frag_normalDirection = true;
				dependencies.NeedFragTangentTransform();
			}

			if( ps.HasRefraction() && !IsShadowOrOutlinePass() ) {
				dependencies.NeedRefraction();
			}

			if( ps.HasTessellation() ) {
				dependencies.NeedTessellation();
			}

			if( ps.HasDisplacement() ) {
				dependencies.NeedDisplacement();
			}






			foreach( SF_Node n in cNodes ) {

				if( n is SFN_Time ) {
					//Debug.Log("TIME DEPENDENCY");
					dependencies.time = true;
				}

				if( n is SFN_SceneColor){
					if((n as SFN_SceneColor).AutoUV())
						dependencies.NeedFragScreenPos();
					dependencies.NeedGrabPass();
				}

				if( n is SFN_ObjectPosition ) {
					if(currentProgram == ShaderProgram.Frag)
						dependencies.NeedFragObjPos();
					else
						dependencies.NeedVertObjPos();
				}

				if( n is SFN_Fresnel ) {
					dependencies.NeedFragViewDirection();
					if(!n.GetInputIsConnected("NRM")) // Normal. If it's not connected, make sure we have the dependency for normals
						dependencies.NeedFragNormals();
				}

				if( n is SFN_FragmentPosition ) {
					dependencies.NeedFragWorldPos();
				}

				if( n is SFN_SceneDepth ){
					dependencies.NeedSceneDepth();
				}

				if( n is SFN_DepthBlend ){
					dependencies.NeedSceneDepth();
					dependencies.frag_pixelDepth = true;
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
					dependencies.NeedFragScreenPos();
				}

				if( n.GetType() == typeof( SFN_Tex2d ) ) {
					if( n.GetInputIsConnected( "MIP" ) ) { // MIP connection
						//dependencies.ExcludeRenderPlatform( RenderPlatform.opengl ); // TODO: Find workaround!
						dependencies.SetMinimumShaderTarget( 3 );
					}
				}

				if( n.GetType() == typeof( SFN_Cubemap ) ) {
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

				if( n.GetType() == typeof( SFN_VertexColor ) ) {
					dependencies.NeedFragVertexColor(); // TODO: Check if it really needs to be frag
				}

				if( n.GetType() == typeof( SFN_TexCoord ) ) {
					switch( ( (SFN_TexCoord)n ).currentUV ) {
						case SFN_TexCoord.UV.uv0:
							dependencies.uv0 = true;
							dependencies.uv0_frag = true;
							break;
						case SFN_TexCoord.UV.uv1:
							dependencies.uv1 = true;
							dependencies.uv1_frag = true;
							break;
					}
				}
				if( n.GetType() == typeof( SFN_Pi ) ) {
					dependencies.const_pi = true;
				}
				if( n.GetType() == typeof( SFN_Phi ) ) {
					dependencies.const_phi = true;
				}
				if( n.GetType() == typeof( SFN_E ) ) {
					dependencies.const_e = true;
				}
				if( n.GetType() == typeof( SFN_Root2 ) ) {
					dependencies.const_root2 = true;
				}
				if( n.GetType() == typeof( SFN_Tau ) ) {
					dependencies.const_tau = true;
				}

				if( n.GetType() == typeof( SFN_HalfVector ) ) {
					dependencies.NeedFragHalfDir();
				}
				if( n.GetType() == typeof( SFN_LightColor ) ) {
					dependencies.NeedLightColor();
				}

				/*
				if( n is SFN_Parallax ) {
					dependencies.NeedFragViewDirection();
					dependencies.NeedFragTangentTransform();
					if( !( n as SFN_Parallax ).GetInputIsConnected( "UVIN" ) ) {
						dependencies.uv0 = true;
					}
				}*/

				if( n.GetType() == typeof( SFN_Cubemap ) ) {
					if( !n.GetInputIsConnected( "DIR" ) ) { // DIR connection, if not connected, we need default reflection vector
						dependencies.NeedFragViewReflection();
					}
				}

				if( n.GetType() == typeof( SFN_LightAttenuation ) ) {
					dependencies.NeedFragAttenuation();
				}

				if( n.GetType() == typeof( SFN_ViewReflectionVector ) ) {
					dependencies.NeedFragViewReflection();
				}

				if( n.GetType() == typeof( SFN_LightVector ) ) {
					dependencies.NeedFragLightDir();
				}

				if( n.GetType() == typeof( SFN_ViewVector ) ) {
					dependencies.NeedFragViewDirection();
				}

				if( n is SFN_Tangent ) {
					dependencies.NeedFragTangents();
				}
				if( n is SFN_Binormal ) {
					dependencies.NeedFragBinormals();
				}
				if( n is SFN_NormalVector ) {
					dependencies.NeedFragNormals();
				}

				

				if( n.GetType() == typeof( SFN_Transform ) ) {
					if( ( n as SFN_Transform ).spaceSelFrom == SFN_Transform.Space.Tangent || (n as SFN_Transform).spaceSelTo == SFN_Transform.Space.Tangent ) {
						dependencies.NeedFragTangentTransform();
					}
				}

			}

			//RemoveGhostNodes(); // TODO: Maybe not here?

			if(SF_Debug.evalFlow)
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
				if( editor.nodeView.treeStatus.propertyList[i].IsProperty() ) {
					string line = editor.nodeView.treeStatus.propertyList[i].property.GetInitializationLine();
					App( line );
				}
			}

			bool transparency = ps.mOut.alphaClip.IsConnectedEnabledAndAvailable() || ps.mOut.alpha.IsConnectedEnabledAndAvailable();

			if(transparency)
				App ("[HideInInspector]_Cutoff (\"Alpha cutoff\", Range(0,1)) = 0.5"); // Hack, but, required for transparency to play along with depth etc

			End();

		}
		void PropertiesCG() {
			for( int i = 0; i < cNodes.Count; i++ ) {
				if( cNodes[i].IsProperty() ) {
					App( cNodes[i].property.GetFilteredVariableLine() );
				}
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
				case PassType.FwdBase:
					App( "#define UNITY_PASS_FORWARDBASE" );
					break;
				case PassType.FwdAdd:
					App( "#define UNITY_PASS_FORWARDADD" );
					break;
				case PassType.ShadColl:
					App( "#define UNITY_PASS_SHADOWCOLLECTOR" );
					App( "#define SHADOW_COLLECTOR_PASS" );
					break;
				case PassType.ShadCast:
					App( "#define UNITY_PASS_SHADOWCASTER" );
					break;
			}


			App( "#include \"UnityCG.cginc\"" );

			if( ShouldUseLightMacros()  )
				App( "#include \"AutoLight.cginc\"" );
			if(ps.lightmapped || IsShadowPass() )
				App( "#include \"Lighting.cginc\"" );

			if( currentPass == PassType.FwdBase )
				App( "#pragma multi_compile_fwdbase" + ps.GetShadowPragmaIfUsed() );
			else if( currentPass == PassType.FwdAdd ) {
				App( "#pragma multi_compile_fwdadd" + ps.GetShadowPragmaIfUsed() );
			} else {
				App( "#pragma fragmentoption ARB_precision_hint_fastest");
				if(currentPass == PassType.ShadColl)
					App( "#pragma multi_compile_shadowcollector" );
				else
					App( "#pragma multi_compile_shadowcaster" );
			}


			
			if( dependencies.DoesExcludePlatforms() )
				App( "#pragma exclude_renderers " + dependencies.GetExcludePlatforms() );
			if( dependencies.IsTargetingAboveDefault() )
				App( "#pragma target " + dependencies.GetShaderTarget() );
			if( editor.nodeView.treeStatus.mipInputUsed)
				App ("#pragma glsl"); // Kills non DX instruction counts
		}
		void EndCG() {
			App( "ENDCG" );
		}

		void AppTag(string k, string v) {
			App( "\""+ k +"\"=\""+ v +"\"" );
		}

		void PassTags() {
			BeginTags();
			if( currentPass == PassType.FwdBase )
				AppTag( "LightMode", "ForwardBase" );
			else if( currentPass == PassType.FwdAdd )
				AppTag( "LightMode", "ForwardAdd" );
			else if( currentPass == PassType.ShadColl )
				AppTag( "LightMode", "ShadowCollector" );
			else if(currentPass == PassType.ShadCast)
				AppTag( "LightMode", "ShadowCaster" );
			End();
		}


		void SubShaderTags() {

			bool ip = ps.ignoreProjector;
			bool doesOffset = ps.queuePreset != ShaderForge.SF_PassSettings.Queue.Geometry || ps.queueOffset != 0;
			bool hasRenderType = ps.renderType != SF_PassSettings.RenderType.None;

			if( !ip && !doesOffset && !hasRenderType )
				return; // No tags!

			BeginTags();
			if(ip)
				AppTag( "IgnoreProjector", "True" );
			if( doesOffset ) {
				string bse = ps.queuePreset.ToString();
				string ofs = "";
				if( ps.queueOffset != 0 )
					ofs = ps.queueOffset > 0 ? ( "+" + ps.queueOffset ) : (ps.queueOffset.ToString()) ;
				AppTag( "Queue", ( bse + ofs ).ToString() );
			}
			if( hasRenderType )
				AppTag("RenderType",ps.renderType.ToString());
				
			End();
		}

		void RenderSetup() {

			if( currentPass == PassType.FwdAdd )
				App("Blend One One");
			else if( currentPass == PassType.FwdBase && ps.UseBlending() ) // Shadow passes and outlines use default blending
				App( ps.GetBlendString() );
			

			if( currentPass == PassType.ShadCast ){
				App( "Cull Off" );
				App( "Offset 1, 1" );
			} else if (currentPass == PassType.Outline){
				App ("Cull Front");
			} else if( ps.UseCulling())
				App( ps.GetCullString() );

			if( ps.UseDepthTest() && !IsShadowOrOutlinePass() ) // Shadow passes and outlines use default
				App( ps.GetDepthTestString() );

			if( !IsShadowOrOutlinePass() ) {
				App( ps.GetZWriteString() );
			}

			App (ps.GetOffsetString());

			
			if(currentPass == PassType.FwdAdd){
				App ("Fog { Color (0,0,0,0) }");
			} else if( !ps.useFog || !(currentPass == PassType.FwdBase || currentPass == PassType.Outline)) {
				App( "Fog {Mode Off}" ); // Turn off fog is user doesn't want it
			} else {
				// Fog overrides!
				if(ps.fogOverrideMode)
					App( "Fog {Mode "+ps.fogMode.ToString()+"}" ); 
				if(ps.fogOverrideColor)
					App ("Fog { Color ("+ps.fogColor.r+","+ps.fogColor.g+","+ps.fogColor.b+","+ps.fogColor.a+") }");
				if(ps.fogOverrideDensity)
					App( "Fog {Density "+ps.fogDensity+"}" ); 
				if(ps.fogOverrideRange)
					App( "Fog {Range "+ps.fogRange.x+","+ps.fogRange.y+"}" ); 
			}


			if( ps.useStencilBuffer ){
				App ("Stencil {");
				scope++;

				App ( ps.GetStencilContent() );

				scope--;
				App ("}");

			}

		}

		void CGvars() {

			if( dependencies.lightColor && !ps.lightmapped && !IsShadowPass() ) // Lightmap and shadows include Lighting.cginc, which already has this
				App( "uniform float4 _LightColor0;" );

			if( dependencies.grabPass )
				App( "uniform sampler2D _GrabTexture;" );

			if( dependencies.frag_sceneDepth )
				App( "uniform sampler2D _CameraDepthTexture;");

			if( dependencies.time ) {
				//App( "uniform float4 _Time;" ); // TODO: _Time too. Maybe replace at the end?
				App( "uniform float4 _TimeEditor;" );
			}
				

			if( ps.lightmapped ) {
				App("#ifndef LIGHTMAP_OFF");
				scope++;
					App( "sampler2D unity_Lightmap;" );
					App( "float4 unity_LightmapST;" );
					App( "#ifndef DIRLIGHTMAP_OFF" );
					scope++;
						App( "sampler2D unity_LightmapInd;" );
					scope--;
					App( "#endif" );
				scope--;
				App( "#endif" );
			}

			PropertiesCG();

		}

		void InitViewDirVert() {
			if( dependencies.vert_viewDirection )
				App( "float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz);" );
		}	
		void InitViewDirFrag() {
			if( dependencies.frag_viewDirection )
				App( "float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);" );
		}
		void InitTangentTransformFrag() {
			if((dependencies.frag_tangentTransform && currentProgram == ShaderProgram.Frag) || (dependencies.vert_tangentTransform && currentProgram == ShaderProgram.Vert) )
				App( "float3x3 tangentTransform = float3x3( "+WithProgramPrefix("tangentDir")+", "+WithProgramPrefix("binormalDir")+", "+WithProgramPrefix("normalDir")+");" );
		}

		


		string LightmapNormalDir() {
			if( editor.materialOutput.normal.IsConnectedAndEnabled() ) {
				return "normalLocal";		
			}
			return "float3(0,0,1)";	
			
		}

		void PrepareLightmapVars() {
			if( !LightmapThisPass() )
				return;
			App( "#ifndef LIGHTMAP_OFF" );
			scope++;
				App( "float4 lmtex = tex2D(unity_Lightmap,i.uvLM);" );
				App("#ifndef DIRLIGHTMAP_OFF");
				scope++;
					App("float3 lightmap = DecodeLightmap(lmtex);");
					App("float3 scalePerBasisVector = DecodeLightmap(tex2D(unity_LightmapInd,i.uvLM));");
					App("UNITY_DIRBASIS");
					App( "half3 normalInRnmBasis = saturate (mul (unity_DirBasis, " + LightmapNormalDir() + "));" );
					App( "lightmap *= dot (normalInRnmBasis, scalePerBasisVector);" );
				scope--;
				App("#else");
				scope++;
					App( "float3 lightmap = DecodeLightmap(tex2D(unity_Lightmap,i.uvLM));" );
				scope--;
				App("#endif");
			scope--;
			App( "#endif" );
		}

		void InitLightDir() {

			if(IsShadowOrOutlinePass())
				return;

			if((currentProgram == ShaderProgram.Frag && !dependencies.frag_lightDirection) || (currentProgram == ShaderProgram.Vert && !dependencies.vert_lightDirection))
				return;

			if(currentPass == PassType.FwdBase ){
				if( ps.lightmapped ) {
					App( "#ifndef LIGHTMAP_OFF" );
					scope++;
						App( "#ifdef DIRLIGHTMAP_OFF" );
						scope++;
				}
				App( "float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);" );
				if( ps.lightmapped ) {
						scope--;
						App( "#else" );
						scope++;
							App( "float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);" );
							App( "lightDirection = mul(lightDirection,tangentTransform); // Tangent to world" );
						scope--;
						App( "#endif" );
					scope--;
					App( "#else" );
					scope++;
						App( "float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);" );
					scope--;
					App( "#endif" );
				}

				return;
			}

			// Point vs directional
			App ("float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - "+WithProgramPrefix("posWorld.xyz")+",_WorldSpaceLightPos0.w));");
			
		}

		void InitHalfVector() {
			if( (!dependencies.frag_halfDirection && currentProgram == ShaderProgram.Frag) || (!dependencies.vert_halfDirection && currentProgram == ShaderProgram.Vert) )
				return;
			App( "float3 halfDirection = normalize(viewDirection+lightDirection);" );
		}

		void InitAttenuation() {

			if(SF_Evaluator.inVert && ps.IsVertexLit() && ShouldUseLightMacros())
				App( "TRANSFER_VERTEX_TO_FRAGMENT(o)" );

			string atten = "LIGHT_ATTENUATION(" + ((currentProgram == ShaderProgram.Frag) ? "i" : "o") + ")";
			if( ps.doubleIncomingLight )
				atten += "*2";

			string inner = ( ShouldUseLightMacros() ? atten : "1" );
			App( "float attenuation = " + inner + ";" );
			if(ps.lightMode != SF_PassSettings.LightMode.Unlit)
				App( "float3 attenColor = attenuation * _LightColor0.xyz;" );
		}


		string GetWithDiffPow(string s){
			if(ps.HasDiffusePower()){
				return "pow(" + s + ", " + ps.n_diffusePower + ")";
			}
			return s;
		}



		void CalcDiffuse() {

			//App( "float atten = 1.0;" );
			AppDebug("Diffuse");



			//InitAttenuation();
			if( ps.IsEnergyConserving() ) {
				App( "float Pi = 3.141592654;" );
				App( "float InvPi = 0.31830988618;" );
			}

			string lmbStr;

			App( "float NdotL = dot( "+VarNormalDir()+", lightDirection );" );

			if( ps.HasTransmission() || ps.HasLightWrapping() ) {

				string fwdLight = "float3 forwardLight = "; // TODO
				string backLight = "float3 backLight = "; // TODO


				if(ps.HasLightWrapping()){
					App( "float3 w = " + ps.n_lightWrap + "*0.5; // Light wrapping" );
					App( "float3 NdotLWrap = NdotL * ( 1.0 - w );" );
					App( fwdLight + GetWithDiffPow("max(float3(0.0,0.0,0.0), NdotLWrap + w )") + ";" );
					if(ps.HasTransmission()){
						App( backLight + GetWithDiffPow("max(float3(0.0,0.0,0.0), -NdotLWrap + w )") + " * " + ps.n_transmission + ";" );
					}
						
				} else {
					App( fwdLight + GetWithDiffPow("max(0.0, NdotL )") + ";" );
					if(ps.HasTransmission()){
						App( backLight + GetWithDiffPow( "max(0.0, -NdotL )") + " * " + ps.n_transmission + ";" );
					}
				}

				lmbStr = "forwardLight";

				if(ps.HasTransmission()){
					lmbStr += "+backLight";
					lmbStr = "("+lmbStr+")";
				}

			

				/*
				if( ps.n_diffusePower == "1" )
					App( "float3 lambert = abs( dot( normalDirection, lightDirection ) );" );
				else
					App( "float3 lambert = pow(abs( dot( normalDirection, lightDirection )), " + ps.n_diffusePower + ");" );
				App( "if( dot( lightDirection, i.normalDir ) < 0 ) // Is the light on the other side?" );
				scope++;
				App( "lambert *= " + ps.n_transmission + "; // Multiply by transmission" );
				scope--;
				App( "lambert *= atten * _LightColor0.xyz;" );
				 */

			} else {



				lmbStr = GetWithDiffPow("max( 0.0, NdotL)");



				//if( ps.n_diffusePower != "1" ) {
				//	lmbStr = "pow(" + lmbStr + "," + ps.n_diffusePower + ")";
				//}

			}

			if( LightmapThisPass() ) {
				App( "#ifndef LIGHTMAP_OFF" );
				scope++;
				App( "float3 diffuse = lightmap;" );
				scope--;
				App( "#else" );
				scope++;
			}


			if( ps.IsEnergyConserving() ) {
				if( ps.HasLightWrapping() ) {
					lmbStr += "/(Pi*(dot(w,float3(0.3,0.59,0.11))+1))";
				} else {
					lmbStr += "*InvPi";
				}
			}

			lmbStr = "float3 diffuse = " + lmbStr + " * attenColor";

			if(ps.useAmbient && currentPass == PassType.FwdBase && !ps.lightprobed){ // Ambient is already in light probe data
				lmbStr += " + " + GetAmbientStr();
			}
			
			lmbStr += ";";


			App( lmbStr );

			if( LightmapThisPass() ) {
				scope--;
				App( "#endif" );
			}


		}

		bool LightmapThisPass() {
			return ps.lightmapped && currentPass == PassType.FwdBase;
		}

		void InitNormalDirVert() {
			if( dependencies.vert_out_normals ) {
				App( "o.normalDir = mul(float4(" + ps.GetNormalSign() + "v.normal,0), _World2Object).xyz;");
			}
		}

		void InitTangentDirVert() {
			App( "o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );" );
		}

		void InitBinormalDirVert() {
			App( "o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);" );
		}

		void InitObjectPos() {
			if(dependencies.frag_objectPos || dependencies.vert_objectPos)
				App( "float4 objPos = mul ( _Object2World, float4(0,0,0,1) );" );
		}

		void InitNormalDirFrag() {

			if(IsShadowOrOutlinePass())
				return;

			if( (!dependencies.frag_normalDirection && currentProgram == ShaderProgram.Frag) )
				return;

			AppDebug("Normals");


			//if(ps.normalQuality == SF_PassSettings.NormalQuality.Normalized){
			//	App ("i.normalDir = normalize(i.normalDir);");
			//}

			


			if( ps.HasNormalMap() ) {
				App( "float3 normalLocal = " + ps.n_normals + ";" );
				App( "float3 normalDirection =  mul( normalLocal, tangentTransform ); // Perturbed normals" );
			} else {
				App( "float3 normalDirection =  i.normalDir;" );
			}

			if( ps.IsDoubleSided() ) {
				App( "" );
				App( "float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface" );
				App( "i.normalDir *= nSign;" );
				App( "normalDirection *= nSign;" );
				App( "" );
			}



		}


		void CalcGloss(){
			AppDebug("Gloss");
			if( ps.remapGlossExponentially ) {
				App( "float gloss = exp2(" + ps.n_gloss + "*10.0+1.0);" );
			} else {
				App( "float gloss = " + ps.n_gloss+";" );
			}
		}

		bool DoAmbientSpecThisPass(){
			return (mOut.ambientSpecular.IsConnectedEnabledAndAvailable() && currentPass == PassType.FwdBase);
		}


		void CalcSpecular() {



			AppDebug("Specular");

			if(!ps.HasDiffuse()){
				App( "float NdotL = dot( "+VarNormalDir()+", lightDirection );" );
			}

			App ("NdotL = max(0.0, NdotL);");

			//if(DoAmbientSpecThisPass() && ps.IsPBL())
				//App ("float NdotR = max(0, dot(viewReflectDirection, normalDirection));"); // WIP

			string s = "float3 specular = ";

			if(ps.maskedSpec && currentPass == PassType.FwdBase){
				s += "(floor(attenuation) * _LightColor0.xyz)"; // TODO; This only works well with directional lights : <
			} else {
				s += "attenColor"; /* * " + ps.n_specular;*/ // TODO: Doesn't this double the spec? Removed for now. Shouldn't evaluate spec twice when using PBL
			}




			//if( mOut.ambientSpecular.IsConnectedEnabledAndAvailable() && currentPass == PassType.FwdBase){
			//	s += "(attenColor + " + ps.n_ambientSpecular + ")";
			//} else {
			//	s += "attenColor";
			//}
			string sAmb = "";
			if(DoAmbientSpecThisPass()){
				sAmb = "float3 specularAmb = " + ps.n_ambientSpecular+" * specularColor"; // TODO: Vis and fresnel
			}


			if( ps.IsPBL() ) {
				s += "*NdotL"; // TODO: Really? Is this the cosine part?

				//if(DoAmbientSpecThisPass())
					//sAmb += " * NdotR";

			}
			
			
			if( ps.lightMode == SF_PassSettings.LightMode.Phong )
				s += " * pow(max(0,dot(reflect(-lightDirection, "+VarNormalDir()+"),viewDirection))";
			if( ps.lightMode == SF_PassSettings.LightMode.BlinnPhong || ps.lightMode == SF_PassSettings.LightMode.PBL ) {
				s += " * pow(max(0,dot(halfDirection,"+VarNormalDir()+"))";
			}
			s += ",gloss)";


			App( "float3 specularColor = " + ps.n_specular + ";" );

			// PBL SHADING, normalization term comes after this
			if( ps.IsPBL() ) {
				
				// FRESNEL TERM
				//App( "float3 specularColor = " + ps.n_specular + ";" );
				if( ps.fresnelTerm ) {
					App( "float HdotL = max(0.0,dot(halfDirection,lightDirection));" );
					string fTermDef = "float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);";
					App( fTermDef );
					s += "*fresnelTerm";


					//if(DoAmbientSpecThisPass()){
					//	App( "float NdotV = max(0.0,dot( "+VarNormalDir()+", viewDirection ));" );
					//	App (fTermDef.Replace("HdotL","NdotV").Replace("fresnelTerm","fresnelTermAmb"));
					//	sAmb += " * fresnelTermAmb";
					//}

				} else {
					s += "*specularColor";
				}
				
				
				// VISIBILITY TERM
				if( ps.visibilityTerm ) {
					//App( "float NdotL = max(0.0,dot( "+VarNormalDir()+", lightDirection ));" ); // This should already be defined in the diffuse calc. TODO: Redefine if diffuse is not used
					//if(!DoAmbientSpecThisPass() && ps.fresnelTerm) // Already defined in that case
						App( "float NdotV = max(0.0,dot( "+VarNormalDir()+", viewDirection ));" );
					App( "float alpha = 1.0 / ( sqrt( (Pi/4.0) * gloss + Pi/2.0 ) );" );
					string vTermDef = "float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );";
					App( vTermDef );
					App( "visTerm = 1.0 / visTerm;" );
					s += "*visTerm";

					// Ambient Specular
					//if(DoAmbientSpecThisPass()){
					//	App ( vTermDef.Replace( "NdotL","NdotR" ).Replace("visTerm","visTermAmb") ); // Define the same, but use reflection dir instead of light dir
					//	App ("visTermAmb = 1.0 / visTermAmb;" );
					//	sAmb += " * visTermAmb";
					//}
				}
				
			} else {
				s += " * specularColor";
			}
			
			
			
			if( ps.IsEnergyConserving() ) {
				// NORMALIZATION TERM
				if( ps.lightMode == SF_PassSettings.LightMode.Phong ){
					App( "float normTerm = (gloss + 2.0 ) / (2.0 * Pi);" );
				} else if( ps.lightMode == SF_PassSettings.LightMode.BlinnPhong || ps.lightMode == SF_PassSettings.LightMode.PBL ) {
					App( "float normTerm = (gloss + 8.0 ) / (8.0 * Pi);" );
				}

				if(DoAmbientSpecThisPass()){
					//sAmb += " * normTerm";
				}

				s += "*normTerm";


			}

			if( DoAmbientSpecThisPass() ){
				App (sAmb + ";");
				s += " + specularAmb";
			}

			s += ";";

			App(s); // Specular




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



		void CalcEmissive(){
			AppDebug("Emissive");
			App ("float3 emissive = " + ps.n_emissive + ";");
		}


		bool DoPassDiffuse(){
			return ps.HasDiffuse() && (currentPass == PassType.FwdBase || currentPass == PassType.FwdAdd);
		}
		bool DoPassEmissive(){ // Emissive should always be in the base pass
			return ps.HasEmissive() && currentPass == PassType.FwdBase;
		}
		bool DoPassSpecular(){ // Spec only in base and add passes
			return ps.HasSpecular() && (currentPass == PassType.FwdBase || currentPass == PassType.FwdAdd);
		}



		void CalcFinalLight() {
			//bool addedOnce = false;
			string finalLightStr = "float3 lightFinal = ";
			if( ps.IsLit() ) {
				finalLightStr += "diffuse";
				if( ps.useAmbient && currentPass == PassType.FwdBase ) {
					finalLightStr += " + UNITY_LIGHTMODEL_AMBIENT.xyz";
					if( ps.doubleIncomingLight ) {
						finalLightStr += "*2";
					}
				}
			}

			finalLightStr += ";";
			App( finalLightStr );
			
		}




		void AppFinalOutput(string color, string alpha) {
			AppDebug("Final Color");
			if( ps.HasRefraction() && currentPass == PassType.FwdBase ) {
				//App( "return fixed4(lerp(tex2D(_GrabTexture, float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + " + ps.n_distortion + ").rgb, " + color + "," + alpha + "),1);" );
				//tex2D(_GrabTexture, float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + " + ps.n_distortion + ")
				App( "return fixed4(lerp(sceneColor.rgb, " + color + "," + alpha + "),1);" );
			} else {
				App( "return fixed4(" + color + "," + alpha + ");" );
			}
			
		}


		string GetAmbientStr(){

			string s = "UNITY_LIGHTMODEL_AMBIENT.xyz";

			if(ps.doubleIncomingLight)
				s += "*2";


			return s;

		}


		bool DoPassSphericalHarmonics(){
			return ps.lightprobed && currentPass == PassType.FwdBase;
		}


		void Lighting() {

			if( IsShadowOrOutlinePass() )
				return;
			AppDebug ("Lighting");

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

			bool attenBuiltin = ps.IsLit()&& ( ps.HasDiffuse() || ps.HasSpecular() );

			if( attenBuiltin || (dependencies.frag_attenuation && SF_Evaluator.inFrag))
				InitAttenuation();

			if( !ps.IsLit() && SF_Evaluator.inFrag ) {


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
				if( doEmissive ){
					CalcEmissive();
					s += somethingAdded ? " + ":"";
					s += "emissive";
					somethingAdded = true;
				}
				if( doCustomLight ){
					s += somethingAdded ? " + ":"";
					s += ps.n_customLighting;
					somethingAdded = true;
				}



				if(!didAddLight)
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
			if( DoPassDiffuse() ) // Diffuse + texture (If not vertex lit)
				CalcDiffuse();

			if( DoPassEmissive() ) // Emissive
				CalcEmissive();

			if( DoPassSpecular() ) { // Specular
				CalcGloss();
				CalcSpecular();
				//AppDebug("Spec done"); 
			}
			
			/*if(!ps.IsLit() && ps.mOut.customLighting.IsConnectedEnabledAndAvailable() ){

				App("float3 lightFinal = " + ps.n_customLighting );

			}*/
			if( /*!ps.IsVertexLit() &&*/ currentProgram == ShaderProgram.Frag ) {

				//string lgFinal = "float3 finalColor = ";
				App ("float3 finalColor = 0;");

				//bool addedSomething = false;

				if( ps.HasDiffuse() ){

					bool ambDiff = ps.mOut.ambientDiffuse.IsConnectedEnabledAndAvailableInThisPass(currentPass);
					bool shLight = DoPassSphericalHarmonics();

					//bool parenthesize = (ambDiff || shLight);

					//string diffuseLight = parenthesize ?  "( diffuse" : "diffuse";

					App ("float3 diffuseLight = diffuse;");
				
					if(ambDiff){
						//diffuseLight += " + " + ps.n_ambientDiffuse;
						App("diffuseLight += " + ps.n_ambientDiffuse + "; // Diffuse Ambient Light");
					}

					if(shLight){

						if(LightmapThisPass()){
							App ("#ifdef LIGHTMAP_OFF");
							scope++;
						}

						if(ps.highQualityLightProbes)
							App ("diffuseLight += ShadeSH9(float4(normalDirection,1))" + (ps.doubleIncomingLight ? ";" : " * 0.5; // Per-Pixel Light Probes / Spherical harmonics"));
							//diffuseLight += " + ShadeSH9(float4(normalDirection,1))" + (ps.doubleIncomingLight ? "" : " * 0.5");
						else
							App ("diffuseLight += i.shLight; // Per-Vertex Light Probes / Spherical harmonics");
							//diffuseLight += " + i.shLight";

						if(LightmapThisPass()){
							scope--;
							App ("#endif");
						}
					}



					//if(parenthesize)
					//	diffuseLight += " )";

					//lgFinal += diffuseLight;
					App ("finalColor += diffuseLight * " + ps.n_diffuse + ";");

					//lgFinal += " * " + ps.n_diffuse;
					//addedSomething = true;
				}

				if(DoPassSpecular()){
					App("finalColor += specular;");
					//lgFinal += addedSomething ? " + ":"";
					//lgFinal += "specular";
					//addedSomething = true;
				}
				if(DoPassEmissive()){
					App("finalColor += emissive;");
					//lgFinal += addedSomething ? " + ":"";
					//lgFinal += "emissive";
					//addedSomething = true;
				}

				//if(!addedSomething)
					//lgFinal += "0"; // TODO: Don't do lighting at all if this is the case


				//lgFinal += ";";
				//App( lgFinal );
			}	
			/*if(currentProgram == ShaderProgram.Frag){*/

				/*
				string finalRGB = "lightFinal * " + ps.n_diffuse;

				if(DoPassSpecular())
					finalRGB += " + specular";
				if(DoPassEmissive())
					finalRGB += " + emissive";

				AppFinalOutput( finalRGB, ps.n_alpha);
				*/

				/*
				if( ps.HasSpecular() || ( ps.HasEmissive() && currentPass == PassType.FwdBase ))
					AppFinalOutput( "lightFinal * " + ps.n_diffuse + " + addLight", ps.n_alpha);
				else
					AppFinalOutput( "lightFinal * " + ps.n_diffuse, ps.n_alpha );
				*/

			/*} else if ( currentProgram == ShaderProgram.Vert){

				string vtxLightOut = "o.vtxLight = diffuse"; // Diffuse light

				//App("o.vtxLight = " + ps.n_diffuse + " + addLight;");

				if(DoPassSpecular())
					vtxLightOut += " + specular";
				if(DoPassEmissive())
					vtxLightOut += " + emissive";


				vtxLightOut += ";";

				App(vtxLightOut);


			}
			*/
		}

		void InitReflectionDir() {
			if( (!dependencies.frag_viewReflection && currentProgram == ShaderProgram.Frag) || (!dependencies.vert_viewReflection && currentProgram == ShaderProgram.Vert) )
				return;
			App( "float3 viewReflectDirection = reflect( -"+VarViewDir()+", "+VarNormalDir()+" );" );
		}

		void InitSceneColorAndDepth(){

			if(dependencies.frag_sceneDepth){
				App("float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));");
			}
			if(dependencies.frag_pixelDepth){
				App("float partZ = i.projPos.z;");
			}

			if(dependencies.grabPass){



				string s = "float4 sceneColor = ";

				string sUv = "float2 sceneUVs = ";
					

				if(ps.HasRefraction() ){
					sUv += "float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + " + ps.n_distortion + ";";
				} else {
					sUv += "float2(1,grabSign)*i.screenPos.xy*0.5+0.5;";
				}

				App (sUv);
				s += "tex2D(_GrabTexture, sceneUVs);";
				App (s);
			}





		}


		string VarNormalDir(){
			if(currentProgram == ShaderProgram.Vert)
				return "o.normalDir";
			return "normalDirection";
		}

		string VarViewDir(){ // TODO: Define view variable, dependency etc
			if(currentProgram == ShaderProgram.Vert)
				return "normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz)";
			return "viewDirection";
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
				App( "float4 uv0 : TEXCOORD0;" );
			if( dependencies.uv1 )
				App( "float4 uv1 : TEXCOORD1;" );
			if( dependencies.vert_in_vertexColor )
				App( "float4 vertexColor : COLOR;" );
		}

		void TransferCommonData() {
			App( "o.vertex = v.vertex;" );
			if( dependencies.vert_in_normals )
				App( "o.normal = v.normal;" );
			if( dependencies.vert_in_tangents )
				App( "o.tangent = v.tangent;" );
			if( dependencies.uv0 )
				App( "o.uv0 = v.uv0;" );
			if( dependencies.uv1 )
				App( "o.uv1 = v.uv1;" );
			if( dependencies.vert_in_vertexColor )
				App( "o.vertexColor = v.vertexColor;" );
		}


		public string GetVertOutTexcoord(bool numberOnly = false) {
			if( numberOnly )
				return dependencies.GetVertOutTexcoord();
			return ( " : TEXCOORD" + dependencies.GetVertOutTexcoord() + ";" );
		}

		void VertexOutputStruct() {
			App( "struct VertexOutput {" );
			scope++;
			{
				if( currentPass == PassType.ShadColl ) {
					App("V2F_SHADOW_COLLECTOR;");
					dependencies.IncrementTexCoord( 5 );
				} else if( currentPass == PassType.ShadCast ) {
					App( "V2F_SHADOW_CASTER;" );
					dependencies.IncrementTexCoord( 1 );
				} else {
					App( "float4 pos : SV_POSITION;" ); // Already included in shadow passes
				}

				if( ps.IsVertexLit() )
					App( "float3 vtxLight : COLOR;" );
				//if( DoPassSphericalHarmonics() && !ps.highQualityLightProbes )
				//	App ("float3 shLight" + GetVertOutTexcoord() );
				if( dependencies.uv0_frag )
					App( "float4 uv0" + GetVertOutTexcoord() );
				if( dependencies.uv1_frag )
					App( "float4 uv1" + GetVertOutTexcoord() );
				if( dependencies.vert_out_worldPos )
					App( "float4 posWorld" + GetVertOutTexcoord() );
				if( dependencies.vert_out_normals )
					App( "float3 normalDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_tangents )
					App( "float3 tangentDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_binormals )
					App( "float3 binormalDir" + GetVertOutTexcoord() );
				if( dependencies.vert_out_screenPos )
					App( "float4 screenPos" + GetVertOutTexcoord() );
				if( dependencies.vert_in_vertexColor )
					App( "float4 vertexColor : COLOR;" );
				if( dependencies.frag_projPos)
					App ("float4 projPos" + GetVertOutTexcoord() );
				if( ShouldUseLightMacros() )
					App( "LIGHTING_COORDS(" + GetVertOutTexcoord( true ) + "," + GetVertOutTexcoord( true ) + ")" );

				bool sh = DoPassSphericalHarmonics() && !ps.highQualityLightProbes;
				bool lm = LightmapThisPass();
				string shlmTexCoord = GetVertOutTexcoord();

				if( lm && sh) {
					App( "#ifndef LIGHTMAP_OFF" );
					scope++;
						App( "float2 uvLM" + shlmTexCoord );
					scope--;
					App( "#else" );
					scope++;
						App ("float3 shLight" + shlmTexCoord );
					scope--;
					App ("#endif");
				} else if(lm){
					App( "#ifndef LIGHTMAP_OFF" );
					scope++;
						App( "float2 uvLM" + shlmTexCoord );
					scope--;
					App( "#endif" );
				} else if(sh){
					App ("float3 shLight" + shlmTexCoord );
				}

			}
			scope--;
			App( "};" );
		}



		public bool ShouldUseLightMacros() {
			return ((currentPass == PassType.FwdAdd || ( currentPass == PassType.FwdBase && !ps.ignoreProjector)) && (dependencies.UsesLightNodes() || ps.IsLit()) );
		}

		public bool IsShadowPass() {
			return currentPass == PassType.ShadCast || currentPass == PassType.ShadColl;
		}

		public bool IsShadowOrOutlinePass(){
			return currentPass == PassType.Outline || IsShadowPass();
		}




		void Vertex() {
			currentProgram = ShaderProgram.Vert;
			App( "VertexOutput vert (VertexInput v) {" );
			scope++;
			App( "VertexOutput o;" );

			

			if( dependencies.uv0_frag )
				App( "o.uv0 = v.uv0;" );
			if( dependencies.uv1_frag )
				App( "o.uv1 = v.uv1;" );
			if( dependencies.vert_out_vertexColor )
				App("o.vertexColor = v.vertexColor;");
			if( DoPassSphericalHarmonics() && !ps.highQualityLightProbes){


				if( LightmapThisPass() ){
					App ("#ifdef LIGHTMAP_OFF");
					scope++;
				}
				App ("o.shLight = ShadeSH9(float4(v.normal * unity_Scale.w,1))" + (ps.doubleIncomingLight ? "" : " * 0.5") + ";");

				if( LightmapThisPass() ){
					scope--;
					App("#endif");
				}
			}
			if( dependencies.vert_out_normals )
				InitNormalDirVert();
			if( dependencies.vert_out_tangents )
				InitTangentDirVert();
			if( dependencies.vert_out_binormals )
				InitBinormalDirVert();

			InitObjectPos();

			if( editor.materialOutput.vertexOffset.IsConnectedAndEnabled() ) {
				App( "v.vertex.xyz += " + ps.n_vertexOffset + ";" );
			}

			if(dependencies.vert_out_worldPos)
				App ("o.posWorld = mul(_Object2World, v.vertex);");



			InitTangentTransformFrag();
			InitViewDirVert();
			InitReflectionDir();
			if( dependencies.frag_lightDirection ) {
				InitLightDir();
			}
			InitHalfVector();

			if(currentPass == PassType.Outline){
				App( "o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*"+ps.n_outlineWidth+",1));" );
			} else {
				App( "o.pos = mul(UNITY_MATRIX_MVP, v.vertex);" );
			}


			if( dependencies.frag_projPos ){
				App( "o.projPos = ComputeScreenPos (o.pos);" );
				App( "COMPUTE_EYEDEPTH(o.projPos.z);" );
			}
		

			if( dependencies.vert_out_screenPos ) { // TODO: Select screen pos accuracy etc

				if(ps.highQualityScreenCoords){
					App( "o.screenPos = o.pos;" ); // Unpacked per-pixel
				} else {
					App( "o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );" );
					App( "o.screenPos.y *= _ProjectionParams.x;" );
				}
			}
			if( LightmapThisPass() ) {
				App( "#ifndef LIGHTMAP_OFF" );
				scope++;
				App( "o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;" );
				scope--;
				App("#endif");
			}

			

			if( currentPass == PassType.ShadColl ) {
				App( "TRANSFER_SHADOW_COLLECTOR(o)" );
			} else if( currentPass == PassType.ShadCast ) {
				App( "TRANSFER_SHADOW_CASTER(o)" );
			} else {
				if(ps.IsVertexLit())
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
			App( "fixed4 frag(VertexOutput i) : COLOR {" );
			scope++;

			InitObjectPos();
			CheckClip();

			InitGrabPassSign();

			if(ps.normalQuality == SF_PassSettings.NormalQuality.Normalized && dependencies.frag_normalDirection){
				App ("i.normalDir = normalize(i.normalDir);");
			}

			InitTangentTransformFrag();
			InitViewDirFrag();
			InitNormalDirFrag();
			InitReflectionDir();

			PrepareLightmapVars();



			if( dependencies.vert_out_screenPos && ps.highQualityScreenCoords ) {
				App( "i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );" );
				App( "i.screenPos.y *= _ProjectionParams.x;" );
			}

			InitSceneColorAndDepth();

			if( dependencies.frag_lightDirection ) {
				InitLightDir();
			}
			InitHalfVector();



			Lighting(); // This is ignored in shadow passes

			if( currentPass == PassType.ShadColl ) {
				App( "SHADOW_COLLECTOR_FRAGMENT(i)" );
			} else if( currentPass == PassType.ShadCast ) {
				App( "SHADOW_CASTER_FRAGMENT(i)" );
			} else if(currentPass == PassType.Outline){
				App ("return fixed4("+ps.n_outlineColor+",0);");
			} else {

				//if(ps.mOut.diffuse.IsConnectedEnabledAndAvailable()){
				//	AppFinalOutput("lightFinal + " + "diffuse", ps.n_alpha); // This is really weird, it should already be included in the light calcs. Do more research // TODO
				//}else
				if(currentPass == PassType.FwdAdd){
					AppFinalOutput("finalColor * " + ps.n_alpha, "0");
				} else {
					AppFinalOutput("finalColor", ps.n_alpha);
				}

				
			}

			End();
		}




		void InitGrabPassSign(){
			if( !dependencies.grabPass )
				return;
			App("#if UNITY_UV_STARTS_AT_TOP");
			scope++;
				App("float grabSign = -_ProjectionParams.x;");
			scope--;
			App("#else");
			scope++;
				App("float grabSign = _ProjectionParams.x;");
			scope--;
			App( "#endif" );
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
			App( "float2 vUV[4]         : TEXCOORD;" );
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
			App( "OutputPatchConstant o;" );
			App( "float ts = Tessellation( v[0] );" );
			App( "o.edge[0] = ts;" );
			App( "o.edge[1] = ts;" );
			App( "o.edge[2] = ts;" );
			App( "o.inside = ts;" );
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
			App( "VertexInput v;" );

			TransferBarycentric( "vertex" );
			if( dependencies.vert_in_normals )
				TransferBarycentric( "normal" );
			if( dependencies.vert_in_tangents )
				TransferBarycentric( "tangent" );
			if( dependencies.uv0 )
				TransferBarycentric( "uv0" );
			if( dependencies.uv1 )
				TransferBarycentric( "uv1" );
			if( dependencies.vert_in_vertexColor )
				TransferBarycentric( "vertexColor" );
			if( dependencies.displacement )
				App( "displacement(v);" );
			App( "VertexOutput o = vert(v);" );
			App( "return o;" );
			scope--;
			App( "}" );

		}

		void TransferBarycentric(string s) {
			App( "v." + s + " = vi[0]." + s + "*bary.x + vi[1]." + s + "*bary.y + vi[2]." + s + "*bary.z;" );
		}


		void FuncTessellation() {
			App( "float Tessellation(TessVertex v){" );
			scope++;
			App( "return " + ps.n_tessellation + ";" );
			scope--;
			App( "}" );
		}

		void FuncDisplacement() {
			if( !dependencies.displacement )
				return;
			App( "void displacement (inout VertexInput v){" );
			scope++;
			App( "v.vertex.xyz +=  "+ps.n_displacement+";" );
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
			FuncDisplacement();
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
			if( !ps.UseClipping() )
				return;
			App( "clip(" + ps.n_alphaClip + " - 0.5);" );
		}


		
		void Fallback() {
			if( !string.IsNullOrEmpty( ps.fallback ) )
				App( "FallBack \"" + ps.fallback + "\"" );
			else
				App( "FallBack \"Diffuse\"" ); // Needed for shadows!
		}

		void WriteCustomEditor(){
			App("CustomEditor \"ShaderForgeMaterialInspector\"");
		}


		public void GrabPass() {
			if( !dependencies.grabPass )
				return;
			App("GrabPass{ }"); // TODO: Select if it's per-object or per-frame
		}




		
		


		void BasePass() {
			currentPass = PassType.FwdBase;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"ForwardBase\"" );
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

		public void LightPass() {

			// TODO: FIX


			// Only when real-time light things are connected. These are:
			// Diffuse
			// Specular
			// Although could be any D:

			bool customLit = dependencies.UsesLightNodes();
			bool builtinLit = ps.IsLit() && (ps.HasDiffuse() || ps.HasSpecular());

			bool needsLightPass = ( builtinLit || customLit ) && ps.UseMultipleLights();

			if( !needsLightPass )
				return;



			currentPass = PassType.FwdAdd;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();
			App( "Pass {" );
			scope++;
			{
				App( "Name \"ForwardAdd\"" );
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


		// This is a custom shadow thing!
		// Only needed when using alpha clip and/or vertex offset (May be needed with Tessellation as well)
		public void ShadowCollectorPass() {
			bool shouldUse = /*ps.shadowReceive &&*/ ( ps.UseClipping() || mOut.vertexOffset.IsConnectedAndEnabled() || mOut.displacement.IsConnectedAndEnabled() );
			if( !shouldUse )
				return;
			currentPass = PassType.ShadColl;
			UpdateDependencies();
			ResetDefinedState();
			dependencies.ResetTexcoordNumbers();

			App( "Pass {" );
			scope++;
			{
				App( "Name \"ShadowCollector\"" );
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
			bool shouldUse = /*ps.shadowCast &&*/ (ps.UseClipping() || mOut.vertexOffset.IsConnectedAndEnabled() || mOut.displacement.IsConnectedAndEnabled());
			if( !shouldUse )
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


		public void OutlinePass(){
			if(!mOut.outlineWidth.IsConnectedAndEnabled())
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





		public void ResetDefinedState() {
			for( int i = 0; i < cNodes.Count; i++ ) {
				cNodes[i].varDefined = false;
			}
		}



		public void Evaluate() {

			if(SF_Debug.evalFlow)
				Debug.Log( "SHADER EVALUATING" );

			editor.ps.fChecker.UpdateAvailability();
			if(!editor.nodeView.treeStatus.CheckCanCompile()){
				return;
			}
			ps.UpdateAutoSettings();
			currentPass = PassType.FwdBase;
			PrepareEvaluation();
			UpdateDependencies();
			shaderString = "";
			scope = 0;

			EditorUtility.UnloadUnusedAssets();
			GC.Collect();


			BeginShader();
			{
				PropertiesShaderLab();
				BeginSubShader();
				{
					SubShaderTags();
					if( ps.LOD > 0 )
						App("LOD " + ps.LOD);

					GrabPass();
					OutlinePass();
					BasePass();
					LightPass();
					ShadowCollectorPass();
					ShadowCasterPass();

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
			Asset shaderAsset = UnityEditor.VersionControl.Provider.GetAssetByPath(editor.GetShaderFilePath());
			if(shaderAsset.locked || shaderAsset.readOnly){
				UnityEditor.VersionControl.Provider.Lock( shaderAsset, false );
				UnityEditor.VersionControl.Provider.Checkout( shaderAsset, CheckoutMode.Both );
			}

			StreamWriter sw = new StreamWriter( editor.GetShaderFilePath() );
			sw.Write(fileContent);
			sw.Flush();
			sw.Close();
			try{
				AssetDatabase.Refresh( ImportAssetOptions.DontDownloadFromCacheServer );
			} catch(Exception e){
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
				case ( "ShaderForge.SFN_Vector4Property" ):
					SFN_Vector4Property vector4Node = (SFN_Vector4Property)node;
					m.SetVector( vector4Node.property.GetVariable(), vector4Node.texture.dataUniform );
					break;
			}
		}




		void End() {
			scope--;
			App( "}" );
		}
		public void App( string s ) {
			shaderString += GetScopeTabs() + s + "\n";
		}
		public void AppDebug( string s ) {
			//if(DEBUG)
			
			string scopeSlashes = GetScopeTabs().Replace(' ','/');
			
			if( scopeSlashes.Length < 2)
				scopeSlashes = "//";


			shaderString += scopeSlashes.Substring(Mathf.Min(s.Length + 2, scopeSlashes.Length - 2)) + " " + s + ":\n";
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
