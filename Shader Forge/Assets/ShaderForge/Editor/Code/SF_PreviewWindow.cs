using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace ShaderForge {
	[Serializable]
	public class SF_PreviewWindow {

		[SerializeField]
		public SF_Editor editor;
		[SerializeField]
		public SF_PreviewSettings settings;

		// Preview assets
		[SerializeField]
		public Mesh mesh;  
		[SerializeField]
		public Material internalMaterial;
		public Material InternalMaterial {
			get {
				if(internalMaterial == null){
					internalMaterial = new Material(editor.currentShaderAsset);
				}
				return internalMaterial;
			}
			set {
				internalMaterial = value;
			}
		}

		[SerializeField]
		public RenderTexture render; // TODO: Why is this separated from the RT itself?
		[SerializeField]
		GUIStyle previewStyle;
		[SerializeField]
		public Texture2D backgroundTexture;

		bool previewIsSetUp = false; // Intentionally non-serialized


		// Input/Rotation
		[SerializeField]
		public bool isDraggingLMB = false;
		[SerializeField]
		Vector2 dragStartPosLMB = Vector2.zero;
		[SerializeField]
		Vector2 rotMeshStart = new Vector2(-30f,0f);
		[SerializeField]
		Vector2 rotMesh = new Vector2(30f,0f);
		[SerializeField]
		Vector2 rotMeshSmooth = new Vector2(-30f,0f);

		// Light Input/Rotation
		[SerializeField]
		public bool isDraggingRMB = false;

		[SerializeField]
		public Camera cam;
		[SerializeField]
		Transform camPivot;
		[SerializeField]
		Light[] lights;

		//public bool drawBgColor = true;

		Mesh _sphereMesh;
		Mesh sphereMesh {
			get {
				if( _sphereMesh == null ) {
					_sphereMesh = GetSFMesh( "sf_sphere" );
				}
				return _sphereMesh;
			}
		}

		// Reflection to call Handles.SetCameraOnlyDrawMesh(this.m_Camera);
		MethodInfo mSetCameraOnlyDrawMesh;

		public SF_PreviewWindow( SF_Editor editor ) {
			settings = new SF_PreviewSettings( this );
			UpdatePreviewBackgroundColor();

			this.editor = editor;
			this.mesh = GetSFMesh( "sf_sphere" );
			SetupPreview();
		}

		[SerializeField] bool enabled = true;
		public void OnEnable() {
			enabled = true;
			SetupPreview();
		}
		public void OnDisable() {
			enabled = false;
			CleanupObjects();
		}

		public Mesh GetSFMesh(string find_name) {
			UnityEngine.Object[] objs = SF_Resources.LoadAll( SF_Resources.pMeshes+"sf_meshes.fbx" );
			if( objs == null ) {
				Debug.LogError( "sf_meshes.fbx missing" );
				return null;
			}
			if( objs.Length == 0 ) {
				Debug.LogError( "sf_meshes.fbx missing sub assets" );
				return null;
			}
			foreach( UnityEngine.Object o in objs ) {
				if( o.name == find_name && o.GetType() == typeof(Mesh)) {
					return o as Mesh;
				}
			}
			Debug.LogError("Mesh " + find_name + " could not be found in sf_meshes.fbx");
			return null;
		}


		public void SetupPreview() {

			previewIsSetUp = true;

			// Create preview camera
			GameObject camObj = new GameObject("Shader Forge Camera");
			camObj.hideFlags = HideFlags.HideAndDontSave;
			cam = camObj.AddComponent<Camera>();
			cam.targetTexture = render;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.renderingPath = RenderingPath.Forward;
			cam.enabled = false;
			cam.useOcclusionCulling = false;
			cam.cameraType = CameraType.Preview;
			cam.fieldOfView = targetFOV;

			// Make sure it only renders using DrawMesh, to make ignore the scene. This is a bit risky, due to using reflection :(
			// BindingFlags bfs = BindingFlags.Static | BindingFlags.NonPublic;
			// Type[] args = new Type[]{ typeof(Camera) };
			// mSetCameraOnlyDrawMesh = typeof( Handles ).GetMethod( "SetCameraOnlyDrawMesh", bfs, null, args, null );
			// mSetCameraOnlyDrawMesh.Invoke( null, new object[]{ cam } );

			// Create pivot/transform to hold it
			camPivot = new GameObject("Shader Forge Camera Pivot").transform;
			camPivot.gameObject.hideFlags = HideFlags.HideAndDontSave;
			cam.clearFlags = CameraClearFlags.Skybox;
			cam.transform.parent = camPivot;

			// Create custom light sources
			lights = new Light[] {
				new GameObject("Light 0").AddComponent<Light>(),
				new GameObject("Light 1").AddComponent<Light>()
			};
			for( int i = 0; i < lights.Length; i++ ) {
				lights[i].gameObject.hideFlags = HideFlags.HideAndDontSave;
				lights[i].type = LightType.Directional;
				lights[i].lightmapBakeType = LightmapBakeType.Realtime;
				lights[i].enabled = false;
			}

			lights[0].intensity = 1f;
			lights[0].transform.rotation = Quaternion.Euler( 30f, 30f, 0f );
			lights[1].intensity = 0.75f;
			lights[1].color = new Color( 1f, 0.5f, 0.25f );
			lights[1].transform.rotation = Quaternion.Euler( 340f, 218f, 177f );
		}

		void CleanupObjects() {
			GameObject.DestroyImmediate( cam.gameObject );
			GameObject.DestroyImmediate( camPivot.gameObject );
			for( int i = 0; i < lights.Length; i++ ) {
				GameObject.DestroyImmediate( lights[i].gameObject );
			}
		}


		public bool SkyboxOn{
			get{
				return cam.clearFlags == CameraClearFlags.Skybox;
			}
			set{
				if(SF_Debug.renderDataNodes)
					cam.clearFlags = CameraClearFlags.Depth;
				else
					cam.clearFlags = value ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
			}
		}

		static Vector2 rotMeshSphere = new Vector2( 22, -18 - 90 - 12 );
		const float fovSphere = 23.4f;

		public void PrepareForDataScreenshot(){

			// Reset rotation
			// Reset zoom
			// Stop auto-rotate

			rotMesh.x = rotMeshSmooth.x = rotMeshSphere.x;
			rotMesh.y = rotMeshSmooth.y = rotMeshSphere.y;
			cam.fieldOfView = targetFOV = smoothFOV = fovSphere;

		}


		public int OnGUI( int yOffset, int maxWidth ) {

			if( enabled == false )
				return yOffset;

			Rect topBar = new Rect( 0, yOffset, maxWidth, 18 );
			

			GUI.Box( topBar, "", EditorStyles.toolbar );

			Rect r = new Rect( topBar );
			r.width = maxWidth / 3;
			r.height = 16;
			r.x += 10;
			r.y += 1;

			//EditorGUILayout.BeginHorizontal();
			//{
			EditorGUI.BeginChangeCheck();
			mesh = (Mesh)EditorGUI.ObjectField(r, mesh, typeof( Mesh ), false );
			if( EditorGUI.EndChangeCheck() ) {
				targetFOV = 35f;
				//editor.Defocus(); // TODO: This is a bit hacky
			}

			r.x += r.width + 10;
			r.width *= 0.5f;
			EditorGUI.BeginChangeCheck();
			GUI.enabled = cam.clearFlags != CameraClearFlags.Skybox;
			//GUI.color = GUI.enabled ? Color.white : new Color(1f,1f,1f,0.5f);
			settings.colorBg = EditorGUI.ColorField( r, "", settings.colorBg );
			cam.backgroundColor = settings.colorBg;

			GUI.enabled = true;
			//GUI.color = Color.white;
			if( EditorGUI.EndChangeCheck() )
				UpdatePreviewBackgroundColor();


			r.x += r.width + 10;
			r.width += 10;


			GUI.enabled = RenderSettings.skybox != null;
			SkyboxOn = GUI.Toggle( r, SkyboxOn, "Skybox" );
			if(RenderSettings.skybox == null && SkyboxOn){
				SkyboxOn = false;
			}
			GUI.enabled = true;

			r.x += r.width + 10;
			settings.previewAutoRotate = GUI.Toggle( r, settings.previewAutoRotate, "Rotate" );


			Rect previewRect = new Rect( topBar );
			previewRect.y += topBar.height;
			previewRect.height = topBar.width;

			
			UpdateCameraZoom();
			DrawMeshGUI( previewRect );
			if(SF_Debug.renderDataNodes)
				GUI.Label(previewRect, "rotMesh.x = " + rotMesh.x + "  rotMesh.y = " + rotMesh.y);

			return (int)previewRect.yMax;
		}

		public void UpdateRenderPath(){
			SFPSC_Lighting.RenderPath rPath = editor.ps.catLighting.renderPath;

			if(rPath == SFPSC_Lighting.RenderPath.Forward){
				cam.renderingPath = RenderingPath.Forward;
			} else if(rPath == SFPSC_Lighting.RenderPath.Deferred){
				cam.renderingPath = RenderingPath.DeferredLighting;
				//pruCam.clearFlags == CameraClearFlags.Depth;
			}
		}

		public void UpdateRot(){
			if(settings.previewAutoRotate){
				rotMesh.y += (float)(editor.deltaTime * -22.5);
			}
			rotMeshSmooth = Vector2.Lerp(rotMeshSmooth,rotMesh,0.5f);
		}

		public void StartDragLMB() {
			isDraggingLMB = true;
			if(settings.previewAutoRotate == true){
				settings.previewAutoRotate = false;
			}
			dragStartPosLMB = Event.current.mousePosition;
			rotMeshStart = rotMesh;
		}

		public void UpdateDragLMB() {
			rotMesh.y = rotMeshStart.y + ( -(dragStartPosLMB.x - Event.current.mousePosition.x) ) * 0.4f;
			rotMesh.x = Mathf.Clamp( rotMeshStart.x + ( -(dragStartPosLMB.y - Event.current.mousePosition.y) ) * 0.4f, -90f, 90f );
		}

		public void StopDragLMB() {
			isDraggingLMB = false;
		}


		public void StartDragRMB() {
			isDraggingRMB = true;
		}

		public void UpdateDragRMB() {

			if( Event.current.isMouse && Event.current.type == EventType.MouseDrag ) {
				float x = ( -( Event.current.delta.x ) ) * 0.4f;
				float y = ( -( Event.current.delta.y ) ) * 0.4f;
				for( int i = 0; i < lights.Length; i++ ) {
					lights[i].transform.RotateAround( Vector3.zero, cam.transform.right, y );
					lights[i].transform.RotateAround( Vector3.zero, cam.transform.up, x );
				}
			}
			

		}

		public void StopDragRMB() {
			isDraggingRMB = false;
		}


		public bool MouseOverPreview() {
			return previewRect.Contains( Event.current.mousePosition );
		}
	
		[SerializeField]
		Rect previewRect = new Rect(0f,0f,1f,1f);
		public void DrawMeshGUI( Rect previewRect ) {

			if( previewRect == default( Rect ) ) {
				previewRect = this.previewRect;
			}

			if( previewRect.width > 1 )
				this.previewRect = previewRect;

			if( Event.current.rawType == EventType.MouseUp ) {
				if( Event.current.button == 0 ) 
					StopDragLMB();
				else if( Event.current.button == 1 ) 
					StopDragRMB();
			}

			if( Event.current.type == EventType.MouseDown && MouseOverPreview() ) {
				if( Event.current.button == 0 )
					StartDragLMB();
				else if( Event.current.button == 1 )
					StartDragRMB();
			}

			if( isDraggingLMB )
				UpdateDragLMB();
			if( isDraggingRMB )
				UpdateDragRMB();


			if( mesh == null || InternalMaterial == null || Event.current.type != EventType.Repaint )
				return;

			

			if( previewStyle == null ) {
				previewStyle = new GUIStyle( EditorStyles.textField );
			}
			previewStyle.normal.background = backgroundTexture;



			bool makeNew = false;
			if( render == null ) {
				makeNew = true;
			} else if( render.width != (int)previewRect.width || render.height != (int)previewRect.height ) {
				RenderTexture.DestroyImmediate( render );
				makeNew = true;
			}

			if( makeNew ) {
				render = new RenderTexture( (int)previewRect.width, (int)previewRect.height, 24, RenderTextureFormat.ARGB32 );
				render.antiAliasing = 8;
			}

			DrawMesh();
			GL.sRGBWrite = ( QualitySettings.activeColorSpace == ColorSpace.Linear );
			GUI.DrawTexture( previewRect, render, ScaleMode.StretchToFill, false );
			GL.sRGBWrite = false;

		}



		public void DrawMesh( RenderTexture overrideRT = null, Material overrideMaterial = null, bool sphere = false ) {
			if( backgroundTexture == null )
				UpdatePreviewBackgroundColor();

			// Make sure all objects are set up properly
			if( previewIsSetUp == false ) {
				SetupPreview();
			}
			

			// TODO: Override RT is used for screenshots, probably
			if( overrideRT != null )
				cam.targetTexture = overrideRT;
			else if( cam.targetTexture == null )
				cam.targetTexture = render;

			UpdateRenderPath();

			SetCustomLight(on:true);

			Mesh drawMesh = sphere ? sphereMesh : mesh;

			float A = sphere ? rotMeshSphere.y : rotMeshSmooth.y;
			float B = sphere ? rotMeshSphere.x : rotMeshSmooth.x;
			Quaternion rotA = Quaternion.Euler( 0f, A, 0f );
			Quaternion rotB = Quaternion.Euler( B, 0f, 0f );
			Quaternion finalRot = rotA * rotB;
			camPivot.rotation = finalRot;
			float meshExtents = drawMesh.bounds.extents.magnitude;


			Vector3 pos = new Vector3( -drawMesh.bounds.center.x, -drawMesh.bounds.center.y, -drawMesh.bounds.center.z );
			cam.transform.localPosition = new Vector3( 0f, 0f, -3f * meshExtents );

			int smCount = drawMesh.subMeshCount;

			Material mat = (overrideMaterial == null) ? InternalMaterial : overrideMaterial;
			for( int i=0; i < smCount; i++ ) {
				Graphics.DrawMesh( drawMesh, Quaternion.identity * pos, Quaternion.identity, mat, 31, cam, i );
			}

			cam.farClipPlane = 3f * meshExtents * 2f;
			cam.nearClipPlane = 0.1f;
			cam.fieldOfView = sphere ? fovSphere : smoothFOV;
			cam.Render();

			// Reset things
			SetCustomLight( on: false );

			if( overrideRT != null )
				cam.targetTexture = render;

			if( sphere ) // Reset if needed. // TODO: What?
				cam.fieldOfView = smoothFOV;
		}




		[SerializeField]
		const float minFOV = 1f;
		[SerializeField]
		float targetFOV = 30f;
		[SerializeField]
		float smoothFOV = 30f;
		[SerializeField]
		const float maxFOV = 60f;

		public void UpdateCameraZoom() {

			if( Event.current.type == EventType.ScrollWheel && MouseOverPreview() ) {
				if(Event.current.delta.y > 0f){
					targetFOV+=2f;
				} else if( Event.current.delta.y < 0f ){
					targetFOV-=2f;
				}
			}
			if( Event.current.type == EventType.Repaint ) {
				targetFOV = Mathf.Clamp( targetFOV, minFOV, maxFOV );
				smoothFOV = Mathf.Lerp( cam.fieldOfView, targetFOV, 0.5f );
			}
		}


		public void UpdatePreviewBackgroundColor() {
			if( backgroundTexture == null ){
				backgroundTexture = new Texture2D( 1, 1, TextureFormat.ARGB32, false, QualitySettings.activeColorSpace == ColorSpace.Linear );
				backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			Color c = settings.colorBg;
			backgroundTexture.SetPixels( new Color[] { c } );
			backgroundTexture.Apply();
		}

		public void SetCustomLight(bool on) {
			if( on ) {
				UnityEditorInternal.InternalEditorUtility.SetCustomLighting( lights, RenderSettings.ambientLight );
			} else {
				UnityEditorInternal.InternalEditorUtility.RemoveCustomLighting();
			}
		}




	}
}