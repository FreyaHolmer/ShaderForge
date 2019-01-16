using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Time : SF_Node {


		public SFN_Time() {

		}

		public override void Initialize() {
			base.Initialize( "Time", InitialPreviewRenderMode.BlitQuad );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.uniform = false;
			base.texture.CompCount = 4;

			base.alwaysDefineVariable = true;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"TSL","t/20",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"T","t",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"TDB","t*2",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"TTR","t*3",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		
		}

		public override bool UpdatesOverTime() {
			return true;
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "_Time";
		}

		public override float EvalCPU( int c ) {
			return 1f;
		}



	}
}