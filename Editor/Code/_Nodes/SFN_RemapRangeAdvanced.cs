using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_RemapRangeAdvanced : SF_Node_Arithmetic {
		

		public SFN_RemapRangeAdvanced() {

		}


		public override void Initialize() {
			base.Initialize( "Remap" );
			base.SearchName = "Remap";
			base.PrepareArithmetic( 5 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;



			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this, "OUT", "", ConType.cOutput, ValueType.VTvPending, false ),
				SF_NodeConnector.Create( this, "IN", "Val", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "IMIN", "iMin", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "IMAX", "iMax", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "OMIN", "oMin", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "OMAX", "oMax", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true )};
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize(connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5] );
			base.extraWidthInput = 6;
			GetConnectorByStringID("IMIN").usageCount = 2;
			GetConnectorByStringID("OMIN").usageCount = 2;

		}


		public override bool IsUniformOutput() {

			if(InputsConnected()){
				return ( GetInputData( "IN" ).uniform && GetInputData( "IMIN" ).uniform && GetInputData( "IMAX" ).uniform && GetInputData( "OMIN" ).uniform && GetInputData( "OMAX" ).uniform );
			}
			return true;


		}
		 



		public override void OnUpdateNode( NodeUpdateType updType = NodeUpdateType.Hard, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}


		public override string[] GetBlitOutputLines() {
			return new string[] {
				"(_omin + ( (_in - _imin) * (_omax - _omin) ) / (_imax - _imin))"
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string val = GetInputCon( "IN" ).Evaluate();
			string iMin = GetInputCon( "IMIN" ).Evaluate();
			string iMax = GetInputCon( "IMAX" ).Evaluate();
			string oMin = GetInputCon( "OMIN" ).Evaluate();
			string oMax = GetInputCon( "OMAX" ).Evaluate();

			return "(" + oMin + " + ( (" + val + " - " + iMin + ") * (" + oMax + " - " + oMin + ") ) / (" + iMax + " - " + iMin + "))";
		}

		// TODO Expose more out here!
		public override float EvalCPU( int c ) {
			float val = GetInputData( "IN", c );
			float iMin = GetInputData( "IMIN", c );
			float iMax = GetInputData( "IMAX", c );
			float oMin = GetInputData( "OMIN", c );
			float oMax = GetInputData( "OMAX", c );

			return oMin + ( (val - iMin) * (oMax - oMin) ) / (iMax - iMin);
		}






	}
}