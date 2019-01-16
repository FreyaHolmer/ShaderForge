using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Tangent : SF_Node {


		public SFN_Tangent() {

		}

		public override void Initialize() {
			base.Initialize( "Tangent Dir.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 1f, 0f, 0f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix("tangentDir");
		}

	}
}