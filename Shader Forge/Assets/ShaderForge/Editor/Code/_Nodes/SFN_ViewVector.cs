using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ViewVector : SF_Node {


		public SFN_ViewVector() {

		}

		public override void Initialize() {
			base.Initialize( "View Dir." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface + "Nodes/vector_view.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 3;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "viewDirection";
		}

	}
}