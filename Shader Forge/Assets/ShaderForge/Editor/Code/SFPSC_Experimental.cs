using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Experimental : SFPS_Category {


		public bool force2point0 = false;
		public bool forceNoShadowPass = false;
		public bool forceNoFallback = false;
		public bool forceSkipModelProjection = false;


		public override string Serialize(){
			string s = "";
			s += Serialize( "f2p0", force2point0.ToString() );
			s += Serialize( "fnsp", forceNoShadowPass.ToString() );
			s += Serialize( "fnfb", forceNoFallback.ToString() );
			s += Serialize( "fsmp", forceSkipModelProjection.ToString() );
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "f2p0":
				force2point0 = bool.Parse( value );
				break;
			case "fnsp":
				forceNoShadowPass = bool.Parse( value );
				break;
			case "fnfb":
				forceNoFallback = bool.Parse( value );
				break;
			case "fsmp":
				forceSkipModelProjection = bool.Parse( value );
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
			force2point0 = UndoableToggle( r, force2point0, "Force Shader Model 2.0", "shader model 2.0 forcing", null );
			r.y += 20;
			forceNoShadowPass = UndoableToggle( r, forceNoShadowPass, "Force no custom shadow pass", "force no custom shadow pass", null );
			r.y += 20;
			forceNoFallback = UndoableToggle( r, forceNoFallback, "Force no fallback", "force no fallback", null );
			r.y += 20;
			forceSkipModelProjection = UndoableToggle( r, forceSkipModelProjection, "Force skip model projection", "force skip model projection", null );
			r.y += 20;

			r.y += prevYpos;

			return (int)r.yMax;
		}




	}
}