using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Divide : SF_Node_Arithmetic {

		public SFN_Divide() {

		}

		public override void Initialize() {
			base.Initialize( "Divide" );
			base.PrepareArithmetic();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" + GetConnectorByStringID( "A" ).TryEvaluate() + "/" + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float a = GetInputData( "A", x, y, c );
			float b = GetInputData( "B", x, y, c );

			if( b == 0f ) {
				if( a == 0f )
					return 1f;
				return ( a > 0 ? float.MaxValue : float.MinValue );
			}
			return a / b;
		}


	}
}