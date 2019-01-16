using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_NormalBlend : SF_Node {

		// SF_Node tNode;

		public SFN_NormalBlend() {

		}


		public override void Initialize() {
			base.Initialize( "Normal Blend" );
			base.showColor = true;
			UseLowerReadonlyValues( false );
			base.alwaysDefineVariable = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			texture.CompCount = 3;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false),
				SF_NodeConnector.Create(this,"BSE","Base",ConType.cInput,ValueType.VTv3,false).SetRequired(true),
				SF_NodeConnector.Create(this,"DTL","Det.",ConType.cInput,ValueType.VTv3,false).SetRequired(true)
			};

			//extraWidthInput = 5;

		}
		

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "BSE" ).uniform && GetInputData( "DTL" ).uniform );
		}

		public override int GetEvaluatedComponentCount() {
			return 3;
		}

		public string BaseNrm() {
			return GetVariableName() + "_nrm_base";
		}
		public string DetailNrm() {
			return GetVariableName() + "_nrm_detail";
		}
		public string CombinedNrm() {
			return GetVariableName() + "_nrm_combined";
		}

		/*
			float3 t = nrmBase + float3(0, 0, 1);
			float3 u = nrmDetail * float3(-1, -1, 1);
			float3 rnm = t*dot(t, u)/t.z - u;
		*/

		public override string[] GetBlitOutputLines() {
			return new string[] { 
				"float3 bse = _bse.xyz + float3(0,0,1);",
				"float3 dtl = _dtl.xyz * float3(-1,-1,1);",
				"float4(bse*dot(bse, dtl)/bse.z - dtl,0)"
			};
		}

		public override string[] GetPreDefineRows() {
			return new string[] {
				"float3 " + BaseNrm() + 	" = " + this["BSE"].TryEvaluate() + " + float3(0,0,1);",
				"float3 " + DetailNrm() + 	" = " + this["DTL"].TryEvaluate() + " * float3(-1,-1,1);",
				"float3 " + CombinedNrm() + " = " + BaseNrm() + "*dot(" + BaseNrm() + ", " + DetailNrm() + ")/" + BaseNrm() + ".z - " + DetailNrm() + ";"
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return CombinedNrm();
		}
		
		public override Vector4 EvalCPU() {

			Vector3 bse = (Vector3)GetInputData( "BSE" ).dataUniform + new Vector3(0,0,1);
			Vector3 dtl = Vector3.Scale( (Vector3)GetInputData( "DTL" ).dataUniform, new Vector3(-1,-1,1));

			Vector3 cmb = bse*Vector3.Dot(bse, dtl)/bse.z - dtl;

			return cmb;
		}




	}
}