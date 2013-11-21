using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_LightVector : SF_Node {


		public SFN_LightVector() {

		}

		public override void Initialize() {
			base.Initialize( "Light Dir." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/vector_light.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 3;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lightDirection"; // normalize(_WorldSpaceLightPos0.xyz);
		}

	}
}