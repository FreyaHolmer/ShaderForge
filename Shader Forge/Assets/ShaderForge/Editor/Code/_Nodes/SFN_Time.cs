using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Time : SF_Node {


		public SFN_Time() {

		}

		public override void Initialize() {
			base.Initialize( "Time" );
			base.showColor = true;
			base.UseLowerPropertyBox( false );
			base.texture.icon = Resources.LoadAssetAtPath( SF_Paths.pInterface+"Nodes/time.png", typeof( Texture2D ) ) as Texture2D;
			base.texture.uniform = true;
			base.texture.CompCount = 4;

			base.alwaysDefineVariable = true;

			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"TSL","t/20",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R),
				SF_NodeConnection.Create(this,"T","t",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G),
				SF_NodeConnection.Create(this,"TDB","t*2",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B),
				SF_NodeConnection.Create(this,"TTR","t*3",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
			};
		
			Debug.Log("Time node! connector count: " + connectors.Length);
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "_Time + _TimeEditor";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return 1f;
		}



	}
}