using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_FaceSign : SF_Node {


		public enum FaceSignType { ZeroAndOne = 0, PlusMinusOne = 1 };
		public static string[] faceSignTypeStr = new string[] { "1 and 0", "1 and -1" };
		public FaceSignType currentType = FaceSignType.ZeroAndOne;

		public SFN_FaceSign() {

		}

		public override void Initialize() {
			base.Initialize( "Face Sign", InitialPreviewRenderMode.BlitQuad );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			UpdateIcon();
			base.texture.CompCount = 1;
			base.neverDefineVariable = true;
			base.shaderGenMode = ShaderGenerationMode.Manual;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"VFACE","",ConType.cOutput,ValueType.VTv1,false)
			};
			texture.dataUniform = Color.white;
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public void UpdateIcon() {
			base.texture.SetIconId( (int)currentType );
		}

		public override Vector4 EvalCPU() {
			float v = 1;
			return new Color( v, v, v );
		}

		public override float EvalCPU( int c ) {
			return 1f;
		}

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			//currentType = (ScreenPosType)EditorGUI.EnumPopup( lowerRect, currentType );
			currentType = (FaceSignType)UndoableEnumPopupNamed(lowerRect, (int)currentType, faceSignTypeStr, "switch face sign type");
			if( EditorGUI.EndChangeCheck() ) {
				UpdateIcon();
				OnUpdateNode();
			}
				
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_BackfaceValue", currentType == FaceSignType.PlusMinusOne ? -1 : 0 );
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			// NeedSceneUVs()
			switch(currentType){
				case FaceSignType.ZeroAndOne:
					return "isFrontFace";
				case FaceSignType.PlusMinusOne:
					return "faceSign";
			}
			Debug.LogError("Invalid face sign category");
			return "";
		}

		public override string SerializeSpecialData() {
			return "fstp:" + (int)currentType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "fstp":
					currentType = (FaceSignType)int.Parse( value );
					UpdateIcon();
					break;
			}
		}

	}
}