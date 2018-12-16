using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Subtract : SF_Node_Arithmetic {

		public SFN_Subtract() {

		}

		public override void Initialize() {
			base.Initialize( "Subtract" );
			base.PrepareArithmetic();
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "_a - _b" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" + GetConnectorByStringID( "A" ).TryEvaluate() + "-" + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return GetInputData( "A", c ) - GetInputData( "B", c );
		}

	}
}