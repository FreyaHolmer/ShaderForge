using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_DepthBlend : SF_Node {


		public SFN_DepthBlend() {

		}

		public override void Initialize() {
			base.Initialize( "Depth Blend" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 1;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1),
				SF_NodeConnector.Create(this,"DIST","Dist",ConType.cInput,ValueType.VTv1),
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 0.3f, 0.6f, 0.3f, 1f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string dist = "";

			if( GetConnectorByStringID("DIST").IsConnected()){
				dist = "/" + GetInputCon("DIST").TryEvaluate();
			}


			return "saturate((sceneZ-partZ)" + dist + ")";
		}

	}
}