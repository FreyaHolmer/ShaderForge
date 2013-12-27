using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShaderForge {

	[System.Serializable]
	public enum RenderPlatform {
		d3d9,	// - Direct3D 9
		d3d11,	// - Direct3D 11
		opengl,	// - OpenGL
		gles,	// - OpenGL ES 2.0
		xbox360,// - Xbox 360
		ps3,	// - PlayStation 3
		flash	// - Flash
	};
	


	public static class SF_Tools {

		// Versioning
		public static int versionNumPrimary = 0;
		public static int versionNumSecondary = 17;
		public static string versionStage = "Beta";
		public static string version = versionNumPrimary + "." + versionNumSecondary.ToString( "D2" );
		public static string versionString = "Shader Forge " + versionStage + " " + version;


		// Misc strings
		public const string bugReportLabel = "Post bugs & ideas";
		public const string bugReportURL = "https://shaderforge.userecho.com/";
		public const string featureListLabel = "Feature list";
		public const string featureListURL = "https://docs.google.com/spreadsheet/ccc?key=0AqHpAiSNy9eDdGRaQ2hIbHpLVWtfbURSb1VRTlF0NGc&usp=sharing";
		public const string manualLabel = "Manual & ToS";
		public const string manualURL = "https://dl.dropboxusercontent.com/u/99714/Unity%20Projects/Shader%20Forge/Documents/ShaderForgeTesterGuide.html";
		public static string[] rendererLabels = new string[]{
			"Direct3D 9",
			"Direct3D 11",
			"OpenGL",
			"OpenGL ES 2.0",
			"Xbox 360",
			"PlayStation 3",
			"Flash"
		};


		// Constants
		public const int connectorMargin = 4;

		// User prefs
		public static bool absColor = true;
		public static bool advancedInspector = true;
		public static int stationaryCursorRadius = 7;
		
		
		public const string minimumUnityVersion = "4.2";
		
		public static bool CanRunShaderForge(){	
		#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_1
			return false;
		#else
			return true;
		#endif
		}
		
		
		
		public static void UnityOutOfDateGUI(){
			GUIStyle st = new GUIStyle(EditorStyles.boldLabel);
			
			st.alignment = TextAnchor.MiddleCenter;
			
			GUILayout.Label(string.Empty);
			GUILayout.Label("You need to install Unity " + SF_Tools.minimumUnityVersion + " or later in order to use Shader Forge",st);
			GUILayout.Label("You are currently running Unity version " + Application.unityVersion, st);
			if(GUILayout.Button("Update Unity")){
				Application.OpenURL("http://unity3d.com/unity/download");
			}
			GUILayout.Label(string.Empty);
		}
		
		

		public static Color VectorToColor( float v ) {
			if( absColor )
				v = Mathf.Abs(v);
			return new Color( v,v,v );
		}

		public static Color VectorToColor( Vector2 v ) {
			if( absColor )
				return new Color( Mathf.Clamp01( Mathf.Abs( v.x ) ), Mathf.Clamp01( Mathf.Abs( v.y ) ), 0f );
			else
				return new Color( Mathf.Clamp01( v.x ), Mathf.Clamp01( v.y ), 0f );
		}

		public static Color VectorToColor( Vector3 v ) {
			if( absColor )
				return new Color( Mathf.Clamp01( Mathf.Abs( v.x ) ), Mathf.Clamp01( Mathf.Abs( v.y ) ), Mathf.Clamp01( Mathf.Abs( v.z ) ) );
			else
				return new Color( Mathf.Clamp01( v.x ), Mathf.Clamp01( v.y ), Mathf.Clamp01( v.z ) );
		}

		public static string AssetToGUID( UnityEngine.Object asset ) {
			return AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( asset ) );
		}

		public static UnityEngine.Object GUIDToAsset( string GUID, Type type ) {
			return AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( GUID ), type );
		}

		public static float Smooth( float x ) {
			return x * x * ( 3f - 2f * x );
		}

		public static float Smoother( float x ) {
			return x * x * x * ( x * ( x * 6f - 15f ) + 10f );
		}

		

		public static string PathFromAbsoluteToProject(string s){
			return s.Substring(Application.dataPath.Length-6);
		}

		public static void LinkButton(Rect r, string label, string URL, GUIStyle style){
			if( GUI.Button( r, label, style ) )
				Application.OpenURL(URL);
		}


		public static void AssignShaderToMaterialAsset( ref Material m, Shader s ) {
			string path = AssetDatabase.GetAssetPath( m );
			AssetDatabase.DeleteAsset( path );
			AssetDatabase.CreateAsset( m = new Material( s ), path );
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
		}

		public static float[] VectorToArray(Vector4 vec) {
			return new float[4] { vec.x, vec.y, vec.z, vec.w };
		}

		public static float[] VectorToArray( float vec ) {
			return new float[4] { vec, vec, vec, vec };
		}

		public static Vector3 ToVector3(Color c) {
			return new Vector3( c.r, c.g, c.b );
		}

		public static Color ToColor( Vector3 vec ) {
			return new Color( vec.x, vec.y, vec.z);
		}

		public static float DistChebyshev(Vector2 a, Vector2 b) {
			return Mathf.Max( Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y) );
		}


		private static MethodInfo _doColorPickerMethod;
		private static MethodInfo DoColorPickerMethod {
			get{
				if(_doColorPickerMethod == null){
					Type t = Type.GetType( "UnityEditor.EditorGUI,UnityEditor" );
					Debug.Log("Type = " + t);
					BindingFlags bfs = BindingFlags.Static | BindingFlags.NonPublic;
					_doColorPickerMethod = t.GetMethod( "DoColorField", bfs, null, new Type[] { typeof( Rect ), typeof( int ), typeof( Color ), typeof( bool ), typeof( bool ) }, null );
				}
				return _doColorPickerMethod;
			}
		}


		// private static Color DoColorField(Rect position, int id, Color value, bool showEyedropper, bool showAlpha)
		public static Color DoColorPicker( Rect position, Color color, bool showEyedropper, bool showAlpha ) {
			//int id = GUIUtility.GetControlID();
			//return (Color)DoColorPickerMethod.Invoke( null, new object[] { position, id, color, showEyedropper, showAlpha } );
			return Color.red;
		}

		public static Color FloatToColor( float f ) {
			return new Color( f, f, f, f );
		}

		public static float Distance( Color a, Color b ) {
			Color v = a - b;
			return Mathf.Sqrt( Dot( v, v ) );
		}

		public static float Dot(Color a, Color b) {
			return ( a.r * b.r + a.g * b.g + a.b * b.b/* + a.a * b.a*/ );
		}

		public static Color Cross( Color a, Color b ) {
			return ToColor( Vector3.Cross( ToVector3( a ), ToVector3( b ) ) );
		}

		public static float Frac( float x ) {
			return x - Mathf.Floor( x );
		}


		public static Rect Encapsulate( Rect s, Rect i ) {
			if( i.xMax > s.xMax )
				s.xMax = i.xMax;
			if( i.xMin < s.xMin )
				s.xMin = i.xMin;
			if( i.yMax > s.yMax )
				s.yMax = i.yMax;
			if( i.yMin < s.yMin )
				s.yMin = i.yMin;
			return s;
		}


		public static void FormatShaderPath(ref string s){
			Regex rgx = new Regex( "[^a-zA-Z0-9/s -_]" ); // Only allow Alphanumeric, forward slash, space, dash and underscore
			s = rgx.Replace( s, "" );
		}

		public static void FormatSerializableComment(ref string s){
			Regex rgx = new Regex( "[^\\w\\s_\\?\\.-]" ); // Only allow Alphanumeric, dot, dash, underscore and questionmark
			s = rgx.Replace( s, "" );
		}

		public static void FormatSerializable( ref string s ) {
			s = s.Replace( ":", "" )
				 .Replace( ";", "" )
				 .Replace( ",", "" )
				 .Replace( "/*", "" )
				 .Replace( "*/", "" );
		}


		public static Rect GetExpanded( Rect r, float px ) {
			r.y -= px;
			r.x -= px;
			r.width += 2 * px;
			r.height += 2 * px;
			return r;
		}


		public static bool Intersects( Rect a, Rect b ) {
			FlipNegative( ref a );
			FlipNegative( ref b );
			bool c1 = a.xMin < b.xMax;
			bool c2 = a.xMax > b.xMin;
			bool c3 = a.yMin < b.yMax;
			bool c4 = a.yMax > b.yMin;
			return c1 && c2 && c3 && c4;
		}

		public static void FlipNegative(ref Rect r) {
			if( r.width < 0 ) 
				r.x -= ( r.width *= -1 );
			if( r.height < 0 )
				r.y -= ( r.height *= -1 );
		}


	}
}
