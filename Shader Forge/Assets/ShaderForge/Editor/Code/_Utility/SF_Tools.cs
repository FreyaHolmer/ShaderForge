using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShaderForge {

	[System.Serializable]
	public enum RenderPlatform {
		d3d9 		= 0,	// - Direct3D 9
		d3d11 		= 1,	// - Direct3D 11 / 12
		glcore 		= 2,	// - OpenGL Core
		gles 		= 3,	// - OpenGL ES 2.0
		gles3		= 4,	// - OpenGL ES 3.0
		metal		= 5,	// - iOS Metal
		d3d11_9x 	= 6,	// - Direct3D 11 windows RT
		xboxone 	= 7,	// - Xbox One
		ps4 		= 8,	// - PlayStation 4
		psp2 		= 9,	// - PlayStation Vita
		n3ds		= 10,	// - Nintendo 3DS
		wiiu		= 11	// - Nintendo Wii U
	};
	
	

	public static class SF_Tools {

		// Versioning
		public static int versionNumPrimary = 1;
		public static int versionNumSecondary = 38;
		public static string versionStage = "";
		public static string version = versionNumPrimary + "." + versionNumSecondary.ToString( "D2" );
		public static string versionString = "Shader Forge v" + version;

		// Misc strings
		public const string bugReportLabel = "Post bugs & ideas";
		public const string bugReportURL = "https://shaderforge.userecho.com/";
		public const string documentationLabel = "Node Documentation";
		public const string documentationURL = "http://www.acegikmo.com/shaderforge/nodes/";
		public static string[] rendererLabels = new string[]{
			"Direct3D 9",
			"Direct3D 11 & 12",
			"OpenGL Core",
			"OpenGL ES 2.0",
			"OpenGL ES 3.0",
			"iOS Metal",
			"Direct3D 11 for Windows RT/Phone",
			"Xbox One",
			"PlayStation 4",
			"PlayStation Vita",
			"Nintendo 3DS",
			"Nintendo Wii U"
		};

		public const string alphabetLower = "abcdefghijklmnopqrstuvwxyz";
		public const string alphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


		// Constants
		public const int connectorMargin = 4;

		// User prefs
		public static bool absColor = false;
		public static bool advancedInspector = true;
		public static int stationaryCursorRadius = 7;
		
		
		public const float minimumUnityVersion = 5.3f;

		private static float currentUnityVersion = 0f;
		public static float CurrentUnityVersion{
			get{
				if(currentUnityVersion == 0f){
					currentUnityVersion = float.Parse(Application.unityVersion.Substring(0,3));
				}
				return currentUnityVersion;
			}
		}
		
		public static bool CanRunShaderForge(){
			return (CurrentUnityVersion >= minimumUnityVersion);
		}


		public static bool HasUnityPro(){
			return UnityEditorInternal.InternalEditorUtility.HasPro();
		}



		
		public static void UnityOutOfDateGUI(){
			GUIStyle st = new GUIStyle(EditorStyles.boldLabel);
			
			st.alignment = TextAnchor.MiddleCenter;
			
			GUILayout.Label(string.Empty);
			GUILayout.Label("You need to install Unity " + SF_Tools.minimumUnityVersion + " or later in order to use Shader Forge", st);
			GUILayout.Label("You are currently running Unity version " + Application.unityVersion, st);
			if(GUILayout.Button("Update Unity")){
				Application.OpenURL("http://unity3d.com/unity/download");
			}
			GUILayout.Label(string.Empty);
		}

		public static int ComponentCountOf(CustomValueType cvt){

			switch(cvt){
			case CustomValueType.Float:
				return 1;
			case CustomValueType.Half:
				return 1;
			case CustomValueType.Fixed:
				return 1;
			case CustomValueType.Float2:
				return 2;
			case CustomValueType.Half2:
				return 2;
			case CustomValueType.Fixed2:
				return 2;
			case CustomValueType.Float3:
				return 3;
			case CustomValueType.Half3:
				return 3;
			case CustomValueType.Fixed3:
				return 3;
			case CustomValueType.Float4:
				return 4;
			case CustomValueType.Half4:
				return 4;
			case CustomValueType.Fixed4:
				return 4;
			case CustomValueType.Sampler2D:
				return 4;
			default:
				// Debug.Log("Invalid component count check of custom value type: " + cvt);
				return 16;
			}


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
			m.shader = s;
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

		public static float Distance( Color a, Color b, int cc ) {
			Color v = a - b;
			return Mathf.Sqrt( Dot( v, v, cc ) );
		}

		public static float Dot(Color a, Color b, int compCount) {

			float retVal = 0f;

			for(int i=0;i<compCount;i++){
				retVal += a[i] * b[i];
			}

			return retVal;
		}

		public static float Dot( Vector4 a, Vector4 b, int compCount ) {

			float retVal = 0f;

			for( int i=0; i < compCount; i++ ) {
				retVal += a[i] * b[i];
			}

			return retVal;
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

		public static void FormatAlphanumeric( ref string s ) {
			Regex rgx = new Regex( "[^a-zA-Z0-9-_]" ); // Only allow Alphanumeric, forward slash, space, dash and underscore
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
				 .Replace( "*/", "" )
				 .Replace("\"", "");
		}

		public static void FormatSerializableVarName( ref string s ){
			FormatShaderPath(ref s);

			s = s.Replace(" ", string.Empty);

			if(s.Length > 0){

				int tmp;
				while(s.Length > 0 && int.TryParse(s[0].ToString(), out tmp)){
					s = s.Substring(1, s.Length-1); // Remove first character if first is a parsable integer
				}

//				if(s.Length == 1){
//					s = s.ToLower(); // Lowercase the one character
//				} else {
//					char first = s[0]; // Lowercase the first character
//					string rest = s.Substring(
//				}
			}

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


		public static float DistanceToLines(Vector2 point, Vector2[] line){
			float shortest = float.MaxValue;

			for (int i = 0; i < line.Length-1; i++) {
				shortest = Mathf.Min(shortest, DistanceToLine(line[i], line[i+1], point));
			}

			return shortest;

		}

		public static float DistanceToLine(Vector2 a, Vector2 b, Vector2 point){
			// Return minimum distance between line segment vw and point p
			float l2 = Vector2.SqrMagnitude(a - b);  // i.e. |w-v|^2 -  avoid a sqrt
			if (l2 == 0.0)
				return Vector2.Distance(point, a);   // v == w case
			// Consider the line extending the segment, parameterized as v + t (w - v).
			// We find projection of point p onto the line. 
			// It falls where t = [(p-v) . (w-v)] / |w-v|^2
			float t = Vector2.Dot(point - a, b - a) / l2;
			if (t < 0.0)
				return Vector2.Distance(point, a);       // Beyond the 'v' end of the segment
			else if (t > 1.0)
				return Vector2.Distance(point, b);  // Beyond the 'w' end of the segment
			Vector2 projection = a + t * (b - a);  // Projection falls on the segment
			return Vector2.Distance(point, projection);
		}



		public static bool LineIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 intersection){

			intersection = Vector2.zero;

			Vector2 s1, s2;
			s1.x = p1.x - p0.x;
			s1.y = p1.y - p0.y;
			s2.x = p3.x - p2.x;
			s2.y = p3.y - p2.y;

			float s, t, d;
			d = -s2.x * s1.y + s1.x * s2.y;

			if(d == 0){
				return false; // Parallel lines, no intersection
			}

			Vector2 pDiff = p0 - p2;
			s = (-s1.y * pDiff.x + s1.x * pDiff.y) / d;
			t = ( s2.x * pDiff.y - s2.y * pDiff.x) / d;
			
			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				intersection = p0 + (t * s1);
				return true; // Intersection!
			}
			
			return false; // No intersection
		}



		// Returns the first intersection it can find
		public static bool LineIntersection(Vector2 p0, Vector2 p1, Vector2[] points, out Vector2 intersection){
			intersection = Vector2.zero;
			for(int i=0;i<points.Length-1;i++){
				if(LineIntersection(p0, p1, points[i], points[i+1], out intersection)){
					return true;
				}
			}
			return false;
		}


		public static bool CompCountOf(ValueType vt, out int cc){
			cc = 0;
			if(vt == ValueType.VTv4 || vt == ValueType.TexAsset)
				cc = 4;
			if(vt == ValueType.VTv3)
				cc = 3;
			if(vt == ValueType.VTv2)
				cc = 2;
			if(vt == ValueType.VTv1)
				cc = 1;
			if(vt == ValueType.VTv1v2 || vt == ValueType.VTv1v3 || vt == ValueType.VTv1v4 || vt == ValueType.VTvPending)
				cc = 1;
			if( vt == ValueType.VTm4x4 || vt == ValueType.VTv4m4x4 ) {
				cc = 16;
				return true;
			}
				

			return cc > 0;

		}


		public static string StringToBase64String(string str) {
			return System.Convert.ToBase64String( GetBytes( str ) );
		}

		public static string Base64StringToString( string encoded ) {
			return GetString( System.Convert.FromBase64String( encoded ) );
		}

		static byte[] GetBytes( string str ) {
			byte[] bytes = new byte[str.Length * sizeof( char )];
			System.Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
			return bytes;
		}

		static string GetString( byte[] bytes ) {
			char[] chars = new char[bytes.Length / sizeof( char )];
			System.Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
			return new string( chars );
		}


	}
}
