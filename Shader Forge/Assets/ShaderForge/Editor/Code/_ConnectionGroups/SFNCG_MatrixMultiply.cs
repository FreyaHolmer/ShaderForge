using UnityEngine;
using System.Collections;


namespace ShaderForge {

	// Used to detect types based on input
	[System.Serializable]
	public class SFNCG_MatrixMultiply : SF_NodeConnectionGroup {



		public SFNCG_MatrixMultiply Initialize( SF_NodeConnector output, params SF_NodeConnector[] inputs ) {
			this.output = output;
			this.inputs = inputs;
			return this;
		}

		public override void Refresh() {

			// ALLOWED COMBOS
			/*
			 * m v = v
			 * v m = v
			 * m m = m
			 */

			// Are none of the inputs connected? In that case, it's all default
			if( NoInputsConnected() ) {
				inputs[0].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				inputs[1].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				output.SetValueTypeAndDefault( ValueType.VTv4m4x4 );
			}
				

			//if( !inputs[0].node.InputsConnected() )
			//	return;

			bool aCon = inputs[0].IsConnected();
			bool bCon = inputs[1].IsConnected();

			bool oneConnected = aCon != bCon;


			if(aCon && bCon){
				ValueType aType = inputs[0].inputCon.valueType;
				ValueType bType = inputs[1].inputCon.valueType;

				if( aType == ValueType.VTv4 && bType == ValueType.VTm4x4 ){
					inputs[0].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
					inputs[1].SetValueTypeAndDefault( ValueType.VTm4x4 );
					output.SetValueTypeAndDefault( ValueType.VTv4 );
				} else if( aType == ValueType.VTm4x4 && bType == ValueType.VTv4 ){
					inputs[0].SetValueTypeAndDefault( ValueType.VTm4x4);
					inputs[1].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
					output.SetValueTypeAndDefault( ValueType.VTv4 );
				} else if( aType == ValueType.VTm4x4 && bType == ValueType.VTm4x4 ){
					inputs[0].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
					inputs[1].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
					output.SetValueTypeAndDefault( ValueType.VTm4x4 );
				} else {
					Debug.LogError( "Invalid input in Matrix multiply" );
					inputs[0].Disconnect();
					output.SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				}
			} else if( oneConnected ) {

				SF_NodeConnector connected = aCon ? inputs[0] : inputs[1];
				SF_NodeConnector unconnected = aCon ? inputs[1] : inputs[0];

				ValueType conType = connected.valueType;

				if(conType == ValueType.VTv4){
					unconnected.SetValueTypeAndDefault( ValueType.VTm4x4);
					output.SetValueTypeAndDefault( ValueType.VTv4 );
				} else {
					unconnected.SetValueTypeAndDefault( ValueType.VTv4m4x4 );
					output.SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				}
			} else {
				inputs[0].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				inputs[1].SetValueTypeAndDefault( ValueType.VTv4m4x4 );
				output.SetValueTypeAndDefault( ValueType.VTv4m4x4 );
			}

		}

		
	}
}