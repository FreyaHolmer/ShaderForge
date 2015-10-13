using UnityEngine;
using System.Collections;

namespace ShaderForge {

	public static class SF_Blit {


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
			mat.shader = Shader.Find( "Hidden/Shader Forge/" + shader );
		}

		public static void Render( RenderTexture target, string shader, string[] inputNames, Texture[] inputTextures ) {

			LoadShaderForMaterial( shader );

			for( int i = 0; i < inputTextures.Length; i++ ) {
				mat.SetTexture( "_" + inputNames[i], inputTextures[i] );
			}

			Render( target, mat );
		}

		public static void Render( RenderTexture target, string shader, params Texture[] inputTextures ) {
			Render( target, shader, defaultInputNames, inputTextures );
		}

		public static void Render( RenderTexture target, Material material ) {
			ApplyComponentCountMask( material );
			Graphics.Blit( null, target, material );
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
			SF_Editor.instance.preview.DrawMesh( target, material );
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
