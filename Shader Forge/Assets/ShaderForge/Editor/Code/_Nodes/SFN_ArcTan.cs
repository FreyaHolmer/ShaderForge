using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcTan : SF_Node_Arithmetic {

		public SFN_ArcTan() {
		}

		public override void Initialize() {
			base.Initialize( "ArcTan" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "atan(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Atan( GetInputData( "IN", x, y, c ) );
		}

	}
}