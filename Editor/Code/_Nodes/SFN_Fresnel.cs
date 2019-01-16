using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Fresnel : SF_Node {


		public SFN_Fresnel() {

		}

		public override void Initialize() {

			base.shaderGenMode = ShaderGenerationMode.ManualModal;
			base.Initialize( "Fresnel", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 1;
			base.vectorDataNode = true;
			

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"NRM","Nrm",ConType.cInput,ValueType.VTv3,false),
				SF_NodeConnector.Create(this,"EXP","Exp",ConType.cInput,ValueType.VTv1,false)
			}; 

			this["NRM"].unconnectedEvaluationValue = "normalDirection";

		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue(0,0);
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			return false;
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string[] GetModalModes() {
			return new string[]{
				"REQONLY",
				"NRM",
				"EXP",
				"NRM_EXP"
			};
		}

		public override string GetCurrentModalMode() {
			if( connectors == null )
				return "REQONLY";

			bool expCon = GetInputIsConnected( "EXP" );
			bool nrmCon = GetInputIsConnected( "NRM" );

			if( !expCon && !nrmCon )
				return "REQONLY";
			if( !expCon && nrmCon )
				return "NRM";
			if( expCon && !nrmCon )
				return "EXP";
			// if( expCon && nrmCon )
				return "NRM_EXP";
		}

		public override string[] GetBlitOutputLines( string mode ) {

			string nrmStr = mode.Contains( "NRM" ) ? "_nrm.xyz" : "normalDirection";

			string s = string.Format(  "1.0-max(0,dot({0}, viewDirection))", nrmStr );

			if( mode.Contains( "EXP" ) ) {
				s = string.Format( "pow( {0}, {1} )", s, "_exp.x" );
			}

			return new string[]{ s };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string dot = "1.0-max(0,dot(" + this["NRM"].TryEvaluate() + ", viewDirection))";

			if( GetInputIsConnected( "EXP" ) ) {
				return "pow(" + dot + "," + this["EXP"].TryEvaluate() + ")";
			}
			return "("+dot+")";

		}

	}
}