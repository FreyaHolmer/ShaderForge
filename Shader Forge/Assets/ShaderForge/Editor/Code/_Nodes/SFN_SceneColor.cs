using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_SceneColor : SF_Node {


		public SFN_SceneColor() {

		}

		public override void Initialize() {
			base.Initialize( "Scene Color" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/scene_color.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.CompCount = 4;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2),
				SF_NodeConnection.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnection.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnection.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnection.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnection.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0.3f, 0.6f, 0.3f, 1f );
		}

		public bool AutoUV(){
			return !GetInputIsConnected( "UVIN" );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string UV = "";

			if(AutoUV())
				UV = "i.screenPos.xy*0.5+0.5";
			else
				UV = GetInputCon( "UVIN" ).Evaluate();

			return "tex2D(_GrabTexture, float2(1,grabSign)*" + UV + ")";
		}

	}
}