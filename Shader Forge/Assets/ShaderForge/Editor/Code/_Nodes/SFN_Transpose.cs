using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Transpose : SF_Node {

		public SFN_Transpose() {
		}

		public override void Initialize() {
			node_height = 58;
			base.Initialize( "Transpose" );
			base.showColor = false;
			base.UseLowerPropertyBox( false, true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTm4x4,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTm4x4,false).SetRequired(true)
			};
		}

		public override void NeatWindow() {
			PrepareWindowColor();
			GUI.BeginGroup( rect );
			Rect r = new Rect( rectInner );
			r = r.Pad( 4 );

			Rect texCoords = new Rect( r );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( r, SF_GUI.Handle_drag, texCoords, alphaBlend: true );

			GUI.EndGroup();
			ResetWindowColor();

		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "transpose(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override Vector4 EvalCPU() {
			return Color.black;
		}

		public override void RefreshValue() {
			RefreshValue( 1, 1 );
		}


	}
}