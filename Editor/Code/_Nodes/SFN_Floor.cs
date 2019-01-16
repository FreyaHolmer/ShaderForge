using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Floor : SF_Node_Arithmetic {

		public SFN_Floor() {
		}

		public override void Initialize() {
			base.Initialize( "Floor" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "floor(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Floor( GetInputData( "IN", c ) );
		}

	}
}