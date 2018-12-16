using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_FogColor : SF_Node {


		public SFN_FogColor() {

		}

		public override void Initialize() {
			base.Initialize( "Fog Color" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.texture.uniform = true;
			base.texture.coloredAlphaOverlay = true;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this, "RGB", "RGB",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this, "R", "R",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnector.Create(this, "G", "G",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green),
				SF_NodeConnector.Create(this, "B", "B",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B).WithColor(Color.blue),
				SF_NodeConnector.Create(this, "A", "A",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override void Update() {
			if( texture.dataUniformColor != RenderSettings.fogColor ) {
				texture.dataUniform = RenderSettings.fogColor;
				OnUpdateNode(NodeUpdateType.Soft, true);
			}		
		}

		public override void OnPreGetPreviewData() {
			texture.dataUniform = RenderSettings.fogColor;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "unity_FogColor";
		}

	}
}