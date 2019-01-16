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
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" + GetConnectorByStringID( "A" ).TryEvaluate() + "/" + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "_a/_b" };
		}

		public override float EvalCPU( int c ) {
			float a = GetInputData( "A", c );
			float b = GetInputData( "B", c );

			if( b == 0f ) {
				if( a == 0f )
					return 1f;
				return ( a > 0 ? float.MaxValue : float.MinValue );
			}
			return a / b;
		}


	}
}