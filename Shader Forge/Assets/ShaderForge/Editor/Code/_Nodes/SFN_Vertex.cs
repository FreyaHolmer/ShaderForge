using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Vertex: SF_Node {


		public SFN_Vertex() {

		}

		public override void Initialize() {
			base.Initialize( "Vertex", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"X","X",ConType.cOutput,	ValueType.VTv1)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"Y","Y",ConType.cOutput,ValueType.VTv1)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"W","W",ConType.cOutput,ValueType.VTv1)	.Outputting(OutChannel.A)
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix( "vertexLocal" );
		}

	}
}