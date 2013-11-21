using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class BuiltInResourcesWindow : EditorWindow {
	[MenuItem( "Window/Built-in styles and icons" )]
	public static void ShowWindow() {
		EditorWindow.GetWindow<BuiltInResourcesWindow>().Show();
	}

	private List<Object> _objects;
	private Vector2 _scrollPos;
	void OnGUI() {
		_scrollPos = GUILayout.BeginScrollView( _scrollPos );

		GUILayout.Label( "Styles", EditorStyles.whiteLargeLabel );
		GUILayout.Label( "  usage: GUIStyle myStyle = (GUIStyle)\"name of style\";" );
		GUILayout.Space( 16 );

		foreach( GUIStyle ss in GUI.skin.customStyles ) {
			GUILayout.BeginHorizontal();
			GUILayout.Space( 16 );
			GUILayout.Label( ss.name, GUILayout.Width( 200 ) );
			GUILayout.Toggle( false, "inactive", ss, GUILayout.Width( 64 ), GUILayout.Height( 64 ) );
			GUILayout.Space( 16 );
			GUILayout.Toggle( true, "active", ss, GUILayout.Width( 64 ), GUILayout.Height( 64 ) );
			GUILayout.EndHorizontal();
			/*
			if( GUILayoutUtility.GetLastRect().yMin > _scrollPos.y+Screen.height ) {
				Debug.Log("breakin");
				GUILayout.EndScrollView();
				return;
			}*/
		}

		GUILayout.Space( 16 );
		GUILayout.Label( "Icons", EditorStyles.whiteLargeLabel );
		GUILayout.Label( "  usage: EditorGUIUtility.FindTexture( \"name of icon\" );" );
		GUILayout.Space( 16 );

		if( _objects == null ) {
			_objects = new List<Object>( Resources.FindObjectsOfTypeAll( typeof( Texture2D ) ) );
			_objects.Sort( ( pA, pB ) => System.String.Compare( pA.name, pB.name, System.StringComparison.OrdinalIgnoreCase ) );
		}
		/*
		foreach( Object oo in _objects ) {
			Texture2D texture = (Texture2D)oo;

			if( texture.name == "" )
				continue;

			GUILayout.BeginHorizontal();
			GUILayout.Space( 16 );
			GUILayout.Label( texture.name + " (" + texture.width + " x " + texture.height + ")", GUILayout.MinWidth( 300 ) );
			GUILayout.Space( 16 );
			Rect r = GUILayoutUtility.GetRect( texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight( false ), GUILayout.ExpandWidth( false ) );
			EditorGUI.DrawTextureTransparent( r, texture );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if( GUILayoutUtility.GetLastRect().yMin > _scrollPos.y + Screen.height ) {
				Debug.Log( "breakin" );
				GUILayout.EndScrollView();
				return;
			}
		}*/

		GUILayout.EndScrollView();
	}
}