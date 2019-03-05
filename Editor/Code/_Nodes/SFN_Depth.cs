using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Depth : SF_Node {


		public SFN_Depth() {

		}

		public override void Initialize() {
			base.Initialize( "Depth", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 1;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0.3f, 0.6f, 0.3f, 1f );
		}

		// (mul( UNITY_MATRIX_V, float4((_WorldSpaceCameraPos.rgb-i.posWorld.rgb),0) ).b - _ProjectionParams.g)

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "partZ";
		}

	}
}