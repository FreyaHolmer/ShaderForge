using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_LightColor : SF_Node {


		public SFN_LightColor() {

		}

		public override void Initialize() {
			base.Initialize( "Light Color", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;
			base.availableInDeferredPrePass = false;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B).WithColor(Color.blue),
				SF_NodeConnector.Create(this,"A","A",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0.5f, 0, 0, 0 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "_LightColor0";
		}

	}
}