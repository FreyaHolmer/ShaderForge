using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Posterize : SF_Node_Arithmetic {

		public SFN_Posterize() {
		}

		public override void Initialize() {
			base.Initialize( "Posterize" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this, "OUT", "", ConType.cOutput, ValueType.VTvPending, false ),
				SF_NodeConnector.Create( this, "IN", "", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "STPS", "Steps", ConType.cInput, ValueType.VTv1, false ).SetRequired( true ).WithUseCount(2)
			};
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);
			base.extraWidthInput = 6;
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "IN" ).uniform && GetInputData( "STPS" ).uniform );
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "floor(_in * _stps) / (_stps - 1)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string mainInput = GetConnectorByStringID( "IN" ).TryEvaluate();
			string steps = GetConnectorByStringID( "STPS" ).TryEvaluate();
	

			return "floor(" + mainInput + " * " + steps + ") / (" + steps + " - 1)";
		}

		public override float EvalCPU( int c ) {
			float steps = GetInputData( "STPS", c );
			return Mathf.Floor( GetInputData( "IN", c ) * steps ) / (steps - 1);
		}

	}
}