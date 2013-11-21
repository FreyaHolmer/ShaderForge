using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ObjectPosition : SF_Node {


		public SFN_ObjectPosition() {

		}

		public override void Initialize() {
			base.Initialize( "Object Pos." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/object_pos.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 4;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnection.Create(this,"X","X",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnection.Create(this,"Y","Y",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green),
				SF_NodeConnection.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B).WithColor(Color.blue),
				SF_NodeConnection.Create(this,"W","W",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "objPos"; // normalize(_WorldSpaceLightPos0.xyz);
		}

	}
}