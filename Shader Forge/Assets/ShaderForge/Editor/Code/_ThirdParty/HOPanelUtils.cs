// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/26 12:25
//--------------------------
// Original author above
// Slightly modified by Joachim Holmér

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils {
	public class HOPanelUtils {
		static Dictionary<EditorWindow, GUIContent> _winTitleContentByEditor;

		public static T ConnectToSourceAsset<T>( string adbFilePath, bool createIfMissing = false ) where T : ScriptableObject {
			if( !HOFileUtils.AssetExists( adbFilePath ) ) {
				if( createIfMissing ) CreateScriptableAsset<T>( adbFilePath );
				else return null;
			}
			T source = (T)Resources.LoadAssetAtPath( adbFilePath, typeof( T ) );
			if( source == null ) {
				// Source changed (or editor file was moved from outside of Unity): overwrite it
				CreateScriptableAsset<T>( adbFilePath );
				source = (T)Resources.LoadAssetAtPath( adbFilePath, typeof( T ) );
			}
			return source;
		}

		public static void SetWindowTitle( EditorWindow editor, Texture icon, string title ) {
			GUIContent titleContent;
			if( _winTitleContentByEditor == null ) _winTitleContentByEditor = new Dictionary<EditorWindow, GUIContent>();
			if( _winTitleContentByEditor.ContainsKey( editor ) ) {
				titleContent = _winTitleContentByEditor[editor];
				if( titleContent != null ) {
					if( titleContent.image != icon ) titleContent.image = icon;
					if( title != null && titleContent.text != title ) titleContent.text = title;
					return;
				}
				_winTitleContentByEditor.Remove( editor );
			}
			titleContent = GetWinTitleContent( editor );
			if( titleContent != null ) {
				if( titleContent.image != icon ) titleContent.image = icon;
				if( title != null && titleContent.text != title ) titleContent.text = title;
				_winTitleContentByEditor.Add( editor, titleContent );
			}
		}

		// ===================================================================================
		// METHODS ---------------------------------------------------------------------------

		static void CreateScriptableAsset<T>( string adbFilePath ) where T : ScriptableObject {
			T data = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset( data, adbFilePath );
		}

		static GUIContent GetWinTitleContent( EditorWindow editor ) {
			const BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			PropertyInfo p = typeof( EditorWindow ).GetProperty( "cachedTitleContent", bFlags );
			if( p == null ) return null;
			return p.GetValue( editor, null ) as GUIContent;
		}
	}
}