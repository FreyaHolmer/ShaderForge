using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

namespace ShaderForge {
	public static class SF_Parser {

		public static SF_Editor editor;
		static List<SF_Link> links;
		public static bool quickLoad = false;
		public static bool settingUp = false;


		// [Source][Con] <---- [Con][Target]
		public struct SF_Link {
			public int sNode; // Source
			string sCon;
			public int tNode; // Target
			string tCon;

			public SF_Link( int sNode, string linkData ) {
				this.sNode = sNode;
				string[] split = linkData.Split( '-' );
				sCon = split[0];
				tNode = int.Parse( split[1] );
				tCon = split[2];
			}

			public void Establish( SF_Editor editor, LinkingMethod linkMethod = LinkingMethod.NoUpdate ) {
				SF_Node source = editor.GetNodeByID( sNode );
				SF_Node target = editor.GetNodeByID( tNode );
				// Debug.Log( "Linking " + target.nodeName + " <- " + source.nodeName );
				
				target.GetConnectorByID(tCon).LinkTo( source.GetConnectorByID(sCon), linkMethod );
			}
			
			
			
			public void Remap(int[] oldIDs, int[] newIDs){
				// Source id switching
				for(int i=0; i<oldIDs.Length; i++){
					if(sNode == oldIDs[i]){
						sNode = newIDs[i];
						break;
					}
				}
				
				// Target id switching
				for(int i=0; i<newIDs.Length; i++){
					if(tNode == oldIDs[i]){
						tNode = newIDs[i];
						break;
					}
				}
				
			}
			
			
			
			

		}


		/*
		static void Temp() {
			byte maj = 2;
			byte min = 6;

			int combined = InternalVersion(maj,min);
			Debug.Log( "Combined: " + combined );
			Debug.Log( "Resplit: " + ExternalVersion( combined ) );
		
		}

		public static string ExternalVersion( int internalVersion ) {
			byte oMaj = (byte)( internalVersion >> 8 );
			byte oMin = (byte)( internalVersion & 0x0000ffff );
			return oMaj + "." + oMin.ToString( "D2" );
		}

		public static int InternalVersion( byte maj, byte min ) {
			return ( ( maj << 8 ) & 0xffff ) | min;
		}
		 * */


		public static bool SerializedNodeIsProperty( string s ){
			return s.Contains(",ptlb:"); // Property label
		}

		public static void ParseNodeDataFromShader( SF_Editor editor, Shader s ) {
			SF_Parser.editor = editor;
			SF_Parser.links = new List<SF_Link>();
			// Search for Shader Forge data
			string data = ExtractShaderForgeData( s );
			if( string.IsNullOrEmpty( data ) ) {
				editor.CreateOutputNode();
				return;
			}
			LoadFromNodeData( data );
		}




		private static void LoadFromNodeData( string data ) {
			// First, split by rows (;)
			string[] rows = data.Split( ';' ); // TODO: Escape ; and | characters in user created comments!

			// TODO: Subshaders etc
			SF_Parser.settingUp = true;
			SF_Parser.quickLoad = true;
			foreach( string row in rows ) {
				if( row.StartsWith( "n:" ) ) {
					//Debug.Log("Deserializing node:" + row);
					DeserializeNode( row.Substring( 2 ), ref links );
					continue;
				}
				if( row.StartsWith( "ps:" ) ) {
					editor.ps.Deserialize( row.Substring( 3 ) );
					continue;
				}
				if( row.StartsWith( "proporder:" ) ) {
					editor.nodeView.treeStatus.DeserializeProps( row.Substring(10) );
					continue;
				}
			}

			// Create all node links
			for( int i = 0; i < links.Count; i++ ) {
				links[i].Establish( editor );
			}

			

			//Debug.Log("All links established, hierarchally refreshing...");
			// Refresh hierarchally

			//Profiler.BeginSample ("MyPieceOfCode");

			//editor.nodeView.HierarchalRefresh();

			//Profiler.EndSample();


			//Debug.Log( "Reconnect pending..." );

			editor.nodeView.ReconnectConnectedPending();
			SF_Parser.quickLoad = false;
			
			//Debug.Log( "Reconnect done, updating auto settings..." );

			// Update auto settings based on everything connected
			editor.ps.UpdateAutoSettings();

			//Debug.Log( "Auto settings done, centering camera..." );

			// Center camera
			editor.nodeView.CenterCamera();
			SF_Parser.settingUp = false;
			SF_Parser.quickLoad = false;


			// Update preview images by refreshing all outermost nodes
			editor.nodeView.HierarchalRefresh();

			//Debug.Log( "Centered camera, recompiling shader..." );
			editor.materialOutput.OnUpdateNode( NodeUpdateType.Hard, true );


		}

