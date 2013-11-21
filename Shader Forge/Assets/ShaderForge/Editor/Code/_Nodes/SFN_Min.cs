using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Min : SF_Node_Arithmetic {

		public SFN_Min() {

		}

		public override void Initialize() {
			base.Initialize( "Min" );
			base.PrepareArithmetic();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "min(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Min( GetInputData( "A" )[x, y, c], GetInputData( "B" )[x, y, c] );
		} 

	}
}