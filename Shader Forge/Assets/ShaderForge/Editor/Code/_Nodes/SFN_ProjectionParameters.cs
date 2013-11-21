using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ProjectionParameters : SF_Node {


		public SFN_ProjectionParameters() {

		}

		public override void Initialize() {
			base.Initialize( "Proj. Params" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/projection_parameters.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 4;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"SGN","Sign",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R),
				SF_NodeConnection.Create(this,"NEAR","Near",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G),
				SF_NodeConnection.Create(this,"FAR","Far",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B),
				SF_NodeConnection.Create(this,"RFAR","1/Far",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
			base.extraWidthOutput = 7;
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "_ProjectionParams";
		}

	}
}