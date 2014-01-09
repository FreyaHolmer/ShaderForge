using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcTan2 : SF_Node_Arithmetic {

		public SFN_ArcTan2() {
		}

		public override void Initialize() {
			base.Initialize( "ArcTan2" );
			base.PrepareArithmetic(2);
			connectors[1].label = "y";
			connectors[2].label = "x";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "atan2(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Atan2( GetInputData( "A", x, y, c ), GetInputData( "B", x, y, c ) );
		}

	}
}