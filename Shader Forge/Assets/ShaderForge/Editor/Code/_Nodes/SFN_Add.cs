using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Add : SF_Node_Arithmetic {

		public SFN_Add() {

		}

		public override void Initialize() {
			base.Initialize( "Add" );
			base.PrepareArithmetic();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" + GetConnectorByStringID( "A" ).TryEvaluate() + "+" + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return GetInputData( "A", x, y, c ) + GetInputData( "B", x, y, c );
		}

	}
}