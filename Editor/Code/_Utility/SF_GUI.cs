using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

namespace ShaderForge {

	public static class SF_GUI {


	


		static Matrix4x4 prevMatrix;

		public static Color[] outdatedStateColors = new Color[]{
			new Color(0.7f, 1f, 0.7f),
			new Color(1f, 1f, 0.7f),
			new Color(1f,0.7f,0.7f)
		};


		public static void DrawLock(Rect r, string tooltip = null, TextAlignment align = TextAlignment.Right){
			if(Event.current.type != EventType.Repaint)
				return;

			Color pCol = GUI.color;
			GUI.color = Color.white;
			SF_Styles.IconLock.Draw(r, false, true, true, false);

			if(tooltip != null && r.Contains(Event.current.mousePosition) ){

				GUIStyle style = EditorStyles.miniButton;

				r.width = style.CalcSize(new GUIContent(tooltip)).x + 8;
				r.height = style.CalcSize(new GUIContent(tooltip)).y + 4;

				r = r.MovedUp();

				if(align == TextAlignment.Left){
					r.x = (r.MovedLeft().x + r.x)/2f;
				}


				GUI.color = new Color(1f,1f,1f,0.8f);
				GUI.Box(r, tooltip, style);

			}

			GUI.color = pCol;


		}

		private static Texture2D LoadTexture(string path, string name){

			//AssetDatabase.LoadAssetAtPath(

			return SF_Resources.Load<Texture2D>(path+name);

			//return (Texture2D)Resources.Load(path + name, typeof(Texture2D) ); // TODO: This has to change into something that's not using resources
		}


		private static Texture2D vectorIconOverlay;
		public static Texture2D VectorIconOverlay{
			get{
				if( vectorIconOverlay == null )
					vectorIconOverlay = SF_Resources.LoadNodeIcon("Data/node_3d_data_mask");
				return vectorIconOverlay;
			}
		}


		private static Texture2D handle_drag;
		public static Texture2D Handle_drag {
			get {
				if( handle_drag == null )
					handle_drag = SF_Resources.LoadInterfaceIcon("handle_drag");
				return handle_drag;
			}
		}



		private static Texture2D logo;
		public static Texture2D Logo {
			get {
				if( logo == null )
					logo = SF_Resources.LoadInterfaceIcon( SkinSuffix("logo") );
				return logo;
			}
		}

		private static Texture2D icon;
		public static Texture2D Icon {
			get {
				if( icon == null )
					icon = SF_Resources.LoadInterfaceIcon( SkinSuffix( "icon" ) );
				return icon;
			}
		}

		private static Texture2D toggle_check_icon;
		public static Texture2D Toggle_check_icon {
			get {
				if( toggle_check_icon == null )
					toggle_check_icon = SF_Resources.LoadInterfaceIcon( SkinSuffix( "chk" ) );
				return toggle_check_icon;
			}
		}

		private static Texture2D screenshot_icon;
		public static Texture2D Screenshot_icon {
			get {
				if( screenshot_icon == null )
					screenshot_icon = SF_Resources.LoadInterfaceIcon( SkinSuffix( "screenshot_icon" ) );
				return screenshot_icon;
			}
		}






		private static Texture2D shader_preset_icon_custom;
		public static Texture2D Shader_preset_icon_custom {
			get {
				if( shader_preset_icon_custom == null )
					shader_preset_icon_custom = SF_Resources.LoadInterfaceIcon( "preset_custom" );
				return shader_preset_icon_custom;
			}
		}

		private static Texture2D shader_preset_icon_litbasic;
		public static Texture2D Shader_preset_icon_litbasic {
			get {
				if( shader_preset_icon_litbasic == null )
					shader_preset_icon_litbasic = SF_Resources.LoadInterfaceIcon( "preset_litbasic" );
				return shader_preset_icon_litbasic;
			}
		}

		private static Texture2D shader_preset_icon_litpbr;
		public static Texture2D Shader_preset_icon_litpbr {
			get {
				if( shader_preset_icon_litpbr == null )
					shader_preset_icon_litpbr = SF_Resources.LoadInterfaceIcon( "preset_litpbr" );
				return shader_preset_icon_litpbr;
			}
		}

		private static Texture2D shader_preset_icon_particleadditive;
		public static Texture2D Shader_preset_icon_particleadditive {
			get {
				if( shader_preset_icon_particleadditive == null )
					shader_preset_icon_particleadditive = SF_Resources.LoadInterfaceIcon( SkinSuffix("preset_particleadditive") );
				return shader_preset_icon_particleadditive;
			}
		}

