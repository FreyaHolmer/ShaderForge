using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDY : SF_Node_Arithmetic {

		public SFN_DDY() {
		}

		public override void Initialize() {
			base.Initialize( "DDY" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "ddy(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return GetInputData( "IN", x, y, c );
		}

	}
}