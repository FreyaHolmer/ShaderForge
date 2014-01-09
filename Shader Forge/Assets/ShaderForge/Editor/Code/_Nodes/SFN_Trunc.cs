using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Trunc : SF_Node_Arithmetic {

		public SFN_Trunc() {
		}

		public override void Initialize() {
			base.Initialize( "Trunc" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "trunc(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float val = GetInputData( "IN", x, y, c );
			return val < 0 ? -Mathf.Floor( -val ) : Mathf.Floor( val );
		}

	}
}