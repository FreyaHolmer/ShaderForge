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
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0.3f, 0.6f, 0.3f, 1f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "sceneZ";
		}

	}
}