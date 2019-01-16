using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_AmbientLight : SF_Node {


		public SFN_AmbientLight() {

		}

		public override void Initialize() {
			base.Initialize( "Ambient Light" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 4;
			base.texture.uniform = true;
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

		public Color GetIconTint() {
			Color c = texture.dataUniform;
			c.a = 1.0f;
			for( int i = 0; i < 3; i++ ) {
				c[i] = 1f - Mathf.Pow( 1f - c[i], 2 );
				c[i] = Mathf.Lerp( 0.5f, 1f, c[i] );
			}
			return c;
		}

		public override void Update() {
			if( ((Color)texture.dataUniform) != RenderSettings.ambientLight ) {
				texture.dataUniform = RenderSettings.ambientLight;
				texture.iconColor = GetIconTint();
				OnUpdateNode(NodeUpdateType.Soft, true);
			}
				
		}

		public override void OnPreGetPreviewData() {
			texture.dataUniform = RenderSettings.ambientLight;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "UNITY_LIGHTMODEL_AMBIENT";
		}

	}
}