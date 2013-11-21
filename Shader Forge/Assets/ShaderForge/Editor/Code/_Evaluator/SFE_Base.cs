using UnityEngine;
using System.Collections;

namespace ShaderForge {

	public class SFE_Base {

		//SF_PassSettings ps;
		//SF_Dependencies deps;
		//SF_ShaderProperty props;
		//SF_Editor editor;

		public SFE_Base( SF_Editor editor, SF_PassSettings ps, SF_Dependencies deps, SF_ShaderProperty props ) {
			//this.editor = editor;
			//this.ps = ps;
			//this.deps = deps;
			//this.props = props;
		}

		public virtual string Evaluate() {
			return ""; // Override
		}

	}
}