		private static Texture2D shader_preset_icon_particlealphablended;
		public static Texture2D Shader_preset_icon_particlealphablended {
			get {
				if( shader_preset_icon_particlealphablended == null )
					shader_preset_icon_particlealphablended = SF_Resources.LoadInterfaceIcon( SkinSuffix("preset_particlealphablended") );
				return shader_preset_icon_particlealphablended;
			}
		}

		private static Texture2D shader_preset_icon_particlemultiplicative;
		public static Texture2D Shader_preset_icon_particlemultiplicative {
			get {
				if( shader_preset_icon_particlemultiplicative == null )
					shader_preset_icon_particlemultiplicative = SF_Resources.LoadInterfaceIcon( SkinSuffix("preset_particlemultiplicative") );
				return shader_preset_icon_particlemultiplicative;
			}
		}

		private static Texture2D shader_preset_icon_sky;
		public static Texture2D Shader_preset_icon_sky {
			get {
				if( shader_preset_icon_sky == null )
					shader_preset_icon_sky = SF_Resources.LoadInterfaceIcon( "preset_sky" );
				return shader_preset_icon_sky;
			}
		}

		private static Texture2D shader_preset_icon_sprite;
		public static Texture2D Shader_preset_icon_sprite {
			get {
				if( shader_preset_icon_sprite == null )
					shader_preset_icon_sprite = SF_Resources.LoadInterfaceIcon( "preset_sprite" );
				return shader_preset_icon_sprite;
			}
		}

		private static Texture2D shader_preset_icon_unlit;
		public static Texture2D Shader_preset_icon_unlit {
			get {
				if( shader_preset_icon_unlit == null )
					shader_preset_icon_unlit = SF_Resources.LoadInterfaceIcon( "preset_unlit" );
				return shader_preset_icon_unlit;
			}
		}

		private static Texture2D shader_preset_icon_highlight;
		public static Texture2D Shader_preset_icon_highlight {
			get {
				if( shader_preset_icon_highlight == null )
					shader_preset_icon_highlight = SF_Resources.LoadInterfaceIcon( "preset_highlight" );
				return shader_preset_icon_highlight;
			}
		}

		private static Texture2D shader_preset_icon_posteffect;
		public static Texture2D Shader_preset_icon_posteffect {
			get {
				if( shader_preset_icon_posteffect == null )
					shader_preset_icon_posteffect = SF_Resources.LoadInterfaceIcon( "preset_posteffect" );
				return shader_preset_icon_posteffect;
			}
		}

		




		private static Texture2D inst_vert;
		public static Texture2D Inst_vert {
			get {
				if( inst_vert == null )
					inst_vert = SF_Resources.LoadInterfaceIcon( SkinSuffix( "inst_vert" ) );
				return inst_vert;
			}
		}

		private static Texture2D inst_vert_tex;
		public static Texture2D Inst_vert_tex {
			get {
				if( inst_vert_tex == null )
					inst_vert_tex = SF_Resources.LoadInterfaceIcon( SkinSuffix( "inst_vert_tex" ) );
				return inst_vert_tex;
			}
		}

		private static Texture2D inst_frag;
		public static Texture2D Inst_frag {
			get {
				if( inst_frag == null )
					inst_frag = SF_Resources.LoadInterfaceIcon( SkinSuffix("inst_frag" ) );
				return inst_frag;
			}
		}

		private static Texture2D inst_frag_tex;
		public static Texture2D Inst_frag_tex {
			get {
				if( inst_frag_tex == null )
					inst_frag_tex = SF_Resources.LoadInterfaceIcon( SkinSuffix( "inst_frag_tex" ) );
				return inst_frag_tex;
			}
		}

		public static void DrawTextureTiled(Rect r, Texture2D tex, bool local = true){
			Rect tCoords = new Rect(
				local ? 0 : (float)r.x/(float)tex.width,
				local ? 0 : (float)r.y/(float)tex.height,
				(float)r.width/(float)tex.width, 
				(float)r.height/(float)tex.height
			);
			GUI.DrawTextureWithTexCoords(r,tex,tCoords);
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
			return ( Event.current.type == EventType.MouseDown ) && ( Event.current.button == 0 );
		}

		public static bool ReleasedLMB() {
			return ( Event.current.type == EventType.MouseUp ) && ( Event.current.button == 0 );
		}

