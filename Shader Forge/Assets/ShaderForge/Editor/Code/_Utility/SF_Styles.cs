using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace ShaderForge {
	public static class SF_Styles {

		private static Texture2D _iconErrorSmall;
		public static Texture2D IconErrorSmall {
			get {
				return ( _iconErrorSmall = _iconErrorSmall ?? EditorGUIUtility.FindTexture( "console.erroricon.sml" ) );
			}
		}
		private static Texture2D _iconWarningSmall;
		public static Texture2D IconWarningSmall {
			get {
				return ( _iconWarningSmall = _iconWarningSmall ?? EditorGUIUtility.FindTexture( "console.warnicon.sml" ) );
			}
		}




		private static GUIStyle largeTextField;
		public static GUIStyle LargeTextField {
			get {
				if( largeTextField == null ) {
					largeTextField = new GUIStyle( EditorStyles.textField );
					//largeTextField.fontStyle = FontStyle.Bold;
					largeTextField.fontSize = 20;
					largeTextField.alignment = TextAnchor.MiddleLeft;

				}
				return largeTextField;
			}
		}


		private static GUIStyle miniLabelOverflow;
		public static GUIStyle MiniLabelOverflow {
			get {
				if( miniLabelOverflow == null ) {
					miniLabelOverflow = new GUIStyle( EditorStyles.miniLabel );
					miniLabelOverflow.clipping = TextClipping.Overflow;
				}
				return miniLabelOverflow;
			}
		}


		private static GUIStyle nodeNameLabelText;
		public static GUIStyle GetNodeNameLabelText() {
			if( nodeNameLabelText == null ) {
				nodeNameLabelText = new GUIStyle( EditorStyles.largeLabel );
				nodeNameLabelText.fontStyle = FontStyle.Bold;
				nodeNameLabelText.fontSize = 11;
				nodeNameLabelText.alignment = TextAnchor.MiddleCenter;
				nodeNameLabelText.normal.textColor = new Color( 0.8f, 0.8f, 0.8f, 1f );
			}
			return nodeNameLabelText;
		}


		private static GUIStyle nodeNameLabelBackground;
		public static Color nodeNameLabelBackgroundColor = new Color( 0.7f, 0.7f, 0.7f );
		public static GUIStyle GetNodeNameLabelBackground() {
			if( nodeNameLabelBackground == null ) {
				nodeNameLabelBackground = new GUIStyle( EditorStyles.textField );
			}
			return nodeNameLabelBackground;
		}

		private static GUIStyle highlightStyle;
		public static GUIStyle HighlightStyle {
			get {
				if( highlightStyle == null ) {
					//if( Application.unityVersion.StartsWith("4") )
					highlightStyle = new GUIStyle( (GUIStyle)"flow node 0 on" );
				}
				return highlightStyle;
			}
		}


		private static GUIStyle selectionStyle;
		public static GUIStyle SelectionStyle {
			get {
				if( selectionStyle == null ) {
					//if( Application.unityVersion.StartsWith("4") )
					selectionStyle = new GUIStyle( (GUIStyle)"SelectionRect" );
				}
				return selectionStyle;
			}
		}

		private static GUIStyle nodeStyle;
		public static GUIStyle NodeStyle {
			get {
				if( nodeStyle == null ) {
					//if( Application.unityVersion.StartsWith( "4" ) )
					nodeStyle = new GUIStyle( (GUIStyle)"flow node 0" );
					nodeStyle.alignment = TextAnchor.UpperCenter;
					nodeStyle.fontSize = 11;
					nodeStyle.font = EditorStyles.standardFont;
					nodeStyle.fontStyle = FontStyle.Bold;
					nodeStyle.padding.top = 23;
					nodeStyle.padding.left = 1;
					//nodeStyle.margin.right = 8;
					//nodeStyle.border.right = 25;
					nodeStyle.border.left = 25;
					if(SF_GUI.ProSkin)
						nodeStyle.normal.textColor = new Color( 1f, 1f, 1f, 0.75f );
					else
						nodeStyle.normal.textColor = new Color( 0f, 0f, 0f, 0.7f );

				}

				return nodeStyle;
			}
		}



		// Thanks to Tenebrous!
		public static void ListStyles() {
			foreach( GUIStyle ss in GUI.skin.customStyles ) {
				GUILayout.Label( ss.name );
				EditorGUILayout.LabelField( ss.name, ss );
			}

			FieldInfo f = typeof( EditorGUIUtility ).GetField( "s_IconGUIContents", BindingFlags.NonPublic | BindingFlags.Static );
			Hashtable ff = (Hashtable)f.GetValue( null );
			foreach( DictionaryEntry fff in ff ) {
				GUILayout.Label( fff.Key.ToString() );
				GUILayout.Label( (GUIContent)fff.Value );
			}
		}



	}
}
