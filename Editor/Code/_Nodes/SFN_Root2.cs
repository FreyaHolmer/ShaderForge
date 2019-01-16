using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Root2 : SFN_Node_Constant {

		public SFN_Root2() {
		}

		public override void Initialize() {
			base.Initialize( "Root 2" );
			base.PrepareConstant( "const_root2", "1.41421356237309504" );
		}

	}
}