using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcTan : SF_Node_Arithmetic {

		public SFN_ArcTan() {
		}

		public override void Initialize() {
			base.Initialize( "ArcTan" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "atan(_in)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "atan(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Atan( GetInputData( "IN", c ) );
		}

	}
}