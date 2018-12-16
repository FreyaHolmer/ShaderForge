/*
using System;
using UnityEngine;
using UnityEngineInternal;
using UnityEditor;
using UnityEditorInternal;

namespace ShaderForge {
	public class SF_ColorPicker : EditorWindow {
		private enum ColorBoxMode {
			SV_H,
			HV_S,
			HS_V,
			BG_R,
			BR_G,
			RG_B,
			EyeDropper
		}
		private enum SliderMode {
			RGB,
			HSV
		}
		private class Styles {
			public GUIStyle pickerBox = "ColorPickerBox";
			public GUIStyle thumb2D = "ColorPicker2DThumb";
			public GUIStyle thumbHoriz = "ColorPickerHorizThumb";
			public GUIStyle thumbVert = "ColorPickerVertThumb";
			public GUIStyle headerLine = "IN Title";
			public GUIStyle colorPickerBox = "ColorPickerBox";
			public GUIStyle background = "ColorPickerBackground";
			public GUIContent eyeDropper = EditorGUIUtility.IconContent( "EyeDropper.Large" );
			public GUIContent colorCycle = EditorGUIUtility.IconContent( "ColorPicker.CycleColor" );
			public GUIContent colorToggle = EditorGUIUtility.TextContent( "ColorPicker.ColorFoldout" );
			public GUIContent sliderToggle = EditorGUIUtility.TextContent( "ColorPicker.SliderFoldout" );
			public GUIContent presetsToggle = new GUIContent( "Presets" );
			public GUIContent sliderCycle = EditorGUIUtility.IconContent( "ColorPicker.CycleSlider" );
		}
		private const int kHueRes = 64;
		private const int kColorBoxSize = 8;
		private const int kEyeDropperHeight = 95;
		private const int kSlidersHeight = 82;
		private const int kColorBoxHeight = 162;
		private const int kPresetsHeight = 300;
		private static SF_ColorPicker s_SharedColorPicker;
		[SerializeField]
		private Color m_Color = Color.black;
		[SerializeField]
		private Color m_OriginalColor;
		[SerializeField]
		private float m_R;
		[SerializeField]
		private float m_G;
		[SerializeField]
		private float m_B;
		[SerializeField]
		private float m_H;
		[SerializeField]
		private float m_S;
		[SerializeField]
		private float m_V;
		[SerializeField]
		private float m_A = 1f;
		[SerializeField]
		private float m_ColorSliderSize = 4f;
		[SerializeField]
		private Texture2D m_ColorSlider;
		[SerializeField]
		private float m_SliderValue;
		[SerializeField]
		private Color[] m_Colors;
		[SerializeField]
		private Texture2D m_ColorBox;
		private static int s_Slider2Dhash = "Slider2D".GetHashCode();
		[SerializeField]
		private bool m_ShowColors = true;
		[SerializeField]
		private bool m_ShowSliders = true;
		[SerializeField]
		private bool m_ShowPresets = true;
		[SerializeField]
		private bool m_IsOSColorPicker;
		[SerializeField]
		private bool m_resetKeyboardControl;
		[SerializeField]
		private bool m_ShowAlpha = true;
		private Texture2D m_RTexture;
		private float m_RTextureG = -1f;
		private float m_RTextureB = -1f;
		private Texture2D m_GTexture;
		private float m_GTextureR = -1f;
		private float m_GTextureB = -1f;
		private Texture2D m_BTexture;
		private float m_BTextureR = -1f;
		private float m_BTextureG = -1f;
		[SerializeField]
		private Texture2D m_HueTexture;
		private float m_HueTextureS = -1f;
		private float m_HueTextureV = -1f;
		[SerializeField]
		private Texture2D m_SatTexture;
		private float m_SatTextureH = -1f;
		private float m_SatTextureV = -1f;
		[SerializeField]
		private Texture2D m_ValTexture;
		private float m_ValTextureH = -1f;
		private float m_ValTextureS = -1f;
		[SerializeField]
		private int m_TextureColorSliderMode = -1;
		[SerializeField]
		private Vector2 m_LastConstantValues = new Vector2( -1f, -1f );
		[NonSerialized]
		private int m_TextureColorBoxMode = -1;
		[SerializeField]
		private float m_LastConstant = -1f;
		[SerializeField]
		private ContainerWindow m_TrackingWindow;
		[SerializeField]
		private SF_ColorPicker.ColorBoxMode m_ColorBoxMode = SF_ColorPicker.ColorBoxMode.BG_R;
		[SerializeField]
		private SF_ColorPicker.ColorBoxMode m_OldColorBoxMode;
		[SerializeField]
		private SF_ColorPicker.SliderMode m_SliderMode = SF_ColorPicker.SliderMode.HSV;
		[SerializeField]
		private Texture2D m_AlphaTexture;
		private float m_OldAlpha = -1f;
		[SerializeField]
		private GUIView m_DelegateView;
		private PresetLibraryEditor<ColorPresetLibrary> m_ColorLibraryEditor;
		private PresetLibraryEditorState m_ColorLibraryEditorState;
		private static SF_ColorPicker.Styles styles;
		public static string presetsEditorPrefID {
			get {
				return "Color";
			}
		}
		private bool colorChanged {
			get;
			set;
		}
		public static bool visible {
			get {
				return SF_ColorPicker.s_SharedColorPicker != null;
			}
		}
		public static Color color {
			get {
				return SF_ColorPicker.get.m_Color;
			}
			set {
				SF_ColorPicker.get.SetColor( value );
			}
		}
		public static SF_ColorPicker get {
			get {
				if( !SF_ColorPicker.s_SharedColorPicker ) {
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll( typeof( SF_ColorPicker ) );
					if( array != null && array.Length > 0 ) {
						SF_ColorPicker.s_SharedColorPicker = (SF_ColorPicker)array[0];
					}
					if( !SF_ColorPicker.s_SharedColorPicker ) {
						SF_ColorPicker.s_SharedColorPicker = ScriptableObject.CreateInstance<SF_ColorPicker>();
						SF_ColorPicker.s_SharedColorPicker.wantsMouseMove = true;
					}
				}
				return SF_ColorPicker.s_SharedColorPicker;
			}
		}
		public string currentPresetLibrary {
			get {
				this.InitIfNeeded();
				return this.m_ColorLibraryEditor.currentLibraryWithoutExtension;
			}
			set {
				this.InitIfNeeded();
				this.m_ColorLibraryEditor.currentLibraryWithoutExtension = value;
			}
		}
		public SF_ColorPicker() {
			base.hideFlags = HideFlags.DontSave;
			this.m_ShowSliders = ( EditorPrefs.GetInt( "CPSliderShow", 1 ) != 0 );
			this.m_SliderMode = (SF_ColorPicker.SliderMode)EditorPrefs.GetInt( "CPSliderMode", 0 );
			this.m_ShowColors = ( EditorPrefs.GetInt( "CPColorShow", 1 ) != 0 );
			this.m_ColorBoxMode = (SF_ColorPicker.ColorBoxMode)EditorPrefs.GetInt( "CPColorMode", 0 );
			this.m_IsOSColorPicker = EditorPrefs.GetBool( "UseOSColorPicker" );
			this.m_ShowPresets = ( EditorPrefs.GetInt( "CPPresetsShow", 1 ) != 0 );
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine( EditorApplication.update, new EditorApplication.CallbackFunction( this.PollOSColorPicker ) );
		}
		private void OnSelectionChange() {
			this.m_resetKeyboardControl = true;
			base.Repaint();
		}
		private void RGBToHSV() {
			EditorGUIUtility.RGBToHSV( new Color( this.m_R, this.m_G, this.m_B, 1f ), out this.m_H, out this.m_S, out this.m_V );
		}
		private void HSVToRGB() {
			Color color = EditorGUIUtility.HSVToRGB( this.m_H, this.m_S, this.m_V );
			this.m_R = color.r;
			this.m_G = color.g;
			this.m_B = color.b;
		}
		private static void swap( ref float f1, ref float f2 ) {
			float num = f1;
			f1 = f2;
			f2 = num;
		}
		private Vector2 Slider2D( Rect rect, Vector2 value, Vector2 maxvalue, Vector2 minvalue, GUIStyle backStyle, GUIStyle thumbStyle ) {
			if( backStyle == null ) {
				return value;
			}
			if( thumbStyle == null ) {
				return value;
			}
			int controlID = GUIUtility.GetControlID( SF_ColorPicker.s_Slider2Dhash, FocusType.Native );
			if( maxvalue.x < minvalue.x ) {
				SF_ColorPicker.swap( ref maxvalue.x, ref minvalue.x );
			}
			if( maxvalue.y < minvalue.y ) {
				SF_ColorPicker.swap( ref maxvalue.y, ref minvalue.y );
			}
			float num = ( thumbStyle.fixedHeight != 0f ) ? thumbStyle.fixedHeight : ( (float)thumbStyle.padding.vertical );
			float num2 = ( thumbStyle.fixedWidth != 0f ) ? thumbStyle.fixedWidth : ( (float)thumbStyle.padding.horizontal );
			Vector2 vector = new Vector2( ( rect.width - (float)( backStyle.padding.right + backStyle.padding.left ) - num2 * 2f ) / ( maxvalue.x - minvalue.x ), ( rect.height - (float)( backStyle.padding.top + backStyle.padding.bottom ) - num * 2f ) / ( maxvalue.y - minvalue.y ) );
			Rect position = new Rect( rect.x + value.x * vector.x + num2 / 2f + (float)backStyle.padding.left - minvalue.x * vector.x, rect.y + value.y * vector.y + num / 2f + (float)backStyle.padding.top - minvalue.y * vector.y, num2, num );
			Event current = Event.current;
			switch( current.GetTypeForControl( controlID ) ) {
				case EventType.MouseDown:
					if( rect.Contains( current.mousePosition ) ) {
						GUIUtility.hotControl = controlID;
						value.x = ( current.mousePosition.x - rect.x - num2 - (float)backStyle.padding.left ) / vector.x + minvalue.x;
						value.y = ( current.mousePosition.y - rect.y - num - (float)backStyle.padding.top ) / vector.y + minvalue.y;
						GUI.changed = true;
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if( GUIUtility.hotControl == controlID ) {
						GUIUtility.hotControl = 0;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if( GUIUtility.hotControl == controlID ) {
						value.x = ( current.mousePosition.x - rect.x - num2 - (float)backStyle.padding.left ) / vector.x + minvalue.x;
						value.y = ( current.mousePosition.y - rect.y - num - (float)backStyle.padding.top ) / vector.y + minvalue.y;
						value.x = Mathf.Clamp( value.x, minvalue.x, maxvalue.x );
						value.y = Mathf.Clamp( value.y, minvalue.y, maxvalue.y );
						GUI.changed = true;
						Event.current.Use();
					}
					break;
				case EventType.Repaint:
					backStyle.Draw( rect, GUIContent.none, controlID );
					thumbStyle.Draw( position, GUIContent.none, controlID );
					break;
			}
			return value;
		}
		private void RGBSliders() {
			bool changed = GUI.changed;
			GUI.changed = false;
			this.m_RTexture = SF_ColorPicker.Update1DSlider( this.m_RTexture, 8, this.m_G, this.m_B, ref this.m_RTextureG, ref this.m_RTextureB, 0, false );
			this.m_GTexture = SF_ColorPicker.Update1DSlider( this.m_GTexture, 8, this.m_R, this.m_B, ref this.m_GTextureR, ref this.m_GTextureB, 1, false );
			this.m_BTexture = SF_ColorPicker.Update1DSlider( this.m_BTexture, 8, this.m_R, this.m_G, ref this.m_BTextureR, ref this.m_BTextureG, 2, false );
			float num = (float)( (int)Mathf.Round( this.m_R * 255f ) );
			float num2 = (float)( (int)Mathf.Round( this.m_G * 255f ) );
			float num3 = (float)( (int)Mathf.Round( this.m_B * 255f ) );
			num = this.TexturedSlider( this.m_RTexture, "R", num, 0f, 255f );
			num2 = this.TexturedSlider( this.m_GTexture, "G", num2, 0f, 255f );
			num3 = this.TexturedSlider( this.m_BTexture, "B", num3, 0f, 255f );
			if( GUI.changed ) {
				this.m_R = num / 255f;
				this.m_G = num2 / 255f;
				this.m_B = num3 / 255f;
				this.RGBToHSV();
			}
			GUI.changed |= changed;
		}
		private static Texture2D Update1DSlider( Texture2D tex, int xSize, float const1, float const2, ref float oldConst1, ref float oldConst2, int idx, bool hsvSpace ) {
			if( !tex || const1 != oldConst1 || const2 != oldConst2 ) {
				if( !tex ) {
					tex = SF_ColorPicker.MakeTexture( xSize, 2 );
				}
				Color[] array = new Color[xSize * 2];
				Color black = Color.black;
				Color black2 = Color.black;
				switch( idx ) {
					case 0:
						black = new Color( 0f, const1, const2, 1f );
						black2 = new Color( 1f, 0f, 0f, 0f );
						break;
					case 1:
						black = new Color( const1, 0f, const2, 1f );
						black2 = new Color( 0f, 1f, 0f, 0f );
						break;
					case 2:
						black = new Color( const1, const2, 0f, 1f );
						black2 = new Color( 0f, 0f, 1f, 0f );
						break;
					case 3:
						black = new Color( 0f, 0f, 0f, 1f );
						black2 = new Color( 1f, 1f, 1f, 0f );
						break;
				}
				SF_ColorPicker.FillArea( xSize, 2, array, black, black2, new Color( 0f, 0f, 0f, 0f ) );
				if( hsvSpace ) {
					SF_ColorPicker.HSVToRGBArray( array );
				}
				oldConst1 = const1;
				oldConst2 = const2;
				tex.SetPixels( array );
				tex.Apply();
			}
			return tex;
		}
		private float TexturedSlider( Texture2D background, string text, float val, float min, float max ) {
			Rect rect = GUILayoutUtility.GetRect( 16f, 16f, GUI.skin.label );
			GUI.Label( new Rect( rect.x, rect.y - 1f, 20f, 16f ), text );
			rect.x += 14f;
			rect.width -= 50f;
			if( Event.current.type == EventType.Repaint ) {
				Rect screenRect = new Rect( rect.x + 1f, rect.y + 2f, rect.width - 2f, rect.height - 4f );
				Graphics.DrawTexture( screenRect, background, new Rect( 0.5f / (float)background.width, 0.5f / (float)background.height, 1f - 1f / (float)background.width, 1f - 1f / (float)background.height ), 0, 0, 0, 0, Color.grey );
			}
			int controlID = EditorGUI.GetControlID( 869045, EditorGUIUtility.native, base.position );
			bool changed = GUI.changed;
			GUI.changed = false;
			val = GUI.HorizontalSlider( new Rect( rect.x, rect.y + 1f, rect.width, rect.height - 2f ), val, min, max, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumbHoriz );
			if( GUI.changed && EditorGUI.s_RecycledEditor.IsEditingControl( controlID ) ) {
				EditorGUI.s_RecycledEditor.EndEditing();
			}
			Rect position = new Rect( rect.xMax + 6f, rect.y, 30f, 16f );
			val = (float)( (int)EditorGUI.DoFloatField( EditorGUI.s_RecycledEditor, position, new Rect( 0f, 0f, 0f, 0f ), controlID, val, EditorGUI.kIntFieldFormatString, EditorStyles.numberField, false ) );
			val = Mathf.Clamp( val, min, max );
			GUI.changed |= changed;
			return val;
		}
		private void HSVSliders() {
			bool changed = GUI.changed;
			GUI.changed = false;
			this.m_HueTexture = SF_ColorPicker.Update1DSlider( this.m_HueTexture, 64, 1f, 1f, ref this.m_HueTextureS, ref this.m_HueTextureV, 0, true );
			this.m_SatTexture = SF_ColorPicker.Update1DSlider( this.m_SatTexture, 8, this.m_H, Mathf.Max( this.m_V, 0.2f ), ref this.m_SatTextureH, ref this.m_SatTextureV, 1, true );
			this.m_ValTexture = SF_ColorPicker.Update1DSlider( this.m_ValTexture, 8, this.m_H, this.m_S, ref this.m_ValTextureH, ref this.m_ValTextureS, 2, true );
			float num = (float)( (int)Mathf.Round( this.m_H * 359f ) );
			float num2 = (float)( (int)Mathf.Round( this.m_S * 255f ) );
			float num3 = (float)( (int)Mathf.Round( this.m_V * 255f ) );
			num = this.TexturedSlider( this.m_HueTexture, "H", num, 0f, 359f );
			num2 = this.TexturedSlider( this.m_SatTexture, "S", num2, 0f, 255f );
			num3 = this.TexturedSlider( this.m_ValTexture, "V", num3, 0f, 255f );
			if( GUI.changed ) {
				this.m_H = num / 359f;
				this.m_S = num2 / 255f;
				this.m_V = num3 / 255f;
				this.HSVToRGB();
			}
			GUI.changed |= changed;
		}
		private static void FillArea( int xSize, int ySize, Color[] retval, Color topLeftColor, Color rightGradient, Color downGradient ) {
			Color b = new Color( 0f, 0f, 0f, 0f );
			Color b2 = new Color( 0f, 0f, 0f, 0f );
			if( xSize > 1 ) {
				b = rightGradient / (float)( xSize - 1 );
			}
			if( ySize > 1 ) {
				b2 = downGradient / (float)( ySize - 1 );
			}
			Color color = topLeftColor;
			int num = 0;
			for( int i = 0; i < ySize; i++ ) {
				Color color2 = color;
				for( int j = 0; j < xSize; j++ ) {
					retval[num++] = color2;
					color2 += b;
				}
				color += b2;
			}
		}
		private static void HSVToRGBArray( Color[] colors ) {
			int num = colors.Length;
			for( int i = 0; i < num; i++ ) {
				Color color = colors[i];
				Color color2 = EditorGUIUtility.HSVToRGB( color.r, color.g, color.b );
				color2.a = color.a;
				colors[i] = color2;
			}
		}
		private void DrawColorSlider( Rect colorSliderRect, Vector2 constantValues ) {
			if( Event.current.type != EventType.Repaint ) {
				return;
			}
			if( this.m_ColorBoxMode != (SF_ColorPicker.ColorBoxMode)this.m_TextureColorSliderMode ) {
				int num = (int)this.m_ColorSliderSize;
				int num2;
				if( this.m_ColorBoxMode == SF_ColorPicker.ColorBoxMode.SV_H ) {
					num2 = 64;
				} else {
					num2 = (int)this.m_ColorSliderSize;
				}
				if( this.m_ColorSlider == null ) {
					this.m_ColorSlider = SF_ColorPicker.MakeTexture( num, num2 );
				}
				if( this.m_ColorSlider.width != num || this.m_ColorSlider.height != num2 ) {
					this.m_ColorSlider.Resize( num, num2 );
				}
			}
			if( this.m_ColorBoxMode != (SF_ColorPicker.ColorBoxMode)this.m_TextureColorSliderMode || constantValues != this.m_LastConstantValues ) {
				Color[] pixels = this.m_ColorSlider.GetPixels( 0 );
				int width = this.m_ColorSlider.width;
				int height = this.m_ColorSlider.height;
				switch( this.m_ColorBoxMode ) {
					case SF_ColorPicker.ColorBoxMode.SV_H:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( 0f, 1f, 1f, 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 1f, 0f, 0f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( pixels );
						break;
					case SF_ColorPicker.ColorBoxMode.HV_S:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( this.m_H, 0f, Mathf.Max( this.m_V, 0.3f ), 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 0f, 1f, 0f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( pixels );
						break;
					case SF_ColorPicker.ColorBoxMode.HS_V:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( this.m_H, this.m_S, 0f, 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 0f, 0f, 1f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( pixels );
						break;
					case SF_ColorPicker.ColorBoxMode.BG_R:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( 0f, this.m_G, this.m_B, 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 1f, 0f, 0f, 0f ) );
						break;
					case SF_ColorPicker.ColorBoxMode.BR_G:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( this.m_R, 0f, this.m_B, 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 0f, 1f, 0f, 0f ) );
						break;
					case SF_ColorPicker.ColorBoxMode.RG_B:
						SF_ColorPicker.FillArea( width, height, pixels, new Color( this.m_R, this.m_G, 0f, 1f ), new Color( 0f, 0f, 0f, 0f ), new Color( 0f, 0f, 1f, 0f ) );
						break;
				}
				this.m_ColorSlider.SetPixels( pixels, 0 );
				this.m_ColorSlider.Apply( true );
			}
			Graphics.DrawTexture( colorSliderRect, this.m_ColorSlider, new Rect( 0.5f / (float)this.m_ColorSlider.width, 0.5f / (float)this.m_ColorSlider.height, 1f - 1f / (float)this.m_ColorSlider.width, 1f - 1f / (float)this.m_ColorSlider.height ), 0, 0, 0, 0, Color.grey );
		}
		public static Texture2D MakeTexture( int width, int height ) {
			return new Texture2D( width, height, TextureFormat.ARGB32, false ) {
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Clamp
				//hideFlags = HideFlags.DontSave
			};
		}
		private void DrawColorSpaceBox( Rect colorBoxRect, float constantValue ) {
			if( Event.current.type != EventType.Repaint ) {
				return;
			}
			if( this.m_ColorBoxMode != (SF_ColorPicker.ColorBoxMode)this.m_TextureColorBoxMode ) {
				int num = 8;
				int num2;
				if( this.m_ColorBoxMode == SF_ColorPicker.ColorBoxMode.HV_S || this.m_ColorBoxMode == SF_ColorPicker.ColorBoxMode.HS_V ) {
					num2 = 64;
				} else {
					num2 = 8;
				}
				if( this.m_ColorBox == null ) {
					this.m_ColorBox = SF_ColorPicker.MakeTexture( num2, num );
				}
				if( this.m_ColorBox.width != num2 || this.m_ColorBox.height != num ) {
					this.m_ColorBox.Resize( num2, num );
				}
			}
			if( this.m_ColorBoxMode != (SF_ColorPicker.ColorBoxMode)this.m_TextureColorBoxMode || this.m_LastConstant != constantValue ) {
				this.m_Colors = this.m_ColorBox.GetPixels( 0 );
				int width = this.m_ColorBox.width;
				int height = this.m_ColorBox.height;
				switch( this.m_ColorBoxMode ) {
					case SF_ColorPicker.ColorBoxMode.SV_H:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( this.m_H, 0f, 0f, 1f ), new Color( 0f, 1f, 0f, 0f ), new Color( 0f, 0f, 1f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( this.m_Colors );
						break;
					case SF_ColorPicker.ColorBoxMode.HV_S:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( 0f, this.m_S, 0f, 1f ), new Color( 1f, 0f, 0f, 0f ), new Color( 0f, 0f, 1f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( this.m_Colors );
						break;
					case SF_ColorPicker.ColorBoxMode.HS_V:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( 0f, 0f, this.m_V, 1f ), new Color( 1f, 0f, 0f, 0f ), new Color( 0f, 1f, 0f, 0f ) );
						SF_ColorPicker.HSVToRGBArray( this.m_Colors );
						break;
					case SF_ColorPicker.ColorBoxMode.BG_R:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( this.m_R, 0f, 0f, 1f ), new Color( 0f, 0f, 1f, 0f ), new Color( 0f, 1f, 0f, 0f ) );
						break;
					case SF_ColorPicker.ColorBoxMode.BR_G:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( 0f, this.m_G, 0f, 1f ), new Color( 0f, 0f, 1f, 0f ), new Color( 1f, 0f, 0f, 0f ) );
						break;
					case SF_ColorPicker.ColorBoxMode.RG_B:
						SF_ColorPicker.FillArea( width, height, this.m_Colors, new Color( 0f, 0f, this.m_B, 1f ), new Color( 1f, 0f, 0f, 0f ), new Color( 0f, 1f, 0f, 0f ) );
						break;
				}
				this.m_ColorBox.SetPixels( this.m_Colors, 0 );
				this.m_ColorBox.Apply( true );
				this.m_LastConstant = constantValue;
				this.m_TextureColorBoxMode = (int)this.m_ColorBoxMode;
			}
			Graphics.DrawTexture( colorBoxRect, this.m_ColorBox, new Rect( 0.5f / (float)this.m_ColorBox.width, 0.5f / (float)this.m_ColorBox.height, 1f - 1f / (float)this.m_ColorBox.width, 1f - 1f / (float)this.m_ColorBox.height ), 0, 0, 0, 0, Color.grey );
		}
		private void InitIfNeeded() {
			if( SF_ColorPicker.styles == null ) {
				SF_ColorPicker.styles = new SF_ColorPicker.Styles();
			}
			if( this.m_ColorLibraryEditorState == null ) {
				this.m_ColorLibraryEditorState = new PresetLibraryEditorState( SF_ColorPicker.presetsEditorPrefID );
				this.m_ColorLibraryEditorState.TransferEditorPrefsState( true );
			}
			if( this.m_ColorLibraryEditor == null ) {
				ScriptableObjectSaveLoadHelper<ColorPresetLibrary> helper = new ScriptableObjectSaveLoadHelper<ColorPresetLibrary>( "colors", SaveType.Text );
				this.m_ColorLibraryEditor = new PresetLibraryEditor<ColorPresetLibrary>( helper, this.m_ColorLibraryEditorState, new Action<int, object>( this.PresetClickedCallback ) );
				this.m_ColorLibraryEditor.previewAspect = 1f;
				this.m_ColorLibraryEditor.minMaxPreviewHeight = new Vector2( 14f, 14f );
				this.m_ColorLibraryEditor.settingsMenuRightMargin = 2f;
				this.m_ColorLibraryEditor.useOnePixelOverlappedGrid = true;
				this.m_ColorLibraryEditor.alwaysShowScrollAreaHorizontalLines = false;
				this.m_ColorLibraryEditor.marginsForGrid = new RectOffset( 0, 0, 0, 0 );
				this.m_ColorLibraryEditor.marginsForList = new RectOffset( 0, 5, 2, 2 );
			}
		}
		private void PresetClickedCallback( int clickCount, object presetObject ) {
			Color color = (Color)presetObject;
			this.SetColor( color );
			this.colorChanged = true;
		}
		private void DoColorSwatchAndEyedropper() {
			GUILayout.BeginHorizontal( new GUILayoutOption[0] );
			if( GUILayout.Button( SF_ColorPicker.styles.eyeDropper, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.Width(40f),
				GUILayout.ExpandWidth(false)
			} ) ) {
				EyeDropper.Start( this.m_Parent );
				this.m_ColorBoxMode = SF_ColorPicker.ColorBoxMode.EyeDropper;
				GUIUtility.ExitGUI();
			}
			Color color = new Color( this.m_R, this.m_G, this.m_B, this.m_A );
			Rect rect = GUILayoutUtility.GetRect( 20f, 20f, 20f, 20f, SF_ColorPicker.styles.SF_ColorPickerBox, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			} );
			EditorGUIUtility.DrawColorSwatch( rect, color, this.m_ShowAlpha );
			if( Event.current.type == EventType.Repaint ) {
				SF_ColorPicker.styles.pickerBox.Draw( rect, GUIContent.none, false, false, false, false );
			}
			GUILayout.EndHorizontal();
		}
		private void DoColorSpaceGUI() {
			GUILayout.BeginHorizontal( new GUILayoutOption[0] );
			this.m_ShowColors = GUILayout.Toggle( this.m_ShowColors, SF_ColorPicker.styles.colorToggle, EditorStyles.foldout, new GUILayoutOption[0] );
			GUI.enabled = this.m_ShowColors;
			if( GUILayout.Button( SF_ColorPicker.styles.colorCycle, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			} ) ) {
				this.m_OldColorBoxMode = ( this.m_ColorBoxMode = ( this.m_ColorBoxMode + 1 ) % SF_ColorPicker.ColorBoxMode.EyeDropper );
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			if( this.m_ShowColors ) {
				bool changed = GUI.changed;
				GUILayout.BeginHorizontal( new GUILayoutOption[]
				{
					GUILayout.ExpandHeight(false)
				} );
				Rect aspectRect = GUILayoutUtility.GetAspectRect( 1f, SF_ColorPicker.styles.pickerBox, new GUILayoutOption[]
				{
					GUILayout.MinWidth(64f),
					GUILayout.MinHeight(64f),
					GUILayout.MaxWidth(256f),
					GUILayout.MaxHeight(256f)
				} );
				EditorGUILayout.Space();
				Rect rect = GUILayoutUtility.GetRect( 8f, 32f, 64f, 128f, SF_ColorPicker.styles.pickerBox );
				rect.height = aspectRect.height;
				GUILayout.EndHorizontal();
				GUI.changed = false;
				switch( this.m_ColorBoxMode ) {
					case SF_ColorPicker.ColorBoxMode.SV_H:
						this.Slider3D( aspectRect, rect, ref this.m_S, ref this.m_V, ref this.m_H, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.HSVToRGB();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.HV_S:
						this.Slider3D( aspectRect, rect, ref this.m_H, ref this.m_V, ref this.m_S, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.HSVToRGB();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.HS_V:
						this.Slider3D( aspectRect, rect, ref this.m_H, ref this.m_S, ref this.m_V, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.HSVToRGB();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.BG_R:
						this.Slider3D( aspectRect, rect, ref this.m_B, ref this.m_G, ref this.m_R, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.RGBToHSV();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.BR_G:
						this.Slider3D( aspectRect, rect, ref this.m_B, ref this.m_R, ref this.m_G, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.RGBToHSV();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.RG_B:
						this.Slider3D( aspectRect, rect, ref this.m_R, ref this.m_G, ref this.m_B, SF_ColorPicker.styles.pickerBox, SF_ColorPicker.styles.thumb2D, SF_ColorPicker.styles.thumbVert );
						if( GUI.changed ) {
							this.RGBToHSV();
						}
						break;
					case SF_ColorPicker.ColorBoxMode.EyeDropper:
						EyeDropper.DrawPreview( Rect.MinMaxRect( aspectRect.x, aspectRect.y, rect.xMax, aspectRect.yMax ) );
						break;
				}
				GUI.changed |= changed;
			}
		}
		private void DoColorSliders() {
			GUILayout.BeginHorizontal( new GUILayoutOption[0] );
			this.m_ShowSliders = GUILayout.Toggle( this.m_ShowSliders, SF_ColorPicker.styles.sliderToggle, EditorStyles.foldout, new GUILayoutOption[0] );
			GUI.enabled = this.m_ShowSliders;
			if( GUILayout.Button( SF_ColorPicker.styles.sliderCycle, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			} ) ) {
				this.m_SliderMode = ( this.m_SliderMode + 1 ) % (SF_ColorPicker.SliderMode)2;
				GUI.changed = true;
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			if( this.m_ShowSliders ) {
				SF_ColorPicker.SliderMode sliderMode = this.m_SliderMode;
				if( sliderMode != SF_ColorPicker.SliderMode.RGB ) {
					if( sliderMode == SF_ColorPicker.SliderMode.HSV ) {
						this.HSVSliders();
					}
				} else {
					this.RGBSliders();
				}
				if( this.m_ShowAlpha ) {
					this.m_AlphaTexture = SF_ColorPicker.Update1DSlider( this.m_AlphaTexture, 8, 0f, 0f, ref this.m_OldAlpha, ref this.m_OldAlpha, 3, false );
					this.m_A = this.TexturedSlider( this.m_AlphaTexture, "A", Mathf.Round( this.m_A * 255f ), 0f, 255f ) / 255f;
				}
			}
		}
		private void DoPresetsGUI() {
			GUILayout.BeginHorizontal( new GUILayoutOption[0] );
			EditorGUI.BeginChangeCheck();
			this.m_ShowPresets = GUILayout.Toggle( this.m_ShowPresets, SF_ColorPicker.styles.presetsToggle, EditorStyles.foldout, new GUILayoutOption[0] );
			if( EditorGUI.EndChangeCheck() ) {
				EditorPrefs.SetInt( "CPPresetsShow", ( !this.m_ShowPresets ) ? 0 : 1 );
			}
			GUILayout.Space( 17f );
			GUILayout.EndHorizontal();
			if( this.m_ShowPresets ) {
				GUILayout.Space( -18f );
				Rect rect = GUILayoutUtility.GetRect( 0f, Mathf.Clamp( this.m_ColorLibraryEditor.contentHeight, 40f, 250f ) );
				this.m_ColorLibraryEditor.OnGUI( rect, this.m_Color );
			}
		}
		private void OnGUI() {
			this.InitIfNeeded();
			if( this.m_resetKeyboardControl ) {
				GUIUtility.keyboardControl = 0;
				this.m_resetKeyboardControl = false;
			}
			EventType type = Event.current.type;
			if( type == EventType.ExecuteCommand ) {
				string commandName = Event.current.commandName;
				switch( commandName ) {
					case "EyeDropperUpdate":
						base.Repaint();
						break;
					case "EyeDropperClicked": {
							Color lastPickedColor = EyeDropper.GetLastPickedColor();
							this.m_R = lastPickedColor.r;
							this.m_G = lastPickedColor.g;
							this.m_B = lastPickedColor.b;
							this.RGBToHSV();
							this.m_ColorBoxMode = this.m_OldColorBoxMode;
							this.m_Color = new Color( this.m_R, this.m_G, this.m_B, this.m_A );
							this.SendEvent( true );
							break;
						}
					case "EyeDropperCancelled":
						base.Repaint();
						this.m_ColorBoxMode = this.m_OldColorBoxMode;
						break;
				}
			}
			EditorGUIUtility.LookLikeControls( 15f, 30f );
			Rect rect = EditorGUILayout.BeginVertical( SF_ColorPicker.styles.background, new GUILayoutOption[0] );
			EditorGUI.BeginChangeCheck();
			this.DoColorSwatchAndEyedropper();
			GUILayout.Space( 10f );
			this.DoColorSpaceGUI();
			GUILayout.Space( 10f );
			this.DoColorSliders();
			GUILayout.Space( 10f );
			if( EditorGUI.EndChangeCheck() ) {
				this.colorChanged = true;
			}
			this.DoPresetsGUI();
			GUILayout.Space( 10f );
			if( this.colorChanged ) {
				EditorPrefs.SetInt( "CPSliderShow", ( !this.m_ShowSliders ) ? 0 : 1 );
				EditorPrefs.SetInt( "CPSliderMode", (int)this.m_SliderMode );
				EditorPrefs.SetInt( "CPColorShow", ( !this.m_ShowColors ) ? 0 : 1 );
				EditorPrefs.SetInt( "CPColorMode", (int)this.m_ColorBoxMode );
			}
			if( this.colorChanged ) {
				this.colorChanged = false;
				this.m_Color = new Color( this.m_R, this.m_G, this.m_B, this.m_A );
				this.SendEvent( true );
			}
			EditorGUILayout.EndVertical();
			if( rect.height > 0f ) {
				this.SetHeight( rect.height );
			}
			if( Event.current.type == EventType.KeyDown ) {
				KeyCode keyCode = Event.current.keyCode;
				if( keyCode != KeyCode.Return ) {
					if( keyCode == KeyCode.Escape ) {
						this.m_Color = this.m_OriginalColor;
						this.SendEvent( false );
						base.Close();
						GUIUtility.ExitGUI();
						return;
					}
					if( keyCode != KeyCode.KeypadEnter ) {
						return;
					}
				}
				base.Close();
			}
		}
		private void SetHeight( float newHeight ) {
			if( newHeight == base.position.height ) {
				return;
			}
			base.minSize = new Vector2( 193f, newHeight );
			base.maxSize = new Vector2( 193f, newHeight );
		}
		private void Slider3D( Rect boxPos, Rect sliderPos, ref float x, ref float y, ref float z, GUIStyle box, GUIStyle thumb2D, GUIStyle thumbHoriz ) {
			Rect colorBoxRect = boxPos;
			colorBoxRect.x += 1f;
			colorBoxRect.y += 1f;
			colorBoxRect.width -= 2f;
			colorBoxRect.height -= 2f;
			this.DrawColorSpaceBox( colorBoxRect, z );
			Vector2 value = new Vector2( x, 1f - y );
			value = this.Slider2D( boxPos, value, new Vector2( 0f, 0f ), new Vector2( 1f, 1f ), box, thumb2D );
			x = value.x;
			y = 1f - value.y;
			Rect colorSliderRect = new Rect( sliderPos.x + 1f, sliderPos.y + 1f, sliderPos.width - 2f, sliderPos.height - 2f );
			this.DrawColorSlider( colorSliderRect, new Vector2( x, y ) );
			z = GUI.VerticalSlider( sliderPos, z, 1f, 0f, box, thumbHoriz );
		}
		private void SendEvent( bool exitGUI ) {
			if( this.m_DelegateView ) {
				Event e = EditorGUIUtility.CommandEvent( "ColorPickerChanged" );
				if( !this.m_IsOSColorPicker ) {
					base.Repaint();
				}
				this.m_DelegateView.SendEvent( e );
				if( !this.m_IsOSColorPicker && exitGUI ) {
					GUIUtility.ExitGUI();
				}
			}
		}
		public void SetColor( Color c ) {
			if( this.m_IsOSColorPicker ) {
				OSColorPicker.color = c;
			} else {
				if( this.m_Color.r == c.r && this.m_Color.g == c.g && this.m_Color.b == c.b && this.m_Color.a == c.a ) {
					return;
				}
				this.m_resetKeyboardControl = true;
				this.m_Color = c;
				this.m_R = c.r;
				this.m_G = c.g;
				this.m_B = c.b;
				this.RGBToHSV();
				this.m_A = c.a;
				base.Repaint();
			}
		}
		public static void Show( GUIView viewToUpdate, Color col ) {
			SF_ColorPicker.Show( viewToUpdate, col, true );
		}
		public static void Show( GUIView viewToUpdate, Color col, bool showAlpha ) {
			SF_ColorPicker.get.m_DelegateView = viewToUpdate;
			SF_ColorPicker.color = col;
			SF_ColorPicker.get.m_OriginalColor = col;
			SF_ColorPicker.get.m_ShowAlpha = showAlpha;
			if( SF_ColorPicker.get.m_IsOSColorPicker ) {
				OSColorPicker.Show( showAlpha );
			} else {
				SF_ColorPicker get = SF_ColorPicker.get;
				get.title = "Color";
				float x = (float)EditorPrefs.GetInt( "CPickerWidth", (int)get.position.width );
				float y = (float)EditorPrefs.GetInt( "CPickerHeight", (int)get.position.height );
				get.minSize = new Vector2( x, y );
				get.maxSize = new Vector2( x, y );
				get.ShowAuxWindow();
			}
		}
		private void PollOSColorPicker() {
			if( this.m_IsOSColorPicker ) {
				if( !OSColorPicker.visible || Application.platform != RuntimePlatform.OSXEditor ) {
					UnityEngine.Object.DestroyImmediate( this );
				} else {
					Color color = OSColorPicker.color;
					if( this.m_Color != color ) {
						this.m_Color = color;
						this.SendEvent( true );
					}
				}
			}
		}
		public void OnDestroy() {
			if( this.m_ColorSlider ) {
				UnityEngine.Object.DestroyImmediate( this.m_ColorSlider );
			}
			if( this.m_ColorBox ) {
				UnityEngine.Object.DestroyImmediate( this.m_ColorBox );
			}
			if( this.m_RTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_RTexture );
			}
			if( this.m_GTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_GTexture );
			}
			if( this.m_BTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_BTexture );
			}
			if( this.m_HueTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_HueTexture );
			}
			if( this.m_SatTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_SatTexture );
			}
			if( this.m_ValTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_ValTexture );
			}
			if( this.m_AlphaTexture ) {
				UnityEngine.Object.DestroyImmediate( this.m_AlphaTexture );
			}
			ColorPicker.s_SharedColorPicker = null;
			if( this.m_IsOSColorPicker ) {
				OSColorPicker.Close();
			}
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove( EditorApplication.update, new EditorApplication.CallbackFunction( this.PollOSColorPicker ) );
			if( this.m_ColorLibraryEditorState != null ) {
				this.m_ColorLibraryEditorState.TransferEditorPrefsState( false );
			}
			this.m_ColorLibraryEditor.UnloadUsedLibraries();
			EditorPrefs.SetInt( "CPickerWidth", (int)base.position.width );
			EditorPrefs.SetInt( "CPickerHeight", (int)base.position.height );
		}
	}
}
*/