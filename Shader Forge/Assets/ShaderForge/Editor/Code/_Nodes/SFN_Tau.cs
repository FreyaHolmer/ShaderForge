using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Tau : SFN_Node_Constant {

		public SFN_Tau() {
		}

		public override void Initialize() {
			base.Initialize( "Tau");
			base.PrepareConstant( "const_tau", "6.28318530718" );
		}

	}
}