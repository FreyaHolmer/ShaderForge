using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;


namespace ShaderForge {

	[System.Serializable]
	public class SFP_Branch : SF_ShaderProperty {

		public new SFP_Branch Initialize( SF_Node node ) {
			base.nameType = "Static Branch";
			base.Initialize( node );
			return this;
		}

		public override void UpdateInternalName() {
			
			string s = nameDisplay;

			s = s.Replace(" ","_");
			
			Regex rgx = new Regex( "[^a-zA-Z0-9_]" );
			s = rgx.Replace( s, "" );

			s = s.ToUpper();

			
			// TODO: Make sure it's valid and unique
			
			nameInternal = s;
		}

		public override string GetMulticompilePragma (){
			return "#pragma multi_compile " + nameInternal;
		}

	}
}