using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcCos : SF_Node_Arithmetic {

		public SFN_ArcCos() {
		}

		public override void Initialize() {
			base.Initialize( "ArcCos" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "acos(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Acos( GetInputData( "IN", x, y, c ) );
		}

	}
}