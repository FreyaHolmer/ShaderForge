using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Negate : SF_Node_Arithmetic {

		public SFN_Negate() {
		}

		public override void Initialize() {
			base.Initialize( "Negate" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(-1*" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return -GetInputData( "IN", x, y, c );
		}

	}
}