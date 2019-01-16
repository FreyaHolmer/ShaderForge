using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcCos : SF_Node_Arithmetic {

		public SFN_ArcCos() {
		}

		public override void Initialize() {
			base.Initialize( "ArcCos" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "acos(_in)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "acos(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return Mathf.Acos( GetInputData( "IN", c ) );
		}

	}
}