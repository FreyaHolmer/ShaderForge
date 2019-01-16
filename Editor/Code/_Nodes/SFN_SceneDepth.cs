using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_SceneDepth : SF_Node {


		public SFN_SceneDepth() {

		}

		public override void Initialize() {
			base.Initialize( "Scene Depth" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 1;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1),
				SF_NodeConnector.Create(this,"UV","UV",ConType.cInput,ValueType.VTv2)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0.3f, 0.6f, 0.3f, 1f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string infix = "";
			if( GetConnectorByStringID( "UV" ).IsConnectedAndEnabled() )
				infix = GetConnectorByStringID( "UV" ).TryEvaluate();
			else
				infix = "sceneUVs";
			return "max(0, LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, " + infix + ")) - _ProjectionParams.g)";
		}

	}
}