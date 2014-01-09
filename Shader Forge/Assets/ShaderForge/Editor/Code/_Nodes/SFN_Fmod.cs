using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Fmod : SF_Node_Arithmetic {

		public SFN_Fmod() {
		}

		public override void Initialize() {
			base.Initialize( "Fmod" );
			base.PrepareArithmetic(2);
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "fmod(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float a = GetInputData( "A", x, y, c );
			float b = GetInputData( "B", x, y, c );
			float r = SF_Tools.Frac(Mathf.Abs(a/b))*Mathf.Abs(b);
			return ( a < 0 ) ? -r : r;
		}

	}
}