using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace ShaderForge{

	public enum DebugType{GhostNodes};

	public static class SF_Debug {


		public static bool The(DebugType type){
			return debugFlags[type];
		}


		public static Dictionary<DebugType,bool> debugFlags = new Dictionary<DebugType, bool>(){
			{ DebugType.GhostNodes, false }
		};
		
	}
}