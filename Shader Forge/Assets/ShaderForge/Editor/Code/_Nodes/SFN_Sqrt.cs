using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Sqrt : SF_Node_Arithmetic {

		public SFN_Sqrt() {
		}

		public override void Initialize() {
			base.Initialize( "Sqrt" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "sqrt(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Sqrt( GetInputData( "IN", c ) );
		}

	}
}