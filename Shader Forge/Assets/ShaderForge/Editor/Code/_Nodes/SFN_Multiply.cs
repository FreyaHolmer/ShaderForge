using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Multiply : SF_Node_Arithmetic {

		public SFN_Multiply() {

		}

		public override void Initialize() {
			base.Initialize( "Multiply" );
			base.PrepareArithmetic();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" +  GetConnectorByStringID("A").TryEvaluate() + "*" + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return GetInputData( "A", x, y, c ) * GetInputData( "B", x, y, c );
		}
	}
}