		public static bool PressedMMB() {
			return ( Event.current.type == EventType.MouseDown ) && ( Event.current.button == 2 );
		}

		public static bool ReleasedRawMMB() {
			return ( Event.current.rawType == EventType.MouseUp ) && ( Event.current.button == 2 );
		}

		public static bool ReleasedRawLMB() {
			return ( Event.current.rawType == EventType.MouseUp ) && ( Event.current.button == 0 );
		}

		public static bool ReleasedRawRMB() {
			return ( Event.current.rawType == EventType.MouseUp ) && ( Event.current.button == 1 );
		}

		public static bool PressedRMB() {
			return ( Event.current.type == EventType.MouseDown ) && ( Event.current.button == 1 );
		}

		public static bool ReleasedRMB() {
			return ( Event.current.type == EventType.MouseUp ) && ( Event.current.button == 1 );
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
			if(Event.current.type != EventType.KeyDown)
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

		public static System.Enum LabeledEnumField( Rect r, string label, System.Enum enumVal, GUIStyle style, bool zoomCompensate = false ) {
			return LabeledEnumField( r, new GUIContent(label), enumVal, style, zoomCompensate);
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

		public static System.Enum LabeledEnumField(Rect r, GUIContent label, System.Enum enumVal, GUIStyle style, bool zoomCompensate = false) {
			Rect leftRect = new Rect( r );
			Rect rightRect = new Rect( r );
			int width = WidthOf( label, style) + 4;
			leftRect.width = width;
			rightRect.xMin += width;
			GUI.Label( leftRect, label, style );

			return SF_GUI.EnumPopup( rightRect, GUIContent.none, enumVal, EditorStyles.popup, zoomCompensate);
			//return EditorGUI.EnumPopup( rightRect, GUIContent.none, enumVal, EditorStyles.popup );
			//return EnumPopupZoomCompensated( rightRect, enumVal );

		}




		// UnityEditor.EditorGUI

		public static Enum EnumPopup(Rect position, GUIContent label, Enum selected, GUIStyle style, bool zoomCompensate = false)
		{


			Type type = selected.GetType();
			if (!type.IsEnum)
			{
				throw new Exception("parameter _enum must be of type System.Enum");
			}
			string[] names = Enum.GetNames(type);
			int num = Array.IndexOf<string>(names, Enum.GetName(type, selected));
			Matrix4x4 prevMatrix = Matrix4x4.identity;
			if(zoomCompensate){
				prevMatrix = GUI.matrix;
				GUI.matrix = Matrix4x4.identity;
			}
			num = EditorGUI.Popup(position, label, num, TempContent((
				from x in names
				select ObjectNames.NicifyVariableName(x)).ToArray<string>()), style);
			if (num < 0 || num >= names.Length)
			{
				if(zoomCompensate)
					GUI.matrix = prevMatrix;
				return selected;
			}
			if(zoomCompensate)
				GUI.matrix = prevMatrix;
			return (Enum)Enum.Parse(type, names[num]);
		}

		/*
		public static int Popup(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(EditorGUI.s_PopupHash, EditorGUIUtility.native, position);
			return EditorGUI.DoPopup(EditorGUI.PrefixLabel(position, controlID, label), controlID, selectedIndex, displayedOptions, style);
		}*/


		// UnityEditor.EditorGUIUtility
		private static GUIContent[] TempContent(string[] texts)
		{
			GUIContent[] array = new GUIContent[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				array[i] = new GUIContent(texts[i]);
			}
			return array;
		}



		/*
		public static Enum EnumPopupZoomCompensated(Rect r, Enum selected ){

			// TODO: Custom enum popup proper zoom positioning

			if(GUI.Button(r,selected.ToString(),EditorStyles.popup)){

				GenericMenu gm = new GenericMenu();
				//gm.AddItem(selected);

				Array enumList = Enum.GetValues(selected.GetType());

				for(int i=0;i < enumList.Length;i++){

					gm.AddItem( new GUIContent(enumList.GetValue(i).ToString()), i == Convert.ToInt32(selected), (object o)=>{Debug.Log(o.ToString());},"Test " + i);


				}

				gm.ShowAsContext();



			}



			return selected;

		}
*/

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


			bool pressedEnter = Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown;

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
			
			Rect rLeft  = new Rect( r );
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

				bool prev = selected == i;
				bool newVal = GUI.Toggle( rTemp, prev, labels[i], style );
				if( newVal != prev ) {
					retval = i;
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

