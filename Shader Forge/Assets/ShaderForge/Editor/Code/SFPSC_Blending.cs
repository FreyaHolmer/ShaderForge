using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {


	public enum DepthTestStencil { Less, Greater, LEqual, GEqual, Equal, NotEqual, Always, Never };
	public enum StencilOp { Keep, Zero, Replace, Invert, IncrSat, DecrSat, IncrWrap, DecrWrap };
	public enum CullMode { BackfaceCulling, FrontfaceCulling, DoubleSided };
	public enum DepthTest { Less, Greater, LEqual, GEqual, Equal, NotEqual, Always };
	public enum RenderType { None, Opaque, Transparent, TransparentCutout, Background, Overlay, TreeOpaque, TreeTransparentCutout, TreeBillboard, Grass, GrassBillboard };
	public enum BlendModePreset {
		Opaque,
		AlphaBlended,
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
		public static string[] strCullMode = new string[] { "Back", "Front", "Off" };
		public static string[] strDepthTest = new string[] { "<", ">", "\u2264 (Default)", "\u2265", "=", "\u2260", "Always" };
		public static int[] queueNumbers = new int[] { 1000, 2000, 2450, 3000, 4000 };
		public static string[] strQueue = new string[] { "Background (1000)", "Opaque Geometry (2000)", "Alpha Clip (2450)", "Transparent (3000)", "Overlay (4000)" };
		public static string[] strDithering = new string[] { "Off", "2x2 matrix", "3x3 matrix", "4x4 matrix" };
		

		
		// Vars
		
		public BlendModePreset blendModePreset = BlendModePreset.Opaque;
		public BlendMode blendSrc = BlendMode.One;
		public BlendMode blendDst = BlendMode.Zero;
		public CullMode cullMode = CullMode.BackfaceCulling;
		public DepthTest depthTest = DepthTest.LEqual;


		/*public byte stencilValue = 128;
		public byte stencilMaskRead = 255;
		public byte stencilMaskWrite = 255;
		public DepthTestStencil stencilComparison = DepthTestStencil.Always;
		public StencilOp stencilPass = StencilOp.Keep;
		public StencilOp stencilFail = StencilOp.Keep;
		public StencilOp stencilFailZ = StencilOp.Keep;*/
		
		public int offsetFactor = 0;
		public int offsetUnits = 0;

		public Dithering dithering = Dithering.Off;
		
		public bool writeDepth = true;
		
		public bool useFog = true;


		public bool autoSort = true;
		public Queue queuePreset = (Queue)1;
		public int queueOffset = 0;
		public RenderType renderType = RenderType.Opaque;
		public bool ignoreProjector = false;


		// Fog
		public bool fogOverrideMode = false;
		public ShaderFogMode fogMode = ShaderFogMode.Global;
		
		public bool fogOverrideColor = false;
		public Color fogColor = RenderSettings.fogColor;
		
		public bool fogOverrideDensity = false;
		public float fogDensity = RenderSettings.fogDensity;
		
		public bool fogOverrideRange = false;
		public Vector2 fogRange = new Vector2(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance);


		//public bool useStencilBuffer = false;






		public override string Serialize(){
			string s = "";

			s += Serialize( "blpr", ( (int)blendModePreset ).ToString() );
			s += Serialize( "bsrc", ( (int)blendSrc ).ToString() );
			s += Serialize( "bdst", ( (int)blendDst ).ToString() );
			s += Serialize( "culm", ( (int)cullMode ).ToString() );
			s += Serialize( "dpts", ( (int)depthTest ).ToString() );
			s += Serialize( "wrdp", writeDepth.ToString() );

			s += Serialize( "dith", ( (int)dithering ).ToString() );
			
			
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
			/*
			s += Serialize ( "stcl", useStencilBuffer.ToString());
			s += Serialize ( "stva", stencilValue.ToString());
			s += Serialize ( "stmr", stencilMaskRead.ToString());
			s += Serialize ( "stmw", stencilMaskWrite.ToString());
			s += Serialize ( "stcp", ((int)stencilComparison).ToString());
			s += Serialize ( "stps", ((int)stencilPass).ToString());
			s += Serialize ( "stfa", ((int)stencilFail).ToString());
			s += Serialize ( "stfz", ((int)stencilFailZ).ToString(), true);
*/
			
			
			// Offset
			s += Serialize ( "ofsf", offsetFactor.ToString());
			s += Serialize ( "ofsu", offsetUnits.ToString());

			return s;
		}

		public override void Deserialize(string key, string value){


			switch( key ) {

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
			case "dith":
				dithering = (Dithering)int.Parse( value );
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
				/*
			case "stcl":
				useStencilBuffer = bool.Parse(value);
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
				break;*/
				
				// Offset
			case "ofsf":
				offsetFactor = int.Parse(value);
				break;
			case "ofsu":
				offsetUnits = int.Parse(value);
				break;
			}

		}

	

		public override float DrawInner(ref Rect r){


			float prevYpos = r.y;
			r.y = 0;
			
			r.y += 20;
			r.xMin += 20; // Indent
			
			BlendModePreset before = blendModePreset;
			GUI.enabled = ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Forward;
			blendModePreset = (BlendModePreset)UndoableLabeledEnumPopup( r, "Blend Mode", blendModePreset, "blend mode");
			GUI.enabled = true;
			if( blendModePreset != before ) {
				ConformBlendsToPreset();
			}
			
			r.y += 20;
			
			if( blendModePreset != BlendModePreset.Opaque ) {
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
				
				Rect rSrcLb =		new Rect(r);			rSrcLb.width = srcStrWidth;
				Rect rSrcField =	new Rect(r);			rSrcField.x = rSrcLb.xMax;	rSrcField.width = fieldWidth;
				Rect rDstLb =		new Rect(r);			rDstLb.x = rSrcField.xMax;	rDstLb.width = dstStrWidth;
				Rect rDstField =	new Rect(rSrcField);	rDstField.x = rDstLb.xMax;
				
				GUI.Label( rSrcLb, srcStr, EditorStyles.miniLabel );
				blendSrc = (BlendMode)UndoableEnumPopup(rSrcField, blendSrc, "blend source" );
				GUI.Label( rDstLb, dstStr, EditorStyles.miniLabel );
				blendDst = (BlendMode)UndoableEnumPopup(rDstField, blendDst, "blend destination" );
				
				if( blendModePreset != BlendModePreset.Custom )
					GUI.enabled = true;
				
				r.y += 20;
			}

			cullMode = (CullMode)UndoableLabeledEnumPopup( r, "Face Culling", cullMode, "face culling" );
			r.y += 20;
			
			
			bool canEditDithering = editor.mainNode.alphaClip.IsConnectedAndEnabled();
			EditorGUI.BeginDisabledGroup( !canEditDithering );
			if( canEditDithering )
				dithering = (Dithering)UndoableLabeledEnumPopupNamed( r, "Dithered alpha clip", dithering, strDithering, "dithered alpha clip" );
			else
				UndoableLabeledEnumPopup( r, "Dithered alpha clip", Dithering.Off, "dithered alpha clip" );
			EditorGUI.EndDisabledGroup();
			
			r.y += 20;
			
			
			OffsetBlock (ref r);
			
			FogBlock(ref r);
			
			SortingBlock(ref r);
			
			
			
			//StencilBlock(ref r); // TODO

			r.y += prevYpos;
			
			return (int)r.yMax;
		}

		public void SetQueuePreset(Queue in_queue) {
			queuePreset = in_queue;
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
				//case BlendModePreset.PremultipliedAlphaBlended:
				//	blendSrc = BlendMode.One;
				//	blendDst = BlendMode.OneMinusSrcAlpha;
				//	break;
			case BlendModePreset.Multiplicative:
				blendSrc = BlendMode.DstColor;
				blendDst = BlendMode.Zero;
				break;
			}
		}
		
		/*
		public string GetStencilContent(){
			string s = "";
			
			s += "Ref " + stencilValue + "\n";
			
			if(stencilMaskRead != (byte)255)
				s += "ReadMask " + stencilMaskRead + "\n";
			if(stencilMaskWrite != (byte)255)
				s += "WriteMask " + stencilMaskWrite + "\n";
			if(stencilComparison != DepthTestStencil.Always)
				s += "Comp " + stencilComparison + "\n";
			if(stencilPass != StencilOp.Keep)
				s += "Pass " + stencilPass + "\n";
			if(stencilFail != StencilOp.Keep)
				s += "Fail " + stencilFail + "\n";
			if(stencilFailZ != StencilOp.Keep)
				s += "ZFail " + stencilFailZ + "\n";
			
			return s;
			
		}*/


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

		/*
		public void StencilBlock(ref Rect r){
			
			bool prevUseStencilBuffer = useStencilBuffer;
			useStencilBuffer = GUI.Toggle(r, useStencilBuffer, useStencilBuffer ? "Stencil Buffer" : "Stencil Buffer...");
			if( useStencilBuffer != prevUseStencilBuffer )
				UpdateAutoSettings();
			r.y += 20;
			if(!useStencilBuffer)
				return;
			r.xMin += 20;
			
			Rect rTmp = r;
			rTmp.width = 84;
			GUI.Label(rTmp,"Reference Value", EditorStyles.miniLabel);
			rTmp = rTmp.MovedRight();
			rTmp.width -= 40;
			stencilValue = (byte)EditorGUI.IntField(rTmp.PadRight(4).PadTop(1).PadBottom(2),stencilValue);
			rTmp = rTmp.MovedRight();
			rTmp.width = r.width-128;
			stencilComparison = (DepthTestStencil)SF_GUI.LabeledEnumFieldNamed(rTmp.PadRight(4).ClampWidth(32,140),strDepthTestStencil, new GUIContent("Comparison"), (int)stencilComparison, EditorStyles.miniLabel);
			r.y += 20;
			
			StencilBitfield(r, "Read Mask", ref stencilMaskRead);
			r.y += 20;
			StencilBitfield(r, "Write Mask", ref stencilMaskWrite);
			r.y += 23;
			stencilPass = (StencilOp)SF_GUI.LabeledEnumFieldNamed(r.PadRight(4),strStencilOp, new GUIContent("Pass"), (int)stencilPass, EditorStyles.miniLabel);
			r.y += 20;
			stencilFail = (StencilOp)SF_GUI.LabeledEnumFieldNamed(r.PadRight(4),strStencilOp, new GUIContent("Fail"), (int)stencilFail, EditorStyles.miniLabel);
			r.y += 20;
			stencilFailZ = (StencilOp)SF_GUI.LabeledEnumFieldNamed(r.PadRight(4),strStencilOp, new GUIContent("Fail Z"), (int)stencilFailZ, EditorStyles.miniLabel);
			r.y += 20;
			r.xMin -= 20;
			r.y += 20;
		}
		
		
		
		public void StencilBitfield( Rect r, string label, ref byte b ){
			StartIgnoreChangeCheck();
			Rect tmp = r;
			tmp.width = 57;
			
			GUI.Label(tmp.PadRight(2),label, SF_Styles.MiniLabelRight);
			
			tmp = tmp.MovedRight();
			tmp.width = 36;
			b = (byte)EditorGUI.IntField(tmp.PadTop(1).PadBottom(2).PadRight(2).PadLeft(2),b);
			
			Rect bitField = r;
			bitField.xMin += 57+36 + 4;
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
				
				if(GUI.Button(bitField.PadTop(2).PadBottom(2),iBit.ToString(),btnStyle)){
					editor.Defocus();
					b = (byte)( (1<<i) ^ b );
				}
				bitField = bitField.MovedRight();
			}
			GUI.color = Color.white;
			
			EndIgnoreChangeCheck();
			
		}*/
		
		
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
		}



		public void UpdateAutoSort() {
			
			if( !autoSort )
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











		// TODO: Double sided support
		public string GetNormalSign() {
			if( cullMode == CullMode.BackfaceCulling )
				return "";
			if( cullMode == CullMode.FrontfaceCulling )
				return "-";
			//if( cullMode == CullMode.DoubleSided )
			return "";
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
			if(offsetFactor != 0 && offsetUnits != 0){
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
		
		public bool UseCulling() {
			return ( cullMode != CullMode.BackfaceCulling );
		}
		public string GetCullString() {
			return "Cull " + strCullMode[(int)cullMode];
		}
		public bool IsDoubleSided() {
			return ( cullMode == CullMode.DoubleSided );
		}
		
		public string GetShadowPragmaIfUsed(){
			if( !ignoreProjector )
				return "_fullshadows";
			return "";
		}




	}
}