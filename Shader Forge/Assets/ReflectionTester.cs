using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class ReflectionTester : MonoBehaviour {


	Vector3 N = Vector3.up;
	Vector3 L = Vector3.right;
	Vector3 R = Vector3.right;

	public Transform tf_light;
	public Transform tf_surface;

	public Color nColor = new Color( 0.5f, 0.5f, 1f );
	public Color lColor = new Color( 1f, 0.8f, 0.1f );
	public Color rColor = new Color( 1f, 0.8f, 0.1f, 0.5f );
	public Color hColor = new Color(0.75f,0.65f,0.55f);

	const int res = 512;
	Vector3[] points = new Vector3[res];
	float[] magnitudes = new float[res];

	public float specularPower = 2f;

	[Range( 0, 1 )]
	public float roughness = 0.5f;
	

	void Update() {

		points = new Vector3[res];
		magnitudes = new float[res];

		float step = Mathf.PI/res;
		Vector3 tfPos = transform.position;

		Vector3 V = Vector3.up;

		for(int i=0;i<res;i++){
			float x = Mathf.Cos(step*i);
			float y = Mathf.Sin(step*i);

			V.x = x;
			V.y = y;

			magnitudes[i] = GetMagnitude(V);
			points[i] = new Vector3( x, y, 0f ) * magnitudes[i] + tfPos;
		}

	}

	float GetMagnitude( Vector3 V ) {
		return 1f;
		/*
		Vector3 H = ( V + L ).normalized;

		float lambert = MaxDot( N, L );
		float phong = Mathf.Pow(MaxDot( R, V ),specularPower);
		float blinn = Mathf.Pow(MaxDot( H, N ),specularPower);


		float NdotH = MaxDot(N,H);
		float NdotL = MaxDot(N,L);
		float VdotH = MaxDot(V,H);
		float NdotV = MaxDot(N,V);


	// BECKMANN

		float bM2 = roughness * roughness;
		float beckAngle = Mathf.Acos( NdotH );
		float beckntan2 = -Mathf.Pow( Mathf.Tan( beckAngle ), 2 );

		float beckNum = Mathf.Exp( beckntan2 / bM2 );
		float beckDen = Mathf.PI * bM2 * Mathf.Pow( Mathf.Cos( beckAngle ), 4 );
		float beckmann = beckNum / beckDen;

	// END BECKMANN

	// COOK STUFF
		float Gs = ( 2f * NdotH * NdotL ) / VdotH;
		float Gm = ( 2f * NdotH * NdotV ) / VdotH;
		float G = Mathf.Min( 1f, Gs, Gm );
		float Fbase = 1f-VdotH;
		float Fexp = Mathf.Pow(Fbase,5);

		//float F = Fexp + 0.06f * ( 1f - Fexp );
		//float refNormalInc = Mathf.Pow( 1f - VdotH, 5f );
		//float F = Mathf.Pow( 1.0f - VdotH, 5.0f );
		//F *= ( 1.0f - lambert );
		//F += lambert;

		float Fe = Mathf.Pow(1f-VdotH, 5);
		float F = Fe + specularPower * (1.0f - Fe);


		//float Db = 1f/();
		float r_sq = specularPower * specularPower;
		//float D = specPow2 / (Mathf.PI * Mathf.Pow(((NdotH*NdotH) * (specPow2 - 1f) + 1f), 2f));
		float c = 1.0f;
		float alpha = Mathf.Acos( NdotH);
		//float D = c * Mathf.Exp( -( alpha / r_sq ) );
		float D = beckmann;


		float cook = (D*F*G) / ( NdotV * NdotL );

	// END COOK STUFF




		// GAUSSIAN

		float gaussian = Mathf.Exp( -Mathf.Pow( ( Vector3.Angle( N, H ) * Mathf.Deg2Rad ) / roughness, 2 ) );

		// END GAUSSIAN



// REMEMBER ME STUFF

		// ALBEDO
		float rm_A = ( 0.5f ) / Mathf.PI;

		// FRESNEL
		float a = Mathf.Pow( ( 1f ) / ( 2f - 1f ), 2f );
		float rm_F = a + ( 1 - a ) * Mathf.Pow( 1f - VdotH, 5 );

		float roughnessValue = 0.8f;

		// ROUGHNESS
		float rm_sp = Mathf.Pow( 2f, 8f * roughnessValue + 1f );
		float roughnessContrib = ( rm_sp + 2f ) / ( 8f * Mathf.PI );

		// NORMALS
		float normalContrib = Mathf.Pow( NdotH, rm_sp );

		float rememberme = ( rm_A + rm_F * roughnessContrib * normalContrib ) * NdotL;

// END REMEMBER ME

		return cook;


		*/
	}

	public float MaxDot(Vector3 a, Vector3 b) {
		return Mathf.Max(0f,Vector3.Dot(a,b));
	}


	void OnDrawGizmos() {

		if( tf_light != null ) {
			L = ( tf_light.position - transform.position ).normalized;
		}
		if( tf_surface != null ) {
			N = -tf_surface.forward;
		}

		R = Vector3.Reflect( -L, N );

		DrawArrow( "N", N, nColor );
		DrawArrow( "L", L, lColor );
		DrawArrow( "R", R, rColor );

		Handles.DrawPolyLine( points );
		Vector3 tfPos = transform.position;
		for( int i = 0; i < res; i++ ) {
			Handles.color = Color.white*magnitudes[i];
			Handles.DrawLine( tfPos, points[i] );
		}


		//Gizmos.DrawLine( transform.position, transform.position + Vector3.up);
	}


	void DrawArrow( string label, Vector3 dir, Color color ) {
		Color prevCol = Handles.color;
		Handles.color = color;
		Handles.ArrowCap( 1, transform.position, Quaternion.LookRotation( dir ), 1f );
		Handles.Label( transform.position + dir*1.2f, label, EditorStyles.toolbarButton );
		Handles.color = prevCol;
	}

}
