using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SF_Settings {

	public const string prefix = "shaderforge_";


	public SF_Settings() {
		// Set up all defaults

	}





	// --------------------------------------------------

	public bool LoadBool(string key) {
		return EditorPrefs.GetBool( key, GetDefaultBool( key ) );
	}
	public string LoadString( string key ) {
		return EditorPrefs.GetString( key, GetDefaultString( key ) );
	}
	public int LoadInt( string key ) {
		return EditorPrefs.GetInt( key, GetDefaultInt( key ) );
	}
	public float LoadFloat( string key ) {
		return EditorPrefs.GetFloat( key, GetDefaultFloat( key ) );
	}


	// --------------------------------------------------

	private bool GetDefaultBool( string s ) {
		return EditorPrefs.GetBool( FormatDefault( s ) );
	}
	private string GetDefaultString( string s ) {
		return EditorPrefs.GetString( FormatDefault( s ) );
	}
	private int GetDefaultInt( string s ) {
		return EditorPrefs.GetInt( FormatDefault( s ) );
	}
	private float GetDefaultFloat( string s ) {
		return EditorPrefs.GetFloat( FormatDefault( s ) );
	}

	// --------------------------------------------------




	private string Format( string s ) {
		return prefix + s;
	}

	private string FormatDefault( string s ) {
		return prefix + s + "_default";
	}
		
		
}
