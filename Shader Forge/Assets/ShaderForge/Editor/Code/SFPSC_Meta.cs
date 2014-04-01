using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Meta : SFPS_Category {



		/* = new bool[7]{
			true,	// - Direct3D 9
			true,	// - Direct3D 11
			true,	// - OpenGL
			true,	// - OpenGL ES 2.0
			false,  // - Xbox 360
			false,	// - PlayStation 3
			false,	// - Flash
			false	// - Direct3D 11 for Windows RT
		};*/
		public bool[] usedRenderers; // TODO: Serialization?
		public string fallback = "";
		public int LOD = 0; // TODO: Serialization?



		public override SFPS_Category PostInitialize (){
			usedRenderers = new bool[8]{ // TODO: Load from project settings
				true,	// - Direct3D 9
				true,	// - Direct3D 11
				true,	// - OpenGL
				true,	// - OpenGL ES 2.0
				false,  // - Xbox 360
				false,	// - PlayStation 3
				false,	// - Flash
				false	// - Direct3D 11 for Windows RT
			};
			return this;
		}


		public override string Serialize(){
			string s = "";
			s += Serialize( "flbk", fallback );
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "flbk":
				fallback = value;
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
			
			
			
			
			EditorGUI.LabelField( r, "Target renderers:" );
			r.xMin += 20;
			r.y += 20;
			r.height = 17;
			float pWidth = r.width;
			
			
			bool onlyDX11 = ps.mOut.tessellation.IsConnectedAndEnabled();
			bool onlyDX = false;//editor.nodeView.treeStatus.mipInputUsed;
			
			
			for(int i=0;i<usedRenderers.Length;i++){
				bool isDX = ( i == 0 || i == 1 );
				bool isDX11 = (i == 1);
				
				r.width = 20;
				
				bool prevEnable = GUI.enabled;
				//bool displayBool = usedRenderers[i];
				
				bool shouldDisable = ( !isDX && onlyDX ) || ( !isDX11 && onlyDX11 );
				
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

			r.y += prevYpos;
			
			return (int)r.yMax;
		}





		MenuCommand mc;
		private void DisplayShaderContext(Rect r) {
			if( mc == null )
				mc = new MenuCommand( this, 0 );
			Material temp = new Material( "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}" ); // This dummy material will make it highlight none of the shaders inside.
			UnityEditorInternal.InternalEditorUtility.SetupShaderMenu( temp ); // Rebuild shader menu
			DestroyImmediate( temp.shader, true ); DestroyImmediate( temp, true ); // Destroy temporary shader and material
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