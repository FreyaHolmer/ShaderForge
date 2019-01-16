using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Length : SF_Node_Arithmetic {

		public SFN_Length() {
		}

		public override void Initialize() {
			base.Initialize( "Length" );
			base.PrepareArithmetic( 1, ValueType.VTvPending, ValueType.VTv1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
			( base.conGroup as SFNCG_Arithmetic ).LockOutType();
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "length(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override Vector4 EvalCPU() {

			switch( GetInputData( "IN" ).CompCount ) {
				case 1:
					return Vector4.one * Mathf.Abs( GetInputData( "IN" ).dataUniform[0] );
				case 2:
					return Vector4.one * ( (Vector2)GetInputData( "IN" ).dataUniform ).magnitude;
				case 3:
					return Vector4.one * ( (Vector3)GetInputData( "IN" ).dataUniform ).magnitude;
				default:
					return Vector4.one * GetInputData( "IN" ).dataUniform.magnitude;
			}

		}

	}
}