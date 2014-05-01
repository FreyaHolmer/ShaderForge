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
					internalMaterial = (Material)Resources.Load("ShaderForgeInternal",typeof(Material));

					if(internalMaterial == null){ 

						string meshesPath = AssetDatabase.GetAssetPath( Resources.Load("Meshes/sf_meshes",typeof(Mesh)) );
						string sf_resourcePath = meshesPath.Substring(0,meshesPath.Length - 20);
						string matPath = sf_resourcePath + "ShaderForgeInternal.mat";
						//Debug.Log(matPath);
						AssetDatabase.CreateAsset(internalMaterial = new Material(editor.currentShaderAsset), matPath );
						//AssetDatabase.Refresh(ImportAssetOptions.DontDownloadFromCacheServer);

					}

				}
				return internalMaterial;
			}
			set {
				internalMaterial = value;
			}
		}

		[SerializeField]
		Texture render;
		[SerializeField]
		GUIStyle previewStyle;
		[SerializeField]
		public Texture2D backgroundTexture;


		// Input/Rotation
		[SerializeField]
		public bool isDragging = false;
		[SerializeField]
		Vector2 dragStartPos = Vector2.zero;
		[SerializeField]
		Vector2 rotMeshStart = new Vector2(-30f,0f);
		[SerializeField]
		Vector2 rotMesh = new Vector2(30f,0f);
		[SerializeField]
		Vector2 rotMeshSmooth = new Vector2(-30f,0f);

		// Used for Reflection to get the preview render
		[SerializeField]
		MethodInfo pruBegin;
		[SerializeField]
		MethodInfo pruEnd;
		//[SerializeField]
		//MethodInfo pruDrawMesh;
		[SerializeField]
		object pruRef;
		[SerializeField]
		Camera pruCam;
		[SerializeField]
		Transform pruCamPivot;
		[SerializeField]
		Light[] pruLights;
		[SerializeField]
		MethodInfo ieuRemoveCustomLighting;
		[SerializeField]
		MethodInfo ieuSetCustomLighting;




		public SF_PreviewWindow( SF_Editor editor ) {

			settings = new SF_PreviewSettings( this );
			UpdatePreviewBackgroundColor();

			this.editor = editor;
			//this.material = (Material)AssetDatabase.LoadAssetAtPath( SF_Paths.pInternal + "ShaderForgeInternal.mat", typeof( Material ) );
			//this.InternalMaterial = (Material)Resources.Load("ShaderForgeInternal",typeof(Material));
			this.mesh = GetSFMesh( "sf_sphere" );

			SetupPreview();

		}


		public Mesh GetSFMesh(string find_name) {
			UnityEngine.Object[] objs = Resources.LoadAll( SF_Paths.pMeshes+"sf_meshes" );
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
			// Reflection of PreviewRenderUtility
			Type pruType = Type.GetType( "UnityEditor.PreviewRenderUtility,UnityEditor" );
			BindingFlags bfs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			pruBegin = pruType.GetMethod( "BeginPreview", bfs );
			pruEnd = pruType.GetMethod( "EndPreview", bfs );
			//pruDrawMesh = pruType.GetMethod( "DrawMesh", bfs, null,
			//new Type[] { typeof( Mesh ), typeof( Vector3 ), typeof( Quaternion ), typeof( Material ), typeof( int ) }, null );
			pruRef = Activator.CreateInstance( pruType );
			FieldInfo pruCamField = pruRef.GetType().GetField( "m_Camera" );
			pruCam = (Camera)pruCamField.GetValue( pruRef );

			pruCamPivot = new GameObject("CameraPivot").transform;
			pruCamPivot.gameObject.hideFlags = HideFlags.HideAndDontSave;
			pruCam.clearFlags = CameraClearFlags.Skybox;
			pruCam.transform.parent = pruCamPivot;


			FieldInfo pruLightsField = pruRef.GetType().GetField( "m_Light" );
			pruLights = (Light[])pruLightsField.GetValue( pruRef );

			// Reflection of InternalEditorUtility
			Type ieuType = Type.GetType( "UnityEditorInternal.InternalEditorUtility,UnityEditor" );
			bfs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			ieuSetCustomLighting = ieuType.GetMethod( "SetCustomLighting", bfs );
			ieuRemoveCustomLighting = ieuType.GetMethod( "RemoveCustomLighting", bfs );
		}


		public bool SkyboxOn{
			get{
				return pruCam.clearFlags == CameraClearFlags.Skybox;
			}
			set{
				pruCam.clearFlags = value ? CameraClearFlags.Skybox : CameraClearFlags.Depth;
			}
		}


		public int OnGUI( int yOffset, int maxWidth ) {

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
			GUI.enabled = pruCam.clearFlags == CameraClearFlags.Depth;
			//GUI.color = GUI.enabled ? Color.white : new Color(1f,1f,1f,0.5f);
			settings.colorBg = EditorGUI.ColorField( r, "", settings.colorBg );
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


			//GUILayout.Label( "Rotate:" );
			
			//GUILayout.Label( "BG" );
			r.x += r.width + 10;
			//bool bef = settings.previewAutoRotate;
			settings.previewAutoRotate = GUI.Toggle( r, settings.previewAutoRotate, "Rotate" );
			//}
			//EditorGUILayout.EndHorizontal();

			//GUI.Label(new Rect(),);

			// DEBUG:

			/*
			if( this.mesh != null ) {
				GUILayout.Label( "Current mesh: " + this.mesh.name );

				GUILayout.Label( "Current path: " + AssetDatabase.GetAssetPath( this.mesh ) );

				GUILayout.Label( "Sub asset: " + AssetDatabase.IsSubAsset( this.mesh ));
			}
			*/

		//	GUILayout.Label( string.Empty, GUILayout.Width( maxWidth ), GUILayout.Height( maxWidth ) );

			Rect previewRect = new Rect( topBar );
			previewRect.y += topBar.height;
			previewRect.height = topBar.width;

			//GUI.Box(previewRect, string.Empty, EditorStyles.textField );
			//				GUI.color = shaderEvaluator.previewBackgroundColor;
			//				GUI.DrawTexture(GUILayoutUtility.GetLastRect(),EditorGUIUtility.whiteTexture);
			//				GUI.color = Color.white;
			UpdateCameraZoom();
			DrawMesh( previewRect );
			//GUI.Box( previewRect, string.Empty/*, EditorStyles.textField*/ );
			//				GUILayout.Box(shaderEvaluator.shaderString,GUILayout.Width(340));

			return (int)previewRect.yMax;
		}

		public void UpdateRenderPath(){
			SFPSC_Lighting.RenderPath rPath = editor.ps.catLighting.renderPath;

			if(rPath == SFPSC_Lighting.RenderPath.Forward){
				pruCam.renderingPath = RenderingPath.Forward;
			} else if(rPath == SFPSC_Lighting.RenderPath.DeferredPrePass){
				pruCam.renderingPath = RenderingPath.DeferredLighting;
				//pruCam.clearFlags == CameraClearFlags.Depth;
			}
		}

		public void UpdateRot(){
			if(settings.previewAutoRotate){
				rotMesh.y += (float)(editor.deltaTime * -22.5);
			}
			rotMeshSmooth = Vector2.Lerp(rotMeshSmooth,rotMesh,0.5f);
		}

		public void StartDrag() {
			isDragging = true;
			if(settings.previewAutoRotate == true){
				settings.previewAutoRotate = false;
			}
			dragStartPos = Event.current.mousePosition;
			rotMeshStart = rotMesh;
		}

		public void UpdateDrag() {
			rotMesh.y = rotMeshStart.y + ( -(dragStartPos.x - Event.current.mousePosition.x) ) * 0.4f;
			rotMesh.x = Mathf.Clamp( rotMeshStart.x + ( -(dragStartPos.y - Event.current.mousePosition.y) ) * 0.4f, -90f, 90f );
		}

		public void StopDrag() {
			isDragging = false;
		}


		public bool MouseOverPreview() {
			return previewRect.Contains( Event.current.mousePosition );
		}
	
		[SerializeField]
		Rect previewRect = new Rect(0f,0f,1f,1f);
		public void DrawMesh( Rect previewRect ) {
			if( previewRect.width > 1 )
				this.previewRect = previewRect;

			if( Event.current.rawType == EventType.mouseUp ) {
				StopDrag();
			}

			if( Event.current.type == EventType.mouseDown && MouseOverPreview() ) {
				StartDrag();
			}

			if( isDragging )
				UpdateDrag();


			if( mesh == null || InternalMaterial == null || Event.current.type != EventType.repaint )
				return;

			if( previewStyle == null ) {
				previewStyle = new GUIStyle( EditorStyles.textField );
			}
			if( backgroundTexture == null )
				UpdatePreviewBackgroundColor();

			previewStyle.normal.background = backgroundTexture;
			if( pruRef == null ) {
				SetupPreview();
			}
			UpdateRenderPath();
			pruBegin.Invoke( pruRef, new object[] { previewRect, previewStyle } );
			PreparePreviewLight();


			// Get rotation



			float A = rotMeshSmooth.y;
			float B = rotMeshSmooth.x;
			Quaternion rotA = Quaternion.Euler( 0f, A, 0f );
			Quaternion rotB = Quaternion.Euler( B, 0f, 0f );
			Quaternion finalRot = rotA * rotB;
			pruCamPivot.rotation = finalRot;
			float meshExtents = mesh.bounds.extents.magnitude;


			//Event.current.

			//Matrix4x4 meshPos = Matrix4x4.TRS()

			Vector3 pos = new Vector3( -mesh.bounds.center.x, -mesh.bounds.center.y, -mesh.bounds.center.z);
			pruCam.transform.localPosition = new Vector3( 0f,0f, -3f * meshExtents );

			int smCount = mesh.subMeshCount;

			for(int i=0;i<smCount;i++){
				Graphics.DrawMesh( mesh, Quaternion.identity*pos, Quaternion.identity, InternalMaterial, 0, pruCam, i );
			}




			pruCam.farClipPlane = 3f * meshExtents * 2f;
			pruCam.nearClipPlane = 0.1f;
			pruCam.fieldOfView = smoothFOV;
			//Debug.Log( targetFOV );
			//pruCam.transform.position = , pruCam.transform.position.z );
			pruCam.Render();
			
			ieuRemoveCustomLighting.Invoke( null, new object[0] );
			render = (Texture)pruEnd.Invoke( pruRef, new object[0] );
			GUI.DrawTexture( previewRect, render, ScaleMode.StretchToFill, false );
			//GUI.Label(previewRect, pruCam.actualRenderingPath.ToString());
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

			if( Event.current.type == EventType.scrollWheel && MouseOverPreview() ) {
				if(Event.current.delta.y > 0f){
					targetFOV+=2f;
				} else if( Event.current.delta.y < 0f ){
					targetFOV-=2f;
				}
					
				
			}
			targetFOV = Mathf.Clamp( targetFOV, minFOV, maxFOV );
			smoothFOV = Mathf.Lerp( pruCam.fieldOfView, targetFOV, 0.25f );
		}


		public void UpdatePreviewBackgroundColor() {
			if( backgroundTexture == null ){
				backgroundTexture = new Texture2D( 2, 2, TextureFormat.ARGB32, false, QualitySettings.activeColorSpace == ColorSpace.Linear );
				backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			Color c = settings.colorBg;
			backgroundTexture.SetPixels( new Color[] { c, c, c, c } );
			backgroundTexture.Apply();
		}


		public void PreparePreviewLight() {

			Color ambient;
			//		if (this.m_LightMode == 0)
			//		{
			pruLights[0].intensity = 1f; // Directional
			pruLights[0].transform.rotation = Quaternion.Euler( 30f, 30f, 0f );
			pruLights[1].intensity = 0.75f;
			pruLights[1].color = new Color(1f,0.5f,0.25f);
			ambient = RenderSettings.ambientLight;
			//		}
			//		else
			//		{
			//			pruLights[0].intensity = 0.5f;
			//			pruLights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
			//			pruLights[1].intensity = 0.5f;
			//			ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			//		}
			ieuSetCustomLighting.Invoke( null, new object[] { pruLights, ambient } );
		}




	}
}