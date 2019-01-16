using UnityEngine;
using System.Collections;

// Web data and access with Shader Forge
// Used for documentation, and perhaps later for update checking as well

namespace ShaderForge{
	public static class SF_Web  {


		const string urlRoot = "http://acegikmo.com/shaderforge/"; // ?search=add";
		const string urlNodes = urlRoot + "nodes/";


		public static void OpenDocumentationForNode(SF_Node node){
			OpenDocumentationForString(node.SearchName);
		}
		public static void OpenDocumentationForNode(SF_EditorNodeData nodeData){
			OpenDocumentationForString(nodeData.SearchName);
		}

		static void OpenDocumentationForString(string s){
			Application.OpenURL( urlNodes + "?search=" + StripExtraChars(s) );
		}


		static string StripExtraChars(string s){
			return s.Replace(" ","").Replace(".","");
		}

	}
}
