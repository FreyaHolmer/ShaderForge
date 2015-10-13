using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDY : SF_Node_Arithmetic {

		public SFN_DDY() {
		}

		public override void Initialize() {
			base.Initialize( "DDY" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "ddy(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return GetInputData( "IN", c );
		}

	}
}