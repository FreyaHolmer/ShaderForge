using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Sign : SF_Node_Arithmetic {

		public SFN_Sign() {
		}

		public override void Initialize() {
			base.Initialize( "Sign" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "sign(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float v = GetInputData( "IN", x, y, c );
			if( v == 0 )
				return 0f;
			return v > 0f ? 1f : -1f;
		}

	}
}