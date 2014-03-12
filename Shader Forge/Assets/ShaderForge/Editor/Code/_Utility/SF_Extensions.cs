using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge{

	public enum RectBorder{
		TopLeft, 	Top, 	TopRight,
		Left, 		Center, Right,
		BottomLeft,	Bottom,	BottomRight
	}

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

		public static Rect PadHorizontal(this Rect r, int pixels ){
			return r.PadLeft(pixels).PadRight(pixels);
		}

		public static Rect PadVertical(this Rect r, int pixels ){
			return r.PadTop(pixels).PadBottom(pixels);
		}


		public static Rect ClampWidth(this Rect r, int min, int max){
			r.width = Mathf.Clamp(r.width, min, max);
			return r;
		}
		public static Rect ClampHeight(this Rect r, int min, int max){
			r.height = Mathf.Clamp(r.height, min, max);
			return r;
		}
		public static Rect ClampSize(this Rect r, int min, int max){
			return r.ClampWidth(min,max).ClampHeight(min,max);
		}
		public static Rect ClampMinSize(this Rect r, int width, int height){
			if(r.width < width)
				r.width = width;
			if(r.height < height)
				r.height = height;
			return r;
		}
		public static Rect ClampMaxSize(this Rect r, int width, int height){
			if(r.width > width)
				r.width = width;
			if(r.height > height)
				r.height = height;
			return r;
		}
		public static Rect ClampMinSize(this Rect r, int size){
			if(r.width < size)
				r.width = size;
			if(r.height < size)
				r.height = size;
			return r;
		}
		public static Rect ClampMaxSize(this Rect r, int size){
			if(r.width > size)
				r.width = size;
			if(r.height > size)
				r.height = size;
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

		public static Rect Lerp(this Rect r, Rect a, Rect b, float t ){
			r.x = Mathf.Lerp(a.x,b.x,t);
			r.y = Mathf.Lerp(a.y,b.y,t);
			r.width = Mathf.Lerp(a.width,b.width,t);
			r.height = Mathf.Lerp(a.height,b.height,t);
			return r;
		}



		public static Rect ScaleSizeBy(this Rect rect, float scale){
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint){
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;

			return result;
		}


		public static Rect GetBorder(this Rect r, RectBorder border, int size, bool showResizeCursor = false){
			Rect retRect = r;

			// Dimensions
			if(border == RectBorder.Left || border == RectBorder.Right)
				retRect.height = r.height-size*2;
			else
				retRect.height = size;

			if(border == RectBorder.Top || border == RectBorder.Bottom)
				retRect.width = r.width-size*2;
			else
				retRect.width = size;
			
			// Position
			if(border == RectBorder.Left || border == RectBorder.Center || border == RectBorder.Right)
				retRect.y += size;
			if(border == RectBorder.BottomLeft || border == RectBorder.Bottom || border == RectBorder.BottomRight)
				retRect.y += r.height-size;

			if(border == RectBorder.Top || border == RectBorder.Center || border == RectBorder.Bottom)
				retRect.x += size;
			if(border == RectBorder.TopRight || border == RectBorder.Right || border == RectBorder.BottomRight)
				retRect.x += r.width-size;


			if(showResizeCursor){

				MouseCursor cursor;

				if(border == RectBorder.Top || border == RectBorder.Bottom)
					cursor = MouseCursor.ResizeVertical;
				else if(border == RectBorder.Left || border == RectBorder.Right)
					cursor = MouseCursor.ResizeHorizontal;
				else if(border == RectBorder.TopLeft || border == RectBorder.BottomRight)
					cursor = MouseCursor.ResizeUpLeft;
				else if(border == RectBorder.BottomLeft || border == RectBorder.TopRight)
					cursor = MouseCursor.ResizeUpRight;
				else
					cursor = MouseCursor.MoveArrow;

				SF_GUI.AssignCursor(retRect,cursor);


			}

			return retRect;

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