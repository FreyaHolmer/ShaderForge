using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ViewReflectionVector : SF_Node {


		public SFN_ViewReflectionVector() {

		}

		public override void Initialize() {
			base.Initialize( "View Refl." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface + "Nodes/vector_view_reflection.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 3;
			connectors = new SF_NodeConnection[]{
			SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
		};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "viewReflectDirection";
		}

	}
}