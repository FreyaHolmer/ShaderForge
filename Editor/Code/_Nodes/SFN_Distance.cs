using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Distance : SF_Node_Arithmetic {

		public SFN_Distance() {

		}

		public override void Initialize() {
			base.Initialize( "Distance" );
			base.PrepareArithmetic( 2, ValueType.VTvPending, ValueType.VTv1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
			( base.conGroup as SFNCG_Arithmetic ).LockOutType();
			
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "distance(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Vector4 EvalCPU() {

			float dist = 0f;
			switch( Mathf.Max( GetInputData( "A" ).CompCount, GetInputData( "B" ).CompCount ) ) {
				case 1:
					dist = Mathf.Abs( GetInputData( "A" ).dataUniform[0] - GetInputData( "B" ).dataUniform[0] );
					break;
				case 2:
					dist = ( ( (Vector2)GetInputData( "A" ).dataUniform ) - ( (Vector2)GetInputData( "B" ).dataUniform ) ).magnitude;
					break;
				case 3:
					dist = ( ( (Vector3)GetInputData( "A" ).dataUniform ) - ( (Vector3)GetInputData( "B" ).dataUniform ) ).magnitude;
					break;
				default:
					dist = ( GetInputData( "A" ).dataUniform - GetInputData( "B" ).dataUniform ).magnitude;
					break;
			}

			return dist * Vector4.one;

		}


	}
}