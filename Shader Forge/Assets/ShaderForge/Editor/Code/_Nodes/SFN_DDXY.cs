using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDXY : SF_Node_Arithmetic {

		public SFN_DDXY() {
		}

		public override void Initialize() {
			base.Initialize( "DDXY" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "fwidth(_in)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "fwidth(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return GetInputData( "IN", c );
		}

	}
}