using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Sqrt : SF_Node_Arithmetic {

		public SFN_Sqrt() {
		}

		public override void Initialize() {
			base.Initialize( "Sqrt" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "sqrt(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Sqrt( GetInputData( "IN", x, y, c ) );
		}

	}
}