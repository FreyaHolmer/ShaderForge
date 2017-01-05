using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {
	[System.Serializable]
	public class SF_StatusBox {

		[SerializeField]
		private SF_Editor editor;
		[SerializeField]
		SF_MinMax vCount = new SF_MinMax();
		[SerializeField]
		SF_MinMax fCount = new SF_MinMax();
		[SerializeField]
		SF_MinMax vtCount = new SF_MinMax();
		[SerializeField]
		SF_MinMax ftCount = new SF_MinMax();
		[SerializeField]
		public RenderPlatform platform;

		[SerializeField]
		private GUIStyle labelStyle;
		[SerializeField]
		private GUIStyle labelStyleCentered;
		[SerializeField]
		private GUIStyle holderStyle;
		[SerializeField]
		private GUIStyle headerStyle;

		public SF_StatusBox() {



		}

		public void Initialize( SF_Editor editor) {
			this.editor = editor;
			labelStyle = new GUIStyle( EditorStyles.label );
			labelStyle.margin = new RectOffset( 0, 0, 0, 0 );
			labelStyle.padding = new RectOffset( 8, 0, 3, 1 );

			labelStyleCentered = new GUIStyle( labelStyle );
			labelStyleCentered.alignment = TextAnchor.MiddleCenter;

			holderStyle = new GUIStyle();
			holderStyle.margin = new RectOffset( 0, 0, 0, 0 );
			holderStyle.padding = new RectOffset( 0, 0, 0, 0 );


			headerStyle = new GUIStyle( EditorStyles.toolbar );
			headerStyle.alignment = TextAnchor.MiddleLeft;
			headerStyle.fontSize = 10;
			//headerStyle.fontStyle = FontStyle.Bold;
		}


		public int OnGUI( int yOffset, int in_maxWidth ) {

			Rect r = new Rect( 0, yOffset, in_maxWidth, 18 );
			
			//string tmp = "Instructions: ";

			//if( Compiled() ) {
				headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			//} else {
				//headerStyle.normal.textColor = new Color( 0f, 0f, 0f, 0.75f );
				//tmp = "(Shader not compiled yet)";
			//}

			GUI.Label( r, string.Empty, EditorStyles.toolbar );	// Toolbar

			Rect iRect = r;

			//Rect rTmp = iRect;
			//rTmp = rTmp.MovedUp();

			//GUI.Label(rTmp, "MIP USED; " + editor.nodeView.treeStatus.mipInputUsed);

			iRect.width = 64f;

			
			GUI.color = new Color(1f,1f,1f,0.5f);
			if( GUI.Button( iRect, "Select", EditorStyles.toolbarButton) ) {
				Selection.activeObject = editor.currentShaderAsset;
				EditorGUIUtility.PingObject(editor.currentShaderAsset);
			}
			GUI.color = Color.white;

			/* Instruction count disabled.
			if(!editor.nodeView.treeStatus.CanDisplayInstructionCount){
				InstructionLabel( ref iRect, SF_Styles.IconWarningSmall, "Instruction count unavailable");
			} else {

				InstructionLabel( ref iRect, SF_GUI.Inst_vert, vCount.ToString() );
				InstructionLabel( ref iRect, SF_GUI.Inst_frag, fCount.ToString() );
				if( !vtCount.Empty() )
					InstructionLabel( ref iRect, SF_GUI.Inst_vert_tex, vtCount.ToString() );
				if( !ftCount.Empty() )
					InstructionLabel( ref iRect, SF_GUI.Inst_frag_tex, ftCount.ToString() );
			}
			*/
			








			//if(Compiled()){
				Color c = GUI.color;
				c.a = 0.5f;
				GUI.color = c;
				r.xMin += iRect.x;
				r.xMax -= 4;
				GUI.Label(r, SF_Tools.rendererLabels[(int)platform],SF_Styles.InstructionCountRenderer);
				GUI.color = Color.white;
			//}






			GUI.color = Color.white;

			return (int)r.yMax;
		}


		public void InstructionLabel(ref Rect iRect, Texture2D icon, string label) {

			iRect.width = icon.width;
			GUI.DrawTexture( iRect, icon );
			iRect.x += iRect.width;
			iRect.width = SF_GUI.WidthOf( label, headerStyle )+2;
			GUI.Label( iRect, label, headerStyle );
			iRect.x += iRect.width;
		}


		private bool Compiled() {
			if( vCount.min == 0 )
				return false;
			return true;
		}

		//private enum LookingFor{  };

		public void UpdateInstructionCount( Shader sh ) {
			// Compiled shader string:
			string[] css = ( new SerializedObject( sh ) ).FindProperty( "m_Script" ).stringValue.Split( '\n' );

			Debug.Log( ( new SerializedObject( sh ) ).FindProperty( "m_Script" ).stringValue );
			Debug.Log(css.Length);

			if(css.Length < 2){
				return;
			}

			ShaderProgram prog = ShaderProgram.Vert;


			List<SFIns_Pass> passes = new List<SFIns_Pass>();
			SFIns_Pass cPass; // current pass

			for( int i = 0; i < css.Length; i++ ) {
				if( css[i].Contains( "Pass {" ) ) { // Found a pass!

					bool ignoreMin = false;
					i++;

					// Shadow passes
					if( css[i].Contains( "Name \"ShadowCaster\"" ) || css[i].Contains( "Name \"ShadowCollector\"" ) ||  css[i].Contains( "Name \"ForwardAdd\"" ) )
						continue;

					if( (css[i].Contains("Name \"PrePassBase\"") || css[i].Contains("Name \"PrePassFinal\"") ) && editor.ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Forward )
						continue;

					if( (css[i].Contains("Name \"ForwardBase\"") || css[i].Contains("Name \"ForwardAdd\"") ) && editor.ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Deferred )
						continue;


						//ignoreMin = true;

					cPass = new SFIns_Pass();

					for ( ; i < css.Length; i++ ) {

						if(css[i].StartsWith("// Vertex combos")){
							prog = ShaderProgram.Vert;
						} else if(css[i].StartsWith("// Fragment combos")){
							prog = ShaderProgram.Frag;
						} else {
							continue;
						}

						// Program found! 
						i++; // Next line...
						// Scan following lines for shader platform data
						for ( ; i < css.Length; i++ ) {
							if(css[i].StartsWith( "//" )){
								cPass.Parse( prog, css[i], ignoreMin );
							} else {
								// No more platform data
								break;
							}
						}
						// Leave this loop and start searching for the next pass if we just found the frag data
						if( prog == ShaderProgram.Frag )
							break;
					}
					// Add the current pass to the list
					passes.Add(cPass);
				}
			}


			// All passes scanned!
			// Show, some sort of instruction count
			// Show sum of all passes min for now
			vCount.Reset();
			fCount.Reset();
			vtCount.Reset();
			ftCount.Reset();

			// Find which program to display instruction count for!
			// if(mac) opengl
			// if(win) d3d9
			// else gles
			// else *any enabled*



			platform = GetPrimaryPlatform();
			int primPlat = (int)platform;



		//	Debug.Log("Primary platform: " + (RenderPlatform)primPlat);



			foreach( SFIns_Pass p in passes ) {
				vCount += p.plats[primPlat].vert;	// Only d3d9 for now // TODO
				fCount += p.plats[primPlat].frag;
				vtCount += p.plats[primPlat].vTex;
				ftCount += p.plats[primPlat].fTex;
			}


			//Debug.Log("vCount = " + vCount);




			/*
			int programID = 0; // 0 = vert  |  1 = frag
			for( int i = 0; i < css.Length; i++ ) {
				if( css[i].Contains( "instructions" ) )
					continue;
				if( css[i].Contains( "# " ) ) {
					if( programID == 0 ) {
						string[] split = css[i].Trim().Split( ' ' );
						vCount = int.Parse( split[1] ); // Vertex instructions TODO: Textures in vertex program
						programID++; // Search for fragment
					} else if( programID == 1 ) {
						string[] split = css[i].Trim().Split( ' ' );
						fCount = int.Parse( split[1] ); // Fragment instructions
						try {
							tCount = int.Parse( split[3] ); // Textures
						} catch {

						}
						
					}
				}
			}
			*/




		}


		public RenderPlatform GetPrimaryPlatform() {

			// Let's check our build target!
			BuildTarget active = EditorUserBuildSettings.activeBuildTarget;

			// Mobile platforms
			// 9 =  BuildTarget.iPhone		// Unity 4.x
			// 9 =  BuildTarget.iOS			// Unity 5.x
			// 28 = BuildTarget.BB10		// Unity 4.x
			// 28 = BuildTarget.BlackBerry	// Unity 5.x

			bool mobile = ( active == BuildTarget.Android || (int)active == 9 || (int)active == 28 );
			if(mobile && editor.ps.catMeta.usedRenderers[(int)RenderPlatform.gles])
				return RenderPlatform.gles;

			// Standalone / Webplayer. In this case, it depends on what the user is using
			// Pick the one that is currently running
			if( Application.platform == RuntimePlatform.OSXEditor && editor.ps.catMeta.usedRenderers[(int)RenderPlatform.glcore] )
				return RenderPlatform.glcore;
			if( Application.platform == RuntimePlatform.WindowsEditor && editor.ps.catMeta.usedRenderers[(int)RenderPlatform.d3d9] )
				return RenderPlatform.d3d9;
			if( Application.platform == RuntimePlatform.WindowsEditor && editor.ps.catMeta.usedRenderers[(int)RenderPlatform.d3d11] )
				return RenderPlatform.d3d11;


			Debug.LogWarning( "[SF] Unhandled platform settings. Make sure your build target (" + active + ") is sensible, and that you've got platforms enabled to compile for" );
			// You're using some weird setup, pick first active one
			for(int i=0;i<12;i++){
				if(editor.ps.catMeta.usedRenderers[i])
					return (RenderPlatform)i;
			}

			Debug.LogError("No renderers compilable, defaulting to d3d9");
			return RenderPlatform.d3d9;
		}


	}
}