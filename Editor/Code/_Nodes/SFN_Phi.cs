using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Phi : SFN_Node_Constant {

		public SFN_Phi() {
		}

		public override void Initialize() {
			base.Initialize( "Phi" );
			base.PrepareConstant( "const_phi", "1.61803398875" );
		}

	}
}