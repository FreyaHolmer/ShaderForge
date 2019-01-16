using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace ShaderForge {

	[System.Serializable]
	public class SF_NodeTreeStatus : ScriptableObject {

		public SF_Editor editor;
		[SerializeField] List<SF_ErrorEntry> errors;
		public List<SF_ErrorEntry> Errors {
			get {
				if( errors == null )
					errors = new List<SF_ErrorEntry>();
				return errors;
			}
			set {
				errors = value;
			}
		}



		public bool mipInputUsed = false; // If this is true, only DX is allowed :< OR: Enable glsl pragma
		public bool texturesInVertShader = false;
		public bool viewDirectionInVertOffset = false;
		public bool usesSceneData = false;
		// public bool lightNodesUsed = false; // Used to re-enable light settings when shader is set to unlit



		// Contains references to all nodes with properties
		// Used in the pass settings listing
		public List<SF_Node> propertyList = new List<SF_Node>();


		public bool CanDisplayInstructionCount {
			get {
				bool dx = ( editor.statusBox.platform == RenderPlatform.d3d9 || editor.statusBox.platform == RenderPlatform.d3d11 );
				return !( mipInputUsed && !dx );
			}

		}



		public string SerializeProps() {
			string s = "proporder:";
			for( int i = 0; i < propertyList.Count; i++ ) {
				if( i != 0 )
					s += "-";
				s += propertyList[i].id.ToString();
			}
			return s;
		}

		public void DeserializeProps( string s ) {
			//Debug.Log("Deserializing properties = " + s);
			string[] split = s.Split( '-' );
			propertyList = new System.Collections.Generic.List<SF_Node>();
			for( int i = 0; i < split.Length; i++ ) {
				//Debug.Log("Found " + GetNodeByID( int.Parse( split[i] )).nodeName);
				//Debug.Log ("Attempting deserialization. int parse of ["+split[i]+"]");

				SF_Node foundNode = GetNodeByID( int.Parse( split[i] ) );
				if( foundNode != null )
					propertyList.Add( foundNode );

			}
		}

		public SF_Node GetNodeByID( int id ) {
			foreach( SF_Node n in editor.nodes ) {
				if( n.id == id )
					return n;
			}
			Debug.LogError( "Property node with ID " + id + " not found while deserializing, removing..." );
			return null;
		}






		public SF_NodeTreeStatus Initialize( SF_Editor editor ) {
			this.editor = editor;
			return this;
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public bool CheckCanCompile() {

			editor.nodeView.RefreshRelaySources();

			if( Errors == null ){
				Errors = new List<SF_ErrorEntry>();
			} else if( Errors.Count > 0 ) {
				for( int i = 0; i < Errors.Count; i++ ) {
					DestroyImmediate( Errors[i] );
				}
				Errors.Clear();
			}

			

			List<SF_Node> cNodes = GetListOfConnectedNodesWithGhosts( out editor.shaderEvaluator.ghostNodes );


			// If any properties are now outside the node graph, remove them from the property list
			/*if(!SF_Parser.settingUp)
				for( int i = propertyList.Count - 1; i >= 0; i-- ) {
					if( !cNodes.Contains( propertyList[i] ) ) {
						propertyList.RemoveAt( i );
					}
				}*/


			//if( editor.shaderEvaluator.ghostNodes != null )
			//Debug.Log( "Ghost nodes: " + editor.shaderEvaluator.ghostNodes.Count );

			texturesInVertShader = false;
			bool foundMipUsed = false;
			//SF_Node mipNode = null;
			usesSceneData = false;

			bool hasFacingNode = false;

			foreach( SF_Node n in cNodes ) {

				// Refresh property list
				if( n.IsProperty() ) {
					if( !n.IsGlobalProperty() ) {
						// Add if it's local and doesn't contain it already
						if( !propertyList.Contains( n ) ) {
							propertyList.Add( n );
						}
					} else {
						// Remove it if it's global and inside the list
						if( propertyList.Contains( n ) ) {
							propertyList.Remove( n );
						}
					}
				}

				if( n is SFN_SceneColor ) {
					usesSceneData = true;
				}

				if( n is SFN_FaceSign ) {
					hasFacingNode = true;
				}


				if( n is SFN_Tex2d || n is SFN_Cubemap ) { // Check MIP input
					if( n.GetInputIsConnected( "MIP" ) ) {
						foundMipUsed = true;
						//mipNode = n;
					}
				}

				//if(SF_Debug.dynamicNodeLoad)
				if( SF_Editor.NodeExistsAndIs( n, "SFN_SkyshopSpec" ) ) {
					//if(n.GetInputIsConnected("GLOSS")){
					foundMipUsed = true;
					//mipNode = n;
					//}
				}





				foreach( SF_NodeConnector con in n.connectors ) {
					if( con.conType == ConType.cOutput )
						continue;
					if( con.required && !con.IsConnected() ) {
						string err = "Missing required";
						err += string.IsNullOrEmpty( con.label ) ? " " : " [" + con.label + "] ";
						err += "input on " + con.node.nodeName;
						Errors.Add( SF_ErrorEntry.Create( err, con, false ) );
					}
				}
			}



			// WARNINGS

			if( editor.ps.catBlending.autoSort ) {

				bool alphaConnected = editor.ps.HasAlpha();
				


				if( editor.ps.catLighting.transparencyMode == SFPSC_Lighting.TransparencyMode.Fade ) {

					bool usingAlphaBlend = editor.ps.catBlending.blendSrc == BlendMode.SrcAlpha && editor.ps.catBlending.blendDst == BlendMode.OneMinusSrcAlpha;

					if( alphaConnected && !usingAlphaBlend ) {
						SF_ErrorEntry error = SF_ErrorEntry.Create( "Opacity is connected, but your shader isn't alpha blended, which is required by the fade transparency mode. Click the icon to make it alpha blended!", true );
						error.action = () => {
							UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
							editor.ps.catBlending.blendModePreset = BlendModePreset.AlphaBlended;
							editor.ps.catBlending.ConformBlendsToPreset();
						};
						Errors.Add( error );
					}

					if( !alphaConnected && usingAlphaBlend ) {
						SF_ErrorEntry error = SF_ErrorEntry.Create( "Opacity is not connected, but your shader is alpha blended. Click the icon to make it opaque!", true );
						error.action = () => {
							UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
							editor.ps.catBlending.blendModePreset = BlendModePreset.Opaque;
							editor.ps.catBlending.ConformBlendsToPreset();
						};
						Errors.Add( error );
					}
				}





				if( editor.ps.catLighting.transparencyMode == SFPSC_Lighting.TransparencyMode.Reflective ) {

					bool usingAlphaBlendPremul = editor.ps.catBlending.blendSrc == BlendMode.One && editor.ps.catBlending.blendDst == BlendMode.OneMinusSrcAlpha;

					if( alphaConnected && !usingAlphaBlendPremul ) {
						SF_ErrorEntry error = SF_ErrorEntry.Create( "Opacity is connected, but your shader isn't using premultiplied alpha blending, which is required by the reflective transparency mode. Click the icon to use premultiplied alpha blending!", true );
						error.action = () => {
							UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
							editor.ps.catBlending.blendModePreset = BlendModePreset.AlphaBlendedPremultiplied;
							editor.ps.catBlending.ConformBlendsToPreset();
						};
						Errors.Add( error );
					}

					if( !alphaConnected && usingAlphaBlendPremul ) {
						SF_ErrorEntry error = SF_ErrorEntry.Create( "Opacity is not connected, but your shader is using premultiplied alpha blending. Click the icon to make it opaque!", true );
						error.action = () => {
							UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
							editor.ps.catBlending.blendModePreset = BlendModePreset.Opaque;
							editor.ps.catBlending.ConformBlendsToPreset();
						};
						Errors.Add( error );
					}
				}



			}
			



			/*
			 	true,	// - Direct3D 9
				true,	// - Direct3D 11
				true,	// - OpenGL
				true,	// - OpenGL ES 2.0
				false,  // - Xbox 360
				false,	// - PlayStation 3
				false,	// - Flash
				false	// - Direct3D 11 for Windows RT
			*/
			bool osx = Application.platform == RuntimePlatform.OSXEditor;
			bool windows = !osx;
			bool ogl = editor.ps.catMeta.usedRenderers[2];
			bool dx9 = editor.ps.catMeta.usedRenderers[0];
			bool dx11 = editor.ps.catMeta.usedRenderers[1];

#if UNITY_5_0
			bool inDx11Mode = UnityEditor.PlayerSettings.useDirect3D11;
#else
			bool inDx11Mode = true;
#endif

			if( osx && !ogl ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Your shader will not render properly on your workstation - you need to have OpenGL enabled when working in OSX. Click the icon to enable OpenGL!", true );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catMeta, "error correction - enable OpenGL" );
					editor.ps.catMeta.usedRenderers[2] = true;
					editor.OnShaderModified( NodeUpdateType.Hard );
				};
				Errors.Add( error );
			} else if( windows ) {
				if( inDx11Mode && !dx11 ) {
					SF_ErrorEntry error = SF_ErrorEntry.Create( "Your shader might not render properly on your workstation - you need to have Direct3D 11 enabled when working in DX11 mode on Windows. Click the icon to enable Direct3D 11!", true );
					error.action = () => {
						UnityEditor.Undo.RecordObject( editor.ps.catMeta, "error correction - enable Direct3D 11" );
						editor.ps.catMeta.usedRenderers[1] = true;
						editor.OnShaderModified( NodeUpdateType.Soft );
					};
					Errors.Add( error );
				} else if( !inDx11Mode && !dx9 ) {
					SF_ErrorEntry error = SF_ErrorEntry.Create( "Your shader might not render properly on your workstation - you need to have Direct3D 9 enabled when working on Windows. Click the icon to enable Direct3D 9!", true );
					error.action = () => {
						UnityEditor.Undo.RecordObject( editor.ps.catMeta, "error correction - enable Direct3D 9" );
						editor.ps.catMeta.usedRenderers[0] = true;
						editor.OnShaderModified( NodeUpdateType.Soft );
					};
					Errors.Add( error );
				}
			}





			if( editor.ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL ) {
				if( editor.ps.HasDiffuse() && !editor.ps.HasSpecular() ) {
					Errors.Add( SF_ErrorEntry.Create( "Using PBL requires metallic/specular to be connected", false ) );
				}
				if( !editor.ps.HasDiffuse() && editor.ps.HasSpecular() ) {
					Errors.Add( SF_ErrorEntry.Create( "Using PBL requires metallic/specular to be connected", false ) );
				}
			}
			



			
			


			List<SF_Node> dupes = new List<SF_Node>();
			SF_Node[] propNodes = cNodes.Where(x=>x.IsProperty()).ToArray();
			for( int i = 0; i < propNodes.Length; i++ ) {
				for( int j = i+1; j < propNodes.Length; j++ ) {
					string nameA = propNodes[i].property.nameInternal;
					string nameB = propNodes[j].property.nameInternal;

					if( nameA == nameB ) {
						dupes.Add( propNodes[j] );
					}

				}
			}
			if( dupes.Count > 0 ) {
				foreach( SF_Node dupe in dupes ) {
					Errors.Add( SF_ErrorEntry.Create( "You have property nodes with conflicting internal names. Please rename one of the " + dupe.property.nameInternal + " nodes", dupe, false ) );
				}
			}




			List<SF_Node> dupesVarname = new List<SF_Node>();
			for( int i = 0; i < cNodes.Count; i++ ) {
				for( int j = i + 1; j < cNodes.Count; j++ ) {

					string nameAvar = cNodes[i].variableName;
					string nameBvar = cNodes[j].variableName;

					if( nameAvar == nameBvar && dupes.Contains( cNodes[j] ) == false ) {
						dupesVarname.Add( cNodes[j] );
					}
				}
			}
			if( dupesVarname.Count > 0 ) {
				foreach( SF_Node dupeVarname in dupesVarname ) {
					Errors.Add( SF_ErrorEntry.Create( "You have nodes with conflicting variable names. Please rename one of the " + dupeVarname.variableName + " nodes", dupeVarname, false ) );
				}
			}

			
			// Make sure you set the shader to double sided
			if( !editor.ps.catGeometry.IsDoubleSided() && hasFacingNode ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "You are using the Face Sign node, but your shader isn't double-sided. Click the icon to fix", false );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catGeometry, "error correction - fix double sided" );
					editor.ps.catGeometry.cullMode = SFPSC_Geometry.CullMode.DoubleSided;
					editor.OnShaderModified( NodeUpdateType.Hard );
				};
				Errors.Add( error );
			}
			





			// Check if there are any textures in the vertex input
			texturesInVertShader = HasNodeInput<SFN_Tex2d>( editor.mainNode.vertexOffset ) || HasNodeInput<SFN_Tex2d>( editor.mainNode.outlineWidth );
			viewDirectionInVertOffset = HasNodeInput<SFN_ViewVector>( editor.mainNode.vertexOffset );



			editor.shaderEvaluator.RemoveGhostNodes();


			if( foundMipUsed ) {
				//if( !mipInputUsed ) // This should be fixed with #pragma glsl
				//	errors.Add( new SF_ErrorEntry( "MIP input is only supported in Direct X", mipNode ) );
				mipInputUsed = true;
			} else {
				mipInputUsed = false;
			}


			int errorCount = Errors.Count( x => !x.isWarning ); // Let it compile, even though it has warnings

			if( errorCount == 0 )
				return true;
			//DisplayErrors();
			return false;
		}

		private bool ConnectedNodeWithInternalNameExists( List<SF_Node> cNodes, string s ) {
			foreach( SF_Node n in cNodes.Where( x => x.IsProperty() ) ) {
				if( n.property.nameInternal == s ) {
					return true;
				}
			}
			return false;
		}



		public bool HasNodeInput<T>( SF_NodeConnector con ) {

			if( con.IsConnectedEnabledAndAvailable() ) {

				if( con.inputCon.node is T ) {
					return true;
				}

				// Recursively loop through inputs of the connnected node
				foreach( SF_NodeConnector c in con.inputCon.node.connectors ) {
					if( c.conType == ConType.cOutput )
						continue;
					if( !c.IsConnected() )
						continue;
					if( HasNodeInput<T>( c ) ) {
						return true;
					}
				}

			}
			return false;
		}





		// Returns all nodes connected to the final node
		public List<SF_Node> GetListOfConnectedNodesWithGhosts( out List<SF_Node> ghosts, bool passDependent = false ) {
			//Debug.Log ("GetListOfConnectedNodesWithGhosts()");
			ResetAllNodeStatuses();
			editor.mainNode.status.SetLeadsToFinalRecursively( all: false, passDependent: passDependent );
			List<SF_Node> filtered = new List<SF_Node>();
			foreach( SF_Node n in editor.nodes ) {
				if( n.status.leadsToFinal )
					filtered.Add( n );
			}

			// Now that's done, let's return the ghost nodes too, if any
			editor.shaderEvaluator.RemoveGhostNodes(); // TODO: Really?
			ghosts = new List<SF_Node>();

			foreach( SF_Node n in filtered ) {
				n.DefineGhostsIfNeeded( ref ghosts );
			}

			//Debug.Log ("GetListOfConnectedNodesWithGhosts, ghosts.Count: " + ghosts.Count);

			filtered.AddRange( ghosts );

			return filtered;
		}

		// Resets all node statuses
		public void ResetAllNodeStatuses() {
			foreach( SF_Node n in editor.nodes ) {
				n.status.Reset();
			}
		}





	}
}