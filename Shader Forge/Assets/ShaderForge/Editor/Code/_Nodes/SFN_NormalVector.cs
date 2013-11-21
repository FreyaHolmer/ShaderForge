using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_NormalVector : SF_Node {


		public bool perturbed;
		Texture2D[] icons;


		public SFN_NormalVector() {

		}

		public override void Initialize() {
			perturbed = false;
			base.Initialize( "Normal Dir." );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			icons = new Texture2D[] { 
				Resources.LoadAssetAtPath( SF_Paths.pInterface + "Nodes/vector_normal.png", typeof( Texture2D ) ) as Texture2D,
				Resources.LoadAssetAtPath( SF_Paths.pInterface + "Nodes/vector_normal_px.png", typeof( Texture2D ) ) as Texture2D
			};
			UpdateIcon();
			base.texture.CompCount = 3;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}

		public override Color NodeOperator( int x, int y ) {
			return new Color( 0, 0, 1, 0 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			if( SF_Evaluator.inVert || SF_Evaluator.inTess )
				return "v.normal";
			return perturbed ? "normalDirection" : "i.normalDir";
		}

		public void UpdateIcon() {
			base.texture.icon = perturbed ? icons[1] : icons[0];
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.xMin += 3;
			perturbed = EditorGUI.Toggle( r, perturbed );
			r.xMin += 17;
			GUI.Label(r,"Perturbed");
			if( EditorGUI.EndChangeCheck() ) {
				UpdateIcon();
				OnUpdateNode();
			}
				
		}

		public override string SerializeSpecialData() {
			return "pt:" + perturbed;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "pt":
					perturbed = bool.Parse( value );
					UpdateIcon();
					break;
			}
		}

	}
}