using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_ValueProperty : SF_ShaderProperty {

		public new SFP_ValueProperty Initialize( SF_Node node ) {
			base.nameType = "Value";
			base.Initialize( node );
			return this;
		}

		public override string GetInitializationLine() {
			string defaultValue = base.node.texture.dataUniform.x.ToString();
			return GetTagString() + GetVariable() + " (\"" + nameDisplay + "\", Float ) = " + defaultValue;
		}

		public override string GetVariableLine() {
			return "uniform "+node.precision.ToCode()+" " + GetVariable() + ";";
		}

		// TODO: Unity UV offsets
		public override string GetFragmentPrepare() {
			return node.precision.ToCode() + "4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}