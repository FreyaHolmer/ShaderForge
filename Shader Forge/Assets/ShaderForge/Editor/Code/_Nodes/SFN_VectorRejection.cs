using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_VectorRejection : SF_Node_Arithmetic {
		
		public SFN_VectorRejection() {

		}

		public override void Initialize() {
			base.Initialize( "Vector Reject." );
			base.PrepareArithmetic(2);
			GetConnectorByStringID("A").usageCount = 2;
			GetConnectorByStringID("B").usageCount = 4;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string a = GetConnectorByStringID( "A" ).TryEvaluate();
			string b = GetConnectorByStringID( "B" ).TryEvaluate();

			string dotLeft = b + " * dot(" + a + "," + b + ")";
			string dotRight = "dot(" + b + "," + b + ")";

			string retStr = "(" + a + " - " + "(" + dotLeft + "/" + dotRight + "))";
			
			return retStr;
		}

		public override Color NodeOperator( int x, int y ) {

			Vector4 a = GetInputData( "A" )[x, y];
			Vector4 b = GetInputData( "B" )[x, y];

			int cc = Mathf.Max(GetInputCon( "A" ).GetCompCount(), GetInputCon( "B" ).GetCompCount());

			float dotLeft = SF_Tools.Dot( a, b, cc );
			float dotRight = SF_Tools.Dot( b, b, cc );

			Vector4 retVec = a - (dotLeft/dotRight) * b;

			return retVec;
		}

	}
}