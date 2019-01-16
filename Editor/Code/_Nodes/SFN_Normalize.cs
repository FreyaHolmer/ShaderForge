using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Normalize : SF_Node_Arithmetic {

		public SFN_Normalize() {
		}

		public override void Initialize() {
			base.Initialize( "Normalize" );
			base.PrepareArithmetic( 1 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "normalize(" + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		/*
		public override float NodeOperator( int c ) {
			return Mathf.Abs( GetInputData( 1, x, y, c ) );
		}*/

		public override Vector4 EvalCPU() {

			Vector4 v = GetInputData( "IN" ).dataUniform;

			switch( GetInputData( "IN" ).CompCount ) {
				case 1:
					float val = Mathf.Sign( v.x );
					return new Vector4( val, val, val, val );
				case 2:
					return (Vector4)((Vector2)v).normalized;
				case 3:
					return (Vector4)( (Vector3)v ).normalized;
				default:
					return v.normalized;
			}
		}

	}
}