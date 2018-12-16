using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Fmod : SF_Node_Arithmetic {

		public SFN_Fmod() {
		}

		public override void Initialize() {
			base.Initialize( "Fmod" );
			base.PrepareArithmetic( 2 );
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "fmod(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			float a = GetInputData( "A", c );
			float b = GetInputData( "B", c );
			float r = SF_Tools.Frac(Mathf.Abs(a/b))*Mathf.Abs(b);
			return ( a < 0 ) ? -r : r;
		}

	}
}