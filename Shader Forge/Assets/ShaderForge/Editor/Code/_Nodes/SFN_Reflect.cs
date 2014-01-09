using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Reflect : SF_Node_Arithmetic {

		public SFN_Reflect() {

		}

		public override void Initialize() {
			base.Initialize( "Reflect" );
			base.PrepareArithmetic( 2 );
			connectors[1].label = "I";
			connectors[2].label = "N";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "reflect(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Color NodeOperator( int x, int y ) {
			Color i = GetInputData( "A" )[x, y];
			Color n = GetInputData( "B" )[x, y];
			float dot = SF_Tools.Dot(i, n);
			return i - 2 * n * dot;
		}


	}
}