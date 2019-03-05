using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_E : SFN_Node_Constant {

		public SFN_E() {
		}

		public override void Initialize() {
			base.Initialize( "e" );
			base.SearchName = "EulersConstant";
			base.PrepareConstant( "const_e", "2.718281828459" );
		}

	}
}