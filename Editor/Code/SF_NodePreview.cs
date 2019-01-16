using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

namespace ShaderForge {
	[System.Serializable]
	public class SF_NodePreview : ScriptableObject {

		public static int R = 0;
		public static int G = 1;
		public static int B = 2;
		public static int A = 3;

		// The color representation 
		public RenderTexture texture;			// RGBA combined
		public RenderTexture[] textureChannels; // RGBA separated, created on-demand

		// Icons, if any
		public Texture2D[] icons;
		public Texture2D iconActive;
		
		public void SetIconId(int id){
			iconActive = icons[id];
		}


		public Color iconColor = Color.white;

		// Whether or not it's uniform
		// Vectors (Uniform = Same color regardless of position)
		// Textures (Non-Uniform = Different color based on position))
		public bool uniform = false;
		public bool coloredAlphaOverlay = false; // Used to render two images on top of eachother, as in the fog node
		//public float[] dataUniform;
		public Vector4 dataUniform;
		public Color dataUniformColor{
			get { return (Color)dataUniform; }
		}

		// My material node, used to get operators
		public SF_Node node;

		// The amount of components used (1-4) // THIS SHOULDN'T BE USED. USE CONNECTOR COMP COUNT INSTEAD
		[SerializeField]
		private int compCount = 1;
		public int CompCount {
			get { return compCount; }
			set {
				if(compCount == value)
					return;
				if( value > 4 || value < 1 ) {
					//Debug.LogError( "Component count out of range: " + value + " on " + node.nodeName + " " + node.id );
					compCount = 4;
				} else {
					compCount = value;
				}
			}
		}



		public SF_NodePreview() {
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;

			if( texture == null ) {
				InitializeTexture();
				if(node)
					node.RefreshValue();
			}

		}

		public RenderTexture CreateNewNodeRT() {
			RenderTexture rt = new RenderTexture( SF_Node.NODE_SIZE, SF_Node.NODE_SIZE, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear ); // TODO: Gamma/Linear?
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.hideFlags = HideFlags.HideAndDontSave;
			return rt;
		}

		public SF_NodePreview Initialize( SF_Node node ) {
			this.node = node; // Parent
			return this;
		}


		public ColorSpace textureColorSpace = ColorSpace.Uninitialized;

		public void InitializeTexture() {
			if(texture == null)
				texture = CreateNewNodeRT();
			textureColorSpace = QualitySettings.activeColorSpace;
		}



		public void DestroyTexture() {
			if( RenderTexture.active == texture )
				RenderTexture.active = null;
			if( texture != null ) {
				texture.Release();
				DestroyImmediate( texture );
			}

			if( textureChannels != null ) {
				for( int i = 0; i < textureChannels.Length; i++ ) {
					if( textureChannels[i] != null ) {
						if( RenderTexture.active == textureChannels[i] )
							RenderTexture.active = null;
						textureChannels[i].Release();
						DestroyImmediate( textureChannels[i] );
					}
				}
			}

			iconActive = null;
			texture = null;
		}
	


		public void LoadAndInitializeIcons(Type type){
			string nodeNameLower = type.Name.ToLower();


			iconActive = SF_Resources.LoadNodeIcon(nodeNameLower); // Main icon

			
			if(iconActive == null){
				//Debug.Log("No icon found for: " + nodeNameLower);
			} else {
				// See if additional ones exist, if it found the first

				List<Texture2D> iconList = new List<Texture2D>();
				iconList.Add(iconActive);

				Texture2D tmp;
				for(int i = 2;i<16;i++){ // max 16, to prevent while-loop locking
					tmp = SF_Resources.LoadNodeIcon(nodeNameLower + "_" + i); // Search for more
					if(tmp == null)
						break;
					iconList.Add(tmp);
				}

				if(iconList.Count > 1)
					icons = iconList.ToArray();

				//while( tmp =
			}
		}

		public void LoadDataTexture(Type type){
			LoadDataTexture("Data/" + type.Name.ToLower());
		}

		public void LoadDataTexture(Type type, string suffix){
			LoadDataTexture("Data/" + type.Name.ToLower() + "_" + suffix);
		}

		public void LoadDataTexture(string path){
			Texture2D nodeIcon = SF_Resources.LoadNodeIcon(path);
			SF_Blit.Render( texture, "ReadPackedData", nodeIcon );
		}

		public void GenerateBaseData( bool render3D = true ) {
			SF_Blit.mat.SetVector( "_OutputMask", Vector4.one );

			SF_Blit.currentNode = node;

			if( uniform ) {
				BlitUniform();
				return;
			}
			
			if( SF_Settings.nodeRenderMode == NodeRenderMode.Viewport ) {
				SF_Blit.RenderUsingViewport( texture, node.GetBlitShaderPath() );
			} else {
				if( render3D )
					SF_Blit.RenderUsingViewport( texture, node.GetBlitShaderPath() );
				else
					SF_Blit.Render( texture, node.GetBlitShaderPath() );
			}

			
		}

