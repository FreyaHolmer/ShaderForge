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
			base.SearchName = "Projection Parameters";
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"SGN","Sign",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"NEAR","Near",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"FAR","Far",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"RFAR","1/Far",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
			base.extraWidthOutput = 7;
		}

		public override Vector4 EvalCPU() {
			return new Color( 0f, 0.7071068f, 0.7071068f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "_ProjectionParams";
		}

	}
}