		// This is the data per-node
		// n:type:SFN_Final,id:6,x:33383,y:32591|0-8-0;
		public static SF_Node DeserializeNode( string row, ref List<SF_Link> linkList) {
			
			
			bool isLinked = row.Contains( "|" );

			string linkData = "";

			// Grab connections, if any, and remove them from the main row
			if( isLinked ) {
				string[] split = row.Split( '|' );
				row = split[0];
				linkData = split[1];
			}


			string[] nData = row.Split( ',' ); // Split the node data
			SF_Node node = null;

			// This is the data in a single node, without link information
			// type:SFN_Final,id:6,x:33383,y:32591
			foreach( string s in nData ) {
				if(SF_Debug.deserialization)
					Debug.Log("Deserializing node: " + s);
				string[] split = s.Split( ':' );
				string dKey = split[0];
				string dValue = split[1];

				switch( dKey ) {
					case "type":
						//Debug.Log( "Deserializing " + dValue );
						node = TryCreateNodeOfType( dValue ); if( node == null ) { Debug.LogError( "Node not found, returning..." ); return null; }
						break;
					case "id":
						node.id = int.Parse( dValue );
						editor.idIncrement = Mathf.Max( editor.idIncrement, node.id + 1 );
						break;
					case "x":
						node.rect.x = int.Parse( dValue );
						break;
					case "y":
						node.rect.y = int.Parse( dValue );
						break;
					case "ptlb":
						node.property.SetName( dValue );
						break;
					case "ptin":
						node.property.nameInternal = dValue;
						break;
					case "cmnt":
						node.comment = dValue;
						break;
					default:
						//Debug.Log("Deserializing KeyValue: " +dKey + " v: " + dValue);
						node.DeserializeSpecialData( dKey, dValue );
						break;
				}
			}

			// Add links to link data, if it's connected
			if( isLinked ) {
				string[] parsedLinks = linkData.Split( ',' );
				foreach( string s in parsedLinks )
					linkList.Add( new SF_Link( node.id, s ) );
			}
			
			
			return node;

		}

		private static SF_Node TryCreateNodeOfType( string nodeType ) {
			SF_Node node = null;
			if( nodeType == "ShaderForge.SFN_Final" ) {
				node = editor.CreateOutputNode();
			} else {
				foreach( SF_EditorNodeData tmp in editor.nodeTemplates ) {
					if( tmp.type == nodeType ) {		// 1 is the type
						node = editor.AddNode( tmp );	// Create the node
						break;
					}
				}
			}
			if( node == null ) {
				Debug.LogError( "Type [" + nodeType + "] not found!" );
			}
			return node;
		}


		public static string ExtractShaderForgeData( Shader s, bool setPath = true, bool findRenderers = true, bool findLOD = true) {

			string path = AssetDatabase.GetAssetPath( s );
			string[] shaderData = File.ReadAllLines( path );

			string returnString = "";

			if( shaderData.Length == 0 || shaderData == null ) {
				//Debug.LogWarning( "Shader file empty" );
				return null;
			}

			bool found_data = false;
			bool found_renderers = !findRenderers;
			bool found_path = !setPath;
			bool found_LOD = !findLOD;

			for( int i = 0; i < shaderData.Length; i++ ) {
				if( shaderData[i].StartsWith( "/*SF_DATA;" ) ) {
					returnString = shaderData[i].Substring( 10, shaderData[i].Length - 12 ); // Exclude comment markup
					found_data = true;
				}
				if( setPath )
					if( shaderData[i].StartsWith( "Shader" ) ) {
						editor.currentShaderPath = shaderData[i].Split( '\"' )[1];
						found_path = true;
					}
				if( findRenderers ) {
					if( shaderData[i].TrimStart().StartsWith( "#pragma only_renderers" ) ) {
						ParseRenderer( shaderData[i].Trim().Split( ' ' ), true);
						found_renderers = true;
					} else if( shaderData[i].TrimStart().StartsWith( "#pragma exclude_renderers" ) ) {
						ParseRenderer( shaderData[i].Trim().Split( ' ' ), false );
						found_renderers = true;
					}
				}
				if( findLOD ) {
					if( shaderData[i].TrimStart().StartsWith( "LOD " ) ) {
						editor.ps.catMeta.LOD = int.Parse(shaderData[i].Trim().Split( ' ' )[1]);
						found_LOD = true;
					} 
				}

				if( found_data && found_path && found_renderers && found_LOD )
					break;
			}



			if( string.IsNullOrEmpty( returnString ) ) {
				//Debug.LogWarning( "Shader did not contain node data" );
			}

			// TODO: check when it was last changed!
			// This is where it should ask you if you want to overwrite the existing data,
			// if it's older than x minutes

			return returnString;
		}


		public static bool ContainsShaderForgeData(Shader s){
			string sfData = SF_Parser.ExtractShaderForgeData( s, false, false, false );
			return !string.IsNullOrEmpty( sfData );
		}
		





		public static void ParseRenderer( string[] arr, bool only ) {
			for( int i = 0; i < editor.ps.catMeta.usedRenderers.Length; i++ ) {
				editor.ps.catMeta.usedRenderers[i] = !only; // Enable or disable all
			}
			for( int i = 2; i < arr.Length; i++ ) { // i = 2 to ignore #pragma x_renderers
				string rndr = arr[i];
				int enm = (int)((RenderPlatform)Enum.Parse( typeof( RenderPlatform ), rndr ));
				editor.ps.catMeta.usedRenderers[enm] = only; // Disable or enable one
			}
		}



	}
}