using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Cross : SF_Node_Arithmetic {

		public SFN_Cross() {

		}

		public override void Initialize() {
			base.Initialize( "Cross" );
			base.PrepareArithmetic( 2, ValueType.VTv3, ValueType.VTv3 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override int GetEvaluatedComponentCount() {
			return 3;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "float4(cross(_a.xyz,_b.xyz),0);" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "cross(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override Vector4 EvalCPU() {
			return SF_Tools.Cross( GetInputData( "A" ).dataUniform, GetInputData( "B" ).dataUniform );
		}


	}
}