using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Sin : SF_Node_Arithmetic {

		public SFN_Sin() {
		}

		public override void Initialize() {
			base.Initialize( "Sin" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "sin(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Sin( GetInputData( "IN", x, y, c ) );
		}

	}
}