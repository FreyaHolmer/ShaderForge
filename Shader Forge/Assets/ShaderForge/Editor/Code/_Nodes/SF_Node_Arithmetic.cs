using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {
	[System.Serializable]
	public class SF_Node_Arithmetic : SF_Node {

		public void PrepareArithmetic(int inputCount = 2, ValueType inputType = ValueType.VTvPending, ValueType outputType = ValueType.VTvPending) {
			base.showColor = true;
			UseLowerReadonlyValues( true );


			if( inputCount == 2 ) {
				connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create( this, "OUT", "", ConType.cOutput, outputType, false ),
				SF_NodeConnection.Create( this, "A", "A", ConType.cInput, inputType, false ).SetRequired( true ),
				SF_NodeConnection.Create( this, "B", "B", ConType.cInput, inputType, false ).SetRequired( true )};
				base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
			} else if( inputCount == 1 ){
				connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create( this, "OUT", "", ConType.cOutput, outputType, false ),
				SF_NodeConnection.Create( this, "IN", "", ConType.cInput, inputType, false ).SetRequired( true )};
				base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);
			}
			
		}

		public override int GetEvaluatedComponentCount() {
			int max = 0;
			foreach(SF_NodeConnection con in connectors){
				if( con.conType == ConType.cOutput )
					continue;
				max = Mathf.Max( max, con.GetCompCount() );
			}
			return max;
		}

		public override bool IsUniformOutput() {
			if( connectors.Length > 2)
				return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform );
			return ( GetInputData( "IN" ).uniform );
		}

		// New system
		public override void RefreshValue() {
			if( connectors.Length == 3 )
				RefreshValue( 1, 2 );
			RefreshValue( 1, 1 );
		}

	}
}