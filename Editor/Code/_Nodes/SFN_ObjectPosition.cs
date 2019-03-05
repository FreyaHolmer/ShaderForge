using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ObjectPosition : SF_Node {


		public SFN_ObjectPosition() {

		}

		public override void Initialize() {
			base.Initialize( "Object Pos." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"X","X",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnector.Create(this,"Y","Y",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green),
				SF_NodeConnector.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B).WithColor(Color.blue),
				SF_NodeConnector.Create(this,"W","W",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "objPos"; // normalize(_WorldSpaceLightPos0.xyz);
		}

	}
}