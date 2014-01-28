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

		public static Vector2 TopLeft(this Rect r){
			return new Vector2(r.x, r.y);
		}

		public static Vector2 TopRight(this Rect r){
			return new Vector2(r.xMax, r.y);
		}

		public static Vector2 BottomRight(this Rect r){
			return new Vector2(r.xMax, r.yMax);
		}

		public static Vector2 BottomLeft(this Rect r){
			return new Vector2(r.x, r.yMax);
		}


		public static Rect Margin(this Rect r, int pixels){
			r.xMax += pixels;
			r.xMin -= pixels;
			r.yMax += pixels;
			r.yMin -= pixels;
			return r;
		}










		public static float ManhattanDistanceToPoint(this Rect r, Vector2 point){

			if(r.Contains(point)){
				return 0f;
			}

			Vector2 clampedPoint = new Vector2(
				Mathf.Clamp(point.x, r.xMin, r.xMax),
				Mathf.Clamp(point.y, r.yMin, r.yMax)
			);

			return ChebyshevDistance(clampedPoint, point);


		}

		public static float ChebyshevDistance(Vector2 a, Vector2 b){
			return Mathf.Max(Mathf.Abs(a.x-b.x),Mathf.Abs(a.y-b.y));
		}

		public static float ManhattanDistance(Vector2 a, Vector2 b){
			return Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
		}


		public static float ShortestManhattanDistanceToRects(this Vector2 point, Rect[] rects){

			float shortest = float.MaxValue;

			for (int i = 0; i < rects.Length; i++) {
				shortest = Mathf.Min (shortest, rects[i].ManhattanDistanceToPoint(point));
			}

			return shortest;

		}

		
		public static float ShortestChebyshevDistanceToPoints(this Vector2 point, Vector2[] points){
			
			float shortest = float.MaxValue;
			
			for (int i = 0; i < points.Length; i++) {
				shortest = Mathf.Min (shortest, ManhattanDistance(point, points[i]) );
			}
			
			return shortest;
			
			
		}



















	}

}