using UnityEngine;
using System.Collections;

namespace ShaderForge{

	public static class SF_Extensions {







		// RECT CLASS
		//-----------

		public static Rect MovedDown(this Rect r){
			r.y += r.height;
			return r;
		}
		public static Rect MovedRight(this Rect r){
			r.x += r.width;
			return r;
		}

	}

}