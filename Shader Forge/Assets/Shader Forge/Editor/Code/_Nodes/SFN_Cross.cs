using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Cross : SF_Node_Arithmetic {

		public SFN_Cross() {

		}

		public override void Initialize() {
			base.Initialize( "Cross" );
			base.PrepareArithmetic( 2, ValueType.VTv3, ValueType.VTv3 );
		}

		public override int GetEvaluatedComponentCount() {
			return 3;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "cross(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Color NodeOperator( int x, int y ) {
			return SF_Tools.Cross( GetInputData( "A" )[x, y], GetInputData( "B" )[x, y] );
		}


	}
}