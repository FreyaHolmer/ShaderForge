using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Distance : SF_Node_Arithmetic {

		public SFN_Distance() {

		}

		public override void Initialize() {
			base.Initialize( "Distance" );
			base.PrepareArithmetic(2,ValueType.VTvPending,ValueType.VTv1);
			( base.conGroup as SFNCG_Arithmetic ).LockOutType();
			
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "distance(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Color NodeOperator( int x, int y ) {

			switch( Mathf.Max( GetInputData( "A" ).CompCount, GetInputData( "B" ).CompCount ) ) {
				case 1:
					return SF_Tools.FloatToColor( Mathf.Abs( GetInputData( "A" )[x, y, 0] - GetInputData( "B" )[x, y, 0] ) );
				case 2:
					return SF_Tools.FloatToColor( ( ( (Vector2)GetInputData( "A" )[x, y] ) - ( (Vector2)GetInputData( "B" )[x, y] ) ).magnitude );
				case 3:
					return SF_Tools.FloatToColor( ( ( (Vector3)GetInputData( "A" )[x, y] ) - ( (Vector3)GetInputData( "B" )[x, y] ) ).magnitude );
				default:
					return SF_Tools.FloatToColor( ( GetInputData( "A" )[x, y] - GetInputData( "B" )[x, y] ).magnitude );
			}

		}


	}
}