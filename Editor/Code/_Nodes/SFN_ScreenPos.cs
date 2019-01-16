using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ScreenPos : SF_Node {

	
		public enum ScreenPosType { Normalized = 0, Tiled = 1, SceneUVs = 2 };
		public ScreenPosType currentType = ScreenPosType.Normalized;

		public SFN_ScreenPos() {

		}

		public override void Initialize() {
			base.Initialize( "Screen Pos.", InitialPreviewRenderMode.BlitQuad );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			UpdateIcon();
			base.texture.CompCount = 2;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false).Outputting(OutChannel.RG),
				SF_NodeConnector.Create(this,"U","U",ConType.cOutput,ValueType.VTv1).WithColor(Color.red).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"V","V",ConType.cOutput,ValueType.VTv1).WithColor(Color.green).Outputting(OutChannel.G)
			};
		}

		public void UpdateIcon() {
			base.texture.SetIconId( (int)currentType );
		}

		/*
		public override Vector4 NodeOperator() {
			return new Color( Screen.width - base.rect.x + x * 0.66666f, Screen.height - base.rect.y + y * 0.66666f, 0, 0 );
		}
		*/

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			//currentType = (ScreenPosType)EditorGUI.EnumPopup( lowerRect, currentType );
			currentType = (ScreenPosType)UndoableEnumPopup(lowerRect, currentType, "switch screen position type");
			if( EditorGUI.EndChangeCheck() ) {
				UpdateIcon();
				OnUpdateNode();
			}
				
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			// NeedSceneUVs()
			switch(currentType){
			case ScreenPosType.Normalized:
				return "(sceneUVs * 2 - 1)";
			case ScreenPosType.Tiled:
				return "float2((sceneUVs.x * 2 - 1)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2 - 1)";
			case ScreenPosType.SceneUVs:
				return "sceneUVs";
			}
			Debug.LogError("Invalid screen position category");
			return "";
		}

		public override string SerializeSpecialData() {
			return "sctp:" + (int)currentType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "sctp":
					currentType = (ScreenPosType)int.Parse( value );
					UpdateIcon();
					break;
			}
		}

	}
}