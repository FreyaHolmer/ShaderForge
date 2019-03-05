using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace ShaderForge {
	[System.Serializable]
	public class SF_ShaderProperty : ScriptableObject {

		public bool tagHideInInspector = false;
		public bool tagHDR = false;
		public bool tagPerRendererData = false;
		public bool tagNoScaleOffset = false;
		public bool tagNormal = false;

		public string nameDisplay = "";	// The displayed name in the material inspector
		public string nameType;		// Used for labeling in the editor
		public string nameInternal = "_";	// The internal shader code name		
		public SF_Node node;

		public bool global = false;
		public bool overrideInternalName = false;

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public string GetTagString() {
			string s = "";
			if( tagHideInInspector )
				s += "[HideInInspector]";
			if( tagNoScaleOffset )
				s += "[NoScaleOffset]";
			if( tagNormal )
				s += "[Normal]";
			if( tagHDR )
				s += "[HDR]";
			if( tagPerRendererData )
				s += "[PerRendererData]";
			return s;
		}

		public void Initialize(SF_Node node){
			this.node = node;
			SetName( node.GetVariableName() );
		}

		public static string FormatInternalName(string s){
			Regex rgx = new Regex( "[^a-zA-Z0-9_]" );
			s = rgx.Replace( s, "" );
			return s;
		}

		public virtual void UpdateInternalName() {

			if(overrideInternalName){
				nameInternal = node.variableName;
				return;
			}

			string s = nameDisplay;


			s = FormatInternalName(s);

			s = "_" + s;


			// TODO: Make sure it's valid and unique

			nameInternal = s;
		}

		public void SetName( string s) {
			nameDisplay = s;
			if( !overrideInternalName )
				UpdateInternalName();
		}

		public void SetBothNameAndInternal(string s){
			s = FormatInternalName(s);
			nameDisplay = s;
			nameInternal = s;
		}

		public void ToggleGlobal(){



			string undoMsg = global ? "make " + nameDisplay + " local" : "make " + nameDisplay + " global";
			Undo.RecordObject(this,undoMsg);
			Undo.RecordObject(node.editor.nodeView.treeStatus,undoMsg);

			List<SF_Node> propList = node.editor.nodeView.treeStatus.propertyList;

			global = !global;


			if(global){

				if(propList.Contains(node)){
					propList.Remove(node);
				}

			} else {

				if(!propList.Contains(node)){
					propList.Add(node);
				}

			}
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
		
		
		public bool CanToggleGlobal(){
			if(this is SFP_ValueProperty)
				return true;
			if(this is SFP_Color)
				return true;
			if(this is SFP_Tex2d && node is SFN_Tex2dAsset)
				return true;
			if(this is SFP_Vector4Property)
				return true;
			//if( this is SFP_Matrix4x4Property )
			//	return true;
			return false;
		}


		public SF_ShaderProperty() {
			// Empty
		}


		public virtual string GetVariable() {
			return nameInternal;
			//return "_" + node.GetVariableName();
		}

		//public virtual string GetVariable() {
		//	return nameInternal; // Override for textures
		//}

		public string GetFilteredVariableLine() {
			//if( this.nameInternal == "_SpecColor" ) { // TODO: Why?
			//	return null;
			//}

			return GetVariableLine();

		}

		public string Serialize(){
			string s = "";
			s += "glob:" + global.ToString();
			s += ",taghide:" + tagHideInInspector.ToString();
			s += ",taghdr:" + tagHDR.ToString();
			s += ",tagprd:" + tagPerRendererData.ToString();
			s += ",tagnsco:" + tagNoScaleOffset.ToString();
			s += ",tagnrm:" + tagNormal.ToString();
			return s;
		}

		public void Deserialize( string key, string value ){
			switch( key ) {
			case "glob":
				global = bool.Parse( value );
				break;
			case "taghide":
				tagHideInInspector = bool.Parse( value );
				break;
			case "taghdr":
				tagHDR = bool.Parse( value );
				break;
			case "tagprd":
				tagPerRendererData = bool.Parse( value );
				break;
			case "tagnsco":
				tagNoScaleOffset = bool.Parse( value );
				break;
			case "tagnrm":
				tagNormal = bool.Parse( value );
				break;
			}
		}

		
		public virtual string GetMulticompilePragma(){
			return ""; // Override for branching
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