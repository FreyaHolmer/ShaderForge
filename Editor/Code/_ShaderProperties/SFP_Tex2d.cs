using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Tex2d : SF_ShaderProperty {


		//public bool isBumpmap = false; // TODO: Is this even used?

		public new SFP_Tex2d Initialize( SF_Node node ) {
			base.nameType = "Texture (2D)";
			base.Initialize( node );
			return this;
		}


		public override string GetInitializationLine() {
			//string defaultValue = isBumpmap ? "\"bump\"" : "\"white\"";

			NoTexValue noTexValue = NoTexValue.Black;

			if(base.node is SFN_Tex2d)
				noTexValue = (base.node as SFN_Tex2d).noTexValue;
			else if(base.node is SFN_Tex2dAsset)
				noTexValue = (base.node as SFN_Tex2dAsset).noTexValue;

			return GetTagString() + GetVariable() + " (\"" + nameDisplay + "\", 2D) = \"" + noTexValue.ToString().ToLower() + "\" {}";
		}

		public override string GetVariableLine() {
			string varName = GetVariable();
			
			string s = "uniform sampler2D " + varName + ";";
			if( !tagNoScaleOffset ) {
				s += " uniform float4 " + varName + "_ST;";
			}

			return s;
		}


		public override string GetFragmentPrepare() {
			return "fixed4 " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}