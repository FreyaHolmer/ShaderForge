using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Step : SF_Node_Arithmetic {

		public SFN_Step() {
		}

		public override void Initialize() {
			base.Initialize( "Step (A <= B)" );
			base.SearchName = "Step";
			base.PrepareArithmetic(2);
			base.showLowerReadonlyValues = false;
			base.connectors[0].label = "<=";
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "step(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			float a = GetInputData( "B", c );
			float b = GetInputData( "B", c );
			return ((a <= b) ? 1.0f : 0.0f);
		}

	}
}