		public void BlitUniform() {
			SF_Blit.Render(texture, dataUniformColor );
		}

		public void ReadData( Texture2D tex, SF_NodePreview uvTex = null ) {
			Graphics.Blit( tex, texture );
		}




		public void OnLostConnection() {
			Fill( Color.black );
		}

		public void Fill( Color col ) {
			SF_Blit.Render( texture, col );
		}

		public Texture RenderAndGetChannel(int ch){
			if(textureChannels == null)
				textureChannels = new RenderTexture[4];
			if( ch < 0 || ch > 3 ) {
				Debug.LogError( "RenderAndGetChannel() got invalid channel " + ch + " of node " + node.nodeName + ". Please report this!" );
			}
			if( textureChannels[ch] == null ) {
				textureChannels[ch] = CreateNewNodeRT();
			}
			SF_Blit.matExtractChannel.SetFloat("_Channel", ch);
			Graphics.Blit( texture, textureChannels[ch], SF_Blit.matExtractChannel );
			return textureChannels[ch];
		}

		public Texture GetTextureByOutputType( OutChannel ch ) {
			if( ch == OutChannel.R ) {
				return RenderAndGetChannel( 0 );
			} else if( ch == OutChannel.G ) {
				return RenderAndGetChannel( 1 );
			} else if( ch == OutChannel.B ) {
				return RenderAndGetChannel( 2 );
			} else if( ch == OutChannel.A ) {
				return RenderAndGetChannel( 3 );
			}
			return texture;			
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

			// Combine the node textures, unless we're quickloading or don't want to load them

			dataUniform = node.EvalCPU();

			SF_Blit.currentNode = node;

			//if( uniform ) {
				//BlitUniform();
			//} else {
				string shaderPath = node.GetBlitShaderPath();
				Texture[] inputTextures = node.ConnectedInputs.Select( x => x.inputCon.node.texture.GetTextureByOutputType( x.inputCon.outputChannel ) ).ToArray();
				string[] inputNames = node.ConnectedInputs.Select( x => x.strID ).ToArray();
				//OutChannel[] inputChannels = node.ConnectedInputs.Select( x => x.inputCon.outputChannel ).ToArray();
				if( SF_Settings.nodeRenderMode == NodeRenderMode.Viewport ) {
					SF_Blit.RenderUsingViewport( texture, shaderPath, inputNames, inputTextures );
				} else if( SF_Settings.nodeRenderMode == NodeRenderMode.Mixed ) {
					SF_Blit.Render( texture, shaderPath, inputNames, inputTextures );
				}
				
			//}

			
			



			/*
			if(!SF_Parser.quickLoad && SF_Settings.DrawNodePreviews) {
				for( int y = 0; y < SF_NodeData.RES; y++ ) {
					for( int x = 0; x < SF_NodeData.RES; x++ ) {
						Color retVector = node.NodeOperator( x, y );
						for( int c = 0; c < 4; c++ ) {
							data[x, y, c] = retVector[c];
						}
					}
				}
			}*

			// Combine uniform
			/*for( int i = 0; i < 4; i++ ) {
				dataUniform[i] = node.NodeOperator( 0, 0, i );
			}*/
			



		}



		public void Draw( Rect r , bool dim = false) {
			if( iconActive != null ) {
				if(node is SFN_Final){ // Large node image
					Rect tmp = new Rect(r.x,r.y-1, iconActive.width, iconActive.height);
					GUI.color = new Color(1f,1f,1f,node.selected ? 1f : 0.5f);
					GUI.DrawTexture( tmp, iconActive, ScaleMode.ScaleToFit, true );
				} else if( coloredAlphaOverlay ) {
					GUI.DrawTexture( r, icons[0] );
					GUI.color = ConvertToDisplayColor( dataUniform, true );
					GUI.DrawTexture( r, icons[1], ScaleMode.ScaleToFit, true );
				} else {
					GUI.color = iconColor;
					if( dim ) {
						GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b, 0.5f );
					}
					GUI.DrawTexture( r, iconActive );
				}
				GUI.color = Color.white;
			} else if( uniform ) {
				GUI.color = ConvertToDisplayColor( dataUniform, true );
				GUI.DrawTexture( r, EditorGUIUtility.whiteTexture );
				GUI.color = Color.white;
			} else {
				GUI.DrawTexture( r, texture, ScaleMode.ScaleAndCrop, false );
				if(node.displayVectorDataMask){
					GUI.DrawTexture( r, SF_GUI.VectorIconOverlay, ScaleMode.ScaleAndCrop, true);
				}
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



		public bool CanCombine( SF_NodePreview a, SF_NodePreview b ) {
			if( a.CompCount == b.CompCount )
				return true;
			if( a.CompCount == 1 || b.CompCount == 1 )
				return true;
			return false;
		}












	}
}