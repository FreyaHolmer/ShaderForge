using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Cubemap : SF_ShaderProperty {


		public new SFP_Cubemap Initialize( SF_Node node ) {
			base.nameType = "Cubemap";
			base.Initialize( node );
			return this;
		}

		public override string GetInitializationLine() {
			string defaultValue = "\"_Skybox\"";
			return GetTagString() + GetVariable() + " (\"" + nameDisplay + "\", Cube) = " + defaultValue + " {}";
		}

		public override string GetVariableLine() {
			return "uniform samplerCUBE " + GetVariable() + ";";
		}

		// TODO: UVs
		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}