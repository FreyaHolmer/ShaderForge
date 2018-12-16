using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge{

	public enum RectBorder{
		TopLeft, 	Top, 	TopRight,
		Left, 		Center, Right,
		BottomLeft,	Bottom,	BottomRight
	}

	public static class SF_Extensions {


		public static bool GetBit( this int value, int bit ){
			return ( ( 1 << bit ) & value ) == ( 1 << bit );
		}

		public static int SetBit( this int value, int bit, bool bitValue ) {
			if(bitValue)
				return value | ( 1 << bit );
			else
				return value & ~( 1 << bit );
		}

		public static string ToColorMaskString( this int value ) {
			// Indexed in reverse order
			// A = 0, B = 1, G = 2, R = 3
			string s = "";
			if( value.GetBit( 0 ) )
				s = "A";
			if( value.GetBit( 1 ) )
				s = "B" + s;
			if( value.GetBit( 2 ) )
				s = "G" + s;
			if( value.GetBit( 3 ) )
				s = "R" + s;
			if( s == "" )
				s = "0";
			return s;
		}


		public static float Average(this float[] floats){

			if(floats == null)
				return 0f;
			if(floats.Length == 0)
				return 0f;
			if(floats.Length == 1)
				return floats[0];

			float avg = 0f;
			for(int i=0;i<floats.Length;i++){
				avg += floats[i];
			}
			avg /= floats.Length;

			return avg;
		}

		public static string[] DisplayStrings(this FloatPrecision fp){
			return new string[]{
				"fixed (11 bit)",
				"half (16 bit)",
				"float (32 bit)"
			};
		}

		public static string ToCode(this FloatPrecision fp){
			if(fp == FloatPrecision.Fixed){
				return "fixed";
			} else if( fp == FloatPrecision.Half){
				return "half";
			} else {
				return "float";
			}
		}

		// LIST
		public static bool AddIfUnique<T>(this List<T> list, T obj){
			if(!list.Contains(obj)){
				list.Add(obj);
				return true;
			}
			return false;
		}



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

		public static Rect[] SplitHorizontal(this Rect r, float t, int padding = 0){
			return new Rect[2]{
				r.PadRight(Mathf.RoundToInt(r.width*(1f-t))).Pad(padding).PadRight(-Mathf.CeilToInt(padding/2f)),
				r.PadLeft(Mathf.RoundToInt(r.width*t)).Pad(padding).PadLeft(-Mathf.FloorToInt(padding/2f))
			};
		}
		public static Rect[] SplitVertical(this Rect r, float t, int padding = 0){
			return new Rect[2]{
				r.PadBottom(Mathf.RoundToInt(r.height*(1f-t))).Pad(padding).PadBottom(-Mathf.CeilToInt(padding/2f)),
				r.PadTop(Mathf.RoundToInt(r.height*t)).Pad(padding).PadTop(-Mathf.FloorToInt(padding/2f))
			};
		}
		public static Rect[] SplitFromLeft(this Rect r, int width, int padding = 0){
			return new Rect[2]{
				r.PadRight((int)(r.width-width)).Pad(padding).PadRight(-Mathf.CeilToInt(padding/2f)),
				r.PadLeft(width).Pad(padding).PadLeft(-Mathf.FloorToInt(padding/2f))
			};
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

		public static Rect Pad(this Rect r, int pixels){
			return r.Margin(-pixels);
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




		public static string ToCgMatrix( this Matrix4x4 mtx ) {
			string s = "{\n";
			s += "    {" + mtx[0, 0] + "," + mtx[0, 1] + "," + mtx[0, 2] + "," + mtx[0, 3] + "},\n";
			s += "    {" + mtx[1, 0] + "," + mtx[1, 1] + "," + mtx[1, 2] + "," + mtx[1, 3] + "},\n";
			s += "    {" + mtx[2, 0] + "," + mtx[2, 1] + "," + mtx[2, 2] + "," + mtx[2, 3] + "},\n";
			s += "    {" + mtx[3, 0] + "," + mtx[3, 1] + "," + mtx[3, 2] + "," + mtx[3, 3] + "}\n}";
			return s;
		}

		public static string SerializeToCSV( this Matrix4x4 mtx ) {
			string s = "";
			s += "m00:" + mtx[0, 0] + ",";
			s += "m01:" + mtx[0, 1] + ",";
			s += "m02:" + mtx[0, 2] + ",";
			s += "m03:" + mtx[0, 3] + ",";
			s += "m10:" + mtx[1, 0] + ",";
			s += "m11:" + mtx[1, 1] + ",";
			s += "m12:" + mtx[1, 2] + ",";
			s += "m13:" + mtx[1, 3] + ",";
			s += "m20:" + mtx[2, 0] + ",";
			s += "m21:" + mtx[2, 1] + ",";
			s += "m22:" + mtx[2, 2] + ",";
			s += "m23:" + mtx[2, 3] + ",";
			s += "m30:" + mtx[3, 0] + ",";
			s += "m31:" + mtx[3, 1] + ",";
			s += "m32:" + mtx[3, 2] + ",";
			s += "m33:" + mtx[3, 3];
			return s;
		}

		public static Matrix4x4 DeserializeKeyValue( this Matrix4x4 mtx, string key, string value ) {
			switch( key ) {
				case "m00":
					mtx[0, 0] = float.Parse( value );
					break;
				case "m01":
					mtx[0, 1] = float.Parse( value );
					break;
				case "m02":
					mtx[0, 2] = float.Parse( value );
					break;
				case "m03":
					mtx[0, 3] = float.Parse( value );
					break;

				case "m10":
					mtx[1, 0] = float.Parse( value );
					break;
				case "m11":
					mtx[1, 1] = float.Parse( value );
					break;
				case "m12":
					mtx[1, 2] = float.Parse( value );
					break;
				case "m13":
					mtx[1, 3] = float.Parse( value );
					break;

				case "m20":
					mtx[2, 0] = float.Parse( value );
					break;
				case "m21":
					mtx[2, 1] = float.Parse( value );
					break;
				case "m22":
					mtx[2, 2] = float.Parse( value );
					break;
				case "m23":
					mtx[2, 3] = float.Parse( value );
					break;

				case "m30":
					mtx[3, 0] = float.Parse( value );
					break;
				case "m31":
					mtx[3, 1] = float.Parse( value );
					break;
				case "m32":
					mtx[3, 2] = float.Parse( value );
					break;
				case "m33":
					mtx[3, 3] = float.Parse( value );
					break;
			}
			return mtx;
		}


		
	

		public static Vector2 xx( this Vector4 v ) {
			return new Vector2( v.x, v.x );
		}
		public static Vector2 xy( this Vector4 v ) {
			return new Vector2( v.x, v.y );
		}
		public static Vector2 xz( this Vector4 v ) {
			return new Vector2( v.x, v.z );
		}
		public static Vector2 xw( this Vector4 v ) {
			return new Vector2( v.x, v.w );
		}
		public static Vector2 yx( this Vector4 v ) {
			return new Vector2( v.y, v.x );
		}
		public static Vector2 yy( this Vector4 v ) {
			return new Vector2( v.y, v.y );
		}
		public static Vector2 yz( this Vector4 v ) {
			return new Vector2( v.y, v.z );
		}
		public static Vector2 yw( this Vector4 v ) {
			return new Vector2( v.y, v.w );
		}
		public static Vector2 zx( this Vector4 v ) {
			return new Vector2( v.z, v.x );
		}
		public static Vector2 zy( this Vector4 v ) {
			return new Vector2( v.z, v.y );
		}
		public static Vector2 zz( this Vector4 v ) {
			return new Vector2( v.z, v.z );
		}
		public static Vector2 zw( this Vector4 v ) {
			return new Vector2( v.z, v.w );
		}
		public static Vector2 wx( this Vector4 v ) {
			return new Vector2( v.w, v.x );
		}
		public static Vector2 wy( this Vector4 v ) {
			return new Vector2( v.w, v.y );
		}
		public static Vector2 wz( this Vector4 v ) {
			return new Vector2( v.w, v.z );
		}
		public static Vector2 ww( this Vector4 v ) {
			return new Vector2( v.w, v.w );
		}
		
		// Do the rest as needed
		public static Vector3 xyw( this Vector4 v ) {
			return new Vector3( v.x, v.y, v.w );
		}
		public static Vector3 yzx( this Vector4 v ) {
			return new Vector3( v.y, v.z, v.x );
		}







	}

}