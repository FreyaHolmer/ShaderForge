using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Abs : SF_Node_Arithmetic {

		public SFN_Abs() {
		}

		public override void Initialize() {
			base.Initialize( "Abs" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "abs(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Abs( GetInputData( "IN", c ) );
		}

	}
}