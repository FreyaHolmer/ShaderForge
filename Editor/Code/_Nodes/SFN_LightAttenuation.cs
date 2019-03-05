using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_LightAttenuation : SF_Node {


		public SFN_LightAttenuation() {

		}

		public override void Initialize() {
			base.Initialize( "Light Atten.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 1;
			base.neverDefineVariable = true;
			base.availableInDeferredPrePass = false;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1,false)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 1f, 1f, 1f, 1f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "attenuation";
		}

	}
}