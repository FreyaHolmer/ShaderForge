using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Experimental : SFPS_Category {


		public bool force2point0 = false;


		public override string Serialize(){
			string s = "";
			s += Serialize( "f2p0", force2point0.ToString());
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "f2p0":
				force2point0 = bool.Parse( value );
				break;
			}

		}

	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			
			r.xMin += 20;
			r.y += 20;
			GUI.DrawTexture(r.ClampSize(0,SF_Styles.IconWarningSmall.width),SF_Styles.IconWarningSmall);
			r.xMin += 20;
			GUI.Label(r, "Experimental features may not work");
			r.xMin -= 20;
			r.y += 20;
			force2point0 = GUI.Toggle( r, force2point0, "Force Shader Model 2.0" );
			r.y += 20;

			r.y += prevYpos;

			return (int)r.yMax;
		}




	}
}