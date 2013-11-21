using UnityEngine;
using UnityEditor;
using System.Collections;


namespace ShaderForge {
	[System.Serializable]
	public class SFN_Tex2d : SF_Node {


		public Texture textureAsset;
		public SF_ShaderProperty shelvedProperty;

		public SFN_Tex2d() {

		}

		public override void Initialize() {
			base.Initialize( "Texture 2D" );
			base.UseLowerPropertyBox( false );
			
			property = ScriptableObject.CreateInstance<SFP_Tex2d>().Initialize( this );
		

			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnection.Create(this,"MIP","MIP",ConType.cInput,ValueType.VTv1),
				SF_NodeConnection.Create(this,"TEX","Tex",ConType.cInput,ValueType.TexAsset).WithColor(SF_Node.colorExposed),
				SF_NodeConnection.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnection.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnection.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnection.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnection.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
			base.texture.CompCount = 4;
		}

		public override bool IsUniformOutput() {
			return false;
		}

		public bool IsNormalMap() {

			if( textureAsset != null ) {
				string path = AssetDatabase.GetAssetPath( textureAsset );
				if( string.IsNullOrEmpty( path ) )
					return false;
				else
					return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).normalmap;
			}


			if( property == null ) {
				if( GetInputIsConnected( "TEX" ) )
					return ( GetInputCon( "TEX" ).node.property as SFP_Tex2d ).isBumpmap;
			} else {
				return ( property as SFP_Tex2d ).isBumpmap;
			}
			

			return false;
		}


		public bool HasAlpha() {
			if( textureAsset == null ) return false;
			string path = AssetDatabase.GetAssetPath( textureAsset );
			if( string.IsNullOrEmpty( path ) ) return false;
			return ( (TextureImporter)UnityEditor.AssetImporter.GetAtPath( path ) ).DoesSourceTextureHaveAlpha();
		}

		// TODO: MIP selection
		public override string Evaluate( OutChannel channel = OutChannel.All ) {


			texture.CompCount = IsNormalMap() ? 3 : 4;

			if( varDefined )
				return GetVariableName();


			bool useLOD = GetInputIsConnected( "MIP" ) || ( SF_Evaluator.inVert || SF_Evaluator.inTess );
			string uvStr = GetInputIsConnected( "UVIN" ) ? GetInputCon( "UVIN" ).Evaluate() : SF_Evaluator.WithProgramPrefix( "uv0.xy" );
			string func = useLOD ? "tex2Dlod" : "tex2D";
			string mip = GetInputIsConnected( "MIP" ) ? GetInputCon( "MIP" ).Evaluate() : "0";

			if( useLOD ) {
				uvStr = "float4(" + uvStr + ",0.0," + mip + ")";
			}



			string variableName = this["TEX"].IsConnected() ? GetInputCon( "TEX" ).node.property.GetVariable() : property.GetVariable();


			string s = func + "(" + variableName + "," + uvStr + ")";
			if( IsNormalMap() ) {
				s = "UnpackNormal(" + s + ")";
			}

			return s;
		}

		// TODO: EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
		public void RenderToTexture() {
			if( textureAsset == null ) {
				//Debug.Log("Texture asset missing");
				return;
			}

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

			if( IsNormalMap() ) {
				UnpackNormals( ref temp );
			}



			RenderTexture.active = null;
			rt.Release(); // Remove RT
			if( GetInputIsConnected( "UVIN" ) ) {
				// UVs
				texture.ReadData( temp, GetInputCon( "UVIN" ).node.texture ); // Read Data from temp texture
			} else
				texture.ReadData( temp );
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

			CheckPropertyInput();

			// If Tex is plugged in, make sure this uses the same asset and all
			if( property == null ) {

				SFN_Tex2dAsset inTex = ( GetInputCon( "TEX" ).node as SFN_Tex2dAsset );

				bool refresh = false;
				if( this.textureAsset == null )
					refresh = true;
				if(!refresh)
					if( inTex.textureAsset != this.textureAsset )
						refresh = true;

				if( refresh ) {
					this.textureAsset = inTex.textureAsset;
					RenderToTexture();
				}
			}


			ProcessInput();
			DrawHighlight();
			PrepareWindowColor();
			DrawWindow();
			ResetWindowColor();
			return !CheckIfDeleted();
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
			textureAsset = null;
		}

		public override void NeatWindow(  ) {

			GUI.skin.box.clipping = TextClipping.Overflow;
			GUI.BeginGroup( rect );

			if( IsProperty() && Event.current.type == EventType.DragPerform && rectInner.Contains(Event.current.mousePosition) ) {
				if( DragAndDrop.objectReferences[0].GetType() == typeof( Texture2D ) ) {
					Event.current.Use();
					textureAsset = DragAndDrop.objectReferences[0] as Texture2D;
					OnAssignedTexture();
				}
			}

			if( IsProperty() && Event.current.type == EventType.dragUpdated ) {
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
			}
			
			//else {
			//GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b,0.5f);
			//GUI.Label( rectInner, "Empty");
			//}
			GUI.color = prev;



			if( IsProperty() && rectInner.Contains( Event.current.mousePosition ) && !SF_NodeConnection.IsConnecting() ) {
				Rect selectRect = new Rect( rectInner );
				selectRect.yMin += 80;
				selectRect.xMin += 40;

				if(GUI.Button( selectRect, "Select", EditorStyles.miniButton )){
					EditorGUIUtility.ShowObjectPicker<Texture2D>( textureAsset, false, "", this.id );
					Event.current.Use();
				}

			}


			if( IsProperty() && Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == this.id ) {
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

		public override void RefreshValue() {
			CheckPropertyInput();
			RenderToTexture();
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

			//( property as SFP_Tex2d ).isBumpmap = ;
			RenderToTexture();
			editor.shaderEvaluator.ApplyProperty( this );
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