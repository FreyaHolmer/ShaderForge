using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcSin : SF_Node_Arithmetic {

		public SFN_ArcSin() {
		}

		public override void Initialize() {
			base.Initialize( "ArcSin" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "asin(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Asin( GetInputData( "IN", x, y, c ) );
		}

	}
}