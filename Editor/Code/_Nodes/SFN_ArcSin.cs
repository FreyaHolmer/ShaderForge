using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcSin : SF_Node_Arithmetic {

		public SFN_ArcSin() {
		}

		public override void Initialize() {
			base.Initialize( "ArcSin" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "asin(_in)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "asin(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Asin( GetInputData( "IN", c ) );
		}

	}
}