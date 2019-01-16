using UnityEngine;
using UnityEditor;
using System.Collections;




namespace ShaderForge {


	public enum NoTexValue{White, Gray, Black, Bump};

	[System.Serializable]
	public class SFN_Tex2d : SF_Node {


		public Texture textureAsset;

		public Texture TextureAsset {
			get {
				if(TexAssetConnected()){
					textureAsset = null;
					return ( GetInputCon( "TEX" ).node as SFN_Tex2dAsset ).textureAsset;
				}
				return textureAsset;
			}
			set {
				textureAsset = value;
			}
		}

		//public bool unpackNormal = false;
		public NoTexValue noTexValue = NoTexValue.White;
		public bool markedAsNormalMap = false;

		public SF_ShaderProperty shelvedProperty;


		public SFN_Tex2d() {

		}

		public override void Initialize() {
			base.Initialize( "Texture 2D" );
			//node_height = (int)(rect.height - 6f); // Odd, but alright...
			base.UseLowerPropertyBox( true, true );

		
			property = ScriptableObject.CreateInstance<SFP_Tex2d>().Initialize( this );
		

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnector.Create(this,"MIP","MIP",ConType.cInput,ValueType.VTv1),
				SF_NodeConnector.Create(this,"TEX","Tex",ConType.cInput,ValueType.TexAsset).WithColor(SF_Node.colorExposed),
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
			base.alwaysDefineVariable = true;
			base.neverDefineVariable = false;
			base.texture.CompCount = 4;
			connectors[0].usageCount = 2; // To define a variable of UVs to use with TRANSFORM_TEX

		}

		public override bool IsUniformOutput() {
			return false;
		}

		public bool IsNormalMap() {

			/*
			if( textureAsset != null ) {
				string path = AssetDatabase.GetAssetPath( textureAsset );
				if( string.IsNullOrEmpty( path ) )
					return false;
				else
					return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).normalmap;
			}


			if( property == null ) {
				if( GetInputIsConnected( "TEX" ) )
					return ( GetInputCon( "TEX" ).node as SFN_Tex2d ).IsNormalMap();
			} else {
				return ( property as SFP_Tex2d ).isBumpmap;
			}*/
			// TODO: Is this right?¨

			if(TexAssetConnected())
				return ( GetInputCon( "TEX" ).node as SFN_Tex2dAsset ).IsNormalMap();
			return markedAsNormalMap;
		}


		public bool TexAssetConnected(){
			if( property == null )
				if( GetInputIsConnected( "TEX" ) )
					return true;
			return false;
		}


		public override string GetBlitShaderSuffix() {

			bool uv = GetInputIsConnected( "UVIN" );
			bool mip = GetInputIsConnected( "MIP" );

			if( uv && mip ) {
				return "UV_MIP";
			} else if( mip ) {
				return "MIP";
			} else if( uv ){
				return "UV";
			} else {
				return "NoInputs";
			}

		}

		public override void PrepareRendering( Material mat ) {
			if( textureAsset != null ) {
				mat.mainTexture = textureAsset;
				mat.SetFloat( "_IsNormal", IsNormalMap() ? 1 : 0 );
			}
		}



		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			Rect tmp = lowerRect;
			tmp.height = 16f;
			noTexValue = (NoTexValue)UndoableLabeledEnumPopup(tmp, "Default", noTexValue, "swith default color of " + property.nameDisplay );
			tmp.y += tmp.height;
			bool preMarked = markedAsNormalMap;


			UndoableToggle(tmp, ref markedAsNormalMap, "Normal map", "normal map decode of " + property.nameDisplay, null);
			//markedAsNormalMap = GUI.Toggle(tmp, markedAsNormalMap, "Normal map" );

			if(EditorGUI.EndChangeCheck()){
				if(markedAsNormalMap && !preMarked)
					noTexValue = NoTexValue.Bump;
				UpdateCompCount();
				UpdateNormalMapAlphaState();
				OnUpdateNode();
			}
		}

