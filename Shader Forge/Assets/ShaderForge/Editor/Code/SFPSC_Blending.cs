using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {


	public enum DepthTestStencil { Less, Greater, LEqual, GEqual, Equal, NotEqual, Always, Never };
	public enum StencilOp { Keep, Zero, Replace, Invert, IncrSat, DecrSat, IncrWrap, DecrWrap };
	public enum DepthTest { Less, Greater, LEqual, GEqual, Equal, NotEqual, Always };
	public enum RenderType { None, Opaque, Transparent, TransparentCutout, Background, Overlay, TreeOpaque, TreeTransparentCutout, TreeBillboard, Grass, GrassBillboard };
	public enum BlendModePreset {
		Opaque,
		AlphaBlended,
		AlphaBlendedPremultiplied,
		Additive,
		Screen,
		Multiplicative,
		Custom
	};
	public enum ShaderFogMode{ Global, Linear, Exp, Exp2 };
	public enum BlendMode { One, Zero, SrcColor, SrcAlpha, DstColor, DstAlpha, OneMinusSrcColor, OneMinusSrcAlpha, OneMinusDstColor, OneMinusDstAlpha };
	public enum Queue { Background, Geometry, AlphaTest, Transparent, Overlay };

	public enum Dithering { Off, Dither2x2, Dither3x3, Dither4x4 };


	[System.Serializable]
	public class SFPSC_Blending : SFPS_Category {

		public static string[] strDepthTestStencil = new string[] { "<", ">", "\u2264", "\u2265", "=", "\u2260", "Always (Default)", "Never" };
		public static string[] strStencilOp = new string[] { "Keep (Default)", "Zero", "Replace", "Invert", "Increase (Clamped)", "Decrease (Clamped)", "Increase (Wrapped)", "Decrease (Wrapped)" };
		public static string[] strDepthTest = new string[] { "<", ">", "\u2264 (Default)", "\u2265", "=", "\u2260", "Always" };
		public static int[] queueNumbers = new int[] { 1000, 2000, 2450, 3000, 4000 };
		public static string[] strQueue = new string[] { "Background (1000)", "Opaque Geometry (2000)", "Alpha Clip (2450)", "Transparent (3000)", "Overlay (4000)" };
		public static string[] strDithering = new string[] { "Off", "2x2 matrix", "3x3 matrix", "4x4 matrix" };

		public static string[] strBlendModePreset = new string[] {
			"Opaque",
			"Alpha Blended",
			"Alpha Blended (Premultiplied)",
			"Additive",
			"Screen",
			"Multiplicative",
			""
		};

		
		// Vars
		
		public BlendModePreset blendModePreset = BlendModePreset.Opaque;
		public BlendMode blendSrc = BlendMode.One;
		public BlendMode blendDst = BlendMode.Zero;
		public DepthTest depthTest = DepthTest.LEqual;


		public byte stencilValue = 128;
		public byte stencilMaskRead = 255;
		public byte stencilMaskWrite = 255;
		public DepthTestStencil stencilComparison = DepthTestStencil.Always;
		public StencilOp stencilPass = StencilOp.Keep;
		public StencilOp stencilFail = StencilOp.Keep;
		public StencilOp stencilFailZ = StencilOp.Keep;
		
		public int offsetFactor = 0;
		public int offsetUnits = 0;

		// colorMask is a bitmask
		// 0 =	____
		// 1 =	___A
		// 2 =	__B_
		// 3 =	__BA
		// 4 =	_G__
		// 5 =	_G_A
		// 6 =	_GB_
		// 7 =	_GBA
		// 8 =	R___
		// 9 =	R__A
		// 10 = R_B_
		// 11 = R_BA
		// 12 = RG__
		// 13 = RG_A
		// 14 = RGB_
		// 15 = RGBA
		public int colorMask = 15;

		public Dithering dithering = Dithering.Off;
		public bool alphaToCoverage = false;
		
		public bool writeDepth = true;
		
		public bool useFog = true;

		public bool perObjectRefraction = true;
		public string refractionPassName = "Refraction";


		public bool autoSort = true;
		public Queue queuePreset = (Queue)1;
		public int queueOffset = 0;
		public RenderType renderType = RenderType.Opaque;
		public bool ignoreProjector = false;


		// Fog
		public bool fogOverrideMode = false;
		public ShaderFogMode fogMode = ShaderFogMode.Global;
		
		public bool fogOverrideColor = false;
		public Color fogColor;
		
		public bool fogOverrideDensity = false;
		public float fogDensity;
		
		public bool fogOverrideRange = false;
		public Vector2 fogRange;


		public bool useStencilBuffer = false;
		public bool allowStencilWriteThroughProperties = false;


		new void OnEnable() {
			fogColor = RenderSettings.fogColor;
			fogDensity = RenderSettings.fogDensity;
			fogRange = new Vector2( RenderSettings.fogStartDistance, RenderSettings.fogEndDistance );
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public override string Serialize(){
			string s = "";
			
			//s += Serialize( "blpr", ( (int)blendModePreset ).ToString() );
			s += Serialize( "bsrc", ( (int)blendSrc ).ToString() );
			s += Serialize( "bdst", ( (int)blendDst ).ToString() );
			s += Serialize( "dpts", ( (int)depthTest ).ToString() );
			s += Serialize( "wrdp", writeDepth.ToString() );

			s += Serialize( "dith", ( (int)dithering ).ToString() );
			s += Serialize( "atcv", alphaToCoverage.ToString() );	// bool

			s += Serialize( "rfrpo", perObjectRefraction.ToString() );
			s += Serialize( "rfrpn", refractionPassName );

			s += Serialize( "coma", colorMask.ToString() );
			
			s += Serialize( "ufog", useFog.ToString() );
			s += Serialize( "aust", autoSort.ToString() );
			s += Serialize( "igpj", ignoreProjector.ToString() );
			s += Serialize( "qofs", queueOffset.ToString() );
			
			s += Serialize( "qpre", ((int)queuePreset).ToString() );
			s += Serialize( "rntp", ( (int)renderType ).ToString() );
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
			s += Serialize( "fgrf", fogRange.y.ToString());			// Fog range Y (Far)
			
			
			// Stencil buffer:
			s += Serialize ( "stcl", useStencilBuffer.ToString());
			s += Serialize ( "atwp", allowStencilWriteThroughProperties.ToString());
			s += Serialize ( "stva", stencilValue.ToString());
			s += Serialize ( "stmr", stencilMaskRead.ToString());
			s += Serialize ( "stmw", stencilMaskWrite.ToString());
			s += Serialize ( "stcp", ((int)stencilComparison).ToString());
			s += Serialize ( "stps", ((int)stencilPass).ToString());
			s += Serialize ( "stfa", ((int)stencilFail).ToString());
			s += Serialize ( "stfz", ((int)stencilFailZ).ToString());
			
			
			// Offset
			s += Serialize ( "ofsf", offsetFactor.ToString());
			s += Serialize ( "ofsu", offsetUnits.ToString());

			return s;
		}

		bool lockSrcDstRead = false;

		public override void Deserialize(string key, string value){



			switch( key ) {

			case "blpr": // This is no longer saved, but in old shaders, we have to read it with old enum indices

		//	0 "Opaque",
		//	1 "Alpha Blended",
		//	- "Alpha Blended (Premultiplied)",
		//	2 "Additive",
		//	3 "Screen",
		//	4 "Multiplicative",

				int iVal = int.Parse( value );
				if( iVal > 1 ) // Offset due to adding premul
					iVal++;
				blendModePreset = (BlendModePreset)iVal;
				ConformBlendsToPreset();
				
				lockSrcDstRead = true;
				break;
			case "bsrc":
				if( lockSrcDstRead )
					break;
				blendSrc = (BlendMode)int.Parse( value );
				break;
			case "bdst":
				if( lockSrcDstRead ) {
					lockSrcDstRead = false;
					break;
				}	
				blendDst = (BlendMode)int.Parse( value );
				ConformPresetToBlend(); 
				break;
			case "dpts":
				depthTest = (DepthTest)int.Parse( value );
				break;
			case "wrdp":
				writeDepth = bool.Parse( value );
				break;
			case "dith":
				dithering = (Dithering)int.Parse( value );
				break;
			case "atcv":
				alphaToCoverage = bool.Parse( value );
				break;
			case "rfrpo":
				perObjectRefraction = bool.Parse( value );
				break;
			case "rfrpn":
				refractionPassName = value;
				break;
			case "coma":
				colorMask = int.Parse( value );
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
				
			case "qpre":
				queuePreset = (Queue)int.Parse( value );
				break;
				
			case "rntp":
				renderType = (RenderType)int.Parse( value );
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
				// Stencil buffer:
			case "stcl":
				useStencilBuffer = bool.Parse(value);
				break;
			case "atwp":
				allowStencilWriteThroughProperties = bool.Parse( value );
				break;
			case "stva":
				stencilValue = byte.Parse(value);
				break;
			case "stmr":
				stencilMaskRead = byte.Parse(value);
				break;
			case "stmw":
				stencilMaskWrite = byte.Parse(value);
				break;
			case "stcp":
				stencilComparison = (DepthTestStencil)int.Parse(value);
				break;
			case "stps":
				stencilPass = (StencilOp)int.Parse(value);
				break;
			case "stfa":
				stencilFail = (StencilOp)int.Parse(value);
				break;
			case "stfz":
				stencilFailZ = (StencilOp)int.Parse(value);
				break;
				
				// Offset
			case "ofsf":
				offsetFactor = int.Parse(value);
				break;
			case "ofsu":
				offsetUnits = int.Parse(value);
				break;
			}

		}


		public string GetGrabTextureName() {
			if( perObjectRefraction ) {
				return "_GrabTexture";
			} else {
				return refractionPassName;
			}
		}

	

		public override float DrawInner(ref Rect r){


			float prevYpos = r.y;
			r.y = 0;
			
			r.y += 20;
			r.xMin += 20; // Indent
			
			BlendModePreset before = blendModePreset;
			GUI.enabled = ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Forward;
			blendModePreset = (BlendModePreset)UndoableLabeledEnumPopupNamed( r, "Blend Mode", blendModePreset, strBlendModePreset, "blend mode");
			GUI.enabled = true;
			if( blendModePreset != before ) {
				ConformBlendsToPreset();
			}
			if( blendModePreset == BlendModePreset.Custom ) {
				GUI.color = new Color(1f,1f,1f,0.5f);
				GUI.Label(r.PadLeft(70).PadTop(-1), "Custom blending. Click select preset", EditorStyles.miniLabel);
				GUI.color = Color.white;
			}
			
			r.y += 20;
			
			//if( blendModePreset != BlendModePreset.Opaque ) {
				//if( blendModePreset != BlendModePreset.Custom )
					//GUI.enabled = false;
				//EditorGUILayout.BeginHorizontal( GUILayout.Width( maxWidth ) );
				//{
				//Indent();
				
			string srcStr = "Source * ";
			string dstStr = " + Destination * ";
			int srcStrWidth = SF_GUI.WidthOf(srcStr,EditorStyles.miniLabel);
			int dstStrWidth = SF_GUI.WidthOf(dstStr,EditorStyles.miniLabel);
			int fieldWidth = Mathf.FloorToInt((r.width - srcStrWidth - dstStrWidth)/2);
				
			Rect rSrcLb =		new Rect(r);			rSrcLb.width = srcStrWidth;
			Rect rSrcField =	new Rect(r);			rSrcField.x = rSrcLb.xMax;	rSrcField.width = fieldWidth;
			Rect rDstLb =		new Rect(r);			rDstLb.x = rSrcField.xMax;	rDstLb.width = dstStrWidth;
			Rect rDstField =	new Rect(rSrcField);	rDstField.x = rDstLb.xMax;

			EditorGUI.BeginChangeCheck();

			GUI.Label( rSrcLb, srcStr, EditorStyles.miniLabel );
			blendSrc = (BlendMode)UndoableEnumPopup(rSrcField, blendSrc, "blend source" );
			GUI.Label( rDstLb, dstStr, EditorStyles.miniLabel );
			blendDst = (BlendMode)UndoableEnumPopup(rDstField, blendDst, "blend destination" );

			if( EditorGUI.EndChangeCheck() ) {
				ConformPresetToBlend();
			}
				
			//if( blendModePreset != BlendModePreset.Custom )
				//GUI.enabled = true;
				
			r.y += 20;
			//}


			UndoableColorMask( r, "Color Mask", ref colorMask );
			r.y += 20;
			
			bool canEditDithering = editor.mainNode.alphaClip.IsConnectedAndEnabled();
			EditorGUI.BeginDisabledGroup( !canEditDithering );
			if( canEditDithering )
				dithering = (Dithering)UndoableLabeledEnumPopupNamed( r, "Dithered alpha clip", dithering, strDithering, "dithered alpha clip" );
			else
				UndoableLabeledEnumPopup( r, "Dithered alpha clip", Dithering.Off, "dithered alpha clip" );
			EditorGUI.EndDisabledGroup();
			r.y += 20;

			bool canEditAlphaToCoverage = editor.mainNode.alphaClip.IsConnectedAndEnabled() || editor.mainNode.alpha.IsConnectedAndEnabled();
			EditorGUI.BeginDisabledGroup( !canEditAlphaToCoverage );
			if( canEditAlphaToCoverage )
				alphaToCoverage = UndoableToggle( r, alphaToCoverage, "Alpha to coverage (forward with MSAA only)", "alpha to coverage" );
			else
				GUI.Toggle( r, false, "Alpha to coverage (forward with MSAA only)" );
			EditorGUI.EndDisabledGroup();
			r.y += 20;
			
			
			OffsetBlock (ref r);


			RefractionBlock( ref r );

			
			FogBlock(ref r);
			
			SortingBlock(ref r);
			
			
			
			StencilBlock(ref r);

			r.y += prevYpos;
			
			return (int)r.yMax;
		}

		static string[] rgba = new string[]{"R","G","B","A"};

		public void UndoableColorMask(Rect r, string label, ref int mask) {

			GUIStyle[] rgbaStyles = new GUIStyle[] {
				EditorStyles.miniButtonLeft,
				EditorStyles.miniButtonMid,
				EditorStyles.miniButtonMid,
				EditorStyles.miniButtonRight
			};

			Rect[] rects = r.SplitFromLeft( 65 );

			GUI.Label( rects[0], label, EditorStyles.miniLabel );


			Rect buttonRect = rects[1];
			buttonRect.width = 23;// Mathf.FloorToInt( buttonRect.width / 4 );
			buttonRect.height = 17;

			for( int i = 0; i < 4; i++ ) {
				//GUI.color = rgbaCols[i];
				bool bitVal = mask.GetBit( 3 - i );
				bool newBit = GUI.Toggle( buttonRect, bitVal, rgba[i], rgbaStyles[i] );
				if( newBit != bitVal ) {
					Undo.RecordObject( this, "edit Color Mask" );
					mask = mask.SetBit( 3 - i, newBit );
				}
				buttonRect = buttonRect.MovedRight();
			}

			buttonRect.width *= 4;
			buttonRect.x += 6;
			GUI.color = Color.gray;
			GUI.Label( buttonRect, mask.ToColorMaskString(), EditorStyles.miniLabel );
			GUI.color = Color.white;
			
			


		}




		public void SetQueuePreset( Queue in_queue ) {
			queuePreset = in_queue;
		}

		public void ConformPresetToBlend() {


			bool matched = false;
			matched |= ApplyIfMatch(BlendModePreset.Opaque,						BlendMode.One,				BlendMode.Zero );
			matched |= ApplyIfMatch(BlendModePreset.Additive,					BlendMode.One,				BlendMode.One );
			matched |= ApplyIfMatch(BlendModePreset.Screen,						BlendMode.OneMinusDstColor, BlendMode.One );
			matched |= ApplyIfMatch(BlendModePreset.Screen,						BlendMode.One,				BlendMode.OneMinusSrcColor );
			matched |= ApplyIfMatch(BlendModePreset.AlphaBlended,				BlendMode.SrcAlpha,			BlendMode.OneMinusSrcAlpha );
			matched |= ApplyIfMatch(BlendModePreset.AlphaBlendedPremultiplied,	BlendMode.One,				BlendMode.OneMinusSrcAlpha );
			matched |= ApplyIfMatch(BlendModePreset.Multiplicative,				BlendMode.DstColor,			BlendMode.Zero );
			matched |= ApplyIfMatch(BlendModePreset.Multiplicative,				BlendMode.Zero,				BlendMode.SrcColor );
			
			if(!matched){
				blendModePreset = BlendModePreset.Custom;
			}

		}


		public bool ApplyIfMatch(BlendModePreset preset, BlendMode src, BlendMode dst) {
			if( blendSrc == src && blendDst == dst ) {
				blendModePreset = preset;
				UpdateAutoSettings();
				return true;
			}
			return false;
				
		}

		public void ConformBlendsToPreset() {
			switch( blendModePreset ) {
			case BlendModePreset.Opaque:
				blendSrc = BlendMode.One;
				blendDst = BlendMode.Zero;
				break;
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
			case BlendModePreset.AlphaBlendedPremultiplied:
				blendSrc = BlendMode.One;
				blendDst = BlendMode.OneMinusSrcAlpha;
				break;
			case BlendModePreset.Multiplicative:
				blendSrc = BlendMode.DstColor;
				blendDst = BlendMode.Zero;
				break;
			}
			editor.preview.InternalMaterial.renderQueue = -1;
		}
		
		
		public string GetStencilContent(){

			string s = "";
			if( allowStencilWriteThroughProperties ) {
				s += "Ref [_Stencil]\n";
				s += "ReadMask [_StencilReadMask]\n";
				s += "WriteMask [_StencilWriteMask]\n";
				s += "Comp [_StencilComp]\n";
				s += "Pass [_StencilOp]\n";
				s += "Fail [_StencilOpFail]\n";
				s += "ZFail [_StencilOpZFail]";
			} else {
				s += "Ref " + stencilValue + "\n";
				if( stencilMaskRead != (byte)255 )
					s += "ReadMask " + stencilMaskRead + "\n";
				if( stencilMaskWrite != (byte)255 )
					s += "WriteMask " + stencilMaskWrite + "\n";
				if( stencilComparison != DepthTestStencil.Always )
					s += "Comp " + stencilComparison + "\n";
				if( stencilPass != StencilOp.Keep )
					s += "Pass " + stencilPass + "\n";
				if( stencilFail != StencilOp.Keep )
					s += "Fail " + stencilFail + "\n";
				if( stencilFailZ != StencilOp.Keep )
					s += "ZFail " + stencilFailZ + "\n";
				s = s.Substring( 0, s.Length - 1 );
			}
			return s;
		}

		public void RefractionBlock( ref Rect r ) {

			

			perObjectRefraction = UndoableToggle( r, perObjectRefraction, "Per-object refraction/scene color (expensive)", "per-object refraction", null );
			r.y += 20;

			ps.StartIgnoreChangeCheck();
			r.xMin += 20;
			Rect right = r;
			right.xMin += 126;
			right.width -= 18;
			Rect left = r;
			left.width -= right.width;
			GUI.enabled = !perObjectRefraction;
			GUI.Label(left, "Texture name/group");
			EditorGUI.BeginChangeCheck();
			refractionPassName = UndoableTextField( right, refractionPassName, "refraction pass name", null, null, true );
			if( EditorGUI.EndChangeCheck() ) {
				editor.ShaderOutdated = UpToDateState.OutdatedSoft;
				SF_Tools.FormatAlphanumeric( ref refractionPassName );
			}
			GUI.enabled = true;
			r.y += 20;
			r.xMin -= 20;



			ps.EndIgnoreChangeCheck();
		}


		public void OffsetBlock(ref Rect r){
			ps.StartIgnoreChangeCheck();
			Rect rOfs = r;
			rOfs.xMax -= 4; // Margin
			rOfs.width = 80;
			GUI.Label(rOfs, "Offset Factor");
			rOfs = rOfs.MovedRight();
			rOfs.width /= 2;
			offsetFactor = UndoableIntField(rOfs,offsetFactor,"offset factor");
			rOfs = rOfs.MovedRight();
			rOfs.width *= 2;
			GUI.Label(rOfs.PadLeft(4), "Offset Units");
			rOfs = rOfs.MovedRight();
			rOfs.width /= 2;
			offsetUnits = UndoableIntField(rOfs,offsetUnits,"offset units");
			r.y += 20;
			ps.EndIgnoreChangeCheck();
		}

		
		public void StencilBlock(ref Rect r){
			
			bool prevUseStencilBuffer = useStencilBuffer;
			useStencilBuffer = GUI.Toggle(r, useStencilBuffer, useStencilBuffer ? "Stencil Buffer" : "Stencil Buffer...");
			if( useStencilBuffer != prevUseStencilBuffer )
				UpdateAutoSettings();
			r.y += 20;
			if(!useStencilBuffer)
				return;
			r.xMin += 20;

			allowStencilWriteThroughProperties = UndoableToggle( r, allowStencilWriteThroughProperties, "Expose stencil as properties", "toggle expose stencil as properties" );
			r.y += 20;

			EditorGUI.BeginDisabledGroup( allowStencilWriteThroughProperties );

			Rect rTmp = r;
			rTmp.width = 88;
			GUI.Label(rTmp,"Reference Value", EditorStyles.miniLabel);
			rTmp = rTmp.MovedRight();
			rTmp.width -= 48;
			stencilValue = (byte)UndoableIntField( rTmp.PadRight( 4 ).PadTop( 1 ).PadBottom( 2 ), stencilValue, "reference value" );
			rTmp = rTmp.MovedRight();
			rTmp.width = r.width-128;
			stencilComparison = (DepthTestStencil)UndoableLabeledEnumPopupNamed( rTmp.PadRight( 4 ).ClampWidth( 32, 140 ), "Comparison", stencilComparison, strDepthTestStencil, "stencil comparison" );
			r.y += 20;
			
			StencilBitfield(r, "Read Mask", ref stencilMaskRead);
			r.y += 20;
			StencilBitfield(r, "Write Mask", ref stencilMaskWrite);
			r.y += 23;
			stencilPass = (StencilOp)UndoableLabeledEnumPopupNamed( r.PadRight( 4 ), "Pass", stencilPass, strStencilOp, "stencil pass" );
			r.y += 20;
			stencilFail = (StencilOp)UndoableLabeledEnumPopupNamed( r.PadRight( 4 ), "Fail", stencilFail, strStencilOp, "stencil fail" );
			r.y += 20;
			stencilFailZ = (StencilOp)UndoableLabeledEnumPopupNamed( r.PadRight( 4 ), "Fail Z", stencilFailZ, strStencilOp, "stencil fail Z" );
			r.y += 20;
			r.xMin -= 20;
			r.y += 20;

			EditorGUI.EndDisabledGroup();
		}
		
		
		
		public void StencilBitfield( Rect r, string label, ref byte b ){
			ps.StartIgnoreChangeCheck();
			Rect tmp = r;
			tmp.width = 62;
			
			GUI.Label(tmp,label, SF_Styles.MiniLabelRight);
			
			tmp = tmp.MovedRight();
			tmp.width = 36;
			b = (byte)EditorGUI.IntField(tmp.PadTop(1).PadBottom(2).PadRight(2).PadLeft(2),b);
			
			Rect bitField = r;
			bitField.xMin += 57+36 + 4 + 8;
			bitField.xMax -= 4;
			
			
			bitField.width /= 8; // 8 bits
			for (int i = 8 - 1; i >= 0; i--) {
				
				GUIStyle btnStyle;
				
				if(i==0)
					btnStyle = EditorStyles.miniButtonRight;
				else if(i==7)
					btnStyle = EditorStyles.miniButtonLeft;
				else
					btnStyle = EditorStyles.miniButtonMid;
				
				bool bit = ( (1<<i) & b ) != 0;
				int iBit = bit ? 1 : 0;
				
				GUI.color = bit ? Color.white : Color.gray;
				
				if( GUI.Button( bitField.PadTop(2).PadBottom(2), iBit.ToString(), btnStyle ) ){
					Undo.RecordObject( this, label + " bit flip" );
					editor.Defocus();
					b = (byte)( (1<<i) ^ b );
				}
				bitField = bitField.MovedRight();
			}
			GUI.color = Color.white;
			
			ps.EndIgnoreChangeCheck();
			
		}
		
		
		public void SortingBlock(ref Rect r) {
			
			
			
			ps.StartIgnoreChangeCheck();
			bool prevAutoSort = autoSort;
			GUI.enabled = ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Forward;
			autoSort = UndoableToggle(r, autoSort, autoSort ? "Auto Sort..." : "Auto Sort", "auto sort", null );
			GUI.enabled = true;
			if( autoSort != prevAutoSort && autoSort )
				UpdateAutoSettings();
			ps.EndIgnoreChangeCheck();
			
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
				//queuePreset = (Queue)EditorGUI.Popup(tRect, (int)queuePreset, strQueue );
				queuePreset = (Queue)UndoableEnumPopupNamed(tRect, queuePreset, strQueue, "render queue order");
				SF_GUI.MoveRight( ref tRect, wPlus );
				GUI.Label(tRect, "+" );
				SF_GUI.MoveRight( ref tRect, wOffset );
				queueOffset = UndoableIntField(tRect, queueOffset, "render queue offset");
				SF_GUI.MoveRight( ref tRect, wEquals );
				GUI.Label( tRect, "=");
				SF_GUI.MoveRight( ref tRect, wResult );
				GUI.Label( tRect, ( queueNumbers[(int)queuePreset] + queueOffset ).ToString());
				r.y += 20;
				//renderType = (RenderType)SF_GUI.LabeledEnumField( r,new GUIContent("Render Type","Defines shader replacement; required for some rendering effects, such as SSAO"), renderType, EditorStyles.miniLabel );
				renderType = (RenderType)UndoableLabeledEnumPopup( r, "Render Type", renderType, "render type" );
				r.y += 20;
				//depthTest = (DepthTest)SF_GUI.LabeledEnumFieldNamed( r, strDepthTest, new GUIContent( "Depth Test", "Compared to the existing geometry in the scene, \nthis determines when to render this geometry. \u2264 is default, meaning:\n\"If this part is closer or as close to the camera as existing geometry, draw me!\"" ), (int)depthTest, EditorStyles.miniLabel );
				depthTest = (DepthTest)UndoableLabeledEnumPopupNamed( r, "Depth Test", depthTest, strDepthTest, "depth test" );
				r.y += 20;
				//ignoreProjector = GUI.Toggle(r, ignoreProjector, "Ignore Projectors" );
				ignoreProjector = UndoableToggle(r, ignoreProjector, "Ignore Projectors", "ignore projectors", null );
				r.y += 20;	
				//writeDepth = GUI.Toggle( r, writeDepth, "Write to Depth buffer" );
				writeDepth = UndoableToggle(r, writeDepth, "Write to Depth buffer", "depth buffer write", null );
				r.y += 20;
			}
			GUI.enabled = prevGUI;
			
			r.xMin -= 20;
			
		}
		
		
		public void FogBlock(ref Rect r) {

			useFog = UndoableToggle(r, useFog, "Receive Fog", "receive fog", null );
			r.y += 20;
			
			if(!useFog)
				return;
			
			r.xMin += 20; // Indent
			
			bool prevGUI = GUI.enabled;
			GUI.enabled = useFog;
			{

				
				CheckboxEnableLine(ref fogOverrideColor, ref r);
				r.height = 17;
				fogColor = EditorGUI.ColorField(r,"Override Fog Color",fogColor);//SF_GUI.LabeledColorField(r,"Override Fog Color",fogDensity,EditorStyles.miniLabel);
				r.height = 20;
				CheckboxEnableLineEnd(ref r);

				
			}
			GUI.enabled = prevGUI;
			r.xMin -= 20;
		}
	


		public void UpdateAutoSettings() {
			//if( blendModePreset == BlendModePreset.Off && editor.mainNode.alpha.IsConnectedAndEnabled() ) {
			//	blendModePreset = BlendModePreset.AlphaBlended;
			//	ConformBlendsToPreset();
			//} // This will now throw a warning
			UpdateAutoSort();
			editor.preview.InternalMaterial.renderQueue = -1;
		}



		public void UpdateAutoSort() {
			
			if( !autoSort )
				return;

			if( editor == null )
				return;

			if( editor.mainNode == null )
				return;

			if( editor.nodeView == null )
				return;
			
			if( editor.mainNode.alpha.IsConnectedAndEnabled() || editor.mainNode.refraction.IsConnectedAndEnabled() || editor.nodeView.treeStatus.usesSceneData ) {
				SetQueuePreset(Queue.Transparent);
				renderType = RenderType.Transparent;
				ignoreProjector = true;
				writeDepth = false;
				return;
			} 
			if( ps.UseClipping() ){
				SetQueuePreset(Queue.AlphaTest);
				renderType = RenderType.TransparentCutout;
				ignoreProjector = false;
				writeDepth = true;
				return;
			}
			if( blendModePreset == BlendModePreset.Opaque ){
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














		public bool UseBlending() {
			if( blendModePreset == BlendModePreset.Opaque )
				return false;
			return true;
		}
		public string GetBlendString() {
			return "Blend " + blendSrc.ToString() + " " + blendDst.ToString();
		}

		public string GetOffsetString(){
			if(offsetFactor != 0 || offsetUnits != 0){
				return "Offset " + offsetFactor + ", " + offsetUnits;
			}
			return "";
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
		
		public string GetShadowPragmaIfUsed(){
			if( !ignoreProjector )
				return "_fullshadows";
			return "";
		}




	}
}