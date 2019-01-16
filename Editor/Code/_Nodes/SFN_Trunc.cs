using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Trunc : SF_Node_Arithmetic {

		public SFN_Trunc() {
		}

		public override void Initialize() {
			base.Initialize( "Trunc" );
			base.PrepareArithmetic(1);
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "trunc(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			float val = GetInputData( "IN", c );
			return val < 0 ? -Mathf.Floor( -val ) : Mathf.Floor( val );
		}

	}
}