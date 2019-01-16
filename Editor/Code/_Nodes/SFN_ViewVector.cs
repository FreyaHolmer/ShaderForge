using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ViewVector : SF_Node {


		public SFN_ViewVector() {

		}

		public override void Initialize() {
			base.Initialize( "View Dir.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "viewDirection";
		}

	}

}