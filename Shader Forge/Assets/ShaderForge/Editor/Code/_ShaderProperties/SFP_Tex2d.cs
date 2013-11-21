using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Tex2d : SF_ShaderProperty {


		public bool isBumpmap = false;

		public new SFP_Tex2d Initialize( SF_Node node ) {
			base.nameType = "Texture (2D)";
			base.Initialize( node );
			return this;
		}


		public override string GetInitializationLine() {
			string defaultValue = isBumpmap ? "\"bump\"" : "\"white\"";
			return GetVariable() + " (\"" + nameDisplay + "\", 2D) = " + defaultValue + " {}";
		}

		public override string GetVariableLine() {
			return "uniform sampler2D " + GetVariable() + ";";
		}

		// TODO: UVs
		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}