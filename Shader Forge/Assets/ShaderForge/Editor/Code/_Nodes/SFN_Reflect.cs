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
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "reflect(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Vector4 EvalCPU() {
			Color i = GetInputData( "A" ).dataUniform;
			Color n = GetInputData( "B" ).dataUniform;
			int cc = Mathf.Max(GetInputCon( "A" ).GetCompCount(), GetInputCon( "B" ).GetCompCount());
			float dot = SF_Tools.Dot(i, n, cc);
			return i - 2 * n * dot;
		}


	}
}