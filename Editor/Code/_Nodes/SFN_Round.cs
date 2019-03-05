using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Round : SF_Node_Arithmetic {

		public SFN_Round() {
		}

		public override void Initialize() {
			base.Initialize( "Round" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "round(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Round( GetInputData( "IN", c ) );
		}

	}
}