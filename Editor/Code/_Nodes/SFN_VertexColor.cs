using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_VertexColor : SF_Node {


		public SFN_VertexColor() {

		}

		public override void Initialize() {
			base.Initialize( "Vertex Color", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix( "vertexColor" );
		}

	}
}