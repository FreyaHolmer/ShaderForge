using UnityEngine;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFP_Slider : SF_ShaderProperty {


		public bool isBumpmap = false;

		public new SFP_Slider Initialize( SF_Node node ) {
			base.nameType = "Slider";
			base.Initialize( node );
			return this;
		}



		public override string GetInitializationLine() {
			string defaultValue = GetCurrent().ToString();
			// name ("display name", Range (min, max)) = number
			return GetTagString() + GetVariable() + " (\"" + nameDisplay + "\", Range(" + GetMin() + ", " + GetMax() + ")) = " + defaultValue;
		}

		float GetMin() {
			return ( node as SFN_Slider ).min;
		}

		float GetMax() {
			return ( node as SFN_Slider ).max;
		}

		float GetCurrent() {
			return ( node as SFN_Slider ).current;
		}

		public override string GetVariableLine() {
			return "uniform " + node.precision.ToCode() + " " + GetVariable() + ";";
		}

		// TODO: Unity UV offsets
		public override string GetFragmentPrepare() {
			return node.precision.ToCode()+" " + GetVariable() + " = " + node.Evaluate() + ";";
		}


	}
}