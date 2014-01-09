using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;


namespace ShaderForge {
	[System.Serializable]
	public class SF_ShaderProperty : ScriptableObject {

		public string nameDisplay = "";	// The displayed name in the material inspector
		public string nameType;		// Used for labeling in the editor
		public string nameInternal = "_";	// The internal shader code name		
		public SF_Node node;


		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public void Initialize(SF_Node node){
			this.node = node;
			SetName( node.GetVariableName() );
		}

		public void UpdateInternalName() {

			string s = nameDisplay;

			Regex rgx = new Regex( "[^a-zA-Z0-9/s-]" );
			s = rgx.Replace( s, "" );

			s = "_" + s;

			// TODO: Make sure it's valid and unique

			nameInternal = s;
		}

		public void SetName(string s) {
			nameDisplay = s;
			UpdateInternalName();
		}
		
		
		
		
		string[] replacements = new string[]{
			"_r","_g",
			"_R","_G",
			"_g","_b",
			"_G","_B",
			"_b","_a",
			"_B","_A",
			"_x","_y",
			"_X","_Y",
			"_y","_z",
			"_Y","_Z",
			"_z","_w",
			"_Z","_W",
			"(R)","(G)",
			"(G)","(B)",
			"(B)","(A)",
			"(r)","(g)",
			"(g)","(b)",
			"(b)","(a)",
			"(X)","(Y)",
			"(Y)","(Z)",
			"(Z)","(W)",
			"(x)","(y)",
			"(y)","(z)",
			"(z)","(w)"
			
		};
		
		public string GetClonedName(){
			
			string oldName = nameDisplay;
			string newName = nameDisplay;
			bool done = false;
			
			for(int i=0;i< replacements.Length;i+=2){
				if(oldName.EndsWith(replacements[i])){
					newName = oldName.Substring(0,oldName.Length - replacements[i].Length) + replacements[i+1];
					done = true;
				}
			}
			
			// Numerical increments
			if(!nameDisplay.StartsWith("node_"))
				if(!done){
					if( TryGetNextNumericalName(ref newName) ){
						done = true;
					}
				}
			
			// Fallback
			if(!done){
				newName = oldName + "_copy";
				done = true;
			}
			
			
			return newName;
			
		}
		
		public bool TryGetNextNumericalName(ref string sOut){
			
			int digits = 0;
			for(int i = nameDisplay.Length-1; i>=0; i-- ){
				if(char.IsNumber(nameDisplay[i]))
					digits++;
				else
					break;
			}
			
			if(digits == 0)
				return false;
			
			
			string strWoNum = nameDisplay.Substring(0,nameDisplay.Length-digits);
			string strNum = nameDisplay.Substring(nameDisplay.Length-digits);
			
			int number = int.Parse(strNum);
			
			number++;
			
			sOut = strWoNum + number.ToString("D"+digits); // Makes sure it's 01 02 etc
			return true;
		}
		
		



		public SF_ShaderProperty() {
			// Empty
		}


		public string GetVariable() {
			return nameInternal;
			//return "_" + node.GetVariableName();
		}

		//public virtual string GetVariable() {
		//	return nameInternal; // Override for textures
		//}

		public string GetFilteredVariableLine() {
			if( this.nameInternal == "_SpecColor" ) { // TODO: Why?
				return null;
			}

			return GetVariableLine();

		}

		public virtual string GetInitializationLine() {
			return ""; // Override, textures need to unpack before usage in the frag shader
		}

		public virtual string GetVariableLine() {
			return ""; // Override
		}

		public virtual string GetFragmentPrepare() {
			return ""; // Override
		}



	}
}