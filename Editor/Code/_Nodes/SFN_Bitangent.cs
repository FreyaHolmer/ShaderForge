using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Bitangent : SF_Node {


		public SFN_Bitangent() {

		}

		public override void Initialize() {
			base.Initialize( "Bitangent Dir.", InitialPreviewRenderMode.BlitSphere  );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0f, 1f, 0f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix( "bitangentDir" );
		}

	}
}