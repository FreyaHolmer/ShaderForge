using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Tangent : SF_Node {


		public SFN_Tangent() {

		}

		public override void Initialize() {
			base.Initialize( "Tangent Dir." );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/vector_tangent.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 3;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 1f, 0f, 0f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix("tangentDir");
		}

	}
}