using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Negate : SF_Node_Arithmetic {

		public SFN_Negate() {
		}

		public override void Initialize() {
			base.Initialize( "Negate" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "(-1*_in)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(-1*" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return -GetInputData( "IN", c );
		}

	}
}