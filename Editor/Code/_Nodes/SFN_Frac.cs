using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Frac : SF_Node_Arithmetic {

		public SFN_Frac() {
		}

		public override void Initialize() {
			base.Initialize( "Frac" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "frac(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return SF_Tools.Frac( GetInputData( "IN", c ) );
		}

	}
}