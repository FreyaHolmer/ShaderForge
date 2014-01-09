using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Clamp01 : SF_Node_Arithmetic {

		public SFN_Clamp01() {
		}

		public override void Initialize() {
			base.Initialize( "Clamp 0-1" );
			base.PrepareArithmetic(1);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "saturate(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Clamp01( GetInputData( "IN", x, y, c ) );
		}

	}
}