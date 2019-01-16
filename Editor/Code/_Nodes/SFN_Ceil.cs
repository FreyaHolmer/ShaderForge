using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Ceil : SF_Node_Arithmetic {

		public SFN_Ceil() {
		}

		public override void Initialize() {
			base.Initialize( "Ceil" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "ceil(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Ceil( GetInputData( "IN", c ) );
		}

	}
}