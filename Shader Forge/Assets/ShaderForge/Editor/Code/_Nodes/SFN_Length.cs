using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Length : SF_Node_Arithmetic {

		public SFN_Length() {
		}

		public override void Initialize() {
			base.Initialize( "Length" );
			base.PrepareArithmetic(1,ValueType.VTvPending,ValueType.VTv1);
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "length(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override Color NodeOperator( int x, int y ) {

			switch( GetInputData( "IN" ).CompCount ) {
				case 1:
					return SF_Tools.FloatToColor( Mathf.Abs( GetInputData( "IN" )[x, y, 0] ) );
				case 2:
					return SF_Tools.FloatToColor( ( (Vector2)GetInputData( "IN" )[x, y] ).magnitude );
				case 3:
					return SF_Tools.FloatToColor( ( (Vector3)GetInputData( "IN" )[x, y] ).magnitude );
				default:
					return SF_Tools.FloatToColor( GetInputData( "IN" )[x, y].magnitude );
			}

		}

	}
}