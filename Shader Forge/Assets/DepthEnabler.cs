using UnityEngine;
using System.Collections;

public class DepthEnabler : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		this.camera.depthTextureMode |= DepthTextureMode.Depth;
		this.camera.SetTargetBuffers( Graphics.activeColorBuffer, Graphics.activeDepthBuffer );
	}
	
	// Update is called once per frame
	void OnDisable() {
		this.camera.depthTextureMode |= DepthTextureMode.Depth;
		this.camera.SetTargetBuffers( Graphics.activeColorBuffer, Graphics.activeDepthBuffer );
	}
}
