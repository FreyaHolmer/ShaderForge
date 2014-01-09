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
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "step(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float a = GetInputData( "A", x, y, c );
			float b = GetInputData( "B", x, y, c );
			return ((a <= b) ? 1.0f : 0.0f);
		}

	}
}