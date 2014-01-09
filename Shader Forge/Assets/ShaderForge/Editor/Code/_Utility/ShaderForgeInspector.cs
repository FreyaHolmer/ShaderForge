using System;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;
using ShaderForge;

namespace UnityEditor {
	[CustomEditor( typeof( Shader ) )]
	public class ShaderForgeInspector : Editor {





		

		private static string[] kPropertyTypes = new string[]
		{
			"Color: ",
			"Vector: ",
			"Float: ",
			"Range: ",
			"Texture: "
		};
		private static string[] kTextureTypes = new string[]
		{
			"No Texture?: ",
			"1D texture: ",
			"Texture: ",
			"Volume: ",
			"Cubemap: ",
			"Any texture: "
		};
		/*
		private static string[] kShaderLevels = new string[]
		{
			"Fixed function",
			"SM1.x",
			"SM2.0",
			"SM3.0",
			"SM4.0",
			"SM5.0"
		};
		*/
		private static string GetPropertyType( Shader s, int index ) {
			ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType( s, index );
			if( propertyType == ShaderUtil.ShaderPropertyType.TexEnv ) {
				return ShaderForgeInspector.kTextureTypes[(int)ShaderUtil.GetTexDim( s, index )];
			}
			return ShaderForgeInspector.kPropertyTypes[(int)propertyType];
		}





		

		void OnEnable() {
			AnalyzeShader();
		}



		



		public bool hasShaderForgeData = false;

		public void AnalyzeShader() {
			hasShaderForgeData = SF_Parser.ContainsShaderForgeData(base.target as Shader);
		}


		public override void OnInspectorGUI() {
			GUI.enabled = true;
			Shader shader = base.target as Shader;
			
			
			if(!SF_Tools.CanRunShaderForge()){
				SF_Tools.UnityOutOfDateGUI();
				return;
			}

			//EditorGUILayout.InspectorTitlebar( false, base.target );


			if( hasShaderForgeData ) {
				if( GUILayout.Button( "Open in Shader Forge" ) ) {
					if(Event.current.rawType != EventType.mouseDown)
						SF_Editor.Init( shader );
				}
			} else {
				GUILayout.BeginHorizontal();
				{
					//GUILayout.Label(SF_Styles.IconWarningSmall,GUILayout.Width(18),GUILayout.Height(18));
					GUI.color = Color.gray;
					GUILayout.Label( "No Shader Forge data found!", EditorStyles.miniLabel );
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
				//GUILayout.Label( "Opening this will clear the shader", EditorStyles.miniLabel );
				//GUI.color = new Color( 1f, 0.8f, 0.8f );
				if( GUILayout.Button( new GUIContent( "Replace with Shader Forge shader", SF_Styles.IconWarningSmall, "This will erase any existing shader code" ), hasShaderForgeData ? "Button" : "MiniButton" ) ) {
					if( SF_GUI.AcceptedNewShaderReplaceDialog() )
						SF_Editor.Init( shader );
				}
				//GUI.color = Color.white;
				
			}
			


			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = hasShaderForgeData ? "MiniButton" : "Button";
					if( GUILayout.Button( "Open source", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					if( GUILayout.Button( "Open compiled", btnStyle ) ) {
						OpenCompiledShader( shader );
					}
				}
				GUILayout.EndHorizontal();
			}
			//EditorGUIUtility.LookLikeInspector();
		}


		


		public static void OpenCompiledShader(Shader s) {
			Type shaderUtil = Type.GetType( "UnityEditor.ShaderUtil,UnityEditor" );
			BindingFlags bfs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo ocs = shaderUtil.GetMethod( "OpenCompiledShader", bfs );
			ocs.Invoke( null, new object[] { s } );
		}

	}
}
