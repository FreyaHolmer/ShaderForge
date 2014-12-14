using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace ShaderForge {

	[System.Serializable]
	public class SF_NodeTreeStatus : ScriptableObject {

		public SF_Editor editor;
		public List<SF_ErrorEntry> errors;
		public bool mipInputUsed = false; // If this is true, only DX is allowed :< OR: Enable glsl pragma
		public bool texturesInVertShader = false;
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
			if( errors == null ){
				errors = new List<SF_ErrorEntry>();
			} else if( errors.Count > 0 ) {
				for( int i = 0; i < errors.Count; i++ ) {
					DestroyImmediate( errors[i] );
				}
				errors.Clear();
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
						errors.Add( SF_ErrorEntry.Create( err, con, false ) );
					}
				}
			}



			// WARNINGS

			bool alphaConnected = editor.mainNode.alpha.IsConnectedEnabledAndAvailable();
			bool notUsingAlphaBlend = editor.ps.catBlending.autoSort && editor.ps.catBlending.blendModePreset != BlendModePreset.AlphaBlended;

			if( alphaConnected && notUsingAlphaBlend ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Alpha is connected, but your shader isn't alpha blended. Click the icon to make it alpha blended!", true );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
					editor.ps.catBlending.blendModePreset = BlendModePreset.AlphaBlended;
					editor.ps.catBlending.ConformBlendsToPreset();
				};
				errors.Add( error );
			}

			if( !alphaConnected && !notUsingAlphaBlend ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Alpha is not connected, but your shader is alpha blended. Click the icon to make it opaque!", true );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catBlending, "error correction" );
					editor.ps.catBlending.blendModePreset = BlendModePreset.Opaque;
					editor.ps.catBlending.ConformBlendsToPreset();
				};
				errors.Add( error );
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
			bool ogl = editor.ps.catMeta.usedRenderers[2];
			bool dx9 = editor.ps.catMeta.usedRenderers[0];
			bool dx11 = editor.ps.catMeta.usedRenderers[1];
			bool anyDx = dx9 || dx11;

			if( osx && !ogl ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Your shader will not render on your workstation - you need to have OpenGL enabled when working in OSX. Click the icon to enable OpenGL!", true );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catMeta, "error correction - enable OpenGL" );
					editor.ps.catMeta.usedRenderers[2] = true;
					editor.OnShaderModified( NodeUpdateType.Hard );
				};
				errors.Add( error );
			} else if( !osx && !anyDx ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Your shader might not render on your workstation - you need to have Direct3D 9 and/or Direct3D 11 enabled when working in Windows. Click the icon to enable DirectX 9!", true );
				error.action = () => {
					UnityEditor.Undo.RecordObject( editor.ps.catMeta, "error correction - enable Direct3D 9" );
					editor.ps.catMeta.usedRenderers[0] = true;
					editor.OnShaderModified( NodeUpdateType.Hard );
				};
				errors.Add( error );
			}




			// LIGHTMAP WARNINGS
			if( editor.ps.catLighting.IsLit() && editor.ps.catLighting.lightmapped ) {

				// This is primarily for Unity 4, Unity 5 has attributes for this instead
				LightmapCondition( cNodes, "_MainTex", editor.mainNode.diffuse, "Texture" );
				LightmapCondition( cNodes, "_Color", editor.mainNode.diffuse, "Color" );
				LightmapCondition( cNodes, "_SpecColor", editor.mainNode.specular, "Color", "Specular/" );
				LightmapCondition( cNodes, "_BumpMap", editor.mainNode.normal, "Texture" );
				LightmapCondition( cNodes, "_Illum", editor.mainNode.emissive, "Texture", "Self-Illumin/" );
			}





			// Check if there are any textures in the vertex input
			texturesInVertShader = HasTextureInput( editor.mainNode.vertexOffset ) || HasTextureInput( editor.mainNode.outlineWidth );




			editor.shaderEvaluator.RemoveGhostNodes();


			if( foundMipUsed ) {
				//if( !mipInputUsed ) // This should be fixed with #pragma glsl
				//	errors.Add( new SF_ErrorEntry( "MIP input is only supported in Direct X", mipNode ) );
				mipInputUsed = true;
			} else {
				mipInputUsed = false;
			}


			int errorCount = errors.Count( x => !x.isWarning ); // Let it compile, even though it has warnings

			if( errorCount == 0 )
				return true;
			//DisplayErrors();
			return false;
		}

		private void LightmapCondition( List<SF_Node> cNodes, string internalName, SF_NodeConnector reqConnection, string propertyType, string pathReq = "" ) {

			bool connected = reqConnection.IsConnectedEnabledAndAvailable();
			bool foundNamedNode = ConnectedNodeWithInternalNameExists( cNodes, internalName );
			bool pathValid = editor.currentShaderPath.Contains( pathReq );

			if( connected && !foundNamedNode ) {
				SF_ErrorEntry error = SF_ErrorEntry.Create( "Lightmapping wants a " + propertyType + " property, with an internal name of " + internalName, true );
				errors.Add( error );
			}

			if( connected && !pathValid ) {
				SF_ErrorEntry errorPath = SF_ErrorEntry.Create( "Lightmapping expects the shader path to contain " + pathReq + " when " + reqConnection.label + " is connected", true );
				errors.Add( errorPath );
			}



		}

		private bool ConnectedNodeWithInternalNameExists( List<SF_Node> cNodes, string s ) {
			foreach( SF_Node n in cNodes.Where( x => x.IsProperty() ) ) {
				if( n.property.nameInternal == s ) {
					return true;
				}
			}
			return false;
		}



		public bool HasTextureInput( SF_NodeConnector con ) {

			if( con.IsConnectedEnabledAndAvailable() ) {

				if( con.inputCon.node is SFN_Tex2d ) {
					return true;
				}

				// Recursively loop through inputs of the connnected node
				foreach( SF_NodeConnector c in con.inputCon.node.connectors ) {
					if( c.conType == ConType.cOutput )
						continue;
					if( !c.IsConnected() )
						continue;
					if( HasTextureInput( c ) ) {
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