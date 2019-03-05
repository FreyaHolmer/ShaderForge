using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_PixelSize : SF_Node {


		public SFN_PixelSize() {

		}

		public override void Initialize() {
			base.Initialize( "Pixel Size" );
			base.SearchName = "Pixel Size";
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.CompCount = 2;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"PXWH","XY",ConType.cOutput,ValueType.VTv2,false).Outputting(OutChannel.RG),
				SF_NodeConnector.Create(this,"PXW","X",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnector.Create(this,"PXH","Y",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green)
			};
			//base.extraWidthOutput = 12;
		}

		public override Vector4 EvalCPU() {
			return new Color( 1f/1920f, 1f/1080f, 0f, 0f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "float2( _ScreenParams.z-1, _ScreenParams.w-1 )";
		}

	}
}