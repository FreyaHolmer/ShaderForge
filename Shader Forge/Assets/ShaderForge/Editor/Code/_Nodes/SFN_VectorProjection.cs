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
			base.PrepareArithmetic( 2 );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			GetConnectorByStringID("A").usageCount = 1;
			GetConnectorByStringID("B").usageCount = 4;
		}

		public override string[] GetBlitOutputLines() {
			string dotLeft = "_b * dot(_a,_b)";
			string dotRight = "dot(_b,_b)";
			string retStr = "(" + dotLeft + "/" + dotRight + ")";
			return new string[] { retStr };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string a = GetConnectorByStringID( "A" ).TryEvaluate();
			string b = GetConnectorByStringID( "B" ).TryEvaluate();

			string dotLeft = b + " * dot(" + a + "," + b + ")";
			string dotRight = "dot(" + b + "," + b + ")";

			string retStr = "(" + dotLeft + "/" + dotRight + ")";
			
			return retStr;
		}

		public override Vector4 EvalCPU() {

			Vector4 a = GetInputData( "A" ).dataUniform;
			Vector4 b = GetInputData( "B" ).dataUniform;

			int cc = Mathf.Max(GetInputCon( "A" ).GetCompCount(), GetInputCon( "B" ).GetCompCount());

			float dotLeft = SF_Tools.Dot( a, b, cc );
			float dotRight = SF_Tools.Dot( b, b, cc );

			Vector4 retVec = (dotLeft/dotRight) * b;

			return retVec;
		}

	}
}