using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Pi : SFN_Node_Constant {

		public SFN_Pi() {
		}

		public override void Initialize() {
			base.Initialize( "Pi" );
			base.PrepareConstant( "const_pi", "3.141592654" );
		}

	}
}