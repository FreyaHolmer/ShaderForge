using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {

	
	[System.Serializable]
	public class SF_PassSettings : ScriptableObject {
		
		// Mat ed
		public SF_Editor editor;
		public SF_FeatureChecker fChecker;


		

		// SERIALIZATION OF VARS
		public string Serialize() {
			string s = "ps:";
			foreach(SFPS_Category cat in cats){
				s += cat.Serialize();
			}
			return s.Substring(0,s.Length-1);
		}


		// TODO: Remove this, keep it in cats
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
			foreach(SFPS_Category cat in cats){
				cat.Deserialize(key, value);
			}
			catBlending.UpdateAutoSort();
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

		public string n_ambientDiffuse {
			get { return mOut.ambientDiffuse.TryEvaluate(); }
		}
		public string n_ambientSpecular {
			get { return mOut.ambientSpecular.TryEvaluate(); }
		}
		public string n_diffuseOcclusion {
			get { return mOut.diffuseOcclusion.TryEvaluate(); }
		}
		public string n_specularOcclusion {
			get { return mOut.specularOcclusion.TryEvaluate(); }
		}
		public string n_customLighting {
			get { return mOut.customLighting.TryEvaluate(); }
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
			get { return editor.mainNode; }
		}


		// GUI controls
		//const int expIndent = 16;
		
		public List<SFPS_Category> cats;
		public SFPSC_Meta catMeta;
		public SFPSC_Properties catProperties;
		public SFPSC_Lighting catLighting;
		public SFPSC_Geometry catGeometry;
		public SFPSC_Blending catBlending;
		public SFPSC_Experimental catExperimental;
		public SFPSC_Console catConsole;

		// Add more here
		
		public int maxWidth;

		public SF_PassSettings() {

		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_PassSettings Initialize( SF_Editor materialEditor ) {

			this.editor = materialEditor;
			fChecker = ScriptableObject.CreateInstance<SF_FeatureChecker>().Initialize(this, materialEditor);

			cats = new List<SFPS_Category>();
			cats.Add( catMeta 			= NewCat<SFPSC_Meta>			(	"Shader Settings"	));
			cats.Add( catProperties 	= NewCat<SFPSC_Properties>		( 	"Properties" 		));
			cats.Add( catLighting 		= NewCat<SFPSC_Lighting>		(	"Lighting"			));
			cats.Add( catGeometry 		= NewCat<SFPSC_Geometry>		(	"Geometry"			));
			cats.Add( catBlending 		= NewCat<SFPSC_Blending>		(	"Blending"			));
			cats.Add( catExperimental 	= NewCat<SFPSC_Experimental>	(	"Experimental"		));
			cats.Add( catConsole        = NewCat<SFPSC_Console>			(	"Console"			));

			return this;
		}

		public T NewCat<T>(string label) where T : SFPS_Category{
			return (T)ScriptableObject.CreateInstance<T>().Initialize(this.editor, this, label);
		}
		
		
		Rect innerScrollRect = new Rect(0,0,0,0);
		Vector2 scrollPos;

		float targetScrollWidth = 0f;
		float currentScrollWidth = 0f;

		// Call this from the editor script
		public bool guiChanged = false;
		public int OnLocalGUI( int yOffset, int in_maxWidth ) {


			if(Event.current.type == EventType.Repaint)
				currentScrollWidth = Mathf.Lerp(currentScrollWidth, targetScrollWidth, 0.3f);

			this.maxWidth = in_maxWidth;
			
			Rect scrollRectPos = new Rect(0f,yOffset,in_maxWidth,Screen.height/EditorGUIUtility.pixelsPerPoint-yOffset-20);
			bool useScrollbar = (innerScrollRect.height > scrollRectPos.height);

			targetScrollWidth = useScrollbar ? 15 : 0;

			int scrollBarWidth = (int)currentScrollWidth;
			
			
			innerScrollRect.width = in_maxWidth-scrollBarWidth;

			guiChanged = false;

			int offset = 0;
			
			if(innerScrollRect.height < scrollRectPos.height)
				innerScrollRect.height = scrollRectPos.height;

			this.maxWidth -= scrollBarWidth;

			int scrollPad = scrollBarWidth-15;
			GUI.BeginGroup(scrollRectPos);
			Rect scrollWrapper = scrollRectPos;
			scrollWrapper.x = 0;
			scrollWrapper.y = 0; // Since it's grouped
			scrollPos = GUI.BeginScrollView(scrollWrapper.PadRight(scrollPad),scrollPos,innerScrollRect,false,true);
			{
				//offset = SettingsMeta( 0 );
				bool showErrors = editor.nodeView.treeStatus.Errors.Count > 0;
				if( !showErrors )
					catConsole.expanded = false;
				EditorGUI.BeginDisabledGroup( !showErrors );
				offset = catConsole.Draw( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
				EditorGUI.EndDisabledGroup();
				offset = catMeta.Draw( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = catProperties.Draw(offset);
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = catLighting.Draw(offset);
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = catGeometry.Draw( offset );
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = catBlending.Draw(offset);
				offset = GUISeparator( offset ); // ----------------------------------------------
				offset = catExperimental.Draw(offset);
				offset = GUISeparator( offset ); // ----------------------------------------------
				
			}
			GUI.EndScrollView();
			GUI.EndGroup();
			this.maxWidth += scrollBarWidth;


			if( guiChanged ) {
				editor.ps = this;
				editor.OnShaderModified(NodeUpdateType.Hard);
			}
			
			innerScrollRect.height = offset;
			return offset;

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
		


		public void UpdateAutoSettings(){
			catBlending.UpdateAutoSettings();
		}



		public int GUISeparator(int yOffset) {
			GUI.Box( new Rect(0,yOffset,maxWidth,1), "", EditorStyles.textField );
			return yOffset + 1;
		}
		
		public bool IsOutlined(){
			return mOut.outlineWidth.IsConnectedEnabledAndAvailable();
		}

		public bool UseClipping() {
			return mOut.alphaClip.IsConnectedEnabledAndAvailable();
		}
		
		public bool HasGloss(){
			return mOut.gloss.IsConnectedEnabledAndAvailable();
		}

		public bool HasNormalMap() {
			return mOut.normal.IsConnectedEnabledAndAvailable();
		}

		public bool HasTangentSpaceNormalMap() {
			return HasNormalMap() && catGeometry.normalSpace == SFPSC_Geometry.NormalSpace.Tangent;
		}
		public bool HasObjectSpaceNormalMap() {
			return HasNormalMap() && catGeometry.normalSpace == SFPSC_Geometry.NormalSpace.Object;
		}
		public bool HasWorldSpaceNormalMap() {
			return HasNormalMap() && catGeometry.normalSpace == SFPSC_Geometry.NormalSpace.World;
		}

		public bool HasRefraction() {
			return mOut.refraction.IsConnectedEnabledAndAvailable();
		}

		public bool HasTessellation() {
			return mOut.tessellation.IsConnectedEnabledAndAvailable();
		}
		public bool HasOutline() {
			return mOut.outlineWidth.IsConnectedEnabledAndAvailable();
		}

		public bool HasDisplacement() {
			return mOut.displacement.IsConnectedEnabledAndAvailable();
		}

		public bool HasEmissive() {
			return mOut.emissive.IsConnectedEnabledAndAvailable();
		}

		public bool HasDiffuse(){
			return mOut.diffuse.IsConnectedEnabledAndAvailable();
		}

		public bool HasAlpha() {
			return mOut.alpha.IsConnectedEnabledAndAvailable();
		}

		public bool HasAlphaClip() {
			return mOut.alphaClip.IsConnectedEnabledAndAvailable();
		}

		public bool HasSpecular(){
			return mOut.specular.IsConnectedEnabledAndAvailable();
		}

		public bool HasDiffusePower(){
			return mOut.diffusePower.IsConnectedEnabledAndAvailable();
		}

		public bool HasAmbientDiffuse() {
			return mOut.ambientDiffuse.IsConnectedEnabledAndAvailable();
		}

		public bool HasAmbientSpecular() {
			return mOut.ambientSpecular.IsConnectedEnabledAndAvailable();
		}

		public bool HasAmbientOcclusionDiffuse() {
			return mOut.diffuseOcclusion.IsConnectedEnabledAndAvailable();
		}
		
		public bool HasAmbientOcclusionSpecular() {
			return mOut.specularOcclusion.IsConnectedEnabledAndAvailable();
		}

		public bool HasTransmission() {
			return mOut.transmission.IsConnectedEnabledAndAvailable();
		}

		public bool HasAddedLight() {
			return HasEmissive() || catLighting.HasSpecular();
		}

		public bool HasLightWrapping() {
			return mOut.lightWrap.IsConnectedEnabledAndAvailable();
		}
	}
}

/*
		public class SF_Serializeable{

			public string key;

			public SF_Serializeable(){

			}

			public virtual string Serialize(bool last = false){
			}

			public virtual void Deserialize(string key, string value){
			}

			protected string Serialize( string key, string value, bool last = false ) {
				return key + ":" + value + (last ? "" : ",");
			}

		}

		public class SFS_Int : SF_Serializeable{

			int val;

			public SFS_Int(string key){
				this.key = key;
			}

			public override string Serialize(bool last = false){
				return Serialize(key,val.ToString(),last);
			}
			
			public override void Deserialize(string key, string value){
				if(key == this.key){
					val = int.Parse(value);
				}
			}

		}*/