using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Cubemap : SF_Node {


		public Cubemap cubemapAsset;
		Texture2D textureAsset;


		public CubemapFace previewFace;

		public SFN_Cubemap() {

		}


		public override void Initialize() {
			base.Initialize( "Cubemap" );
			base.UseLowerPropertyBox( true, true );
			base.texture.CompCount = 4;
			property = ScriptableObject.CreateInstance<SFP_Cubemap>().Initialize( this );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"DIR","DIR",ConType.cInput,ValueType.VTv3),
				SF_NodeConnector.Create(this,"MIP","MIP",ConType.cInput,ValueType.VTv1),
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
		}


		public override bool IsUniformOutput() {
			return false;
		}

		// TODO: MIP selection
		public override string Evaluate( OutChannel channel = OutChannel.All ) {


			//float3 reflectDirection = reflect( -viewDirection, normalDirection );

			string DIR = GetInputIsConnected( "DIR" ) ? GetConnectorByStringID( "DIR" ).TryEvaluate() : "viewReflectDirection";
			string func = GetInputIsConnected( "MIP" ) ? "texCUBElod" : "texCUBE";

			if( GetInputIsConnected( "MIP" ) ) {
				DIR = "float4(" + DIR + "," + GetConnectorByStringID( "MIP" ).TryEvaluate() + ")";
			}

			string s = func + "(" + property.GetVariable() + "," + DIR + ")";

			return s;
		}

		// TODO: EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);





		public void RenderToTexture() {

			if( cubemapAsset == null ) {
				Debug.Log( "Cubemap asset missing" );
				return;
			}

			Texture2D tex = new Texture2D( cubemapAsset.width, cubemapAsset.height, TextureFormat.ARGB32, false );
			try{
				tex.SetPixels( cubemapAsset.GetPixels( previewFace ) );
			} catch( Exception e ) {
				Debug.LogWarning("Cubemap texture preview failed: " + e.ToString());
			}
			
			tex.Apply();


			RenderTexture rt = new RenderTexture( cubemapAsset.width, cubemapAsset.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default );
			rt.Create();
			Graphics.Blit( tex/*cubemapAsset/*GetTextureFaceAsset(CubemapFace.PositiveZ)*/, rt );
			RenderTexture.active = rt;
			// The data is now in the RT, in an arbitrary res
			// TODO: Sample it with normalized coords down into a 128x128
			// Save it temporarily in a texture
			Texture2D temp = new Texture2D( cubemapAsset.width, cubemapAsset.height, TextureFormat.ARGB32, false );
			temp.ReadPixels( new Rect( 0, 0, cubemapAsset.width, cubemapAsset.height ), 0, 0 );

			RenderTexture.active = null;
			rt.Release(); // Remove RT
			texture.ReadData( temp ); // Read Data from temp texture
			UnityEngine.Object.DestroyImmediate( temp ); // Destroy temp texture

		}


		public Texture GetTextureFaceAsset( CubemapFace face ) {
			if( cubemapAsset == null )
				return null;

			// Reflection of this:
			// TextureUtil.GetSourceTexture(Cubemap cubemapRef, CubemapFace face);
			Debug.Log( "GET FACE ASSET:" );
			Type textureUtil = Type.GetType( "UnityEditor.TextureUtil,UnityEditor" );
			Debug.Log( "textureUtil = " + textureUtil );
			BindingFlags bfs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo getSourceTexture = textureUtil.GetMethod( "GetSourceTexture", bfs, null, new Type[] { typeof( Cubemap ), typeof( CubemapFace ) }, null );
			Debug.Log( "getSourceTexture = " + getSourceTexture );
			Texture2D tex = (Texture2D)getSourceTexture.Invoke( null, new object[] { cubemapAsset, face } );
			Debug.Log( "tex = " + tex );

			return tex;
		}

		public override void OnDelete() {
			cubemapAsset = null;
		}


		public override bool Draw() {
			ProcessInput();
			DrawHighlight();
			PrepareWindowColor();
			DrawWindow();
			ResetWindowColor();
			return true;//!CheckIfDeleted();
		}

		public override void NeatWindow( ) {

			GUI.skin.box.clipping = TextClipping.Overflow;

			GUI.BeginGroup( rect );

			EditorGUI.BeginChangeCheck();
			DrawLowerPropertyBox();
			bool changedFace = EditorGUI.EndChangeCheck();

			//GUI.DragWindow();

			EditorGUI.BeginChangeCheck();

			Cubemap newCubemap = (Cubemap)EditorGUI.ObjectField( rectInner, cubemapAsset, typeof( Cubemap ), false );
			if(newCubemap != cubemapAsset){
				if(newCubemap == null){
					UndoRecord("unassign cubemap from " + property.nameDisplay);
				} else {
					UndoRecord("switch cubemap to " + newCubemap.name + " in " + property.nameDisplay);
				}
				cubemapAsset = newCubemap;
			}

			

			if( changedFace || EditorGUI.EndChangeCheck() ) {
				RenderToTexture();
				OnUpdateNode();
			}
			GUI.EndGroup();

		}

		public override void DrawLowerPropertyBox() {
			PrepareWindowColor();
			previewFace = (CubemapFace)UndoableEnumPopup(lowerRect, previewFace, "switch displayed cubemap face");
			//previewFace = (CubemapFace)EditorGUI.EnumPopup( lowerRect, previewFace );
			ResetWindowColor();
		}

		public override string SerializeSpecialData() {
			string s = property.Serialize() + ",";
			if( cubemapAsset != null )
				s += "cube:" + SF_Tools.AssetToGUID( cubemapAsset ) + ",";
			s += "pvfc:" + (int)previewFace;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize( key, value );
			switch( key ) {
				case "cube":
					cubemapAsset = (Cubemap)SF_Tools.GUIDToAsset( value, typeof( Cubemap ) );
					break;
				case "pvfc":
					previewFace = (CubemapFace)int.Parse( value );
					break;
			}
			if( cubemapAsset != null )
				RenderToTexture();

		}


	}
}
