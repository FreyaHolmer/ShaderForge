using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Lighting : SFPS_Category {


		public LightPrecision lightPrecision = LightPrecision.Fragment;
		public NormalQuality normalQuality = NormalQuality.Normalized;
		public LightMode lightMode = LightMode.BlinnPhong;
		public LightCount lightCount = LightCount.Multi;

		public bool useAmbient = true;
		public bool maskedSpec = true;
		public bool doubleIncomingLight = false;
		//public bool shadowCast = true;
		//public bool shadowReceive = true;
		public bool lightmapped = false;
		public bool lightprobed = false;
		public bool energyConserving = false;
		public bool remapGlossExponentially = true;
		// Optional PBL terms
		public bool fresnelTerm = true;
		public bool visibilityTerm = true;

		public enum LightPrecision { Vertex, Fragment };
		public string[] strLightPrecision = new string[] { "Per-Vertex", "Per-Fragment" };
		public enum NormalQuality { Interpolated, Normalized };
		public string[] strNormalQuality = new string[] { "Interpolated", "Normalized" };
		public enum LightMode { Unlit, BlinnPhong, Phong, PBL };
		public string[] strLightMode = new string[] { "Unlit/Custom", "Blinn-Phong", "Phong", "PBL" };
		public enum LightCount { Single, Multi };
		public string[] strLightCount = new string[] { "Single Directional", "Multi-light"};


		public override string Serialize(){
			string s = "";
			s += Serialize( "lico", ( (int)lightCount ).ToString() );
			s += Serialize( "lgpr", ( (int)lightPrecision ).ToString() );
			s += Serialize( "nrmq", ( (int)normalQuality ).ToString() );
			s += Serialize( "limd", ( (int)lightMode ).ToString() );
			s += Serialize( "uamb", useAmbient.ToString() );
			s += Serialize( "mssp", maskedSpec.ToString() );
			s += Serialize( "lmpd", lightmapped.ToString() );
			s += Serialize( "lprd", lightprobed.ToString() );
			s += Serialize( "enco", energyConserving.ToString());
			s += Serialize( "frtr", fresnelTerm.ToString() );
			s += Serialize( "vitr", visibilityTerm.ToString() );
			s += Serialize( "dbil", doubleIncomingLight.ToString() );
			s += Serialize( "rmgx", remapGlossExponentially.ToString());
			//s += Serialize( "shdc", shadowCast.ToString() );
			//s += Serialize( "shdr", shadowReceive.ToString() );
			return s;
		}

		public override void Deserialize(string key, string value){


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
			case "uamb":
				useAmbient = bool.Parse( value );
				break;
			case "mssp":
				maskedSpec = bool.Parse( value );
				break;
			/*case "shdc":
				shadowCast = bool.Parse( value );
				break;
			case "shdr":
				shadowReceive = bool.Parse( value );
				break;*/
			case "dbil":
				doubleIncomingLight = bool.Parse( value );
				break;
			case "lico":
				lightCount = (LightCount)int.Parse( value );
				break;
			case "frtr":
				fresnelTerm = bool.Parse( value );
				break;
			case "vitr":
				visibilityTerm = bool.Parse( value );
				break;
			case "lmpd":
				lightmapped = bool.Parse( value );
				break;
			case "lprd":
				lightprobed = bool.Parse( value );
				break;
			case "enco":
				energyConserving = bool.Parse( value );
				break;
				
				
			case "rmgx":
				remapGlossExponentially = bool.Parse( value );
				break;
			}

		}

	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			
			r.xMin += 20;
			r.y += 20;
			lightMode = (LightMode)SF_GUI.ContentScaledToolbar( r, "Light Mode", (int)lightMode, strLightMode );
			r.y += 20;

			
			SF_GUI.ConditionalToggle(r, ref doubleIncomingLight,
			                         usableIf: 				ps.catLighting.IsLit(),
			                         disabledDisplayValue: 	false,
			                         label: 					"Double incoming light"
			                         );
			r.y += 20;
			
			SF_GUI.ConditionalToggle(r, ref remapGlossExponentially,
			                         usableIf: 				ps.HasGloss(),
			                         disabledDisplayValue: 	false,
			                         label: 					"Remap gloss from [0-1] to [1-2048]"
			                         );
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
			
			
			
			lightCount = (LightCount)SF_GUI.ContentScaledToolbar(r, "Light Count", (int)lightCount, strLightCount );
			r.y += 20;
			
			
			//lightPrecision = (LightPrecision)ContentScaledToolbar(r, "Light Quality", (int)lightPrecision, strLightPrecision ); // TODO: Too unstable for release
			//r.y += 20;	
			
			normalQuality = (NormalQuality)SF_GUI.ContentScaledToolbar(r, "Normal Quality", (int)normalQuality, strNormalQuality );
			r.y += 20;
			
			SF_GUI.ConditionalToggle(r, ref lightmapped,
			                         usableIf: 				ps.HasDiffuse() && lightMode != LightMode.Unlit,
			                         disabledDisplayValue: 	false,
			                         label: 					"Lightmap support" 
			                         );
			r.y += 20;

			//lightprobed = GUI.Toggle( r, lightprobed, "Light probe support" );
			SF_GUI.ConditionalToggle(r, ref lightprobed,
			                         usableIf: 				ps.HasDiffuse() && lightMode != LightMode.Unlit,
			                         disabledDisplayValue: 	false,
			                         label: 					"Light probe support"
			                         );
			r.y += 20;
			
			
			/*shadowCast = GUI.Toggle( r, shadowCast, "Cast shadows" );
			r.y += 20;
			shadowReceive = GUI.Toggle( r, shadowReceive, "Receive shadows" );
			r.y += 20;*/
			
			
			
			
			//GUI.enabled = IsLit();
			
			
			
			SF_GUI.ConditionalToggle(r, ref useAmbient,
			                         usableIf: 				!lightprobed && ps.catLighting.IsLit(),
			                         disabledDisplayValue: 	lightprobed,
			                         label: 					"Receive Ambient Light"
			                         );
			r.y += 20;
			
			/*
			if(lightprobed){
				GUI.enabled = false;
				GUI.Toggle( r, true, "Receive Ambient Light" );
				GUI.enabled = true;
			}else{
				useAmbient = GUI.Toggle( r, useAmbient, "Receive Ambient Light" );
			}*/
			
			
			//r.y += 20;
			if(ps.catLighting.HasSpecular()){
				maskedSpec = GUI.Toggle( r, maskedSpec, "Mask directional light specular by shadows" );
			} else {
				GUI.enabled = false;
				GUI.Toggle( r, false, "Mask directional light specular by shadows" );
				GUI.enabled = true;
			}
			r.y += 20;
		
			r.y += prevYpos;

			return (int)r.yMax;
		}







		public bool UseMultipleLights() {
			return lightCount == LightCount.Multi;
		}

		public bool IsVertexLit() {
			return ( IsLit() && ( lightPrecision == LightPrecision.Vertex ) );
		}
		
		public bool IsFragmentLit() {
			return ( IsLit() && ( lightPrecision == LightPrecision.Fragment ) );
		}

		public bool IsLit() {
			return ( lightMode != LightMode.Unlit && ( ps.HasDiffuse() || HasSpecular()) );
		}
		
		public bool IsEnergyConserving() {
			return IsLit() && (energyConserving || lightMode == LightMode.PBL);
		}
		
		public bool IsPBL() {
			return lightMode == LightMode.PBL;
		}
		
		public bool HasSpecular() {
			return ( lightMode == LightMode.BlinnPhong || lightMode == LightMode.Phong || lightMode == LightMode.PBL ) && ( ps.mOut.specular.IsConnectedAndEnabled() );
		}





	



	}
}