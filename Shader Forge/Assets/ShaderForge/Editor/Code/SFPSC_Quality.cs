using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Quality : SFPS_Category {


		public bool highQualityScreenCoords = true;
		public bool highQualityLightProbes = false;
		//public bool edgeLengthTessellation = false;

		public enum TessellationMode{Regular, EdgeLength/*, EdgeLengthCulled*/};
		public string[] tessModeStr = new string[]{"Regular", "Edge length based"/*, "Edge length based with frustrum culling"*/};


		public TessellationMode tessellationMode = TessellationMode.Regular;


		public override string Serialize(){
			string s = "";
			s += Serialize( "hqsc", highQualityScreenCoords.ToString());
			s += Serialize( "hqlp", highQualityLightProbes.ToString());
			s += Serialize( "tesm", ((int)tessellationMode).ToString());
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
			case "tesm":
				tessellationMode = (TessellationMode)int.Parse( value );
				break;
			}

		}

	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			
			r.xMin += 20;
			r.y += 20;


			highQualityScreenCoords = UndoableToggle( r, highQualityScreenCoords, "Per-pixel screen coordinates", "per-pixel screen coordinates", null );
			r.y += 20;

			highQualityLightProbes = UndoableToggle( r, highQualityLightProbes, "Per-pixel light probe sampling", "per-pixel light probe sampling", null );
			r.y += 20;

			GUI.enabled = ps.HasTessellation();
			tessellationMode = (TessellationMode)UndoableLabeledEnumPopupNamed( r, "Tessellation Mode",tessellationMode, tessModeStr, "tessellation mode" );
			GUI.enabled = true;
			r.y += 20;

			r.y += prevYpos;

			return (int)r.yMax;
		}




	}
}