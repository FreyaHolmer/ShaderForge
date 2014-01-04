using UnityEngine;
using System.Collections;

public class MaterialArray : MonoBehaviour {



	public Material material;
	public Mesh mesh;

	public int resolution = 8;

	// Use this for initialization
	void Start () {
		for(float x=0;x<resolution;x++){
			for(float y=0;y<resolution;y++){


				GameObject go = new GameObject("Mesh_" + x + "_" + y,typeof(MeshFilter),typeof(MeshRenderer));
				go.transform.position = new Vector3(x*2f,0f,y*2f);
				go.GetComponent<MeshFilter>().sharedMesh = mesh;
				go.renderer.material = material;
				go.renderer.material.SetFloat("_Specular",x/(resolution-1));
				go.renderer.material.SetFloat("_Gloss",y/(resolution-1));

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
