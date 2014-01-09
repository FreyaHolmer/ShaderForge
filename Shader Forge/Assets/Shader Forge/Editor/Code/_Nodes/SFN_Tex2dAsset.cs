using UnityEngine;
using UnityEditor;
using System.Collections;


namespace ShaderForge {
	[System.Serializable]
	public class SFN_Tex2dAsset : SF_Node {


		public Texture textureAsset;					// TODO: Use a parent class, this looks ridiculous
		public NoTexValue noTexValue = NoTexValue.White;// TODO: Use a parent class, this looks ridiculous
		public bool markedAsNormalMap = false; 			// TODO: Use a parent class, this looks ridiculous

		public SFN_Tex2dAsset() {

		}

		public override void Initialize() {
			base.Initialize( "Texture Asset" );
			node_height = (int)(rect.height - 6f); // Odd, but alright...
			base.UseLowerPropertyBox( true, true );
			base.texture.CompCount = 4;
			property = ScriptableObject.CreateInstance<SFP_Tex2d>().Initialize( this );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"TEX","Tex",ConType.cOutput,ValueType.TexAsset).WithColor(SF_Node.colorExposed)
			};
		}

		public override bool IsUniformOutput() {
			return false;
		}



		public bool IsNormalMap() {
			return markedAsNormalMap;
		}


		public bool IsAssetNormalMap() {

			if( textureAsset == null ){
				return false;
			} else {
				string path = AssetDatabase.GetAssetPath( textureAsset );
				if( string.IsNullOrEmpty( path ) )
					return false;
				else
					return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).normalmap;
			}

		}

		public bool HasAlpha() {
			if( textureAsset == null ) return false;
			string path = AssetDatabase.GetAssetPath( textureAsset );
			if( string.IsNullOrEmpty( path ) ) return false;
			return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).DoesSourceTextureHaveAlpha();
		}

		// TODO: MIP selection
		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			if( varDefined )
				return GetVariableName();

			/*
			bool useLOD = GetInputIsConnected( 1 ) || (SF_Evaluator.inVert || SF_Evaluator.inTess);
			string uvStr = GetInputIsConnected( 0 ) ? GetInputCon( 0 ).Evaluate() : SF_Evaluator.WithProgramPrefix( "uv0.xy" );
			string func = useLOD ? "tex2Dlod" : "tex2D";
			string mip = GetInputIsConnected( 1 ) ? GetInputCon( 1 ).Evaluate() : "0";

			if( useLOD ) {
				uvStr = "float4(" + uvStr + ",0.0," + mip + ")";
			}


			string s = func + "(" + property.GetVariable() + "," + uvStr + ")";
			if( IsNormalMap() ) {
				s = "UnpackNormal(" + s + ")";
			}
			*/
			Debug.LogError( "Invalid evaluation of " + property.name );
			return "";
		}

		// TODO: EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
		public void RenderToTexture() {
			if( textureAsset == null ) {
				//Debug.Log("Texture asset missing");

				// TODO: Use a parent class, this looks ridiculous
				// TODO: Use a parent class, this looks ridiculous
				texture.uniform = true;

				Color c = new Color(0f,0f,0f,0f);
				switch(noTexValue){
				case NoTexValue.Black:
					c = new Color(0f,0f,0f,1f);
					break;
				case NoTexValue.Gray:
					c = new Color(0.5f,0.5f,0.5f,1f);
					break;
				case NoTexValue.White:
					c = new Color(1f,1f,1f,1f);
					break;
				case NoTexValue.Bump:
					c = new Color(0.5f,0.5f,1f,1f);
					break;
				}
				texture.dataUniform = c;
				 


				return;
			}

			texture.uniform = false;
			// TODO: Use a parent class, this looks ridiculous
			// TODO: Use a parent class, this looks ridiculous




			SF_GUI.AllowIndieRenderTextures();

			RenderTexture rt = new RenderTexture( textureAsset.width, textureAsset.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default );
			rt.wrapMode = textureAsset.wrapMode;
			rt.Create();
			Graphics.Blit( textureAsset, rt );
			RenderTexture.active = rt;
			// The data is now in the RT, in an arbitrary res
			// TODO: Sample it with normalized coords down into a 128x128
			// Save it temporarily in a texture
			Texture2D temp = new Texture2D( textureAsset.width, textureAsset.height, TextureFormat.ARGB32, false );
			temp.wrapMode = textureAsset.wrapMode;
			temp.ReadPixels( new Rect( 0, 0, textureAsset.width, textureAsset.height ), 0, 0 );

			if( IsAssetNormalMap() ) {
				UnpackNormals( ref temp );
			}



			RenderTexture.active = null;
			rt.Release(); // Remove RT
			texture.ReadData( temp ); // Read Data from temp texture
			Object.DestroyImmediate( temp ); // Destroy temp texture

		}

		public void UnpackNormals( ref Texture2D t ) {
			Color[] colors = t.GetPixels();
			for( int i = 0; i < colors.Length; i++ ) {
				colors[i] = UnpackNormal( colors[i] );
			}
			t.SetPixels( colors );
			t.Apply();
		}

		public Color UnpackNormal( Color c ) {
			Vector3 normal = Vector3.zero;

			normal = new Vector2( c.a, c.g ) * 2f - Vector2.one;
			normal.z = Mathf.Sqrt( 1f - normal.x * normal.x - normal.y * normal.y );

			// TODO: Check color clamp method!
			return SF_Tools.VectorToColor( normal );
		}



		public override bool Draw() {
			ProcessInput();
			DrawHighlight();
			PrepareWindowColor();
			DrawWindow();
			ResetWindowColor();
			return !CheckIfDeleted();
		}

		public override void OnDelete() {
			textureAsset = null;
		}

		public override void NeatWindow(  ) {

			GUI.skin.box.clipping = TextClipping.Overflow;
			GUI.BeginGroup( rect );

			if( Event.current.type == EventType.DragPerform && rectInner.Contains(Event.current.mousePosition) ) {
				if( DragAndDrop.objectReferences[0].GetType() == typeof( Texture2D ) ) {
					Event.current.Use();
					textureAsset = DragAndDrop.objectReferences[0] as Texture2D;
					OnAssignedTexture();
				}
			}
			
			if(Event.current.type == EventType.dragUpdated) {
				if(DragAndDrop.objectReferences.Length > 0){
					if( DragAndDrop.objectReferences[0].GetType() == typeof( Texture2D ) ) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						editor.nodeBrowser.CancelDrag();
						Event.current.Use();
					} else {
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					}
				} else {
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}



			Color prev = GUI.color;
			if( textureAsset ) {
				GUI.color = Color.white;
				GUI.DrawTexture( rectInner, texture.Texture );
			} //else {
			//GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b,0.5f);
			//GUI.Label( rectInner, "Empty");
			//}

			if( showLowerPropertyBox ) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}

			GUI.color = prev;



			if( rectInner.Contains( Event.current.mousePosition ) && !SF_NodeConnector.IsConnecting() ) {
				Rect selectRect = new Rect( rectInner );
				selectRect.yMin += 80;
				selectRect.xMin += 40;

				if(GUI.Button( selectRect, "Select", EditorStyles.miniButton )){
					EditorGUIUtility.ShowObjectPicker<Texture2D>( textureAsset, false, "", this.id );
					Event.current.Use();
				}

			}

			
			if( Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == this.id ) {
				Event.current.Use();
				textureAsset = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
				OnAssignedTexture();
			}

			GUI.EndGroup();



		//	GUI.DragWindow();

			
			

			/*
			EditorGUI.BeginChangeCheck();
			textureAsset = (Texture)EditorGUI.ObjectField( rectInner, textureAsset, typeof( Texture ), false );
			if( EditorGUI.EndChangeCheck() ) {
				OnAssignedTexture();
			}
			 * */

		}

		public void OnAssignedTexture() {

			/*
			if( HasAlpha() ) {
				connectors[6].enableState = EnableState.Enabled;
				base.texture.CompCount = 4;
			} else {
				connectors[6].Disconnect();
				connectors[6].enableState = EnableState.Hidden;
				base.texture.CompCount = 3;
			}*/
			RefreshNoTexValueAndNormalUnpack();
			RenderToTexture();
			editor.shaderEvaluator.ApplyProperty( this );
			OnUpdateNode();
		}



		// TODO: Use a parent class, this looks ridiculous
		// TODO: Use a parent class, this looks ridiculous
		// TODO: Use a parent class, this looks ridiculous
		public void RefreshNoTexValueAndNormalUnpack(){
			bool newAssetIsNormalMap = false;
			
			string path = AssetDatabase.GetAssetPath( textureAsset );
			if( string.IsNullOrEmpty( path ) )
				newAssetIsNormalMap = false;
			else
				newAssetIsNormalMap = ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).normalmap;
			
			if(newAssetIsNormalMap){
				noTexValue = NoTexValue.Bump;
				markedAsNormalMap = true;
			} else if( noTexValue == NoTexValue.Bump){
				noTexValue = NoTexValue.Black;
				markedAsNormalMap = false;
			}
			
		}


		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			Rect tmp = lowerRect;
			tmp.height = 16f;
			noTexValue = (NoTexValue)SF_GUI.LabeledEnumField( tmp, "Default", noTexValue, EditorStyles.miniLabel );
			tmp.y += tmp.height;
			markedAsNormalMap = GUI.Toggle(tmp, markedAsNormalMap, "Normal map" );
			if(EditorGUI.EndChangeCheck())
				OnUpdateNode();
		}


		public override string SerializeSpecialData() {
			if( textureAsset == null )
				return null;
			return "tex:" + SF_Tools.AssetToGUID( textureAsset );
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "tex":
					textureAsset = (Texture)SF_Tools.GUIDToAsset( value, typeof( Texture ) );
					OnAssignedTexture();
					break;
			}
		}


	}
}