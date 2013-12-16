using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleArrange : MonoBehaviour {


	public float radius;

#if UNITY_EDITOR
	void OnDrawGizmos(){
		Gizmos.color = new Color(1f,1f,1f,0.15f);
		Gizmos.DrawWireSphere(transform.position, radius);
		Arrange();
	}
#endif





	void Arrange(){

		if(Application.isPlaying)
			return;

		Transform[] allObjs = GetComponentsInChildren<Transform>();
		List<Transform> objects = new List<Transform>();
		objects.AddRange(allObjs);
		objects.Remove(transform);

		for(float i=0;i<objects.Count;i++){

			float frac = i/objects.Count;

			Vector3 dir = Vector3.zero;

			dir.x = Mathf.Cos(frac*Mathf.PI*2f);
			dir.z = Mathf.Sin(frac*Mathf.PI*2f);

			objects[(int)i].localPosition = dir * radius;
			objects[(int)i].forward = -dir;

		}

	}

}
