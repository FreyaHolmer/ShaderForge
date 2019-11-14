using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Matrix4x4Property : SF_ShaderProperty {

		public new SFP_Matrix4x4Property Initialize( SF_Node node ) {
			base.nameType = "Matrix 4x4";
			base.Initialize( node );
			global = true;
			return this;
		}

		public override string GetInitializationLine() {
			return "";// GetTagString() + GetVariable() + " (\"" + nameDisplay + "\", Vector) = (" + GetValue().r + "," + GetValue().g + "," + GetValue().b + "," + GetValue().a + ")";
		}

		Color GetValue() {
			return Color.black;
		}

		public override string GetCGType() => node.precision.ToCode() + "4x4";

	}
}