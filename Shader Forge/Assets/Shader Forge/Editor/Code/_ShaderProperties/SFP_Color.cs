using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Color : SF_ShaderProperty {


		public bool isBumpmap = false;

		public new SFP_Color Initialize( SF_Node node ) {
			base.nameType = "Color";
			base.Initialize( node );
			return this;
		}



		public override string GetInitializationLine() {
			return GetVariable() + " (\"" + nameDisplay + "\", Color) = (" + GetValue().r + "," + GetValue().g + "," + GetValue().b + "," + GetValue().a + ")";
		}

		Color GetValue() {
			return ( node as SFN_Color ).texture.dataUniform;
		}

		public override string GetVariableLine() {
			return "uniform float4 " + GetVariable() + ";";
		}

		// TODO: Unity UV offsets
		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}