using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Abs : SF_Node_Arithmetic {

		public SFN_Abs() {
		}

		public override void Initialize() {
			base.Initialize( "Abs" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "abs(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Abs( GetInputData( "IN", x, y, c ) );
		}

	}
}