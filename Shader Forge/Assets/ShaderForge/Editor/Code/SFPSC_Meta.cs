using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Meta : SFPS_Category {

		public enum Inspector3DPreviewType { Sphere, Plane, Skybox };
		public string[] strInspector3DPreviewType = new string[] { "3D object", "2D sprite", "Sky"};
		public enum BatchingMode { Enabled, Disabled, DisableDuringLODFade };
		public string[] strBatchingMode = new string[] { "Enabled", "Disabled", "Disabled during LOD fade" };
		public Inspector3DPreviewType previewType = Inspector3DPreviewType.Sphere;

		public BatchingMode batchingMode = BatchingMode.Enabled;
		public bool canUseSpriteAtlas = false;
		public bool compileToAllPlatforms = true;
		public bool[] usedRenderers; // TODO: Serialization?
		public string fallback = "";
		public int LOD = 0; // TODO: Serialization?

		public List<string> cgIncludes = new List<string>();

		/*
		d3d9 		= 0,	// - Direct3D 9
		d3d11 		= 1,	// - Direct3D 11
		glcore 		= 2,	// - OpenGL Core
		gles 		= 3,	// - OpenGL ES 2.0
		gles3		= 4,	// - OpenGL ES 3.0
		metal		= 5,	// - iOS Metal
		d3d11_9x 	= 6,	// - Direct3D 11 windows RT
		xboxone 	= 7,	// - Xbox One
		ps4 		= 8,	// - PlayStation 4
		psp2 		= 10	// - PlayStation Vita
		n3ds 		= 11	// - Nintendo 3DS
		wiiu		= 12,	// - Nintendo Wii U
		*/

		public override SFPS_Category PostInitialize (){
			usedRenderers = new bool[13]{ // TODO: Load from project settings
				true,	// - Direct3D 9
				true,	// - Direct3D 11
				true,	// - OpenGL Core
				true,	// - OpenGL ES 2.0
				true,  // - OpenGL ES 3.0
				true,	// - iOS Metal
				false,	// - Direct3D 11 windows RT
				false,	// - Xbox One
				false,	// - PlayStation 4
				false,	// - PlayStation Vita
				false,	// - Nintendo 3DS
				false,	// - Wii U
				false	// - PlayStation 5
			};
			return this;
		}


		char[] splitChars = new char[] { '|' };
		string SerializeCgIncludes() {
			string serialized = "";
			for( int i = 0; i < cgIncludes.Count; i++ ) {
				serialized += cgIncludes[i];
				if( i < cgIncludes.Count - 1 )
					serialized += splitChars[0];
			}
			string encoded = SF_Tools.StringToBase64String( serialized );

			return encoded;
		}

		void DeserializeCgIncludes( string serialized ) {
			string decoded = SF_Tools.Base64StringToString( serialized );
			cgIncludes = new List<string>( decoded.Split( splitChars ) );

			// Remove empty entries
			for( int i = cgIncludes.Count - 1; i >= 0; i-- )
				if( cgIncludes[i] == string.Empty )
					cgIncludes.RemoveAt( i );
		}

		public override string Serialize(){
			string s = "";
			s += Serialize( "flbk", fallback );
			s += Serialize( "iptp", ((int)previewType).ToString() );
			s += Serialize( "cusa", canUseSpriteAtlas.ToString() );
			s += Serialize( "bamd", ( (int)batchingMode ).ToString() );
			s += Serialize( "cgin", SerializeCgIncludes() );
			s += Serialize( "cpap", compileToAllPlatforms.ToString() );
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "flbk":
				fallback = value;
				break;
			case "iptp":
				previewType = (Inspector3DPreviewType)int.Parse(value);
				break;
			case "cusa":
				canUseSpriteAtlas = bool.Parse(value);
				break;
			case "bamd":
				batchingMode = (BatchingMode)int.Parse( value );
				break;
			case "cgin":
				DeserializeCgIncludes( value );
				break;
			case "cpap":
				compileToAllPlatforms = bool.Parse( value );
				break;
			}

		}
	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			
			r.xMin += 20;
			r.y += 20;

			
			EditorGUI.LabelField( r, "Path", EditorStyles.miniLabel );
			r.xMin += 30;
			r.height = 17;
			r.xMax -= 3;
			ps.StartIgnoreChangeCheck();
			GUI.SetNextControlName( "shdrpath" );
			string prev = editor.currentShaderPath;
			//editor.currentShaderPath = GUI.TextField( r, editor.currentShaderPath,EditorStyles.textField );
			editor.currentShaderPath = UndoableTextField( r, editor.currentShaderPath, "shader path", null, editor, showContent:false );
			if( editor.currentShaderPath != prev ) {
				SF_Tools.FormatShaderPath( ref editor.currentShaderPath );
			}
			if( Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath" ) {
				editor.Defocus();
				editor.OnShaderModified( NodeUpdateType.Hard );
			}
			ps.EndIgnoreChangeCheck();
			r.xMin -= 30;
			r.height = 20;
			r.xMax += 3;
			r.y += 20;
			
			
			
			
			EditorGUI.LabelField( r, "Fallback", EditorStyles.miniLabel );
			Rect rStart = new Rect( r );
			r.xMin += 50;
			r.height = 17;
			r.xMax -= 47;
			ps.StartIgnoreChangeCheck();
			GUI.SetNextControlName( "shdrpath" );
			prev = fallback;
			fallback = UndoableTextField( r, fallback, "shader fallback", null, null, showContent:false );
			r.x += r.width + 2;
			r.width = 42;
			ShaderPicker( r, "Pick");
			if( fallback != prev ) {
				SF_Tools.FormatShaderPath( ref fallback );
			}
			if( Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath" ) {
				editor.Defocus();
				editor.OnShaderModified( NodeUpdateType.Hard );
			}
			ps.EndIgnoreChangeCheck();
			r = rStart;
			r.y += r.height;
			
			
			EditorGUI.LabelField( r, "LOD", EditorStyles.miniLabel );
			r.xMin += 30;
			r.height = 17;
			r.xMax -= 3;
			LOD = UndoableIntField( r, LOD, "LOD");
			r.xMin -= 30;
			r.height = 20;
			r.xMax += 3;
			r.y += 20;


			canUseSpriteAtlas = UndoableToggle( r, canUseSpriteAtlas, "Allow using atlased sprites", "allow using atlased sprites", null );
			r.y += 20;

			batchingMode = (BatchingMode)UndoableLabeledEnumPopupNamed( r, "Draw call batching", batchingMode, strBatchingMode, "draw call batching" );
			r.y += 20;

			previewType = (Inspector3DPreviewType)UndoableLabeledEnumPopupNamed( r, "Inspector preview mode", previewType, strInspector3DPreviewType, "inspector preview mode" );
			r.y += 20;

			r.y += 5;


			if( cgIncludes.Count == 0 ) {
				Rect rBtn = r;
				rBtn.height -= 4;
				rBtn.width = 100;
				if( GUI.Button( rBtn, "Add CG Include", EditorStyles.miniButton ) ) {
					Undo.RecordObject( this, "add CG include" );
					cgIncludes.Add( "" );
				}
				//r.y += 20;
			} else {
				EditorGUI.LabelField( r, "CG Includes:" );
				r.y += 20;


				int removeTarget = -1;

				for( int i = 0; i < cgIncludes.Count; i++ ) {

					Rect smallRect = r;
					smallRect.width = 20;
					smallRect.height -= 2;

					if( GUI.Button( smallRect, "-" ) ) {
						removeTarget = i;
					}

					r.xMin += 22;

					Rect textFieldRect = r;
					textFieldRect.height -= 2;
					textFieldRect.width -= 3;
					cgIncludes[i] = UndoableTextField( textFieldRect, cgIncludes[i], "cg include", null );
					textFieldRect.x += 1;
					GUI.color = new Color( 1f, 1f, 1f, 0.3f );
					GUI.Label( textFieldRect, "<color=#00000000>" + cgIncludes[i] + "</color>.cginc", SF_Styles.RichLabel );
					GUI.color = Color.white;
					r.y += 20;

					r.xMin -= 22;
				}

				if( removeTarget != -1 ) {
					Undo.RecordObject( this, "remove CG include" );
					cgIncludes.RemoveAt( removeTarget );
				}

				Rect buttonRect = r;
				buttonRect.width = 20;
				buttonRect.height -= 2;
				if( GUI.Button( buttonRect, "+" ) ) {
					Undo.RecordObject( this, "add CG include" );
					cgIncludes.Add( "" );
				}
			}


			r.y += 30;


			compileToAllPlatforms = UndoableToggle( r, compileToAllPlatforms, "Compile to all platforms", "toggle compile to all platforms" );
			r.y += r.height;

			if( compileToAllPlatforms == false ) {
				r.x += 20;
				EditorGUI.LabelField( r, "Target renderers:" );
				r.xMin += 5;
				r.y += 20;
				r.height = 17;
				float pWidth = r.width;
			
			
				bool onlyDX11GlCore = ps.mOut.tessellation.IsConnectedAndEnabled();


				for(int i=0;i<usedRenderers.Length;i++){
					bool isDX11orGlCore = ( i == (int)RenderPlatform.d3d11 ) || i == (int)RenderPlatform.glcore;
				
					r.width = 20;
				
					bool prevEnable = GUI.enabled;
					//bool displayBool = usedRenderers[i];

					bool shouldDisable = !isDX11orGlCore && onlyDX11GlCore;
				
					if( shouldDisable ) {
						GUI.enabled = false;
						EditorGUI.Toggle( r, false );
					} else {
						usedRenderers[i] = UndoableToggle( r, usedRenderers[i], SF_Tools.rendererLabels[i] + " renderer");
						//usedRenderers[i] = EditorGUI.Toggle( r, usedRenderers[i] );
					}
				
				
					r.width = pWidth;
					r.xMin += 20;
					EditorGUI.LabelField( r, SF_Tools.rendererLabels[i], EditorStyles.miniLabel );
				
					if( shouldDisable ) {
						GUI.enabled = prevEnable;
					}
				
					r.xMin -= 20;
					r.y += r.height+1;
				}


				

			}
			r.x -= 20;
			r.y += 5;
			r.y += prevYpos;
			return (int)r.yMax;
		}





		MenuCommand mc;
		private void DisplayShaderContext(Rect r) {
			if( mc == null )
				mc = new MenuCommand( this, 0 );
			EditorUtility.DisplayPopupMenu( r, "CONTEXT/ShaderPopup", mc ); // Display shader popup
		}
		private void OnSelectedShaderPopup( string command, Shader shader ) {
			if( shader != null ) {
				if( fallback != shader.name ) {
					Undo.RecordObject(this, "pick fallback shader");
					fallback = shader.name;
					editor.Defocus();
					//editor.OnShaderModified( NodeUpdateType.Hard );
				}
			}
		}
		
		public void ShaderPicker(Rect r, string s){
			if( GUI.Button( r, s, EditorStyles.popup ) ) {
				DisplayShaderContext(r);
			}
		}





	}
}