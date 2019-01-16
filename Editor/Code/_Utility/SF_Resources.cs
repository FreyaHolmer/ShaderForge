using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge{

	public static class SF_Resources {

		public const string pInterface = "Interface/";
		public const string pFonts = pInterface + "Fonts/";
		public const string pMeshes = "Meshes/";
		public const string pGpuRendering = "GPU Rendering/";


		public static T Load<T>(string name) where T : UnityEngine.Object {
			return (T)AssetDatabase.LoadAssetAtPath(InternalResourcesPath + name, typeof(T) );
		}

		public static UnityEngine.Object[] LoadAll(string name) {
			return AssetDatabase.LoadAllAssetsAtPath(InternalResourcesPath + name );
		}

		public static Texture2D LoadInterfaceIcon(string name){
			string path = InternalResourcesPath + "Interface/" + name;
			Texture2D retTex = (Texture2D)AssetDatabase.LoadAssetAtPath(path + ".png", typeof(Texture2D) );
			if(retTex == null){
				retTex = (Texture2D)AssetDatabase.LoadAssetAtPath(path + ".tga", typeof(Texture2D) );
			}
			return retTex;
		}

		public static Texture2D LoadNodeIcon(string name){
			return (Texture2D)AssetDatabase.LoadAssetAtPath(InternalResourcesPath + "Interface/Nodes/" + name + ".png", typeof(Texture2D) );
		}
	

		private static string internalResourcesPath = "";
		public static string InternalResourcesPath{
			get{
				if(string.IsNullOrEmpty(internalResourcesPath)){
					string path;
					if(SearchForInternalResourcesPath(out path)){
						internalResourcesPath = path;
					} else {
						Debug.LogError("Unable to locate the internal resources folder. Make sure your Shader Forge installation is intact");
						SF_Editor.instance.Close();
					}
				}
				return internalResourcesPath;
			}
		}



		private static bool SearchForInternalResourcesPath( out string path ){
			path = "";
			string partialPath = "/ShaderForge/Editor/InternalResources/";
			string foundPath = null;
			foreach(string s in AssetDatabase.GetAllAssetPaths()){
				if(s.Contains(partialPath)){
					foundPath = s;
					break;
				}
			}
			if(foundPath == null){
				return false;
			}
			string[] split = foundPath.Replace(partialPath,"#").Split('#');
			path = split[0] + partialPath;
			return true;
		}




	}
}
