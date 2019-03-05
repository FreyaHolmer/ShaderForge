using UnityEngine;
using System.Collections;

namespace ShaderForge {

	public static class SF_Blit {



		static Mesh _blitQuad;
		public static Mesh blitQuad {
			get {
				if( _blitQuad == null ) {

					_blitQuad = new Mesh();

					Vector3[] verts = new Vector3[]{
						new Vector3(-1,1),
						new Vector3(1,1),
						new Vector3(-1,-1),
						new Vector3(1,-1)
					};
					Vector2[] uvs = new Vector2[]{
						new Vector2(0,1),
						new Vector2(1,1),
						new Vector2(0,0),
						new Vector2(1,0)
					};
					Vector3[] normals = new Vector3[]{
						Vector3.back,
						Vector3.back,
						Vector3.back,
						Vector3.back
					};
					Vector4[] tangents = new Vector4[]{
						new Vector4(1,0,0,1),
						new Vector4(1,0,0,1),
						new Vector4(1,0,0,1),
						new Vector4(1,0,0,1)
					};
					Color[] colors = new Color[]{
						new Color(1f,0f,0f,0f),
						new Color(0f,1f,0f,0f),
						new Color(0f,0f,1f,0f),
						new Color(0f,0f,0f,1f),
					};

					int[] triangles = new int[]{
						0,1,2,
						1,3,2
					};

					_blitQuad.vertices = verts;
					_blitQuad.triangles = triangles;
					_blitQuad.normals = normals;
					_blitQuad.tangents = tangents;
					_blitQuad.colors = colors;
					_blitQuad.uv = uvs;
					_blitQuad.uv2 = uvs;
					_blitQuad.uv3 = uvs;
					_blitQuad.uv4 = uvs;

				}
				return _blitQuad;
			}
		}



		static GameObject blitObject;
		public static Camera _blitCamera;
		public static Camera blitCamera {
			get {
				if( blitObject == null ) {
					blitObject = new GameObject( "SHADER_FORGE_BLIT_CAMERA" );
					blitObject.hideFlags = HideFlags.HideAndDontSave;
					blitObject.transform.position = new Vector3( 0, 0, -4f );
					_blitCamera = blitObject.AddComponent<Camera>();
					_blitCamera.enabled = false;
					_blitCamera.clearFlags = CameraClearFlags.Nothing;
					_blitCamera.orthographic = true;
					_blitCamera.orthographicSize = 1f;
					_blitCamera.nearClipPlane = 3.5f;
					_blitCamera.farClipPlane = 4.5f;
					_blitCamera.enabled = false;
					_blitCamera.cullingMask = ( 1 << 7 );
				}
				return _blitCamera;
			}
		}






		static string[] defaultInputNames = new string[] {
			"_A",
			"_B",
			"_C",
			"_D",
			"_E",
			"_F",
			"_G",
			"_H",
			"_I",
			"_J",
			"_K",
			"_L"
		};

		public static SF_Node currentNode;
		static Material _mat;
		public static Material mat {
			get {
				if( _mat == null )
					_mat = new Material( Shader.Find( "Hidden/Shader Forge/FillColor" ) );
				return _mat;
			}
		}

		static Material _matColor;
		public static Material matColor {
			get {
				if( _matColor == null )
					_matColor = new Material( Shader.Find( "Hidden/Shader Forge/FillColor" ) );
				return _matColor;
			}
		}

		static Material _matExtractChannel;
		public static Material matExtractChannel {
			get {
				if( _matExtractChannel == null )
					_matExtractChannel = new Material( Shader.Find( "Hidden/Shader Forge/ExtractChannel" ) );
				return _matExtractChannel;
			}
		}

		public static void RenderUsingViewport( RenderTexture target, string shader ) {
			LoadShaderForMaterial( shader );
			RenderUsingViewport( target, mat );
		}

		static void LoadShaderForMaterial( string shader ) {
			Shader s = Shader.Find( "Hidden/Shader Forge/" + shader );
			if(s == null)
				Debug.LogError("Shader not found: " + shader );
			mat.shader = s;
		}

		public static void Render( RenderTexture target, string shader, string[] inputNames, Texture[] inputTextures ) {

			LoadShaderForMaterial( shader );

			for( int i = 0; i < inputTextures.Length; i++ ) {
				mat.SetTexture( "_" + inputNames[i], inputTextures[i] );
			}

			Render( target, mat );
		}

		public static void RenderUsingViewport( RenderTexture target, string shader, string[] inputNames, Texture[] inputTextures ) {
			LoadShaderForMaterial( shader );
			for( int i = 0; i < inputTextures.Length; i++ ) {
				mat.SetTexture( "_" + inputNames[i], inputTextures[i] );
			}
			RenderUsingViewport( target, mat );
		}

		public static void Render( RenderTexture target, string shader, params Texture[] inputTextures ) {
			Render( target, shader, defaultInputNames, inputTextures );
		}

		public static void Render( RenderTexture target, Material material ) {
			ApplyComponentCountMask( material );

			blitCamera.targetTexture = target;
			Graphics.DrawMesh( blitQuad, Vector3.zero, Quaternion.identity, material, 7, blitCamera );
			blitCamera.Render();
			blitCamera.targetTexture = null;

			//Graphics.SetRenderTarget( target );
			//Graphics.DrawMesh( blitQuad, Matrix4x4.identity, material, -1, null );
			//Graphics.DrawMeshNow( blitQuad, -Vector3.forward * 0.5f, Quaternion.identity );
			
			//Graphics.Blit( null, target, material );
		}

		static void ApplyComponentCountMask(Material material) {
			int cc = GetComponentCountAndPrepare(material);
			Vector4 mask = CompCountToMask( cc );
			material.SetVector( "_OutputMask", mask );
		}

		static int GetComponentCountAndPrepare(Material material) {
			if( currentNode != null ) {
				currentNode.PrepareRendering( material );
				return currentNode.ReadComponentCountFromFirstOutput();
			}
			return 4;
		}

		public static void RenderUsingViewport( RenderTexture target, Material material ) {
			ApplyComponentCountMask( material );
			bool sphere = SF_Settings.nodeRenderMode == NodeRenderMode.Spheres || SF_Settings.nodeRenderMode == NodeRenderMode.Mixed;
			SF_Editor.instance.preview.DrawMesh( target, material, sphere );
		}

		public static Vector4 CompCountToMask(int cc) {
			if( cc == 2 )
				return new Vector4( 1, 1, 0, 0 );
			if( cc == 3 )
				return new Vector4( 1, 1, 1, 0 );
			return Vector4.one;
		}

		public static void Render( RenderTexture target, Color color ) {
			matColor.color = color;
			Render( target, matColor );
		}




	}

}
