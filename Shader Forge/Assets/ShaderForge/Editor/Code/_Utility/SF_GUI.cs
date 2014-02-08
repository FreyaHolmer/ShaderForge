using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace ShaderForge {

	public static class SF_GUI {

		static Matrix4x4 prevMatrix;

		public static Color[] outdatedStateColors = new Color[]{
			new Color(0.7f, 1f, 0.7f),
			new Color(1f, 1f, 0.7f),
			new Color(1f,0.7f,0.7f)
		};


		private static Texture2D handle_drag;
		public static Texture2D Handle_drag {
			get {
				if( handle_drag == null )
					handle_drag = (Texture2D)Resources.Load( SF_Paths.pInterface + "handle_drag", typeof(Texture2D) );
				return handle_drag;
			}
		}

		private static Texture2D logo;
		public static Texture2D Logo {
			get {
				if( logo == null )
					logo = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix("logo"), typeof(Texture2D) );
				return logo;
			}
		}

		private static Texture2D icon;
		public static Texture2D Icon {
			get {
				if( icon == null )
					icon = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix( "icon" ), typeof(Texture2D) );
				return icon;
			}
		}

		private static Texture2D screenshot_icon;
		public static Texture2D Screenshot_icon {
			get {
				if( screenshot_icon == null )
					screenshot_icon = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix( "screenshot_icon" ), typeof(Texture2D) );
				return screenshot_icon;
			}
		}

		private static Texture2D inst_vert;
		public static Texture2D Inst_vert {
			get {
				if( inst_vert == null )
					inst_vert = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix( "inst_vert" ), typeof(Texture2D) );
				return inst_vert;
			}
		}

		private static Texture2D inst_vert_tex;
		public static Texture2D Inst_vert_tex {
			get {
				if( inst_vert_tex == null )
					inst_vert_tex = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix( "inst_vert_tex" ), typeof(Texture2D) );
				return inst_vert_tex;
			}
		}

		private static Texture2D inst_frag;
		public static Texture2D Inst_frag {
			get {
				if( inst_frag == null )
					inst_frag = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix("inst_frag" ), typeof(Texture2D) );
				return inst_frag;
			}
		}

		private static Texture2D inst_frag_tex;
		public static Texture2D Inst_frag_tex {
			get {
				if( inst_frag_tex == null )
					inst_frag_tex = (Texture2D)Resources.Load( SF_Paths.pInterface + SkinSuffix( "inst_frag_tex" ), typeof(Texture2D) );
				return inst_frag_tex;
			}
		}




		public static string SkinSuffix(string s) {
			return s + ( SF_GUI.ProSkin ? "" : "_light" );
		}


		/*
		public static void StartZoomPanel( float zoom, Rect rect ) {
			float zoomInv = 1f / zoom;

			GUI.EndGroup(); // Leave parent group to avoid clipping issues
			Rect clippedArea = rect.ScaleSizeBy( zoomInv, rect.TopLeft() );

			//clippedArea.x -= clippedArea.width * zoomInv * 0.25f;
			//clippedArea.y -= clippedArea.height * zoomInv * 0.25f;
			clippedArea.width *= zoom;
			clippedArea.height *= zoom;
			clippedArea.y += GetEditorTabHeight();
			GUI.BeginGroup( clippedArea, EditorStyles.numberField );

			prevMatrix = GUI.matrix;
			Matrix4x4 Translation = Matrix4x4.TRS( new Vector3( clippedArea.x, clippedArea.y, 0 ), Quaternion.identity, Vector3.one );
			Matrix4x4 Scale = Matrix4x4.Scale( new Vector3( zoom, zoom, zoom ) );
			GUI.matrix = Translation * Scale * Translation.inverse * GUI.matrix;
		}

		public static void EndZoomPanel() {
			GUI.matrix = prevMatrix;
			GUI.EndGroup();
			GUI.BeginGroup( new Rect( 0.0f, GetEditorTabHeight(), Screen.width, Screen.height ) ); // Remake parent
		}
		 * */


		static MethodInfo allowIndieMethod;
		static MethodInfo AllowIndieMethod {
			get{
				if( allowIndieMethod == null ) {
					Type t = typeof( EditorUtility );
					BindingFlags bfs = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
					allowIndieMethod = t.GetMethod( "SetTemporarilyAllowIndieRenderTexture", bfs, null, new Type[] { typeof( bool ) }, null );
				}
				return allowIndieMethod;
			}
		}
		public static void AllowIndieRenderTextures() {
			AllowIndieMethod.Invoke( null, new object[] { true } );//EditorUtility.SetTemporarilyAllowIndieRenderTexture( true );
		}


		public static bool AcceptedNewShaderReplaceDialog() {
			return EditorUtility.DisplayDialog( "Delete existing shader?", "This shader was not created in Shader Forge. Are you sure you want to remove all existing shader data and open it in Shader Forge?", "Yes", "Cancel" );
		}



		public static int GetEditorTabHeight() {
			return 21; // TODO: This is correct when docked, not floating
		}

		public static void AssignCursorForPreviousRect( MouseCursor cursor ) {
			EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), cursor );
		}

		public static void AssignCursor( Rect r, MouseCursor cursor ) {
			EditorGUIUtility.AddCursorRect( r, cursor );
		}

		public static bool PressedLMB( Rect r ) {
			return ( PressedLMB() && r.Contains(Event.current.mousePosition));
		}

		public static bool PressedLMB() {
			return ( Event.current.type == EventType.mouseDown ) && ( Event.current.button == 0 );
		}

		public static bool ReleasedLMB() {
			return ( Event.current.type == EventType.mouseUp ) && ( Event.current.button == 0 );
		}

		public static bool PressedMMB() {
			return ( Event.current.type == EventType.mouseDown ) && ( Event.current.button == 2 );
		}

		public static bool ReleasedRawMMB() {
			return ( Event.current.rawType == EventType.mouseUp ) && ( Event.current.button == 2 );
		}

		public static bool ReleasedRawLMB() {
			return ( Event.current.rawType == EventType.mouseUp ) && ( Event.current.button == 0 );
		}

		public static bool ReleasedRawRMB() {
			return ( Event.current.rawType == EventType.mouseUp ) && ( Event.current.button == 1 );
		}

		public static bool PressedRMB() {
			return ( Event.current.type == EventType.mouseDown ) && ( Event.current.button == 1 );
		}

		public static bool ReleasedRMB() {
			return ( Event.current.type == EventType.mouseUp ) && ( Event.current.button == 1 );
		}

		public static bool HoldingAlt() {
			return (Event.current.modifiers & EventModifiers.Alt) != 0; // Alt is held
		}

		public static bool HoldingBoxSelect() {
			return HoldingAlt(); // Alt is held. TODO: Make a toggle for (Alt cam) vs (Alt select)
		}

		public static bool HoldingShift() {
			return ( Event.current.modifiers & EventModifiers.Shift ) != 0; // Shift is held
		}

		public static bool HoldingControl() {
			if( Application.platform == RuntimePlatform.OSXEditor )
				return ( Event.current.modifiers & EventModifiers.Command ) != 0; // Command is held
			else {
				return ( Event.current.control ); // Control is held
			}
			
		}
		
		public static bool PressedDelete(){
			if(Event.current.type != EventType.keyDown)
				return false;
			
			if(Event.current.keyCode == KeyCode.Delete) // Windows / Mac extended keyboard delete
				return true;
			
			bool holdingCommand = HoldingControl();
			bool pressedBackspace = (Event.current.keyCode == KeyCode.Backspace);
			
			if(holdingCommand && pressedBackspace) // Mac laptop style delete
				return true;
			
			return false;
			
		}

		public static bool PressedCameraMove(){
			return ( PressedLMB() || PressedMMB() );
		}

		public static bool ReleasedCameraMove(){
			return ( ReleasedRawLMB() || ReleasedRawMMB() );
		}

		public static bool MultiSelectModifierHeld(){
			return ( HoldingShift() || HoldingControl() );
		}

		public const byte ColBgPro = (byte)56;
		public const byte ColBgFree = (byte)194;
		public static void UseBackgroundColor() {
			byte v = EditorGUIUtility.isProSkin ? ColBgPro : ColBgFree;
			GUI.color = new Color32( v, v, v, (byte)255 );
		}

		public static Color selectionColor = new Color32( (byte)41, (byte)123, (byte)194, (byte)255 );
		public static Color selectionColorBright = new Color32( (byte)54, (byte)162, (byte)255, (byte)255 );
		public static Color selectionColorBrighter = new Color32( (byte)175, (byte)218, (byte)255, (byte)255 );
	/*	public static Color SelectionColor {
			get {
				if( selectionColor == null )
					selectionColor
				return selectionColor;
			}
		}*/

		public static int WidthOf(GUIContent s, GUIStyle style){
			return (int)style.CalcSize(  s ).x;
		}

		public static int WidthOf( string s, GUIStyle style ) {
			return (int)style.CalcSize( new GUIContent(s) ).x;
		}

		public static System.Enum LabeledEnumField( Rect r, string label, System.Enum enumVal, GUIStyle style ) {
			return LabeledEnumField( r, new GUIContent(label), enumVal, style);
		}

		public static void MoveRight( ref Rect r, int newWidth ) {
			r.x += r.width;
			r.width = newWidth;
		}

		public static int LabeledEnumFieldNamed( Rect r, string[] names, GUIContent label, int enumVal, GUIStyle style ) {
			Rect leftRect = new Rect( r );
			Rect rightRect = new Rect( r );
			int width = WidthOf( label, style ) + 4;
			leftRect.width = width;
			rightRect.xMin += width;
			GUI.Label( leftRect, label, style );
			return EditorGUI.Popup( rightRect, (int)enumVal, names );
		}

		public static System.Enum LabeledEnumField(Rect r, GUIContent label, System.Enum enumVal, GUIStyle style) {
			Rect leftRect = new Rect( r );
			Rect rightRect = new Rect( r );
			int width = WidthOf( label, style) + 4;
			leftRect.width = width;
			rightRect.xMin += width;
			GUI.Label( leftRect, label, style );
			return EditorGUI.EnumPopup( rightRect, enumVal );
		}


		public static void FillBackground( Rect r ) {
			Color pCol = GUI.color;
			SF_GUI.UseBackgroundColor();
			GUI.DrawTexture( r, EditorGUIUtility.whiteTexture );
			GUI.color = pCol;
		}


		public static void EnterableFloatField( SF_Node n, Rect r, ref float val, GUIStyle style ) {
			if( style == null )
				style = EditorStyles.textField;
			string field_name = n.GetType().ToString() + "_" + n.id;


			GUI.SetNextControlName( field_name );
			EditorGUI.BeginChangeCheck();
			val = EditorGUI.FloatField( r, val, style );


			bool pressedEnter = Event.current.keyCode == KeyCode.Return;

			if( pressedEnter ) {
				EditorGUI.EndChangeCheck();
				//Debug.Log("Pressed enter with focus on " + GUI.GetNameOfFocusedControl() + ", should have been " + field_name);
				if(GUI.GetNameOfFocusedControl() == field_name){
					//Debug.Log("Pressed enter!");
					n.OnUpdateNode( NodeUpdateType.Hard );
				}
			} else if( EditorGUI.EndChangeCheck() ) {
				n.OnUpdateNode( NodeUpdateType.Soft );
			}
		}


		public static void EnterableTextField( SF_Node n, Rect r, ref string str, GUIStyle style, bool update = true ) {
			if( style == null )
				style = EditorStyles.textField;
			string field_name = n.GetType().ToString() + "_txt_" + n.id;


			GUI.SetNextControlName( field_name );
			EditorGUI.BeginChangeCheck();
			str = EditorGUI.TextField( r, str, style );

			bool pressedEnter = Event.current.keyCode == KeyCode.Return;

			if(update){
				if( pressedEnter ) {
					if( GUI.GetNameOfFocusedControl() == field_name )
						n.OnUpdateNode( NodeUpdateType.Hard );
					EditorGUI.EndChangeCheck();
				} else if( EditorGUI.EndChangeCheck() ) {
					n.OnUpdateNode( NodeUpdateType.Soft );
				}
			} else if(EditorGUI.EndChangeCheck()){
				n.editor.ShaderOutdated = UpToDateState.OutdatedSoft;
			}

		}
		
		
		public static void ConditionalToggle(Rect r, ref bool value, bool usableIf, bool disabledDisplayValue, string label){
			if(usableIf){
				value = GUI.Toggle(r, value, label);
			} else {
				GUI.enabled = false;
				GUI.Toggle(r, disabledDisplayValue, label);
				GUI.enabled = true;
			}
		}


		public static int ContentScaledToolbar(Rect r, string label, int selected, string[] labels ) {
			
			r.height = 15;
			
			Rect rLeft = new Rect( r );
			Rect rRight = new Rect( r );
			
			rLeft.width = SF_GUI.WidthOf( label, EditorStyles.miniLabel )+4;
			rRight.width = r.width - rLeft.width;
			rRight.x += rLeft.width;
			
			GUI.Label( rLeft, label, EditorStyles.miniLabel);
			
			
			// Full pixel width of strings:
			float[] lblPxWidth = new float[labels.Length];
			float pxWidthTotal = 0;
			for( int i = 0; i < labels.Length; i++ ) {
				lblPxWidth[i] = SF_GUI.WidthOf( labels[i], EditorStyles.miniButtonMid );
				pxWidthTotal += lblPxWidth[i];
			}
			
			// Scale all buttons to fit the rect
			float scale = rRight.width / pxWidthTotal;
			for( int i = 0; i < labels.Length; i++ ) {
				lblPxWidth[i] *= scale;
			}
			
			
			
			
			GUIStyle style = EditorStyles.miniButtonLeft;
			int retval = selected;
			
			Rect rTemp = new Rect(rRight);
			
			for( int i = 0; i < labels.Length; i++ ) {
				
				rTemp.width = lblPxWidth[i];
				
				if( i == labels.Length - 1 ) {
					style = EditorStyles.miniButtonRight;
				} else if( i > 0 ) {
					style = EditorStyles.miniButtonMid;
				}
				
				if( GUI.Button(rTemp, labels[i], style ) )
					retval = i;
				
				if( selected == i && GUI.enabled ) {
					if( EditorGUIUtility.isProSkin ) {
						GUI.Box( rTemp, "" );
						GUI.Box( rTemp, "" );
						GUI.Box( rTemp, "" );
					} else {
						GUI.color = new Color( 0f, 0f, 0f, 0.3f );
						GUI.Box( rTemp, "" );
						GUI.color = Color.white;
					}
					
				}
				
				rTemp.x += rTemp.width;
			}
			GUI.color = Color.white;
			return retval;
			
		}



		public static bool ProSkin {
			get{
				return EditorGUIUtility.isProSkin;
			}
		}

	}
}

