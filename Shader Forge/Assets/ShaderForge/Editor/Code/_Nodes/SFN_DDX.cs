using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDX : SF_Node_Arithmetic {

		public SFN_DDX() {
		}

		public override void Initialize() {
			base.Initialize( "DDX" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "ddx(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return GetInputData( "IN", x, y, c );
		}

	}
}