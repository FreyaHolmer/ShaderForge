using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Cos : SF_Node_Arithmetic {

		public SFN_Cos() {
		}

		public override void Initialize() {
			base.Initialize( "Cos" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "cos(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Cos( GetInputData( "IN", c ) );
		}

	}
}