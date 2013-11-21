using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Max : SF_Node_Arithmetic {

		public SFN_Max() {

		}

		public override void Initialize() {
			base.Initialize( "Max" );
			base.PrepareArithmetic();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "max(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return Mathf.Max( GetInputData( "A" )[x, y, c], GetInputData( "B" )[x, y, c] );
		} 

	}
}