using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ShaderForge {
	public static class SF_Parser {

		public static SF_Editor editor;
		static List<SF_Link> links;
		public static bool quickLoad = false;
		public static bool settingUp = false;





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

		public static bool ParseNodeDataFromShader( SF_Editor editor, Shader s ) {
			SF_Parser.editor = editor;
			SF_Parser.links = new List<SF_Link>();
			// Search for Shader Forge data
			float version;
			string data = ExtractShaderForgeData( s, out version );
			if( string.IsNullOrEmpty( data ) ) {
				editor.CreateOutputNode();
				return true; // Empty shader
			}
			string missingNode;
			bool didLoadFlawlessly = LoadFromNodeData( data, version, out missingNode );
			 
			if( !didLoadFlawlessly ) {
				EditorUtility.DisplayDialog( "Failed to load shader", "Failed to open shader due to missing the node [" + missingNode + "]", "Close" );
				editor.Close();
//				editor.Init();
				//editor.closeMe = true;
				//editor.initialized = false;
				//SF_Editor.instance = null;
				//editor.Close();
				return false;
			}

			return true;
				
		}




		private static bool LoadFromNodeData( string data, float version, out string missingNode ) {
			// First, split by rows (;)
			missingNode = "";
			string[] rows = data.Split( ';' ); // TODO: Escape ; and | characters in user created comments!

			// TODO: Subshaders etc
			SF_Parser.settingUp = true;
			SF_Parser.quickLoad = true;
			foreach( string row in rows ) {
				if( row.StartsWith( "n:" ) ) {
					//Debug.Log("Deserializing node:" + row);
					SF_Node node = SF_Node.Deserialize( row.Substring( 2 ), ref links );
					if( node == null ) {
						missingNode = row.Substring( 2 ).Split(',')[0].Split(':')[1];
						SF_Parser.settingUp = false;
						SF_Parser.quickLoad = false;
						return false; // Interrupt node loading, node wasn't found
					}
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


			// If this was created in a version older than 0.37, reverse the node tree around its center point
			if( version <= 0.36f){

				Debug.Log("Reversing node tree due to shader being created before the reversal in 0.37");
				
				// Average node position
				float avgX = editor.nodes.Average(x => x.rect.center.x);
			
				// Reverse all nodes
				foreach(SF_Node node in editor.nodes){
					Vector2 old = node.rect.center;
					node.rect.center = new Vector2(2 * avgX - old.x, old.y);
				}

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
			editor.mainNode.OnUpdateNode( NodeUpdateType.Hard, true );

			return true;
		}






		public static string ExtractShaderForgeData( Shader s, out float version, bool setPath = true, bool findRenderers = true, bool findLOD = true) {

			string path = AssetDatabase.GetAssetPath( s );
			string[] shaderData = File.ReadAllLines( path );

			string returnString = "";

			version = 0f;

			if( shaderData.Length == 0 || shaderData == null ) {
				//Debug.LogWarning( "Shader file empty" );
				return null;
			}

			bool found_data = false;
			bool found_renderers = !findRenderers;
			bool found_path = !setPath;
			bool found_LOD = !findLOD;


			for( int i = 0; i < shaderData.Length; i++ ) {
				if(shaderData[i].Contains("Shader created with Shader Forge")){
					string[] split = shaderData[i].Trim().Split(' ');
					string verStr = split[split.Length-1];

					if( verStr.StartsWith( "v" ) )
						verStr = verStr.Substring( 1 );

					version = float.Parse(verStr);
				}
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
			float version;
			string sfData = SF_Parser.ExtractShaderForgeData( s, out version, false, false, false );
			return !string.IsNullOrEmpty( sfData );
		}
		
		public static void ParseRenderer( string[] arr, bool only ) {
			for( int i = 0; i < editor.ps.catMeta.usedRenderers.Length; i++ ) {
				editor.ps.catMeta.usedRenderers[i] = !only; // Enable or disable all
			}
			for( int i = 2; i < arr.Length; i++ ) { // i = 2 to ignore #pragma x_renderers
				string rndr = arr[i];
				if( rndr == "flash" ) {
					Debug.LogWarning( "Flash is no longer supported by Unity, and was removed from the shader" );
					continue;
				}
				if( rndr == "ps3" ) {
					Debug.LogWarning( "PS3 is no longer supported by Unity since 5.5, and was removed from the shader" );
					continue;
				}
				if( rndr == "xbox360" ) {
					Debug.LogWarning( "Xbox 360 is no longer supported by Unity since 5.5, and was removed from the shader" );
					continue;
				}
				if( rndr == "opengl" ) {
					rndr = "glcore";
				}
				if( rndr == "switch" ) {
					rndr = "nswitch";
				}
				int enm = (int)((RenderPlatform)Enum.Parse( typeof( RenderPlatform ), rndr ));
				editor.ps.catMeta.usedRenderers[enm] = only; // Disable or enable one
			}
		}

	}
}