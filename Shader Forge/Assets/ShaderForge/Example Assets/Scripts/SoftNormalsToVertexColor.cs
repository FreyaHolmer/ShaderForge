using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class SoftNormalsToVertexColor : MonoBehaviour {

	public enum Method { Simple, AngularDeviation };

	public Method method = Method.AngularDeviation;
	public bool generateOnAwake = false;
	public bool generateNow = false;

	void OnDrawGizmos() {
		if( generateNow ) {
			generateNow = false;
			TryGenerate();
		}
	}

	void Awake() {
		if(generateOnAwake)
			TryGenerate();
	}

	void TryGenerate() {
		MeshFilter mf = GetComponent<MeshFilter>();
		if( mf == null ) {
			Debug.LogError( "MeshFilter missing on the vertex color generator", gameObject );
			return;
		}
		if( mf.sharedMesh == null ) {
			Debug.LogError( "Assign a mesh to the MeshFilter before generating vertex colors", gameObject );
			return;
		}
		Generate(mf.sharedMesh);
		Debug.Log("Vertex colors generated", gameObject);
	}

	void Generate(Mesh m) {

		Vector3[] n = m.normals;
		Vector3[] v = m.vertices;
		Color[] colors = new Color[n.Length];
		List<List<int>> groups = new List<List<int>>();

		for( int i = 0; i < v.Length; i++ ) {		// Group verts at the same location
			bool added = false;
			foreach( List<int> group in groups ) {	// Add to exsisting group if possible
				if( v[group[0]] == v[i] ) {
					group.Add(i);
					added = true;
					break;
				}
			}
			if( !added ) {							// Create new group if necessary
				List<int> newList = new List<int>();
				newList.Add( i );
				groups.Add( newList );
			}
		}

		foreach( List<int> group in groups ) { // Calculate soft normals
			Vector3 avgNrm = Vector3.zero;
			foreach( int i in group ) { // TODO: This can actually be improved. Averaging will not give the best outline.
				avgNrm += n[i];
			}
			avgNrm.Normalize(); // Average normal done
			if( method == Method.AngularDeviation ) {
				float avgDot = 0f; // Calculate deviation to alter length
				foreach( int i in group ) {
					avgDot += Vector3.Dot( n[i], avgNrm );
				}
				avgDot /= group.Count;
				float angDeviation = Mathf.Acos( avgDot ) * Mathf.Rad2Deg;
				float aAng = 180f - angDeviation - 90;
				float lMult = 0.5f / Mathf.Sin( aAng * Mathf.Deg2Rad );  // 0.5f looks better empirically, but mathematically it should be 1f
				avgNrm *= lMult;
			}

			foreach( int i in group ) {
				colors[i] = new Color( avgNrm.x, avgNrm.y, avgNrm.z ); // Save normals as colors
			}
		}

		m.colors = colors; // Assign as vertex colors

#if UNITY_EDITOR
		EditorUtility.SetDirty( this );
		SceneView.RepaintAll();
#endif

	}
	

}