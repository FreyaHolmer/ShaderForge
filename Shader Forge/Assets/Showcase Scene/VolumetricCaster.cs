using UnityEngine;
using UnityEditor;
using System.Collections;

public class VolumetricCaster : MonoBehaviour {


	void OnDrawGizmos(){


		Vector3 a = Local(new Vector3(0.5f,	-0.5f,	0f));
		Vector3 b = Local(new Vector3(0.5f,	0.5f,	0f));
		Vector3 c = Local(new Vector3(-0.5f,0.5f,	0f));
		Vector3 d = Local(new Vector3(-0.5f,-0.5f,	0f));






		DrawEdge("A",a,b);
		DrawEdge("B",b,c);
		DrawEdge("C",c,d);
		DrawEdge("D",d,a);


	}

	public void DrawEdge(string edgeName, Vector3 a, Vector3 b){
		Gizmos.color = Color.black;
		Gizmos.DrawLine(a,b);
		Handles.color = Color.black;
		Handles.Label((a+b)/2,edgeName);
	}

	public Vector3 Local(Vector3 pt){
		return transform.TransformPoint(pt);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
