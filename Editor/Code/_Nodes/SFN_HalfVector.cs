using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_HalfVector : SF_Node {


		public SFN_HalfVector() {

		}

		public override void Initialize() {
			base.Initialize( "Half Dir.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.availableInDeferredPrePass = false;
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0.7071068f, 0f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "halfDirection"; // normalize(_WorldSpaceLightPos0.xyz);
		}

	}
}