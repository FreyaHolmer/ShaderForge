using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Node_Constant : SF_Node {


		public float constFloat;
		public string constStr;

		

		public SFN_Node_Constant() {
		}

		public void PrepareConstant(string icon, string constant){
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			base.showLowerReadonlyValues = true;
			base.texture.uniform = true;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;
			constStr = constant;
			constFloat = float.Parse( constant );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1,false)
			};
			base.texture.CompCount = 1;
			base.texture.dataUniform[0] = constFloat;
			node_height = Mathf.RoundToInt( node_height * 0.6666666666f );
			node_width = Mathf.RoundToInt( node_width * 0.6666666666f );
			InitializeDefaultRect( rect.center );

		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return constStr;
		}

		public override float EvalCPU( int c ) {
			return constFloat;
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		/*
		public override void DrawLowerPropertyBox() {
			GUI.Label( lowerRect, texture.dataUniform[0].ToString(), EditorStyles.textField );
		}*/

	}
}