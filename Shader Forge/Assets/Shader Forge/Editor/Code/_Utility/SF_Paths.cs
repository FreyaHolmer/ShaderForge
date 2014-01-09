using UnityEngine;
using System.Collections;

public static class SF_Paths {

	public const string pInterface = "Interface/";
	public const string pMeshes = "Meshes/";



	
	public static Texture2D GetIcon(string type){
		return (Texture2D)Resources.Load( "Interface/Nodes/"+ type, typeof(Texture2D) );
	}

}
