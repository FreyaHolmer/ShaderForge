using System;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;
using ShaderForge;
using System.Globalization;
using System.Linq;

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



		static Type sipp;
		// static ConstructorInfo newSipp;
		static PropertyInfo sippCurrentMode;
		static PropertyInfo sippCurrentPlatformMask;
		static PropertyInfo sippCurrentVariantStripping;

		static Type sutil;
		static MethodInfo sutilHasShadowCasterPass;
		static MethodInfo sutilGetRenderQueue;
		static MethodInfo sutilGetLOD;
		static MethodInfo sutilDoesIgnoreProjector;
		static MethodInfo sutilHasSurfaceShaders;
		static MethodInfo sutilHasShaderSnippets;
		static MethodInfo sutilOpenParsedSurfaceShader;
		static MethodInfo sutilOpenCompiledShader;
		static MethodInfo sutilGetShaderErrorCount;
		static MethodInfo sutilGetShaderErrors;

		static Type shinsp;
		static MethodInfo shinspGetErrorListUI;
		

		//static Type disableBatchingType;

		static PropertyInfo shaderDisableBatching;

		// static MethodInfo editorGUIMouseButtonDown;

		// static PropertyInfo guiLayoutUtilityTopLevel;
		// static MethodInfo guilayoutgroupGetLast;

		static MethodInfo editorGuiUtilityLoadIcon;
		static MethodInfo editorGuiUtilityTextContent;



		void OnEnable() {
			AnalyzeShader();

			// Reflect some things
			sipp = Type.GetType( "UnityEditor.ShaderInspectorPlatformsPopup, UnityEditor" );
			// newSipp = sipp.GetConstructor( new Type[] { typeof( string ) } );
			BindingFlags bfs = BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty;
			BindingFlags privStatic = BindingFlags.NonPublic | BindingFlags.Static;
			sippCurrentMode = sipp.GetProperty( "currentMode", bfs );
			sippCurrentPlatformMask = sipp.GetProperty( "currentPlatformMask", bfs );
			sippCurrentVariantStripping = sipp.GetProperty( "currentVariantStripping", bfs );

			shinsp = Type.GetType( "UnityEditor.ShaderInspector, UnityEditor" );
			shinspGetErrorListUI = shinsp.GetMethod( "ShaderErrorListUI", privStatic );

			sutil = Type.GetType( "UnityEditor.ShaderUtil, UnityEditor" );
			sutilHasShadowCasterPass = sutil.GetMethod( "HasShadowCasterPass", privStatic );
			sutilGetRenderQueue = sutil.GetMethod( "GetRenderQueue", privStatic );
			sutilGetLOD = sutil.GetMethod( "GetLOD", privStatic );
			sutilDoesIgnoreProjector = sutil.GetMethod( "DoesIgnoreProjector", privStatic );
			sutilHasSurfaceShaders = sutil.GetMethod( "HasSurfaceShaders", privStatic );
			sutilHasShaderSnippets = sutil.GetMethod( "HasShaderSnippets", privStatic );
			sutilOpenParsedSurfaceShader = sutil.GetMethod( "OpenParsedSurfaceShader", privStatic );
			sutilOpenCompiledShader = sutil.GetMethod( "OpenCompiledShader", privStatic );
			sutilGetShaderErrorCount = sutil.GetMethod( "GetShaderErrorCount", privStatic );
			sutilGetShaderErrors = sutil.GetMethod( "GetShaderErrors", privStatic );

			//BindingFlags priv = BindingFlags.NonPublic;
			shaderDisableBatching = typeof( Shader ).GetProperty( "disableBatching", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty );

			// disableBatchingType = Type.GetType( "UnityEngine.DisableBatchingType, UnityEngine" );

			// editorGUIMouseButtonDown = typeof( EditorGUI ).GetMethod( "ButtonMouseDown", privStatic, null, new Type[]{typeof(Rect),typeof(GUIContent),typeof(FocusType),typeof(GUIStyle)}, null );

			// Type guilaGroup = Type.GetType( "UnityEngine.GUILayoutGroup, UnityEngine" );
			// guilayoutgroupGetLast = guilaGroup.GetMethod( "GetLast", BindingFlags.Instance | BindingFlags.Public );
			// guiLayoutUtilityTopLevel = typeof( GUILayoutUtility ).GetProperty( "topLevel", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty );
			editorGuiUtilityLoadIcon = typeof( EditorGUIUtility ).GetMethod( "LoadIcon", privStatic );
			editorGuiUtilityTextContent = typeof( EditorGUIUtility ).GetMethod( "TextContent", privStatic );
		}



		void InitStyles() {
			Styles.errorIcon = (Texture2D)editorGuiUtilityLoadIcon.Invoke( null, new object[] { "console.erroricon.sml" } ); // EditorGUIUtility.LoadIcon( "console.erroricon.sml" );
			Styles.warningIcon = (Texture2D)editorGuiUtilityLoadIcon.Invoke( null, new object[] { "console.warnicon.sml" } ); // EditorGUIUtility.LoadIcon( "console.warnicon.sml" );
			Styles.showSurface = (GUIContent)editorGuiUtilityTextContent.Invoke( null, new object[] { "Show generated code|Show generated code of a surface shader" } ); // EditorGUIUtility.TextContent( "Show generated code|Show generated code of a surface shader" );
			Styles.showCurrent = (GUIContent)editorGuiUtilityTextContent.Invoke( null, new object[] { "Compile and show code | ▾" } ); // new GUIContent( "Compile and show code | ▾" );
			Styles.messageStyle = "CN StatusInfo";
			Styles.evenBackground = "CN EntryBackEven";
			Styles.no = (GUIContent)editorGuiUtilityTextContent.Invoke( null, new object[] { "no" } ); // EditorGUIUtility.TextContent( "no" );
			Styles.builtinSurfaceShader = (GUIContent)editorGuiUtilityTextContent.Invoke( null, new object[] { "Built-in surface shader" } ); // EditorGUIUtility.TextContent( "Built-in surface shader" );
			Styles.initialized = true;
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
					if(Event.current.rawType != EventType.MouseDown)
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
					if( SF_GUI.AcceptedNewShaderReplaceDialog() ) {
						SF_Editor.Init( shader );
						SF_Editor.instance.ps.fChecker.UpdateAvailability();
						SF_Editor.instance.OnShaderModified( NodeUpdateType.Hard );
					}
					
				}
				//GUI.color = Color.white;
				
			}
			


			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = hasShaderForgeData ? "MiniButton" : "Button";
					if( GUILayout.Button( "Open shader code", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					//if( GUILayout.Button( "Open compiled", btnStyle ) ) {
					//	OpenCompiledShader( shader );
					//}
				}
				GUILayout.EndHorizontal();
			}

			DrawUnitysInspector();

		}


		


		public static void OpenCompiledShader(Shader s) {
			
			Type shaderUtil = Type.GetType( "UnityEditor.ShaderUtil,UnityEditor" );
			BindingFlags bfs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo ocs = shaderUtil.GetMethod( "OpenCompiledShader", bfs );

			string vStr = Application.unityVersion;

			int vMajor = int.Parse(""+vStr[0]);
			int vMinor = int.Parse(""+vStr[2]);

			float vFloat = vMajor + vMinor/10f;

			bool newMethod = vFloat >= 4.5f;

			if(newMethod)
				ocs.Invoke( null, new object[] { s, true } );
			else
				ocs.Invoke( null, new object[] { s } );
		}







		private Vector2 m_ScrollPosition = Vector2.zero;


		public void DrawUnitysInspector() {

			if( !Styles.initialized ) {
				InitStyles();
			}

			Shader shader = this.target as Shader;
            if (shader == null)
            {
                    return;
            }
            GUI.enabled = true;
            EditorGUI.indentLevel = 0;
            this.ShowShaderCodeArea(shader);
            if (shader.isSupported)
            {
                    EditorGUILayout.LabelField("Cast shadows", (!(bool)sutilHasShadowCasterPass.Invoke(null, new object[]{shader})) ? "no" : "yes", new GUILayoutOption[0]);
					EditorGUILayout.LabelField( "Render queue", ((int)sutilGetRenderQueue.Invoke( null, new object[] { shader } )).ToString( CultureInfo.InvariantCulture ), new GUILayoutOption[0] );
					EditorGUILayout.LabelField( "LOD", ( (int)sutilGetLOD.Invoke( null, new object[] { shader } ) ).ToString( CultureInfo.InvariantCulture ), new GUILayoutOption[0] );
					EditorGUILayout.LabelField( "Ignore projector", ( !(bool)sutilDoesIgnoreProjector.Invoke( null, new object[] { shader } ) ) ? "no" : "yes", new GUILayoutOption[0] );
                    string label;
					switch( (int)shaderDisableBatching.GetValue( shader, null ) )
                    {
                    case 0:
                            label = "no";
                            break;
                    case 1:
                            label = "yes";
                            break;
                    case 2:
                            label = "when LOD fading is on";
                            break;
                    default:
                            label = "unknown";
                            break;
                    }
                    EditorGUILayout.LabelField("Disable batching", label, new GUILayoutOption[0]);
                    ShowShaderProperties(shader);
            }
        }
        private void ShowShaderCodeArea(Shader s) {
                ShowSurfaceShaderButton(s);
                this.ShowCompiledCodeButton(s);
                this.ShowShaderErrors(s);
        }
		private void ShowShaderErrors( Shader s ) {
			ShaderMessage[] messages = ShaderUtil.GetShaderMessages( s );
			if( messages.Length > 0 ) {
				this.m_ScrollPosition = GUILayout.BeginScrollView( this.m_ScrollPosition, false, false );
				foreach( ShaderMessage msg in messages ) {
					using( new GUILayout.VerticalScope( EditorStyles.helpBox ) ) {
						Texture2D icon = msg.severity == Rendering.ShaderCompilerMessageSeverity.Error ? SF_Styles.IconErrorSmall : SF_Styles.IconWarningSmall;
						GUILayout.Label( new GUIContent( $"{msg.message} at line {msg.line} on {msg.platform}", icon ) );
					}
				}
				GUILayout.EndScrollView();
			}
		}
        private void ShowShaderProperties(Shader s)
        {
                GUILayout.Space(5f);
                GUILayout.Label("Properties:", EditorStyles.boldLabel, new GUILayoutOption[0]);
                int propertyCount = ShaderUtil.GetPropertyCount(s);
                for (int i = 0; i < propertyCount; i++)
                {
                        string propertyName = ShaderUtil.GetPropertyName(s, i);
                        string label = GetPropertyType(s, i) + ShaderUtil.GetPropertyDescription(s, i);
                        EditorGUILayout.LabelField(propertyName, label, new GUILayoutOption[0]);
                }
        }

        private void ShowCompiledCodeButton(Shader s)
        {
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Compiled code", EditorStyles.miniButton);
			bool flag = (bool)sutilHasShaderSnippets.Invoke( null, new object[] { s } ) || (bool)sutilHasSurfaceShaders.Invoke( null, new object[] { s } );
			if (flag)
			{
				GUIContent showCurrent = Styles.showCurrent;
				Rect rect = GUILayoutUtility.GetRect(showCurrent, EditorStyles.miniButton, new GUILayoutOption[]
				{
						GUILayout.ExpandWidth(false)
				});

				/* This is super annoying, and I'm not entirely sure what it's used for. Inspecting compiled code of multiple things? Not sure
				Rect position = new Rect(rect.xMax - 16f, rect.y, 16f, rect.height);
				bool mDown = (bool)editorGUIMouseButtonDown.Invoke( null, new object[] { position, GUIContent.none, FocusType.Passive, GUIStyle.none } );
				// EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none)
				if( mDown )
				{
					Rect last = (Rect)guilayoutgroupGetLast.Invoke( guiLayoutUtilityTopLevel.GetValue( null, null ), null ); //GUILayoutUtility.topLevel.GetLast();
					PopupWindow.Show( last, (PopupWindowContent)newSipp.Invoke(null, new object[]{s}) );
					GUIUtility.ExitGUI();
				}*/

				if (GUI.Button(rect, showCurrent, EditorStyles.miniButton))
				{
					sutilOpenCompiledShader.Invoke( null, new object[] { s, sippCurrentMode.GetValue( null, null ), sippCurrentPlatformMask.GetValue( null, null ), (int)sippCurrentVariantStripping.GetValue( null, null ) == 0 } );
					//ShaderUtil.OpenCompiledShader( s, sippCurrentMode.GetValue( null, null ), sippCurrentPlatformMask.GetValue( null, null ), (int)sippCurrentVariantStripping.GetValue( null, null ) == 0 );
					GUIUtility.ExitGUI();
				}
			}
			else
			{
					GUILayout.Button("none (fixed function shader)", GUI.skin.label, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
        }
        private static void ShowSurfaceShaderButton(Shader s)
        {
				bool flag = (bool)sutilHasSurfaceShaders.Invoke( null, new object[] { s } );
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.PrefixLabel("Surface shader", EditorStyles.miniButton);
                if (flag)
                {
                        if (!(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s)) == null))
                        {
                                if (GUILayout.Button(Styles.showSurface, EditorStyles.miniButton, new GUILayoutOption[]
                                {
                                        GUILayout.ExpandWidth(false)
                                }))
                                {
									sutilOpenParsedSurfaceShader.Invoke(null, new object[]{s});
									GUIUtility.ExitGUI();
                                }
                        }
                        else
                        {
                                GUILayout.Button(Styles.builtinSurfaceShader, GUI.skin.label, new GUILayoutOption[0]);
                        }
                }
                else
                {
                        GUILayout.Button(Styles.no, GUI.skin.label, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndHorizontal();
        }


		class Styles {
			public static bool initialized = false;
			public static Texture2D errorIcon;
			public static Texture2D warningIcon;
			public static GUIContent showSurface;
			public static GUIContent showCurrent;
			public static GUIStyle messageStyle;
			public static GUIStyle evenBackground;
			public static GUIContent no;
			public static GUIContent builtinSurfaceShader;
		}


	}


}
