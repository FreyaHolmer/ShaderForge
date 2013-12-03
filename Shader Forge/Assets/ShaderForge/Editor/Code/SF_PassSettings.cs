using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	
	[System.Serializable]
	public class SF_PassSettings : ScriptableObject {


		// Mat ed
		public SF_Editor editor;
		public SF_FeatureChecker fChecker;

		// Data types
		public enum LightPrecision { Vertex, Fragment };
		public string[] strLightPrecision = new string[] { "Per-Vertex", "Per-Fragment" };
		public enum NormalQuality { Interpolated, Normalized };
		public string[] strNormalQuality = new string[] { "Interpolated", "Normalized" };
		public enum LightMode { Unlit, Lambert, BlinnPhong, Phong, PBL };
		public string[] strLightMode = new string[] { "Unlit", "Lambert", "Blinn-Phong", "Phong", "PBL" };
		public enum LightCount { Single, Multi };
		public string[] strLightCount = new string[] { "Single Directional", "Multi-light"};
		public enum CullMode { BackfaceCulling, FrontfaceCulling, DoubleSided };
		public string[] strCullMode = new string[] { "Back", "Front", "Off" };
		public enum DepthTest { Less, Greater, LEqual, GEqual, Equal, NotEqual, Always };
		public string[] strDepthTest = new string[] { "<", ">", "\u2264 (Default)", "\u2265", "=", "\u2260", "Always" };
		public enum Queue { Background, Geometry, AlphaTest, Transparent, Overlay };
		public static int[] queueNumbers = new int[] { 1000, 2000, 2450, 3000, 4000 };
		public string[] strQueue = new string[] { "Background (1000)", "Opaque Geometry (2000)", "Alpha Clip (2450)", "Transparent (3000)", "Overlay (4000)" };
		

		// 
		public enum RenderType { None, Opaque, Transparent, TransparentCutout, Background, Overlay, TreeOpaque, TreeTransparentCutout, TreeBillboard, Grass, GrassBillboard };
		public enum BlendModePreset {
			Off, 
			AlphaBlended, 
			Additive,
			Screen,
			Multiplicative, 
			Custom
		};

		public enum ShaderFogMode{ Global, Linear, Exp, Exp2 };
		
		public enum BlendMode { One, Zero, SrcColor, SrcAlpha, DstColor, DstAlpha, OneMinusSrcColor, OneMinusSrcAlpha, OneMinusDstColor, OneMinusDstAlpha };


		// Vars
		public LightPrecision lightPrecision = LightPrecision.Fragment;
		public NormalQuality normalQuality = NormalQuality.Normalized;
		public LightMode lightMode = LightMode.BlinnPhong;
		public BlendModePreset blendModePreset = BlendModePreset.Off;

		[SerializeField] private BlendMode blendSrc = BlendMode.One;
		[SerializeField] private BlendMode blendDst = BlendMode.One;
		[SerializeField] private CullMode cullMode = CullMode.BackfaceCulling;
		[SerializeField] private DepthTest depthTest = DepthTest.LEqual;
		[SerializeField] private LightCount lightCount = LightCount.Multi;

		public bool writeDepth = true;
		public bool useAmbient = true;
		public bool useFog = true;
		public bool doubleIncomingLight = false;

		// Optional PBL terms
		public bool fresnelTerm = true;
		public bool visibilityTerm = true;

		// SubShader vars
		public bool autoSort = true;
		public Queue queuePreset = (Queue)1;
		public int queueOffset = 0;
		public RenderType renderType = RenderType.Opaque;
		public bool ignoreProjector = false;
		public bool lightmapped = false;
		public bool energyConserving = false;
		public bool remapGlossExponentially = true;
		//



		// Fog
		public bool fogOverrideMode = false;
		public ShaderFogMode fogMode = ShaderFogMode.Global;

		public bool fogOverrideColor = false;
		public Color fogColor = RenderSettings.fogColor;
	
		public bool fogOverrideDensity = false;
		public float fogDensity = RenderSettings.fogDensity;

		public bool fogOverrideRange = false;
		public Vector2 fogRange = new Vector2(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance);
		//



		// Shader vars
		/* = new bool[7]{
			true,	// - Direct3D 9
			true,	// - Direct3D 11
			true,	// - OpenGL
			true,	// - OpenGL ES 2.0
			false,  // - Xbox 360
			false,	// - PlayStation 3
			false	// - Flash
		};*/
		public bool[] usedRenderers;
		public int LOD = 0;
		public string fallback = "";
		//

		// SERIALIZATION OF VARS
		public string Serialize() {
			string s = "ps:";
			s += Serialize( "lgpr", ( (int)lightPrecision ).ToString() );
			s += Serialize( "nrmq", ( (int)normalQuality ).ToString() );
			s += Serialize( "limd", ( (int)lightMode ).ToString() );
			s += Serialize( "blpr", ( (int)blendModePreset ).ToString() );
			s += Serialize( "bsrc", ( (int)blendSrc ).ToString() );
			s += Serialize( "bdst", ( (int)blendDst ).ToString() );
			s += Serialize( "culm", ( (int)cullMode ).ToString() );
			s += Serialize( "dpts", ( (int)depthTest ).ToString() );
			s += Serialize( "wrdp", writeDepth.ToString() );
			s += Serialize( "uamb", useAmbient.ToString() );
			s += Serialize( "ufog", useFog.ToString() );
			s += Serialize( "aust", autoSort.ToString() );
			s += Serialize( "igpj", ignoreProjector.ToString() );
			s += Serialize( "qofs", queueOffset.ToString() );
			s += Serialize( "lico", ( (int)lightCount ).ToString() );
			s += Serialize( "qpre", ((int)queuePreset).ToString() );
			s += Serialize( "flbk", fallback );
			s += Serialize( "rntp", ( (int)renderType ).ToString() );
			s += Serialize( "lmpd", lightmapped.ToString() );
			s += Serialize( "enco", energyConserving.ToString());
			s += Serialize( "frtr", fresnelTerm.ToString() );
			s += Serialize( "vitr", visibilityTerm.ToString() );
			s += Serialize( "dbil", doubleIncomingLight.ToString() );
			s += Serialize( "rmgx", remapGlossExponentially.ToString());

			s += Serialize( "fgom", fogOverrideMode.ToString()); 	// bool
			s += Serialize( "fgoc", fogOverrideColor.ToString());	// bool
			s += Serialize( "fgod", fogOverrideDensity.ToString());	// bool
			s += Serialize( "fgor", fogOverrideRange.ToString());	// bool

			s += Serialize( "fgmd", ((int)fogMode).ToString()); 	// FogMode
			s += Serialize( "fgcr", fogColor.r.ToString());			// Fog Color
			s += Serialize( "fgcg", fogColor.g.ToString());			// Fog Color
			s += Serialize( "fgcb", fogColor.b.ToString());			// Fog Color
			s += Serialize( "fgca", fogColor.a.ToString());			// Fog Color
			s += Serialize( "fgde", fogDensity.ToString());			// float
			s += Serialize( "fgrn", fogRange.x.ToString());			// Fog range X (Near)
			s += Serialize( "fgrf", fogRange.y.ToString(), true);	// Fog range Y (Far)



			return s;
		}

		private string Serialize( string key, string value, bool last = false ) {
			return key + ":" + value + (last ? "" : ",");
		}

		// DESERIALIZATION OF VARS
		public void Deserialize(string s) {
			string[] split = s.Split(',');
			for( int i = 0; i < split.Length; i++ ) {
				string[] keyval = split[i].Split(':');
				Deserialize( keyval[0], keyval[1] );
			}
		}

		public void Deserialize( string key, string value ) {
			switch( key ) {
			case "lgpr":
				lightPrecision = (LightPrecision)int.Parse( value );
				break;
			case "nrmq":
				normalQuality = (NormalQuality)int.Parse( value );
				break;
			case "limd":
				lightMode = (LightMode)int.Parse( value );
				break;
			case "blpr":
				blendModePreset = (BlendModePreset)int.Parse( value );
				break;
			case "bsrc":
				blendSrc = (BlendMode)int.Parse( value );
				break;
			case "bdst":
				blendDst = (BlendMode)int.Parse( value );
				break;
			case "culm":
				cullMode = (CullMode)int.Parse( value );
				break;
			case "dpts":
				depthTest = (DepthTest)int.Parse( value );
				break;
			case "wrdp":
				writeDepth = bool.Parse( value );
				break;
			case "uamb":
				useAmbient = bool.Parse( value );
				break;
			case "ufog":
				useFog = bool.Parse( value );
				break;
			case "aust":
				autoSort = bool.Parse( value );
				break;
			case "igpj":
				ignoreProjector = bool.Parse( value );
				break;
			case "qofs":
				queueOffset = int.Parse( value );
				break;
			case "lico":
				lightCount = (LightCount)int.Parse( value );
				break;
			case "qpre":
				queuePreset = (Queue)int.Parse( value );
				break;
			case "flbk":
				fallback = value;
				break;
			case "rntp":
				renderType = (RenderType)int.Parse( value );
				break;
			case "lmpd":
				lightmapped = bool.Parse( value );
				break;
			case "enco":
				energyConserving = bool.Parse( value );
				break;
			case "frtr":
				fresnelTerm = bool.Parse( value );
				break;
			case "vitr":
				visibilityTerm = bool.Parse( value );
				break;
			case "dbil":
				doubleIncomingLight = bool.Parse( value );
				break;
			case "rmgx":
				remapGlossExponentially = bool.Parse( value );
				break;

			// Fog booleans
			case "fgom":
				fogOverrideMode = bool.Parse( value );
				break;
			case "fgoc":
				fogOverrideColor = bool.Parse( value );
				break;
			case "fgod":
				fogOverrideDensity = bool.Parse( value );
				break;
			case "fgor":
				fogOverrideRange = bool.Parse( value );
				break;
			
			// Fog values
			case "fgmd":
				fogMode = (ShaderFogMode)int.Parse( value );
				break;
			case "fgcr":
				fogColor.r = float.Parse( value );
				break;
			case "fgcg":
				fogColor.g = float.Parse( value );
				break;
			case "fgcb":
				fogColor.b = float.Parse( value );
				break;
			case "fgca":
				fogColor.a = float.Parse( value );
				break;
			case "fgde":
				fogDensity = float.Parse( value );
				break;
			case "fgrn":
				fogRange.x = float.Parse( value );
				break;
			case "fgrf":
				fogRange.y = float.Parse( value );
				break;
			}

		}



		// END SERIALIZATION


		// Node/auto vars
		public string n_diffuse {
			get { return mOut.diffuse.TryEvaluate(); } // Vector3 only
		}
		public string n_alpha {
			get { return mOut.alpha.TryEvaluate(); }
		}
		public string n_alphaClip {
			get { return mOut.alphaClip.TryEvaluate(); }
		}
		public string n_diffusePower {
			get { return mOut.diffusePower.TryEvaluate(); }
		}
		public string n_gloss {
			get { return mOut.gloss.TryEvaluate(); }
		}
		public string n_specular {
			get { return mOut.specular.TryEvaluate(); } // Vector3 only
		}
		public string n_normals {
			get { return mOut.normal.TryEvaluate(); } // Vector3 only
		}
		public string n_emissive {
			get { return mOut.emissive.TryEvaluate(); } // Vector3 only
		}
		public string n_transmission {
			get { return mOut.transmission.TryEvaluate(); }
		}
		public string n_lightWrap {
			get { return mOut.lightWrap.TryEvaluate(); }
		}
		public string n_outlineWidth {
			get { return mOut.outlineWidth.TryEvaluate(); }
		}
		public string n_outlineColor {
			get { return mOut.outlineColor.TryEvaluate(); }
		}
		public string n_distortion {
			get { return mOut.refraction.TryEvaluate(); }
		}
		public string n_vertexOffset {
			get { return mOut.vertexOffset.TryEvaluate(); }
		}
		public string n_displacement {
			get { return mOut.displacement.TryEvaluate(); }
		}
		public string n_tessellation {
			get { return mOut.tessellation.TryEvaluate(); }
		}
		public SFN_Final mOut {
			get { return editor.materialOutput; }
		}


		// GUI controls
		const int expIndent = 16;
		private bool foldMeta = false;
		private bool foldProps = false;
		private bool foldLighting = false;
		private bool foldBlending = false;
		
		
		
		private int maxWidth;

		public SF_PassSettings() {

		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_PassSettings Initialize( SF_Editor materialEditor ) {
			usedRenderers = new bool[7]{ // TODO: Load from project settings
				true,	// - Direct3D 9
				true,	// - Direct3D 11
				true,	// - OpenGL
				true,	// - OpenGL ES 2.0
				false,  // - Xbox 360
				false,	// - PlayStation 3
				false	// - Flash
			};
			this.editor = materialEditor;
			fChecker = ScriptableObject.CreateInstance<SF_FeatureChecker>().Initialize(this, materialEditor);
			return this;
		}
		
		
		Rect innerScrollRect = new Rect(0,0,0,0);
		Vector2 scrollPos;

		// Call this from the editor script
		bool guiChanged = false;
		public int OnLocalGUI( int yOffset, int in_maxWidth ) {

			this.maxWidth = in_maxWidth;
			
			Rect scrollRectPos = new Rect(0f,yOffset,in_maxWidth,Screen.height-yOffset-20);
			
			bool useScrollbar = (innerScrollRect.height > scrollRectPos.height);
			
			int scrollBarWidth = useScrollbar ? 15 : 0;
			
			
			innerScrollRect.width = in_maxWidth-scrollBarWidth;

			guiChanged = false;
			
			
			
			int offset;
			
			if(innerScrollRect.height < scrollRectPos.height)
				innerScrollRect.height = scrollRectPos.height;
			
			//Debug.Log ("Inner height: " + innerScrollRect.height + " Outer height: " + scrollRectPos.height + " Event: " + Event.current.type.ToString());

			this.maxWidth -= scrollBarWidth;
			scrollPos = GUI.BeginScrollView(scrollRectPos,scrollPos,innerScrollRect,false,useScrollbar);
			{
				offset = SettingsMeta( 0 );
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = SettingsProperties( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = SettingsLighting( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = SettingsBlendingDepth( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
			}
			GUI.EndScrollView();
			this.maxWidth += scrollBarWidth;
			


			//	bool changed = EditorGUI.EndChangeCheck();


			if( guiChanged ) {
				editor.ps = this;
				editor.OnShaderModified(NodeUpdateType.Hard);
			}
			
			innerScrollRect.height = offset;
			return offset;

		}


		public bool StartExpanderChangeCheck(Rect r, ref bool foldVar, string labelContracted, string labelExpanded ) {

			
			Color prev = GUI.color;
			GUI.color = new Color(0,0,0,0);
			if( GUI.Button( r, string.Empty , EditorStyles.foldout ) )
				foldVar = !foldVar;
			GUI.color = prev;
			EditorGUI.Foldout( r, foldVar, foldVar ? labelExpanded : labelContracted );
			
			EditorGUI.BeginChangeCheck();
			if( !foldVar )
				return false;
			return true;
		}

		public bool EndExpanderChangeCheck() {
			return EditorGUI.EndChangeCheck();
		}
		
		

		public SF_Node draggingProperty = null;
		public float dragStartOffsetY = 0;
		public int dragStartIndex;
		public float startMouseY;

		public float DragRectPosY {
			get {
				return Event.current.mousePosition.y + dragStartOffsetY;
			}
		}


		const float propertyHeight = 60f;
		public int SettingsProperties( int yOffset ) {
			GUI.color = SF_Node.colorExposedDark;
			Rect topRect = new Rect( 0f, yOffset, maxWidth, 20 );
			Rect r = new Rect( topRect );

			int propCount = editor.nodeView.treeStatus.propertyList.Count;

			GUI.color = SF_Node.colorExposedDarker;
			if( foldProps ) {
				Rect fullArea = new Rect( r );
				fullArea.height += 23 + propertyHeight * propCount;
				GUI.DrawTexture( fullArea, EditorGUIUtility.whiteTexture );
			} else {
				GUI.DrawTexture( r, EditorGUIUtility.whiteTexture );
			}
			GUI.color = SF_Node.colorExposed;


			if( !StartExpanderChangeCheck( r, ref foldProps, " Properties...", " Properties" ) ) {
				GUI.color = Color.white;
				return (int)r.yMax;
			}





			if( editor.nodeView.treeStatus.propertyList.Count == 0 ) {
				r.y += 16;
				GUI.enabled = false;
				GUI.Label( r, "No properties in this shader yet" );
				GUI.enabled = true;
				r.y -= 16;
			}
			

			r.y += 23;
			r.xMin += 20; // Indent
			r.xMax -= 3;

			

			r.height = propertyHeight;

			

			// On drop...
			if( draggingProperty != null && SF_GUI.ReleasedRawLMB()) {


				int moveDist = Mathf.RoundToInt( ( Event.current.mousePosition.y - startMouseY ) / propertyHeight );

				// Execute reordering!
				if( moveDist != 0 ) { // See if it actually moved to another slot
					int newIndex = Mathf.Clamp( dragStartIndex + moveDist, 0, propCount - 1 );
					editor.nodeView.treeStatus.propertyList.RemoveAt( dragStartIndex );
					//if( newIndex > dragStartIndex )
					//	newIndex--;
					editor.nodeView.treeStatus.propertyList.Insert( newIndex, draggingProperty );
				}

				draggingProperty = null;
				

			}

			float yStart = r.y;

			
			int i = 0;
			foreach(SF_Node prop in editor.nodeView.treeStatus.propertyList){
				bool draggingThis = ( draggingProperty == prop );
				bool dragging = (draggingProperty != null);

				r.y = yStart + propertyHeight * i;

				if( draggingThis ) {
					r.x -= 5;
					r.y = Mathf.Clamp(Event.current.mousePosition.y + dragStartOffsetY, yStart, yStart+propertyHeight*(propCount-1));
				} else if( dragging ) {
					if( i < dragStartIndex ){
						float offset = propertyHeight + SF_Tools.Smoother( Mathf.Clamp( r.y - DragRectPosY, -propertyHeight, 0 ) / -propertyHeight ) * -propertyHeight;
						r.y += offset;
					} else if( i > dragStartIndex) {
						r.y -= propertyHeight - SF_Tools.Smoother( Mathf.Clamp( r.y - DragRectPosY, 0, propertyHeight ) / propertyHeight ) * propertyHeight;
					}
				}


				
				

				GUI.Box( r, string.Empty, draggingThis ? SF_Styles.HighlightStyle : SF_Styles.NodeStyle );
				bool mouseOver = r.Contains( Event.current.mousePosition );
				




				// We're now in the property box
				// We need: Grabber, Text field, Internal label



				bool imagePreview = (prop.property is SFP_Tex2d || prop.property is SFP_Cubemap);
				bool colorInput = ( prop.property is SFP_Color );


				// GRABBER
				Rect gRect = SF_Tools.GetExpanded( r, -6);
				gRect.width = gRect.height/2f;

				gRect.yMin += 8;

				Rect gRectCoords = new Rect( gRect );

				gRectCoords.x = 0;
				gRectCoords.y = 0;
				gRectCoords.width /= SF_GUI.Handle_drag.width;
				gRectCoords.height /= SF_GUI.Handle_drag.height;
				GUI.DrawTextureWithTexCoords( gRect, SF_GUI.Handle_drag, gRectCoords );
				gRect.yMin -= 8;
				/*
				if( propCount > 1 ) {
					if( gRect.Contains( Event.current.mousePosition ) && SF_GUI.PressedLMB() && !dragging ) {
						dragStartOffsetY = r.y - Event.current.mousePosition.y;
						draggingProperty = prop;
						dragStartIndex = i;
						startMouseY = Event.current.mousePosition.y;
					}	
					SF_GUI.AssignCursor( gRect,MouseCursor.Pan);
					GUI.DrawTextureWithTexCoords(gRect, SF_GUI.Handle_drag, gRectCoords );
				}
				*/
				

				

				// Property type name
				Color c = GUI.color;
				c.a = 0.5f;
				GUI.color = c;
				Rect propTypeNameRect = new Rect( gRect );
				//propTypeNameRect.x += propTypeNameRect.width + 8;
				propTypeNameRect.y -= 5;
				if( imagePreview || colorInput )
					propTypeNameRect.width = r.width - r.height - 38;
				else
					propTypeNameRect.width = r.width - 48;
				propTypeNameRect.height = 16;
				//if( prop.property != null )
				GUI.Label( propTypeNameRect, prop.property.nameType, EditorStyles.miniLabel );
				propTypeNameRect.x += gRect.width + 8;
				c.a = 1f;
				GUI.color = c;
				//else
					//return (int)r.yMax;


				// INTERNAL NAME
				
				if( mouseOver ) {
					c.a = 0.5f;
					GUI.color = c;
					Rect intRect = new Rect( propTypeNameRect );
					intRect.xMin += intRect.width - SF_GUI.WidthOf( prop.property.nameInternal, EditorStyles.label );
					//SF_GUI.AssignCursor( intRect, MouseCursor.Text );
					GUI.Label( intRect, prop.property.nameInternal, EditorStyles.label );
					c.a = 1f;
					GUI.color = c;
				}
				


				// DISPLAY NAME
				Rect dispNameRect = new Rect( propTypeNameRect );
				dispNameRect.y += 18;
				//dispNameRect.x += dispNameRect.width + 4;
				//dispNameRect.height = 16;
				//dispNameRect.y += 10;
				//dispNameRect.width = ( r.width - dispNameRect.width - texRect.width - 20 ) * 0.5f;

				StartIgnoreChangeCheck();
				string bef = prop.property.nameDisplay;
				SF_GUI.AssignCursor( dispNameRect, MouseCursor.Text );
				//if( mouseOver )
					SF_GUI.EnterableTextField( prop, dispNameRect, ref prop.property.nameDisplay, EditorStyles.textField );
				//else
					//GUI.Label( dispNameRect, prop.property.nameDisplay, EditorStyles.boldLabel );
				if( prop.property.nameDisplay != bef ) { // Changed
					prop.property.UpdateInternalName();
				}
				EndIgnoreChangeCheck();



				


				// Texture preview
				Rect texRect = new Rect( 0, 0, 0, 0 );
				c = GUI.color;
				if( imagePreview ) {
					texRect = SF_Tools.GetExpanded(new Rect( r ), -4);
					texRect.xMin += texRect.width - texRect.height;
					//texRect.x += gRect.width + 4;
					//texRect.width = texRect.height;
					GUI.Box( SF_Tools.GetExpanded( texRect, 1f ), string.Empty, SF_Styles.NodeStyle );
					GUI.color = Color.white;
					GUI.DrawTexture( texRect, prop.texture.Texture );
					GUI.color = c;
				}


				if( prop.property is SFP_Slider ) {

					SFN_Slider slider = ( prop as SFN_Slider );

					StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height+5;
					sR.width = 28;
					GUI.Label( sR, "Min" );
					sR.x += sR.width;
					SF_GUI.EnterableFloatField(prop, sR, ref slider.min, EditorStyles.textField );
					sR.x += sR.width;

					sR.width = r.width - 164;

					float beforeSlider = slider.current;

					string sliderName = "slider" + slider.id;
					GUI.SetNextControlName( sliderName );
					SF_GUI.AssignCursor( sR, MouseCursor.Arrow );
					sR.xMin += 4;
					sR.xMax -= 4;
					slider.current = GUI.HorizontalSlider( sR, slider.current, slider.min, slider.max );
					if( beforeSlider != slider.current ) {
						GUI.FocusControl( sliderName );
						slider.OnValueChanged();
					}
						
					sR.x += sR.width+4;
					sR.width = 32;
					SF_GUI.EnterableFloatField( prop, sR, ref slider.max, EditorStyles.textField );
					sR.x += sR.width;
					GUI.Label( sR, "Max" );

					EndIgnoreChangeCheck();

				} else if( colorInput ) {


					SFN_Color colNode = ( prop as SFN_Color );
					
					texRect = SF_Tools.GetExpanded( new Rect( r ), -4 );
					texRect.xMin += texRect.width - texRect.height;
					//GUI.Box( SF_Tools.GetExpanded( texRect, 1f ), string.Empty, SF_Styles.NodeStyle );
					GUI.color = Color.white;
					texRect.yMax -= 21;
					texRect.yMin += 15;
					texRect.xMin += 2;
					//texRect.xMax -= 2;

					SF_GUI.AssignCursor( texRect, MouseCursor.Arrow );

					StartIgnoreChangeCheck();
					Color col = EditorGUI.ColorField( texRect, colNode.texture.dataUniform );
					EndIgnoreChangeCheck();
					colNode.SetColor( col );
					GUI.color = c;
				} else if( prop.property is SFP_Vector4Property ) {

					SFN_Vector4Property vec4 = ( prop as SFN_Vector4Property );

					StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height + 5;
					sR.width = 20;

					int lbWidth = 12;
					sR.width = lbWidth;
					GUI.Label( sR, "X", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					SF_GUI.EnterableFloatField( prop, sR, ref vec4.texture.dataUniform.r, EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width + 3;


					sR.width = lbWidth;
					GUI.Label( sR, "Y", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					SF_GUI.EnterableFloatField( prop, sR, ref vec4.texture.dataUniform.g, EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width+3;


					sR.width = lbWidth;
					GUI.Label( sR, "Z", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					SF_GUI.EnterableFloatField( prop, sR, ref vec4.texture.dataUniform.b, EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width + 3;


					sR.width = lbWidth;
					GUI.Label( sR, "W", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					SF_GUI.EnterableFloatField( prop, sR, ref vec4.texture.dataUniform.a, EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					//sR.x += sR.width;
					
						

					EndIgnoreChangeCheck();

				} else if( prop.property is SFP_ValueProperty ) {

					SFN_ValueProperty val = ( prop as SFN_ValueProperty );

					StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height + 5;
					sR.width = 20;

					sR.width = 35;
					GUI.Label( sR, "Value", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 55;
					SF_GUI.EnterableFloatField( prop, sR, ref val.texture.dataUniform.r, EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					EndIgnoreChangeCheck();
				}












				if( r.Contains( Event.current.mousePosition ) && SF_GUI.PressedLMB() && !dragging ) {
					dragStartOffsetY = r.y - Event.current.mousePosition.y;
					draggingProperty = prop;
					dragStartIndex = i;
					startMouseY = Event.current.mousePosition.y;
					editor.Defocus();
				}
				SF_GUI.AssignCursor( r, MouseCursor.Pan );








				if( draggingThis )
					r.x += 5;
				r.y += propertyHeight;
				i++;
			}


			r.y = yStart + propCount * propertyHeight;
			r.height = 20;


			

			

		//	r.y += r.height;


			if( EndExpanderChangeCheck() )
				guiChanged = true;

			GUI.color = Color.white;
			return (int)r.yMax;
		}




		public int SettingsMeta( int yOffset ) {
			Rect topRect = new Rect( 0f, yOffset, maxWidth, 20 );
			Rect r = new Rect( topRect );

			if( !StartExpanderChangeCheck( r, ref foldMeta, " Shader Settings...", " Shader Settings" ) ) {
				return (int)r.yMax;
			}
			r.y += 20;
			r.xMin += 20; // Indent


			EditorGUI.LabelField( r, "Path", EditorStyles.miniLabel );
			r.xMin += 30;
			r.height = 17;
			r.xMax -= 3;
			StartIgnoreChangeCheck();
			GUI.SetNextControlName( "shdrpath" );
			string prev = editor.currentShaderPath;
			editor.currentShaderPath = GUI.TextField( r, editor.currentShaderPath,EditorStyles.textField );
			if( editor.currentShaderPath != prev ) {
				SF_Tools.FormatShaderPath( ref editor.currentShaderPath );
			}
			if( Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath" ) {
				editor.Defocus();
				editor.OnShaderModified( NodeUpdateType.Hard );
			}
			EndIgnoreChangeCheck();
			r.xMin -= 30;
			r.height = 20;
			r.xMax += 3;
			r.y += 20;




			EditorGUI.LabelField( r, "Fallback", EditorStyles.miniLabel );
			Rect rStart = new Rect( r );
			r.xMin += 50;
			r.height = 17;
			r.xMax -= 47;
			StartIgnoreChangeCheck();
			GUI.SetNextControlName( "shdrpath" );
			prev = fallback;
			fallback = GUI.TextField( r, fallback, EditorStyles.textField );
			r.x += r.width + 2;
			r.width = 42;
			ShaderPicker( r, "Pick");
			if( fallback != prev ) {
				SF_Tools.FormatShaderPath( ref fallback );
			}
			if( Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "shdrpath" ) {
				editor.Defocus();
				editor.OnShaderModified( NodeUpdateType.Hard );
			}
			EndIgnoreChangeCheck();
			r = rStart;
			r.y += r.height;






			EditorGUI.LabelField( r, "LOD", EditorStyles.miniLabel );
			r.xMin += 30;
			r.height = 17;
			r.xMax -= 3;
			LOD = EditorGUI.IntField( r, LOD );
			r.xMin -= 30;
			r.height = 20;
			r.xMax += 3;
			r.y += 20;




			EditorGUI.LabelField( r, "Target renderers:" );
			r.xMin += 20;
			r.y += 20;
			r.height = 17;
			float pWidth = r.width;


			bool onlyDX11 = mOut.tessellation.IsConnectedAndEnabled();
			bool onlyDX = editor.nodeView.treeStatus.mipInputUsed;
			

			for(int i=0;i<usedRenderers.Length;i++){
				bool isDX = ( i == 0 || i == 1 );
				bool isDX11 = (i == 1);
				
				r.width = 20;

				bool prevEnable = GUI.enabled;
				//bool displayBool = usedRenderers[i];

				bool shouldDisable = ( !isDX && onlyDX ) || ( !isDX11 && onlyDX11 );

				if( shouldDisable ) {
					GUI.enabled = false;
					EditorGUI.Toggle( r, false );
				} else {
					usedRenderers[i] = EditorGUI.Toggle( r, usedRenderers[i] );
				}

				
				r.width = pWidth;
				r.xMin += 20;
				EditorGUI.LabelField( r, SF_Tools.rendererLabels[i], EditorStyles.miniLabel );

				if( shouldDisable ) {
					GUI.enabled = prevEnable;
				}

				r.xMin -= 20;
				r.y += r.height+1;
			}
			//r.height = 20;
			

			// Path
			// Renderers






			EndIgnoreChangeCheck();

			//r.y += 20;
			if( EndExpanderChangeCheck() )
				guiChanged = true;
			return (int)r.yMax;

		}


		public int SettingsBlendingDepth(int yOffset) {

			Rect topRect = new Rect( 0f, yOffset, maxWidth, 20 );
			Rect r = new Rect(topRect);

			GUI.SetNextControlName( "defocus" );
			if( !StartExpanderChangeCheck( r, ref foldBlending, " Blending & Depth...", " Blending & Depth" ) ) {
				return (int)r.yMax;
			}

			r.y += 20;
			r.xMin += 20; // Indent

			BlendModePreset before = blendModePreset;
			blendModePreset = (BlendModePreset)SF_GUI.LabeledEnumField( r, "Blend Mode", blendModePreset, EditorStyles.miniLabel );
			if( blendModePreset != before ) {
				ConformBlendsToPreset();
			}

			r.y += 20;

			if( blendModePreset != BlendModePreset.Off ) {
				if( blendModePreset != BlendModePreset.Custom )
					GUI.enabled = false;
				//EditorGUILayout.BeginHorizontal( GUILayout.Width( maxWidth ) );
				//{
				//Indent();

				string srcStr = "Source * ";
				string dstStr = " + Destination * ";
				int srcStrWidth = SF_GUI.WidthOf(srcStr,EditorStyles.miniLabel);
				int dstStrWidth = SF_GUI.WidthOf(dstStr,EditorStyles.miniLabel);
				int fieldWidth = Mathf.FloorToInt((r.width - srcStrWidth - dstStrWidth)/2);

				Rect rSrcLb =		new Rect( r );			rSrcLb.width = srcStrWidth;
				Rect rSrcField =	new Rect(r);			rSrcField.x = rSrcLb.xMax;	rSrcField.width = fieldWidth;
				Rect rDstLb =		new Rect(r);			rDstLb.x = rSrcField.xMax;	rDstLb.width = dstStrWidth;
				Rect rDstField =	new Rect(rSrcField);	rDstField.x = rDstLb.xMax;

				GUI.Label( rSrcLb, srcStr, EditorStyles.miniLabel );
				blendSrc = (BlendMode)EditorGUI.EnumPopup(rSrcField, blendSrc );
				GUI.Label( rDstLb, dstStr, EditorStyles.miniLabel );
				blendDst = (BlendMode)EditorGUI.EnumPopup(rDstField, blendDst );

				if( blendModePreset != BlendModePreset.Custom )
					GUI.enabled = true;

				r.y += 20;
			}

			cullMode = (CullMode)SF_GUI.LabeledEnumField( r, "Face Culling", cullMode, EditorStyles.miniLabel );
			r.y += 20;


			FogBlock(ref r);

			SortingBlock(ref r);

			if( EndExpanderChangeCheck() )
				guiChanged = true;

			return (int)r.yMax;
		}


		


		private bool prevChangeState;
		public void StartIgnoreChangeCheck() {
			prevChangeState = EditorGUI.EndChangeCheck(); // Don't detect changes when toggling
		}

		public void EndIgnoreChangeCheck() {
			EditorGUI.BeginChangeCheck(); // Don't detect changes when toggling
			if( prevChangeState ) {
				GUI.changed = true;
			}
		}


		public void SortingBlock(ref Rect r) {

			

			StartIgnoreChangeCheck();
				bool prevAutoSort = autoSort;
				autoSort = GUI.Toggle(r, autoSort, autoSort ? "Auto Sort..." : "Auto Sort" );
				if( autoSort != prevAutoSort && autoSort )
					UpdateAutoSettings();
			EndIgnoreChangeCheck();

			r.xMin += 20;
			r.y += 20;

			bool prevGUI = GUI.enabled;
			GUI.enabled = !autoSort;
			{
				int wOrder = SF_GUI.WidthOf( "Order", EditorStyles.miniLabel) + 2;
				int wPlus = SF_GUI.WidthOf( "+", EditorStyles.miniLabel ) + 2;
				int wOffset = 32;
				int wEquals = SF_GUI.WidthOf( "=", EditorStyles.miniLabel );
				int wResult = 32;
				int wField = Mathf.FloorToInt(r.width - wOrder - wPlus - wEquals - wOffset - wResult);
				Rect tRect = new Rect( r );
				tRect.width = wOrder;
				GUI.Label(tRect, new GUIContent( "Order", "Determines in which order this shader is rendered relative to others" ), EditorStyles.miniLabel );
				SF_GUI.MoveRight( ref tRect, wField );
				queuePreset = (Queue)EditorGUI.Popup(tRect, (int)queuePreset, strQueue );
				SF_GUI.MoveRight( ref tRect, wPlus );
				GUI.Label(tRect, "+" );
				SF_GUI.MoveRight( ref tRect, wOffset );
				queueOffset = EditorGUI.IntField(tRect, queueOffset );
				SF_GUI.MoveRight( ref tRect, wEquals );
				GUI.Label( tRect, "=");
				SF_GUI.MoveRight( ref tRect, wResult );
				GUI.Label( tRect, ( queueNumbers[(int)queuePreset] + queueOffset ).ToString());
				r.y += 20;
				renderType = (RenderType)SF_GUI.LabeledEnumField( r,new GUIContent("Render Type","Defines shader replacement; required for some rendering effects, such as SSAO"), renderType, EditorStyles.miniLabel );
				r.y += 20;
				depthTest = (DepthTest)SF_GUI.LabeledEnumFieldNamed( r, strDepthTest, new GUIContent( "Depth Test", "Compared to the existing geometry in the scene, \nthis determines when to render this geometry. \u2264 is default, meaning:\n\"If this part is closer or as close to the camera as existing geometry, draw me!\"" ), (int)depthTest, EditorStyles.miniLabel );
				r.y += 20;	
				ignoreProjector = GUI.Toggle(r, ignoreProjector, "Ignore Projectors" );
				r.y += 20;	
				writeDepth = GUI.Toggle( r, writeDepth, "Write to Depth buffer" );
				r.y += 20;	
			}
			GUI.enabled = prevGUI;

			r.xMin -= 20;

		}


		public void FogBlock(ref Rect r) {

			useFog = GUI.Toggle( r, useFog, "Receive Fog" );
			r.y += 20;
			
			if(!useFog)
				return;

			r.xMin += 20; // Indent
			
			bool prevGUI = GUI.enabled;
			GUI.enabled = useFog;
			{
		
				CheckboxEnableLine(ref fogOverrideMode, ref r);
				fogMode = (ShaderFogMode)SF_GUI.LabeledEnumField(r,"Override Fog Mode",fogMode,EditorStyles.label);
				CheckboxEnableLineEnd(ref r);

				CheckboxEnableLine(ref fogOverrideColor, ref r);
				r.height = 17;
				fogColor = EditorGUI.ColorField(r,"Override Fog Color",fogColor);//SF_GUI.LabeledColorField(r,"Override Fog Color",fogDensity,EditorStyles.miniLabel);
				r.height = 20;
				CheckboxEnableLineEnd(ref r);


				CheckboxEnableLine(ref fogOverrideDensity, ref r);
				fogDensity = EditorGUI.FloatField(r,"Override Fog Density",fogDensity);//SF_GUI.LabeledFloatField(r,"Override Fog Density",fogDensity,EditorStyles.miniLabel);
				CheckboxEnableLineEnd(ref r);



				CheckboxEnableLine(ref fogOverrideRange, ref r);
				fogRange.x = EditorGUI.FloatField(r,"Override Fog Range Near",fogRange.x); //SF_GUI.LabeledVector2Field(r,"Override Fog Density",fogDensity,EditorStyles.miniLabel);
				r.y += 20;
				fogRange.y = EditorGUI.FloatField(r,"Override Fog Range Far",fogRange.y); //SF_GUI.LabeledVector2Field(r,"Override Fog Density",fogDensity,EditorStyles.miniLabel);
				CheckboxEnableLineEnd(ref r);


			}
			GUI.enabled = prevGUI;
			r.xMin -= 20;
		}

		public void CheckboxEnableLine(ref bool b, ref Rect r){
			Rect rCopy = r;
			rCopy.width = r.height;
			b = GUI.Toggle(rCopy,b,string.Empty);
			GUI.enabled = b;
			r.xMin += 20;
		}

		public void CheckboxEnableLineEnd(ref Rect r){
			r.y += 20;
			r.xMin -= 20;
			GUI.enabled = true;
		}

		public void SetQueuePreset(Queue in_queue) {
			queuePreset = in_queue;
		}


		
		MenuCommand mc;
		private void DisplayShaderContext(Rect r) {
			if( mc == null )
				mc = new MenuCommand( this, 0 );
			Material temp = new Material( "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}" ); // This dummy material will make it highlight none of the shaders inside.
			UnityEditorInternal.InternalEditorUtility.SetupShaderMenu( temp ); // Rebuild shader menu
			DestroyImmediate( temp.shader, true ); DestroyImmediate( temp, true ); // Destroy temporary shader and material
			EditorUtility.DisplayPopupMenu( r, "CONTEXT/ShaderPopup", mc ); // Display shader popup
		}
		private void OnSelectedShaderPopup( string command, Shader shader ) {
			if( shader != null ) {
				if( fallback != shader.name ) {
					fallback = shader.name;
					editor.Defocus();
					//editor.OnShaderModified( NodeUpdateType.Hard );
				}
			}
		}

		public void ShaderPicker(Rect r, string s){
			if( GUI.Button( r, s, EditorStyles.popup ) ) {
				DisplayShaderContext(r);
			}
		}


		public void UpdateAutoSettings() {
			if( blendModePreset == BlendModePreset.Off && editor.materialOutput.alpha.IsConnectedAndEnabled() ) {
				blendModePreset = BlendModePreset.AlphaBlended;
				ConformBlendsToPreset();
			}
			UpdateAutoSort();
		}


		public void UpdateAutoSort() {

			if( !autoSort )
				return;

			if( editor.materialOutput.alpha.IsConnectedAndEnabled() || editor.materialOutput.refraction.IsConnectedAndEnabled() || editor.nodeView.treeStatus.usesSceneData ) {
				SetQueuePreset(Queue.Transparent);
				renderType = RenderType.Transparent;
				ignoreProjector = true;
				writeDepth = false;
				return;
			}
			if( UseClipping() ){
				SetQueuePreset(Queue.AlphaTest);
				renderType = RenderType.TransparentCutout;
				ignoreProjector = false;
				writeDepth = true;
				return;
			}
			if( blendModePreset == BlendModePreset.Off ){
				SetQueuePreset(Queue.Geometry);
				renderType = RenderType.Opaque;
				ignoreProjector = false;
				writeDepth = true;
				return;
			} else {
				SetQueuePreset( Queue.Transparent );
				renderType = RenderType.Transparent;
				ignoreProjector = true;
				writeDepth = false;
				return;
			}
		}

		public int SettingsLighting(int yOffset) {

			Rect r = new Rect( 0f, yOffset, maxWidth, 20 );

			if( !StartExpanderChangeCheck(r, ref foldLighting, " Lighting...", " Lighting" ) )
				return (int)r.yMax;
			r.xMin += 20;
			r.y += 20;
			lightMode = (LightMode)ContentScaledToolbar( r, "Light Mode", (int)lightMode, strLightMode );
			r.y += 20;
			doubleIncomingLight = GUI.Toggle( r, doubleIncomingLight, "Double incoming light" );
			r.y += 20;
			remapGlossExponentially = GUI.Toggle( r, remapGlossExponentially, "Remap gloss from [0-1] to [1-2048]" );
			r.y += 20;

			if( lightMode == LightMode.PBL ) {
				fresnelTerm = GUI.Toggle( r, fresnelTerm, "[PBL] Fresnel term");
				r.y += 20;
				visibilityTerm = GUI.Toggle( r, visibilityTerm, "[PBL] Visibility term" );
				r.y += 20;
			}

			if( lightMode == LightMode.Unlit || lightMode == LightMode.PBL )
				GUI.enabled = false;
			{

				//bool b = energyConserving;
				if( lightMode == LightMode.PBL )
					GUI.Toggle( r, true, "Energy Conserving" ); // Dummy display of a checked energy conserve
				else
					energyConserving = GUI.Toggle( r, energyConserving, "Energy Conserving" );

				r.y += 20;
				GUI.enabled = true;
			}

			

			lightCount = (LightCount)ContentScaledToolbar(r, "Light Count", (int)lightCount, strLightCount );
			r.y += 20;

			if( lightMode == LightMode.Unlit )
				GUI.enabled = false;
			{
				//lightPrecision = (LightPrecision)ContentScaledToolbar(r, "Light Quality", (int)lightPrecision, strLightPrecision ); // TODO: Too unstable for release
				//r.y += 20;	
				normalQuality = (NormalQuality)ContentScaledToolbar(r, "Normal Quality", (int)normalQuality, strNormalQuality );
				r.y += 20;
				lightmapped = GUI.Toggle( r, lightmapped, "Lightmap support" );
				r.y += 20;
				GUI.enabled = true;
			}
			useAmbient = GUI.Toggle( r, useAmbient, "Receive Ambient Light" );
			r.y += 20;
			
			if( EndExpanderChangeCheck() )
				guiChanged = true;

			return (int)r.yMax;
		}

		public void Indent() {
			GUILayout.Space( expIndent );
		}

		public int GUISeparator(int yOffset) {
			GUI.Box( new Rect(0,yOffset,maxWidth,1), "", EditorStyles.textField );
			return yOffset + 1;
		}



		public void ConformBlendsToPreset() {
			switch( blendModePreset ) {
				case BlendModePreset.Additive:
					blendSrc = BlendMode.One;
					blendDst = BlendMode.One;
					break;
				case BlendModePreset.Screen:
					blendSrc = BlendMode.One;
					blendDst = BlendMode.OneMinusSrcColor;
					break;
				case BlendModePreset.AlphaBlended:
					blendSrc = BlendMode.SrcAlpha;
					blendDst = BlendMode.OneMinusSrcAlpha;
					break;
				case BlendModePreset.Multiplicative:
					blendSrc = BlendMode.DstColor;
					blendDst = BlendMode.Zero;
					break;
			}
		}

		public bool UseZWrite() {
			return writeDepth;
		}
		public string GetZWriteString() {
			if( !writeDepth )
				return "ZWrite Off";
			return "";
		}

		public bool UseDepthTest() {
			return ( depthTest != DepthTest.LEqual );
		}
		public string GetDepthTestString() {
			return "ZTest " + depthTest.ToString();
		}

		public bool UseCulling() {
			return ( cullMode != CullMode.BackfaceCulling );
		}
		public string GetCullString() {
			return "Cull " + strCullMode[(int)cullMode];
		}
		public bool IsDoubleSided() {
			return ( cullMode == CullMode.DoubleSided );
		}

		public bool UseClipping() {
			return mOut.alphaClip.IsConnectedAndEnabled();
		}

		public bool UseMultipleLights() {
			return lightCount == LightCount.Multi;
		}

		public bool UseBlending() {
			if( blendModePreset == BlendModePreset.Off )
				return false;
			return true;
		}
		public string GetBlendString() {
			return "Blend " + blendSrc.ToString() + " " + blendDst.ToString();
		}

		public bool IsOutlined(){
			return mOut.outlineWidth.IsConnectedAndEnabled();
		}

		// TODO: Double sided support
		public string GetNormalSign() {
			if( cullMode == CullMode.BackfaceCulling )
				return "";
			if( cullMode == CullMode.FrontfaceCulling )
				return "-";
			//if( cullMode == CullMode.DoubleSided )
			return "";
		}

		public int ContentScaledToolbar(Rect r, string label, int selected, string[] labels ) {

			r.height = 15;

			Rect rLeft = new Rect( r );
			Rect rRight = new Rect( r );

			rLeft.width = SF_GUI.WidthOf( label, EditorStyles.miniLabel )+4;
			rRight.width = r.width - rLeft.width;
			rRight.x += rLeft.width;

			GUI.Label( rLeft, label, EditorStyles.miniLabel);


			// Full pixel width of strings:
			float[] lblPxWidth = new float[labels.Length];
			float pxWidthTotal = 0;
			for( int i = 0; i < labels.Length; i++ ) {
				lblPxWidth[i] = SF_GUI.WidthOf( labels[i], EditorStyles.miniButtonMid );
				pxWidthTotal += lblPxWidth[i];
			}

			// Scale all buttons to fit the rect
			float scale = rRight.width / pxWidthTotal;
			for( int i = 0; i < labels.Length; i++ ) {
				lblPxWidth[i] *= scale;
			}




			GUIStyle style = EditorStyles.miniButtonLeft;
			int retval = selected;

			Rect rTemp = new Rect(rRight);

			for( int i = 0; i < labels.Length; i++ ) {

				rTemp.width = lblPxWidth[i];

				if( i == labels.Length - 1 ) {
					style = EditorStyles.miniButtonRight;
				} else if( i > 0 ) {
					style = EditorStyles.miniButtonMid;
				}

				if( GUI.Button(rTemp, labels[i], style ) )
					retval = i;

				if( selected == i && GUI.enabled ) {
					if( EditorGUIUtility.isProSkin ) {
						GUI.Box( rTemp, "" );
						GUI.Box( rTemp, "" );
						GUI.Box( rTemp, "" );
					} else {
						GUI.color = new Color( 0f, 0f, 0f, 0.3f );
						GUI.Box( rTemp, "" );
						GUI.color = Color.white;
					}
					
				}

				rTemp.x += rTemp.width;
			}
			GUI.color = Color.white;
			return retval;

		}

		public bool IsVertexLit() {
			return ( IsLit() && ( lightPrecision == LightPrecision.Vertex ) );
		}

		public bool IsFragmentLit() {
			return ( IsLit() && ( lightPrecision == LightPrecision.Fragment ) );
		}


		public bool IsLit() {
			return ( lightMode != LightMode.Unlit );
		}

		public bool IsEnergyConserving() {
			return IsLit() && (energyConserving || lightMode == SF_PassSettings.LightMode.PBL);
		}

		public bool IsPBL() {
			return lightMode == SF_PassSettings.LightMode.PBL;
		}

		public bool HasSpecular() {
			return ( lightMode == LightMode.BlinnPhong || lightMode == LightMode.Phong || lightMode == LightMode.PBL ) && ( mOut.specular.IsConnectedAndEnabled() );
		}

		public bool HasNormalMap() {
			return mOut.normal.IsConnectedAndEnabled();
		}

		public bool HasRefraction() {
			return mOut.refraction.IsConnectedAndEnabled();
		}

		public bool HasTessellation() {
			return mOut.tessellation.IsConnectedAndEnabled();
		}

		public bool HasDisplacement() {
			return mOut.displacement.IsConnectedAndEnabled();
		}

		public bool HasEmissive() {
			return mOut.emissive.IsConnectedAndEnabled();
		}

		public bool HasTransmission() {
			return mOut.transmission.IsConnectedAndEnabled();
		}

		//public bool HasAnisotropicLight() {
		//	return mOut.anisotropicDirection.IsConnectedAndEnabled();
		//}

		public bool HasAddedLight() {
			return HasEmissive() || HasSpecular() ;
		}

		public bool HasLightWrapping() {
			return mOut.lightWrap.IsConnectedAndEnabled();
		}

		public string GetShadowPragmaIfUsed(){
			if( !ignoreProjector )
				return "_fullshadows";
			return "";
		}



	}
}