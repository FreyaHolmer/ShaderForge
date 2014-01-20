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
			base.PrepareArithmetic(5);



			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this, "OUT", "", ConType.cOutput, ValueType.VTvPending, false ),
				SF_NodeConnector.Create( this, "IN", "Val", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "IMIN", "iMin", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "IMAX", "iMax", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "OMIN", "oMin", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "OMAX", "oMax", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true )};
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize(connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5] );

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

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string inVal = GetInputCon( "IN" ).Evaluate();

			string val = GetInputCon( "IN" ).Evaluate();
			string iMin = GetInputCon( "IMIN" ).Evaluate();
			string iMax = GetInputCon( "IMAX" ).Evaluate();
			string oMin = GetInputCon( "OMIN" ).Evaluate();
			string oMax = GetInputCon( "OMAX" ).Evaluate();

			return "(" + oMin + " + ( (" + val + " - " + iMin + ") * (" + oMax + " - " + oMin + ") ) / (" + iMax + " - " + iMin + "))";
		}

		// TODO Expose more out here!
		public override float NodeOperator( int x, int y, int c ) {
			float val = GetInputData( "IN", x, y, c );
			float iMin = GetInputData( "IMIN", x, y, c );
			float iMax = GetInputData( "IMAX", x, y, c );
			float oMin = GetInputData( "OMIN", x, y, c );
			float oMax = GetInputData( "OMAX", x, y, c );

			return oMin + ( (val - iMin) * (oMax - oMin) ) / (iMax - iMin);
		}






	}
}