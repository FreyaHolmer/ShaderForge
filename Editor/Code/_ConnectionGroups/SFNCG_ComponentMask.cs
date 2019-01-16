using UnityEngine;
using System.Collections;


namespace ShaderForge {

	// Used to detect types based on input
	[System.Serializable]
	public class SFNCG_ComponentMask : SF_NodeConnectionGroup {



		public void Initialize( SF_NodeConnector output, params SF_NodeConnector[] inputs ) {
			this.output = output;
			this.inputs = inputs;
		}


		public int GetOutputComponentCount() {
			return inputs[0].inputCon.GetCompCount();// + (inputs[0].node as SFN_ComponentMask).outputComponentCount ;
		}


		public override void Refresh() {
			// Are none of the inputs connected? In that case, it's all default

			// ALLOWED COMBOS
			/*
			 * v1 v1 = v2
			 * v1 v2 = v3
			 * v1 v3 = v4
			 * v2 v1 = v3
			 * v2 v2 = v4
			 * v3 v1 = v4 
			 */

			if( NoInputsConnected() )
				ResetValueTypes();

			// If a matrix is plugged in here, disconnect it immediately
			if( inputs[0].inputCon.valueType == ValueType.VTm4x4 || inputs[0].inputCon.valueType == ValueType.VTv4m4x4 ) {
				inputs[0].Disconnect();
				return;
			}

			if( !inputs[0].node.InputsConnected() )
				return;

			int inCompSum = GetOutputComponentCount();

			if( inCompSum < 2 ) {
				Debug.LogError( "Input sum is somehow " + inCompSum + " on " + inputs[0].node.nodeName );
				inputs[1].Disconnect(); // This should never happen
				return;
			}

			if( inCompSum > 4 ) { // TODO: Error message
				Debug.LogWarning( "User connected vectors summing to " + inputs[0].node.nodeName );
				inputs[1].Disconnect();
				return;
			}


			switch( inCompSum ) {
				case 2:
					output.valueType = ValueType.VTv2;
					break;
				case 3:
					output.valueType = ValueType.VTv3;
					break;
				case 4:
					output.valueType = ValueType.VTv4;
					break;
			}






			/*
		

			// If any input is non-pending, use that as base to assign the rest.
			// Inputs:
			ValueType baseInType = GetBaseInputType();
			ValueType genericInType = GetGenericInputType();
			AssignToEmptyInputs( genericInType );

			// Output:
			if( InputsMissing() ) {
				if( baseInType == ValueType.VTv1 )
					output.valueType = ValueType.VTvPending;
				else
					output.valueType = baseInType;
			} else {
				output.valueType = GetDominantInputType();
			}
			*/
		}

		/*

		public ValueType GetGenericInputType() {
			ValueType vt = GetBaseInputType();
			switch( vt ) {
				case ValueType.VTv1:
					return ValueType.VTvPending;
				case ValueType.VTv2:
					return ValueType.VTv1v2;
				case ValueType.VTv3:
					return ValueType.VTv1v3;
				case ValueType.VTv4:
					return ValueType.VTv1v4;
				default:
					Debug.LogError( "Invalid attempt to get generic input type from " + vt );
					return ValueType.VTvPending;
			}
		}

		public ValueType GetDominantInputType() {
			ValueType dom = inputs[0].valueType;
			for( int i = 1 ; i < inputs.Length ; i++ ) {
				dom = GetDominantType( dom, inputs[i].valueType);
			}
			return dom;
		}

		public ValueType GetDominantType(ValueType a, ValueType b) {
			if( a == b )
				return a;

			if( a == ValueType.VTvPending )
				return b;

			if( b == ValueType.VTvPending )
				return a;

			if( a == ValueType.VTv1 ) {
				if( IsVectorType( b ) )
					return b;
				else
					return a;
			}
			if( b == ValueType.VTv1 ) {
				if( IsVectorType( a ) )
					return a;
				else
					return b;
			}

			Debug.LogError( "You should not be able to get here! Dominant pending type returned" );
			return ValueType.VTvPending;
		}


		public ValueType GetBaseInputType() {

			ValueType retType = ValueType.VTvPending;

			foreach( SF_NodeConnection nc in inputs ) {
				retType = GetDominantType( retType, nc.valueType );
			}

			if( retType == ValueType.VTvPending )
				Debug.LogError( "You should not be able to get here! Pending type returned" );
			return retType;
		}

		public static bool CompatibleTypes( ValueType tInput, ValueType tOutput ) {

			// If they are the same type, they are of course compatible
			if( tInput == tOutput )
				return true;
			// If the input is a pending vector, any output vector is compatible
			if( tInput == ValueType.VTvPending && IsVectorType( tOutput ) )
				return true;
			// Check multi-type for v1/v2
			if( tInput == ValueType.VTv1v2 && ( tOutput == ValueType.VTv1 || tOutput == ValueType.VTv2 ) )
				return true;
			// Check multi-type for v1/v3
			if( tInput == ValueType.VTv1v3 && ( tOutput == ValueType.VTv1 || tOutput == ValueType.VTv3 ) )
				return true;
			// Check multi-type for v1/v4
			if( tInput == ValueType.VTv1v4 && ( tOutput == ValueType.VTv1 || tOutput == ValueType.VTv4 ) )
				return true;
			// Didn't find any allowed link, return false
			return false;
		}
		*/
	}

}