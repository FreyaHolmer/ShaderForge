using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Tan : SF_Node_Arithmetic {

		public SFN_Tan() {
		}

		public override void Initialize() {
			base.Initialize( "Tan" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "tan(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Tan( GetInputData( "IN", x, y, c ) );
		}

	}
}