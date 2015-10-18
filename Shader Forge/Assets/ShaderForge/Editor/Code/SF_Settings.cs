using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;


namespace ShaderForge {

	public enum SF_Setting{
		CurveShape,				// int	Bezier/Linear/etc
		AutoCompile, 			// bool	True/False
		HierarchalNodeMove, 	// bool	True/False
		//DrawNodePreviews,		// bool	True/False
		QuickPickScrollWheel,	// bool	True/False
		ControlMode,			// int	Shader Forge / Unity / Unreal
		ShowVariableSettings,	// bool True/False
		ShowNodeSidebar,		// bool True/False
		NodeRenderMode			// int Mixed / MixedRealtime / Spheres / SpheresRealtime / ViewportRealtime
	};

	public enum ControlMode { ShaderForge, UnityMaya, Unreal };
	public enum NodeRenderMode { Mixed, MixedRealtime, Spheres, SpheresRealtime, ViewportRealtime };

	public class SF_Settings {

		public const string prefix = "shaderforge_";
		public const string suffixDefault = "_default";

		public SF_Settings() {

		}

		public static void InitializeSettings() {
			// Set up all defaults
			SetDefaultBool( SF_Setting.HierarchalNodeMove, 		false 									);
			SetDefaultBool( SF_Setting.QuickPickScrollWheel,	true 									);
			SetDefaultBool( SF_Setting.ShowVariableSettings,	false									);
			SetDefaultBool( SF_Setting.ShowNodeSidebar, 		true									);
			SetDefaultInt ( SF_Setting.NodeRenderMode,			(int)NodeRenderMode.ViewportRealtime	);
		}


		// Cached, for speed
		public static bool autoCompile;
		public static bool hierarchalNodeMove;
		public static bool quickPickScrollWheel;
		public static bool showVariableSettings;
		public static bool showNodeSidebar;
		public static NodeRenderMode nodeRenderMode;

		// These two are called in OnEnable and OnDisable in SF_Editor
		public static void LoadAllFromDisk() {
			autoCompile				= LoadBool( SF_Setting.AutoCompile );
			hierarchalNodeMove		= LoadBool( SF_Setting.HierarchalNodeMove );
			quickPickScrollWheel	= LoadBool( SF_Setting.QuickPickScrollWheel );
			showVariableSettings	= LoadBool( SF_Setting.ShowVariableSettings );
			showNodeSidebar			= LoadBool( SF_Setting.ShowNodeSidebar );
			nodeRenderMode			= (NodeRenderMode)LoadInt( SF_Setting.NodeRenderMode );
		}
		public static void SaveAllToDisk() {
			SaveBool( SF_Setting.AutoCompile, autoCompile );
			SaveBool( SF_Setting.HierarchalNodeMove, hierarchalNodeMove );
			SaveBool( SF_Setting.QuickPickScrollWheel, quickPickScrollWheel );
			SaveBool( SF_Setting.ShowVariableSettings, showVariableSettings );
			SaveBool( SF_Setting.ShowNodeSidebar, showNodeSidebar );
			SaveInt( SF_Setting.NodeRenderMode, (int)nodeRenderMode );
		}



		// --------------------------------------------------
		public static bool LoadBool( SF_Setting setting ) {
			string key = KeyOf(setting);
			return EditorPrefs.GetBool( key, EditorPrefs.GetBool( key + suffixDefault ) );
		}
		public static string LoadString( SF_Setting setting ) {
			string key = KeyOf(setting);
			return EditorPrefs.GetString( key, EditorPrefs.GetString( key + suffixDefault ) );
		}
		public static int LoadInt( SF_Setting setting ) {
			string key = KeyOf(setting);
			return EditorPrefs.GetInt( key, EditorPrefs.GetInt( key + suffixDefault) );
		}
		public static float LoadFloat( SF_Setting setting ) {
			string key = KeyOf(setting);
			return EditorPrefs.GetFloat( key, EditorPrefs.GetFloat( key + suffixDefault) );
		}
		// --------------------------------------------------
		private static string KeyOf( SF_Setting setting ){
			return prefix + setting.ToString();
		}
		// --------------------------------------------------
		private static void SetDefaultBool( SF_Setting setting, bool value ){
			string key = KeyOf(setting);
			EditorPrefs.SetBool(key + suffixDefault, value);
			if(!EditorPrefs.HasKey(key)){
				SaveBool(setting, value);
			}
		}
		private static void SetDefaultString(SF_Setting setting, string value){
			string key = KeyOf(setting);
			EditorPrefs.SetString(key + suffixDefault, value);
			if(!EditorPrefs.HasKey(key)){
				SaveString(setting, value);
			}
		}
		private static void SetDefaultInt(SF_Setting setting, int value){
			string key = KeyOf(setting);
			EditorPrefs.SetInt(key + suffixDefault, value);
			if(!EditorPrefs.HasKey(key)){
				SaveInt(setting, value);
			}
		}
		private static void SetDefaultFloat(SF_Setting setting, float value){
			string key = KeyOf(setting);
			EditorPrefs.SetFloat(key + suffixDefault, value);
			if(!EditorPrefs.HasKey(key)){
				SaveFloat(setting, value);
			}
		}
		// --------------------------------------------------
		public static void SaveBool( SF_Setting setting, bool value ){
			string key = KeyOf(setting);
			EditorPrefs.SetBool(key, value);
		}
		public static void SaveString(SF_Setting setting, string value){
			string key = KeyOf(setting);
			EditorPrefs.SetString(key, value);
		}
		public static void SaveInt(SF_Setting setting, int value){
			string key = KeyOf(setting);
			EditorPrefs.SetInt(key, value);
		}
		public static void SaveFloat(SF_Setting setting, float value){
			string key = KeyOf(setting);
			EditorPrefs.SetFloat(key, value);
		}

	}

}
