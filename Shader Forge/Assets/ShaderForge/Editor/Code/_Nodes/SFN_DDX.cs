using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DDX : SF_Node_Arithmetic {

		public SFN_DDX() {
		}

		public override void Initialize() {
			base.Initialize( "DDX" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "ddx(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return GetInputData( "IN", c );
		}

	}
}