using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace ShaderForge {

	[System.Serializable]
	public class SF_NodeTreeStatus : ScriptableObject {

		public SF_Editor editor;
		public List<SF_ErrorEntry> errors;
		public bool mipInputUsed = false; // If this is true, only DX is allowed :<
		public bool usesSceneData = false;
		// public bool lightNodesUsed = false; // Used to re-enable light settings when shader is set to unlit



		// Contains references to all nodes with properties
		// Used in the pass settings listing
		public List<SF_Node> propertyList = new List<SF_Node>();



		public string SerializeProps() {
			string s = "proporder:";
			for( int i = 0; i < propertyList.Count;i++ ) {
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
				propertyList.Add( GetNodeByID( int.Parse( split[i] ) ) );
			}
		}

		public SF_Node GetNodeByID( int id ) {
			foreach( SF_Node n in editor.nodes ) {
				if( n.id == id )
					return n;
			}
			Debug.LogError( "Property node with ID " + id + " not found while deserializing" );
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
			if(errors == null)
				errors = new List<SF_ErrorEntry>();
			errors.Clear();
			
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

			bool foundMipUsed = false;
			SF_Node mipNode = null;
			usesSceneData = false;
			foreach( SF_Node n in cNodes ) {

				// Refresh property list
				if( n.IsProperty() ) {
					// Add if it doesn't contain it already
					if( !propertyList.Contains( n ) ) {
						propertyList.Add( n );
					}
				}

				if( n is SFN_SceneColor ){
					usesSceneData = true;
				}


				if( n is SFN_Tex2d || n is SFN_Cubemap ) { // Check MIP input
					if( n.GetInputIsConnected( "MIP" ) ) {
						foundMipUsed = true;
						mipNode = n;
					}
				}



				foreach( SF_NodeConnection con in n.connectors ) {
					if( con.conType == ConType.cOutput )
						continue;
					if(con.required && !con.IsConnected()){
						string err = "Missing required ";
						err += string.IsNullOrEmpty(con.label) ? "" : "[" + con.label + "]";
						err += " input on " + con.node.nodeName;
						errors.Add( new SF_ErrorEntry( err, con ) );
					}
				}
			}

			editor.shaderEvaluator.RemoveGhostNodes();


			if( foundMipUsed ) {
				if( !mipInputUsed )
					errors.Add( new SF_ErrorEntry( "MIP input is only supported in Direct X", mipNode ) );
				mipInputUsed = true;
			} else {
				mipInputUsed = false;
			}



			



			if(errors.Count == 0)
				return true;
			DisplayErrors();
			return false;
		}

		public void DisplayErrors(){
			//foreach(SF_ErrorEntry err in errors){
				// Debug.Log(err.error);
			//}
		}


		// Returns all nodes connected to the final node
		public List<SF_Node> GetListOfConnectedNodesWithGhosts(out List<SF_Node> ghosts) {
			//Debug.Log ("GetListOfConnectedNodesWithGhosts()");
			ResetAllNodeStatuses();
			editor.materialOutput.status.SetLeadsToFinalRecursively(true);
			List<SF_Node> filtered = new List<SF_Node>();
			foreach( SF_Node n in editor.nodes ) {
				if( n.status.leadsToFinal )
					filtered.Add(n);
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

