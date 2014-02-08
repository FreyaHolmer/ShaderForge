using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Quality : SFPS_Category {


		public bool highQualityScreenCoords = true;
		public bool highQualityLightProbes = false;


		public override string Serialize(){
			string s = "";
			s += Serialize( "hqsc", highQualityScreenCoords.ToString());
			s += Serialize( "hqlp", highQualityLightProbes.ToString());
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "hqsc":
				highQualityScreenCoords = bool.Parse( value );
				break;
			case "hqlp":
				highQualityLightProbes = bool.Parse( value );
				break;

			}

		}

	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			
			r.xMin += 20;
			r.y += 20;

			highQualityScreenCoords = GUI.Toggle( r, highQualityScreenCoords, "Per-pixel screen coordinates" );
			r.y += 20;
			highQualityLightProbes = GUI.Toggle( r, highQualityLightProbes, "Per-pixel light probe sampling" );
			r.y += 20;

			r.y += prevYpos;

			return (int)r.yMax;
		}




	}
}