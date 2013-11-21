using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ShaderForge {
	[System.Serializable]
	public class SF_NodePreview : ScriptableObject {

		public static int R = 0;
		public static int G = 1;
		public static int B = 2;
		public static int A = 3;

		// The color representation 
		private Texture2D texture;
		public Texture2D Texture {
			get {
				if( texture == null )
					UpdateColorPreview();
				return texture;
			}
		}

		// Icon, if any
		public Texture2D icon;

		public Color iconColor = Color.white;

		// The actual data
		public SF_NodeData data;

		// Whether or not it's uniform
		// Vectors (Uniform = Same color regardless of position)
		// Textures (Non-Uniform = Different color based on position))
		public bool uniform = false;
		//public float[] dataUniform;
		public Color dataUniform;

		// My material node, used to get operators
		public SF_Node node;

		// The amount of components used (1-4)
		[SerializeField]
		private int compCount = 1;
		public int CompCount {
			get { return compCount; }
			set {
				if( value > 4 || value < 1 ) {
					Debug.LogError( "Component count out of range: " + value + " on " + node.nodeName + " " + node.id );
				} else {
					compCount = value;
					UpdateColorPreview();
				}
			}
		}



		// Accessor
		public float this[int a, int b, int c] {
			get {
				node.OnPreGetPreviewData();
				if( uniform ) {
					return ( compCount == 1 ) ? dataUniform[0] : dataUniform[c];
				}
				return data[a, b, c];
			}
			set {
				Debug.LogWarning( "Access violation" );
			}
		}

		public Vector4 this[int a, int b] {
			get {
				if( uniform ) {
					return new Vector4( dataUniform[0], dataUniform[1], dataUniform[2], dataUniform[3] );
				}
				return new Vector4( data[a, b, 0], data[a, b, 1], data[a, b, 2], data[a, b, 3] );
			}
			set {
				Debug.LogWarning( "Access violation" );
			}
		}



		public SF_NodePreview() {
			InitializeTexture();
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;

			if( texture == null ) {
				InitializeTexture();
				node.RefreshValue();
				ReadData( texture );
			}

		}



		public SF_NodePreview Initialize( SF_Node node ) {

			// Parent
			this.node = node;

			// The color representation
			//InitializeTexture();

			// The actual data
			data = ScriptableObject.CreateInstance<SF_NodeData>().Initialize( this.node );
			dataUniform = new Color(0f,0f,0f,0f);
			return this;
		}


		public void InitializeTexture() {
			texture = new Texture2D( 128, 128, TextureFormat.ARGB32, false, false/*LINEAR, true?*/);
			texture.wrapMode = TextureWrapMode.Clamp;
		}



		public void DestroyTexture() {
			DestroyImmediate( texture );
			icon = null;
			texture = null;
		}



		public void GenerateTexcoord() {
			//		Color[] colors = new Color[16384];
			for( int y = 0; y < 128; y++ ) {
				for( int x = 0; x < 128; x++ ) {
					data[x, y, 0] = x / 128f;
					data[x, y, 1] = y / 128f;
					data[x, y, 2] = 0f;
					data[x, y, 3] = 0f;
				}
			}
			UpdateColorPreview();
		}



		public void ReadData( Texture2D tex, SF_NodePreview uvTex = null ) {

			for( int y = 0; y < 128; y++ ) {
				for( int x = 0; x < 128; x++ ) {

					Color col;
					float U;
					float V;

					if( uvTex ) {
						Color c = uvTex[x, y];
						U = c.r;
						V = c.g;
					} else {
						U = x / 128f;
						V = y / 128f;
					}

					col = tex.GetPixelBilinear( U, V );

					data[x, y, 0] = col.r;
					data[x, y, 1] = col.g;
					data[x, y, 2] = col.b;
					data[x, y, 3] = col.a;
				}
			}

			UpdateColorPreview();

		}




		public void OnLostConnection() {
			Fill( Color.black );
		}

		public void Fill( Color col ) {

			//if(data == null)
			//data = new float[128, 128, 4]; // DEBUG

			for( int y = 0; y < 128; y++ ) {
				for( int x = 0; x < 128; x++ ) {
					for( int c = 0; c < 4; c++ ) { // Channels
						data[x, y, c] = col[c];
					}
				}
			}
			UpdateColorPreview();
		}



		public void UpdateColorPreview() {
			Color[] texCols = new Color[128 * 128];
			for( int y = 0; y < 128; y++ ) {
				for( int x = 0; x < 128; x++ ) {
					texCols[y * 128 + x] = new Color( data[x, y, 0], data[x, y, 1], data[x, y, 2], data[x, y, 3] );
				}
			}
			if( texture == null )
				InitializeTexture();

			texture.SetPixels( texCols );
			texture.Apply();
		}


		// When evaluating nodes, run the overridden operator from the node itself
		public void Combine( /*SF_NodePreview a, SF_NodePreview b */) {

	
			// Check if it can combine first
			if( !node.CanEvaluate() ) {
				Debug.LogError( "Cannot evaluate" );
				Fill( Color.black );
				return;
			}

			CompCount = node.GetEvaluatedComponentCount();

			// It can combine! Since this node is dynamic, adapt its component count
			//CompCount = Mathf.Max( a.CompCount, b.CompCount );

			uniform = node.IsUniformOutput();

			// Combine the node textures
			for( int y = 0; y < 128; y++ ) {
				for( int x = 0; x < 128; x++ ) {
					Color retVector = node.NodeOperator( x, y );
					for( int c = 0; c < 4; c++ ) {
						data[x, y, c] = retVector[c];
					}
				}
			}

			// Combine uniform
			/*for( int i = 0; i < 4; i++ ) {
				dataUniform[i] = node.NodeOperator( 0, 0, i );
			}*/
			dataUniform = node.NodeOperator( 0, 0 );



			// After assigning data, update the visible preview
			UpdateColorPreview();


		}

		//public void CombineUniform( SF_NodePreview a, SF_NodePreview b ) {


		//}
		 


		public void Draw( Rect r ) {

			if( icon != null ) {
				GUI.color = iconColor;
				GUI.DrawTexture( r, icon );
				GUI.color = Color.white;
			} else if( uniform ) {
				GUI.color = ConvertToDisplayColor( dataUniform, true );
				GUI.DrawTexture( r, EditorGUIUtility.whiteTexture );
				GUI.color = Color.white;
			} else {
				GUI.DrawTexture( r, Texture, ScaleMode.ScaleAndCrop, false );
			}

		}

		public static float[] ColorToFloatArr( Color c ) {
			return new float[] { c.r, c.g, c.b, c.a };
		}

		public Color ConvertToDisplayColor( Color fa, bool forceVisible = false ) {
			if( CompCount == 1 ) {
				return new Color( fa[0], fa[0], fa[0], forceVisible ? 1f : fa[0] );
			} else if( CompCount == 2 ) {
				return new Color( fa[0], fa[1], 0f, forceVisible ? 1f : 0f );
			} else if( CompCount == 3 ) {
				return new Color( fa[0], fa[1], fa[2], forceVisible ? 1f : 0f );
			}
			return new Color( fa[0], fa[1], fa[2], forceVisible ? 1f : fa[3] );
		}


		//	public void EvaluateUniform(MaterialNode a, MaterialNode b){
		//		uniform = true;
		//		vector = new float[Mathf.Max(a.vector.Length,b.vector.Length)];
		//		if(a.vector.Length == b.vector.Length){
		//			vector = OperatorSameLength(a.vector,b.vector); // SAMPLE
		//		} else if(a.vector.Length == 1 || b.vector.Length == 1){
		//			if(a.vector.Length == 1)
		//				vector = OperatorSingleScalarLength(b.vector,a.vector[0]); // SAMPLE
		//			else
		//				vector = OperatorSingleScalarLength(a.vector,b.vector[0]); // SAMPLE
		//		} else {
		//			// Invalid!
		//			Debug.LogWarning("Invalid cast between vector" + a.vector.Length + " and vector" + b.vector.Length + " on " + name + " node");
		//			vector = new float[4]{0,0,0,0};
		//			texture = null;
		//		}
		//	}




		public bool CanCombine( SF_NodePreview a, SF_NodePreview b ) {
			if( a.CompCount == b.CompCount )
				return true;
			if( a.CompCount == 1 || b.CompCount == 1 )
				return true;
			return false;
		}












	}
}