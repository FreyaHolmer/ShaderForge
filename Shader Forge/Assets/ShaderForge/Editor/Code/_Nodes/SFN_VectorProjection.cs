using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_VectorProjection : SF_Node_Arithmetic {
		
		public SFN_VectorProjection() {

		}

		public override void Initialize() {
			base.Initialize( "Vector Project." );
			base.PrepareArithmetic(2);
			GetConnectorByStringID("A").usageCount = 1;
			GetConnectorByStringID("B").usageCount = 4;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string a = GetConnectorByStringID( "A" ).TryEvaluate();
			string b = GetConnectorByStringID( "B" ).TryEvaluate();

			string dotLeft = b + " * dot(" + a + "," + b + ")";
			string dotRight = "dot(" + b + "," + b + ")";

			string retStr = "(" + dotLeft + "/" + dotRight + ")";
			
			return retStr;
		}

		public override Color NodeOperator( int x, int y ) {

			Vector4 a = GetInputData( "A" )[x, y];
			Vector4 b = GetInputData( "B" )[x, y];

			float dotLeft = SF_Tools.Dot( a, b );
			float dotRight = SF_Tools.Dot( b, b );

			Vector4 retVec = (dotLeft/dotRight) * b;

			return retVec;
		}

	}
}