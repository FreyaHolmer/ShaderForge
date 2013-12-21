using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ScreenPos : SF_Node {


		public enum ScreenPosType { Normalized = 0, Tiled = 1 };
		public ScreenPosType currentType = ScreenPosType.Normalized;

		public SFN_ScreenPos() {

		}

		public override void Initialize() {
			base.Initialize( "Screen Pos." );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			UpdateIcon();
			base.texture.CompCount = 2;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false).Outputting(OutChannel.RG),
				SF_NodeConnection.Create(this,"U","U",ConType.cOutput,ValueType.VTv1).WithColor(Color.red).Outputting(OutChannel.R),
				SF_NodeConnection.Create(this,"V","V",ConType.cOutput,ValueType.VTv1).WithColor(Color.green).Outputting(OutChannel.G)
			};
		}

		public void UpdateIcon() {
			base.texture.SetIconId( (int)currentType );
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( Screen.width - base.rect.x + x * 0.66666f, Screen.height - base.rect.y + y * 0.66666f, 0, 0 );
		}


		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			currentType = (ScreenPosType)EditorGUI.EnumPopup( lowerRect, currentType );
			if( EditorGUI.EndChangeCheck() ) {
				UpdateIcon();
				OnUpdateNode();
			}
				
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			if( currentType == ScreenPosType.Tiled)
				return "float2(i.screenPos.x*(_ScreenParams.r/_ScreenParams.g), i.screenPos.y)";
			return "i.screenPos";
			
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