		public void UpdateNormalMapAlphaState(){
			if(markedAsNormalMap){
				GetConnectorByStringID("A").Disconnect();
				GetConnectorByStringID("A").enableState = EnableState.Hidden;
			} else {
				GetConnectorByStringID("A").enableState = EnableState.Enabled; // No alpha channel when unpacking normals
			}
		}

		public override int GetEvaluatedComponentCount() {
			if( IsNormalMap() )
				return 3;
			return 4;
		}

		public bool HasAlpha() {
			if( TextureAsset == null ) return false;
			string path = AssetDatabase.GetAssetPath( TextureAsset );
			if( string.IsNullOrEmpty( path ) ) return false;
			return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).DoesSourceTextureHaveAlpha();
		}

		private void UpdateCompCount(){
			texture.CompCount = IsNormalMap() ? 3 : 4; // TODO: This doesn't work when opening shaders. Why?
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {


			UpdateCompCount();

			if( varDefined )
				return GetVariableName();


			bool useLOD = GetInputIsConnected( "MIP" ) || ( SF_Evaluator.inVert || SF_Evaluator.inTess );
			string uvStr = GetInputIsConnected( "UVIN" ) ? GetInputCon( "UVIN" ).Evaluate() : SF_Evaluator.WithProgramPrefix( SF_Evaluator.inFrag ? "uv0" : "texcoord0" );
			string func = useLOD ? "tex2Dlod" : "tex2D";
			string mip = GetInputIsConnected( "MIP" ) ? GetInputCon( "MIP" ).Evaluate() : "0";

			string variableName = this["TEX"].IsConnected() ? GetInputCon( "TEX" ).node.property.GetVariable() : property.GetVariable();

			bool useTilingLocally = IsProperty() && !property.tagNoScaleOffset;
			bool useTilingByAsset = this["TEX"].IsConnected() && !this["TEX"].inputCon.node.property.tagNoScaleOffset;
			if( useTilingLocally || useTilingByAsset )
				uvStr = "TRANSFORM_TEX(" + uvStr + ", " + variableName + ")";

			if( useLOD ) {
				uvStr = "float4(" + uvStr + ",0.0," + mip + ")";
			}


			string s = func + "(" + variableName + "," + uvStr + ")";
			if( IsNormalMap() ) {
				s = "UnpackNormal(" + s + ")";
			}

			return s;
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

			CheckPropertyInput();

			// If Tex is plugged in, make sure this uses the same asset and all
			if( property == null ) {

				SFN_Tex2dAsset inTex = ( GetInputCon( "TEX" ).node as SFN_Tex2dAsset );

				bool refresh = false;
				if( this.TextureAsset == null )
					refresh = true;
				if(!refresh)
					if( inTex.textureAsset != this.TextureAsset )
						refresh = true;

				if( refresh ) {
					this.TextureAsset = inTex.textureAsset;
					//RenderToTexture();
				}
			}


			ProcessInput();
			DrawHighlight();
			PrepareWindowColor();

			DrawWindow();
			ResetWindowColor();
			return true;//!CheckIfDeleted();
		}


		public void CheckPropertyInput() {
			if( property != null && connectors[2].IsConnected() ) {
				shelvedProperty = property;
				property = null;
				if( editor.nodeView.treeStatus.propertyList.Contains( this ) )
					editor.nodeView.treeStatus.propertyList.Remove( this );
			} else if( property == null && !connectors[2].IsConnected() ) {
				property = shelvedProperty;
				shelvedProperty = null;
				if( !editor.nodeView.treeStatus.propertyList.Contains( this ) )
					editor.nodeView.treeStatus.propertyList.Add( this );
			}
		}


		public override void OnDelete() {
			TextureAsset = null;
		}

		public override void NeatWindow(  ) {

			rect.height = TexAssetConnected() ? NODE_HEIGHT : NODE_HEIGHT + 34;

			GUI.skin.box.clipping = TextClipping.Overflow;
			GUI.BeginGroup( rect );

			if( IsProperty() && Event.current.type == EventType.DragPerform && rectInner.Contains(Event.current.mousePosition) ) {
				Object droppedObj = DragAndDrop.objectReferences[0];
				if( droppedObj is Texture2D || droppedObj is RenderTexture) {
					Event.current.Use();
					TextureAsset = droppedObj as Texture;
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



			Color prev = GUI.color;
			if( TextureAsset ) {
				GUI.color = Color.white;
				GUI.DrawTexture( rectInner, texture.texture, ScaleMode.StretchToFill, false ); // TODO: Doesn't seem to work
				if(displayVectorDataMask){
					GUI.DrawTexture( rectInner, SF_GUI.VectorIconOverlay, ScaleMode.ScaleAndCrop, true);
				}
			}

			if( showLowerPropertyBox && !TexAssetConnected()) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}
			
			//else {
			//GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b,0.5f);
			//GUI.Label( rectInner, "Empty");
			//}
			GUI.color = prev;



			if( IsProperty()){

				bool draw = rectInner.Contains( Event.current.mousePosition ) && !SF_NodeConnector.IsConnecting();

				Rect selectRect = new Rect( rectInner );
				selectRect.yMin += 80;
				selectRect.xMin += 40;
				Color c = GUI.color;
				GUI.color = new Color( 1, 1, 1, draw ? 1 : 0 );
				if(GUI.Button( selectRect, "Select", EditorStyles.miniButton )){
					EditorGUIUtility.ShowObjectPicker<Texture>( TextureAsset, false, "", this.id );
					Event.current.Use();
				}
				GUI.color = c;

			}


			if( IsProperty() && Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == this.id ) {
				Event.current.Use();
				Texture newTextureAsset = EditorGUIUtility.GetObjectPickerObject() as Texture;
				if(newTextureAsset != TextureAsset){
					if(newTextureAsset == null){
						UndoRecord("unassign texture of " + property.nameDisplay);
					} else {
						UndoRecord("switch texture to " + newTextureAsset.name + " in " + property.nameDisplay);
					}
					TextureAsset = newTextureAsset;
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
			CheckPropertyInput();
			base.RefreshValue(0,0);
			//RenderToTexture();
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


			UpdateNormalMapAlphaState();
			//RenderToTexture();
			editor.shaderEvaluator.ApplyProperty( this );
			OnUpdateNode(NodeUpdateType.Soft);
		}

		public void RefreshNoTexValueAndNormalUnpack(){
			bool newAssetIsNormalMap = false;
			
			string path = AssetDatabase.GetAssetPath( TextureAsset );
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
				UpdateNormalMapAlphaState();
			} else if( noTexValue == NoTexValue.Bump){
				noTexValue = NoTexValue.Black;
				markedAsNormalMap = false;
				UpdateNormalMapAlphaState();
			}

			UpdateCompCount();

		}


		public override string SerializeSpecialData() {
			string s = "";
			if( property != null )
				s += property.Serialize() + ",";

			if( TextureAsset != null )
				s += "tex:" + SF_Tools.AssetToGUID( TextureAsset ) + ",";
			s += "ntxv:" + ((int)noTexValue).ToString() + ",";
			s += "isnm:" + markedAsNormalMap.ToString();

			return s;

		}

		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize( key, value );
			switch( key ) {
			case "tex":
				TextureAsset = (Texture)SF_Tools.GUIDToAsset( value, typeof( Texture ) );
				OnAssignedTexture();
				break;
			case "ntxv":
				noTexValue = (NoTexValue)int.Parse(value);
				break;
			case "isnm":
				markedAsNormalMap = bool.Parse(value);
				UpdateNormalMapAlphaState();
				UpdateCompCount();
				break;
			}
		}


	}
}