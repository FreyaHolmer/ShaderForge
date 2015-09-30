using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDXY : SF_Node_Arithmetic {

		public SFN_DDXY() {
		}

		public override void Initialize() {
			base.Initialize( "DDXY" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "fwidth(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return GetInputData( "IN", x, y, c );
		}

	}
}