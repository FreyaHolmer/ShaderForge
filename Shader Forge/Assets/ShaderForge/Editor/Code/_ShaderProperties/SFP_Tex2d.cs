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
			string varName = GetVariable();
			return "uniform sampler2D " + varName + "; uniform float4 " + varName + "_ST;"; // TODO: Check if texture wants to use offsets or not
		}


		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}