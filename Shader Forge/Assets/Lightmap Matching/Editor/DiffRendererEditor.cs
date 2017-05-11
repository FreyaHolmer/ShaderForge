using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiffRenderer))]
public class DiffRendererEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		//DiffRenderer dr = target as DiffRenderer;
		/*
		if( GUILayout.Button( "Render SF" ) ) {
			BlitWithCamera( dr.camSF, dr.rtSF );
		}
		if( GUILayout.Button( "Render Unity" ) ) {
			BlitWithCamera( dr.camUnity, dr.rtUnity );
		}
		if( GUILayout.Button( "Combine Diff" ) ) {
			// BlitWithCamera( camSF, rtDiff );
		}*/
	}

	public void BlitWithCamera(Camera cam, RenderTexture rt){

		// Cache
		Rect prevRect = cam.rect;

		// Prepare
		RenderTexture.active = rt;
		cam.targetTexture = rt;
		rt.DiscardContents( true, true );
		//cam.rect = new Rect( Vector2.zero, Vector2.one );

		// Render
		//cam.Render();
		Graphics.SetRenderTarget( null );
		

		// Reset
		cam.rect = prevRect;
		cam.targetTexture = null;
	}
	
}
