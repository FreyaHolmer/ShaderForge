using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Frac : SF_Node_Arithmetic {

		public SFN_Frac() {
		}

		public override void Initialize() {
			base.Initialize( "Frac" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "frac(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return SF_Tools.Frac( GetInputData( "IN", x, y, c ) );
		}

	}
}