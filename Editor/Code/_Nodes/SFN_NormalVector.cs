using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_NormalVector : SF_Node {


		public bool perturbed;

		public SFN_NormalVector() {

		}

		public override void Initialize() {
			perturbed = false;
			base.Initialize( "Normal Dir.", InitialPreviewRenderMode.BlitSphere );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			//UpdateIcon();
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
			base.lockedVariableName = true;
		}

		public override Vector4 EvalCPU() {
			return new Color( 0, 0, 1, 0 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			if( SF_Evaluator.inVert || SF_Evaluator.inTess )
				return "v.normal";
			return perturbed ? "normalDirection" : "i.normalDir";
		}
		/*
		public void UpdateIcon() {
			if(perturbed){
				texture.LoadDataTexture(this.GetType(), "2");
			} else {
				texture.LoadDataTexture(this.GetType());
			}
			base.texture.SetIconId( perturbed ? 1 : 0 );
		}*/

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.xMin += 3;
			perturbed = EditorGUI.Toggle( r, perturbed );
			r.xMin += 17;
			GUI.Label(r,"Perturbed");
			if( EditorGUI.EndChangeCheck() ) {
				//UpdateIcon();
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
					//UpdateIcon();
					break;
			}
		}

	}
}