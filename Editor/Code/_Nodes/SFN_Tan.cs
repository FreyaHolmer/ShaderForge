using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Tan : SF_Node_Arithmetic {

		public SFN_Tan() {
		}

		public override void Initialize() {
			base.Initialize( "Tan" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "tan(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Tan( GetInputData( "IN", c ) );
		}

	}
}