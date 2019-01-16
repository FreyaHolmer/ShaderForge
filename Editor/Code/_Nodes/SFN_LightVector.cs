using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_LightVector : SF_Node {


		public SFN_LightVector() {

		}

		public override void Initialize() {
			base.Initialize( "Light Dir.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			base.availableInDeferredPrePass = false;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lightDirection"; // normalize(_WorldSpaceLightPos0.xyz);
		}

	}
}