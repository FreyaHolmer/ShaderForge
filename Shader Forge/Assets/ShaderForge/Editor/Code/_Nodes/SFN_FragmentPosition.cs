using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_FragmentPosition : SF_Node {


		public SFN_FragmentPosition() {

		}

		public override void Initialize() {
			base.Initialize( "World Pos." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/fragment_pos.png", typeof( Texture2D ) ) as Texture2D;
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
			if( SF_Evaluator.inFrag )
				return "i.posWorld";
			else if( SF_Evaluator.inVert )
				return "mul(_Object2World, v.vertex)";
			else if( SF_Evaluator.inTess )
				return "mul(_Object2World, v.vertex)";
			else{
				Debug.Log( "Evaluated into unknown shader program" );
				return null;
			}
				
		}

	}
}