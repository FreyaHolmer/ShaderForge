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
			base.Initialize( "Texture Asset", InitialPreviewRenderMode.BlitQuad );
			node_height = (int)(rect.height - 6f); // Odd, but alright...
			base.UseLowerPropertyBox( true, true );
			base.texture.CompCount = 4;
			base.showColor = true;
			neverDefineVariable = true;
			isFloatPrecisionBasedVariable = false;
			base.shaderGenMode = ShaderGenerationMode.Manual;
			//alwaysDefineVariable = true;
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

			string path = AssetDatabase.GetAssetPath( textureAsset );
			if( string.IsNullOrEmpty( path ) )
				return false;
			else{
				AssetImporter importer = UnityEditor.AssetImporter.GetAtPath( path );
				if(importer is TextureImporter)
					return ((TextureImporter)importer).textureType == TextureImporterType.NormalMap;
				else
					return false; // When it's a RenderTexture or ProceduralTexture
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

			//if( varDefined )
				return GetVariableName();
			//else
				//DefineVariable(); // This lags for some reason

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
			//Debug.LogError( "Invalid evaluation of " + property.name );
//			return "";
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
			if( IsGlobalProperty()){
				rect.height = (int)(NODE_HEIGHT + 16f + 2);
			} else {
				rect.height = (int)(NODE_HEIGHT + 32f + 2);
			}

			ProcessInput();
			DrawHighlight();
			PrepareWindowColor();
			DrawWindow();
			ResetWindowColor();
			return true;//!CheckIfDeleted();
		}

		public override void OnDelete() {
			textureAsset = null;
		}

		public override void NeatWindow(  ) {

			GUI.skin.box.clipping = TextClipping.Overflow;
			GUI.BeginGroup( rect );

			if(IsGlobalProperty()){
				GUI.enabled = false;
			}

			if( IsProperty() && Event.current.type == EventType.DragPerform && rectInner.Contains(Event.current.mousePosition) ) {
				Object droppedObj = DragAndDrop.objectReferences[0];
				if( droppedObj is Texture2D || droppedObj is RenderTexture) {
					Event.current.Use();
					textureAsset = droppedObj as Texture;
					OnAssignedTexture();
				}
			}
			
			if( IsProperty() && Event.current.type == EventType.DragUpdated ) {
				if(DragAndDrop.objectReferences.Length > 0){
					Object dragObj = DragAndDrop.objectReferences[0];
					if( dragObj is Texture2D || dragObj is RenderTexture) {
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

			if(IsGlobalProperty()){
				GUI.enabled = true;
			}



			Color prev = GUI.color;
			if( textureAsset ) {
				GUI.color = Color.white;
				GUI.DrawTexture( rectInner, texture.texture, ScaleMode.StretchToFill, false );
			} //else {
			//GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b,0.5f);
			//GUI.Label( rectInner, "Empty");
			//}

			if( showLowerPropertyBox ) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}

			GUI.color = prev;



			if( rectInner.Contains( Event.current.mousePosition ) && !SF_NodeConnector.IsConnecting() && !IsGlobalProperty() ) {
				Rect selectRect = new Rect( rectInner );
				selectRect.yMin += 80;
				selectRect.xMin += 40;

				if(GUI.Button( selectRect, "Select", EditorStyles.miniButton )){
					EditorGUIUtility.ShowObjectPicker<Texture>( textureAsset, false, "", this.id );
					Event.current.Use();
				}

			}

			
			if( !IsGlobalProperty() && Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == this.id ) {
				Event.current.Use();
				Texture newTextureAsset = EditorGUIUtility.GetObjectPickerObject() as Texture;
				if(newTextureAsset != textureAsset){
					if(newTextureAsset == null){
						UndoRecord("unassign texture of " + property.nameDisplay);
					} else {
						UndoRecord("switch texture to " + newTextureAsset.name + " in " + property.nameDisplay);
					}
					textureAsset = newTextureAsset;
					OnAssignedTexture();
				}

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

		public override void RefreshValue() {
			base.RefreshValue( 0, 0 );
			//RenderToTexture();
		}

		public override int GetEvaluatedComponentCount() {
			if( IsNormalMap() )
				return 3;
			return 4;
		}

		public override void PrepareRendering( Material mat ) {
			if( textureAsset != null ) {
				mat.mainTexture = textureAsset;
				mat.SetFloat( "_IsNormal", IsNormalMap() ? 1 : 0 );
			}
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
			else{
				AssetImporter importer = UnityEditor.AssetImporter.GetAtPath( path );
				if(importer is TextureImporter)
					newAssetIsNormalMap = ((TextureImporter)importer ).textureType == TextureImporterType.NormalMap;
				else
					newAssetIsNormalMap = false; // When it's a RenderTexture or ProceduralTexture
			}
			
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
			if(!IsGlobalProperty()){
				noTexValue = (NoTexValue)UndoableLabeledEnumPopup(tmp, "Default", noTexValue, "swith default color of " + property.nameDisplay );
				//noTexValue = (NoTexValue)SF_GUI.LabeledEnumField( tmp, "Default", noTexValue, EditorStyles.miniLabel );
				tmp.y += tmp.height;
			}
			bool preMarked = markedAsNormalMap;
			UndoableToggle(tmp, ref markedAsNormalMap, "Normal map", "normal map decode of " + property.nameDisplay, null);
			//markedAsNormalMap = GUI.Toggle(tmp, markedAsNormalMap, "Normal map" );
			if(EditorGUI.EndChangeCheck()){

				if(markedAsNormalMap && !preMarked)
					noTexValue = NoTexValue.Bump;
				OnUpdateNode();

			}

		}


		public override string SerializeSpecialData() {

			string s = property.Serialize() + ",";

			if( textureAsset != null )
				s += "tex:" + SF_Tools.AssetToGUID( textureAsset ) + ",";
			s += "ntxv:" + ( (int)noTexValue ).ToString() + ",";
			s += "isnm:" + markedAsNormalMap.ToString();

			return s;

		}

		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize( key, value );
			switch( key ) {
				case "tex":
					textureAsset = (Texture)SF_Tools.GUIDToAsset( value, typeof( Texture ) );
					OnAssignedTexture();
					break;
				case "ntxv":
					noTexValue = (NoTexValue)int.Parse( value );
					break;
				case "isnm":
					markedAsNormalMap = bool.Parse( value );
					break;
			}
		}


	}
}