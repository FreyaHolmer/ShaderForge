
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Tilt shift")

class TiltShift extends PostEffectsBase {
	public var tiltShiftShader : Shader;
	private var tiltShiftMaterial : Material = null;
	
	public var renderTextureDivider : int = 2;	
	public var blurIterations : int = 2;
	public var enableForegroundBlur : boolean = true;
	public var foregroundBlurIterations : int = 2;
	public var maxBlurSpread : float = 1.5f;
	
	public var focalPoint : float = 30.0f;
	public var smoothness : float = 1.65f;
	
	public var visualizeCoc : boolean = false;
	
	// these values will be automatically determined
	
	private var start01 : float = 0.0f;
	private var distance01 : float = 0.2f;
	private var end01 : float = 1.0f;
	private var curve : float = 1.0f;
		
	function CheckResources () : boolean {	
		CheckSupport (true);	
	
		tiltShiftMaterial = CheckShaderAndCreateMaterial (tiltShiftShader, tiltShiftMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
				
		var widthOverHeight : float = (1.0f * source.width) / (1.0f * source.height);
		var oneOverBaseSize : float = 1.0f / 512.0f;		

		// clamp some values
		
		renderTextureDivider = renderTextureDivider < 1 ? 1 : renderTextureDivider;
		renderTextureDivider = renderTextureDivider > 4 ? 4 : renderTextureDivider;
		blurIterations = blurIterations < 1 ? 0 : blurIterations;
		blurIterations = blurIterations > 4 ? 4 : blurIterations;
		
		// automagically calculate parameters based on focalPoint

		var focalPoint01 : float = camera.WorldToViewportPoint (focalPoint * camera.transform.forward + camera.transform.position).z / (camera.farClipPlane);	
	
		distance01 = focalPoint01;
		start01 = 0.0;
		end01 = 1.0;			
		start01 = Mathf.Min (focalPoint01 - Mathf.Epsilon, start01);
		end01 = Mathf.Max (focalPoint01 + Mathf.Epsilon, end01);
		curve = smoothness * distance01;
		
		// resources

		var cocTex : RenderTexture = RenderTexture.GetTemporary (source.width, source.height, 0); 
		var cocTex2 : RenderTexture = RenderTexture.GetTemporary (source.width, source.height, 0); 
		var lrTex1 : RenderTexture = RenderTexture.GetTemporary (source.width / renderTextureDivider, source.height / renderTextureDivider, 0); 
		var lrTex2 : RenderTexture = RenderTexture.GetTemporary (source.width / renderTextureDivider, source.height / renderTextureDivider, 0); 
		
		// coc		
		
		tiltShiftMaterial.SetVector ("_SimpleDofParams", Vector4 (start01, distance01, end01, curve));
		tiltShiftMaterial.SetTexture ("_Coc", cocTex);
				
		if (enableForegroundBlur) {
			Graphics.Blit (source, cocTex, tiltShiftMaterial, 0);
			Graphics.Blit (cocTex, lrTex1); // downwards (only really needed if lrTex resolution is different)
			
			for (var fgBlurIter : int = 0; fgBlurIter < foregroundBlurIterations; fgBlurIter++ ) {
				tiltShiftMaterial.SetVector ("offsets", Vector4 (0.0, (maxBlurSpread * 0.75f) * oneOverBaseSize, 0.0, 0.0));	
				Graphics.Blit (lrTex1, lrTex2, tiltShiftMaterial, 3); 
				tiltShiftMaterial.SetVector ("offsets", Vector4 ((maxBlurSpread * 0.75f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));	
				Graphics.Blit (lrTex2, lrTex1, tiltShiftMaterial, 3);	
			}
			
			Graphics.Blit (lrTex1, cocTex2, tiltShiftMaterial, 7);	 // upwards (only really needed if lrTex resolution is different)
			tiltShiftMaterial.SetTexture ("_Coc", cocTex2);
		} else {
			RenderTexture.active = cocTex;
			GL.Clear (false, true, Color.black);
		}
		
		// combine coc's
		Graphics.Blit (source, cocTex, tiltShiftMaterial, 5);
		tiltShiftMaterial.SetTexture ("_Coc", cocTex);
		
		// downsample & blur

		Graphics.Blit (source, lrTex2);
		
		for (var iter : int = 0; iter < blurIterations; iter++ ) {
			tiltShiftMaterial.SetVector ("offsets", Vector4 (0.0, (maxBlurSpread * 1.0f) * oneOverBaseSize, 0.0, 0.0));	
			Graphics.Blit (lrTex2, lrTex1, tiltShiftMaterial, 6); 
			tiltShiftMaterial.SetVector ("offsets", Vector4 ((maxBlurSpread * 1.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));	
			Graphics.Blit (lrTex1, lrTex2, tiltShiftMaterial, 6);		
		}		
		
		tiltShiftMaterial.SetTexture ("_Blurred", lrTex2);
				
		Graphics.Blit (source, destination, tiltShiftMaterial, visualizeCoc ? 4 : 1);	

		RenderTexture.ReleaseTemporary (cocTex);
		RenderTexture.ReleaseTemporary (cocTex2);
		RenderTexture.ReleaseTemporary (lrTex1);
		RenderTexture.ReleaseTemporary (lrTex2);
	}	
}
