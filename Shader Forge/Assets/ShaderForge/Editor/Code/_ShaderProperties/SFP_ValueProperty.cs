using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_ValueProperty : SF_ShaderProperty {


		public bool isBumpmap = false;

		public new SFP_ValueProperty Initialize( SF_Node node ) {
			base.nameType = "Value";
			base.Initialize( node );
			return this;
		}



		public override string GetInitializationLine() {
			string defaultValue = "0";
			// name ("display name", Range (min, max)) = number
			return GetVariable() + " (\"" + nameDisplay + "\", Float ) = " + defaultValue;
		}

		public override string GetVariableLine() {
			return "uniform float " + GetVariable() + ";";
		}

		// TODO: Unity UV offsets
		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}