using UnityEngine;
using System.Collections;

namespace ShaderForge{

	public static class SF_Extensions {







		// RECT CLASS
		//-----------

		public static Rect MovedDown(this Rect r, 	int count = 1){
			for (int i = 0; i < count; i++) {
				r.y += r.height;
			}
			return r;
		}
		public static Rect MovedUp(this Rect r, 	int count = 1){
			for (int i = 0; i < count; i++) {
				r.y -= r.height;
			}
			return r;
		}
		public static Rect MovedRight(this Rect r, 	int count = 1){
			for (int i = 0; i < count; i++) {
				r.x += r.width;
			}
			return r;
		}
		public static Rect MovedLeft(this Rect r, 	int count = 1){
			for (int i = 0; i < count; i++) {
				r.x -= r.width;
			}
			return r;
		}


		public static Rect PadBottom(this Rect r, int pixels ){
			r.yMax -= pixels;
			return r;
		}
		
		public static Rect PadTop(this Rect r, int pixels ){
			r.yMin += pixels;
			return r;
		}


		public static Rect PadRight(this Rect r, int pixels ){
			r.xMax -= pixels;
			return r;
		}

		public static Rect PadLeft(this Rect r, int pixels ){
			r.xMin += pixels;
			return r;
		}


		public static Rect ClampWidth(this Rect r, int min, int max){
			r.width = Mathf.Clamp(r.width, min, max);
			return r;
		}

	}

}