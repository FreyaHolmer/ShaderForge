using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ShaderForge {

	public class GUILines {

		public static Color connectionColor = new Color( 1f, 1f, 1f, 0.3f );

		public static void Initialize() {

		}





		public static void DrawLine( Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias ) {
			Handles.BeginGUI();
			Handles.color = color;
			if( antiAlias )
				Handles.DrawAAPolyLine( width, new Vector3[] { pointA, pointB } );
			else
				Handles.DrawPolyLine( new Vector3[] { pointA, pointB } );
			Handles.EndGUI();
		}


		

		public static void Highlight( Rect r, float offset, int strength = 1 ) {

			//float width = 4;
			//offset = 3;
			//Color color = Color.yellow;
			r.xMax += 1;
			r = SF_Tools.GetExpanded( r, offset );
			
			/*
			Vector2 tl = new Vector2( r.x, r.y);
			Vector2 tr = new Vector2( r.xMax, r.y );
			Vector2 bl = new Vector2( r.x, r.yMax );
			Vector2 br = new Vector2( r.xMax, r.yMax );
			Vector2 dn = new Vector2( 0f, width * 3 );
			Vector2 rg = new Vector2( width * 3, 0f );
			*/
			
			//Color prevCol = GUI.color;
			//GUI.color = color;

			for( int i = 0; i < strength; i++ ) {
				GUI.Box( r, string.Empty, SF_Styles.HighlightStyle );
			}
			//GUI.color = prevCol;

			/*
			for( int i = 0; i < strength; i++ ) {
				GUI.Box( r, string.Empty, SF_Styles.HighlightStyle );
			}*/
			 

			//GUI.Box( r, string.Empty, (GUIStyle)"flow node 0 on" );
			/*
			for( int i = 0; i < strength; i++ ) {
				DrawLine( tl, tr, color, width, true );
				DrawLine( tr, br, color, width, true );
				DrawLine( br, bl, color, width, true );
				DrawLine( bl, tl, color, width, true );
				
				
				DrawLine( tl + rg, tl + dn, color, width, true );
				DrawLine( tr - rg, tr + dn, color, width, true );
				DrawLine( bl + rg, bl - dn, color, width, true );
				DrawLine( br - rg, br - dn, color, width, true );
				
			}*/

		}


		

		public static void DrawCubicBezier( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color color, float width, bool antiAlias, int segments ) {
			Initialize();
			Vector2 lastV = CubicBezier( p0, p1, p2, p3, 0 );
			for( int i = 1; i <= segments; i++ ) {
				Vector2 v = CubicBezier( p0, p1, p2, p3, i / (float)segments );
				DrawLine( lastV, v, color, width, antiAlias );
				lastV = v;
			}
		}

		public static void DrawCubicBezierOffset( float offset, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color color, float width, bool antiAlias, int segments ) {
			Initialize();
			Vector2 lastV = CubicBezierOffset( offset, p0, p1, p2, p3, 0 );
			for( int i = 1; i <= segments; i++ ) {
				Vector2 v = CubicBezierOffset( offset, p0, p1, p2, p3, i / (float)segments );
				DrawLine( lastV, v, color, width, antiAlias );
				lastV = v;
			}
		}

		private static Vector2 CubicBezierOffset( float offset, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t ) {
			Vector2 a = QuadBezier( p0, p1, p2, t );
			Vector2 b = QuadBezier( p1, p2, p3, t );
			Vector2 origin = Lerp( a, b, t );
			Vector2 tangent = ( b - a ).normalized;
			Vector2 normal = new Vector2( -tangent.y, tangent.x );
			return origin + normal * offset;
		}

		private static Vector2 CubicBezier( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t ) {
			Vector2 a = QuadBezier( p0, p1, p2, t );
			Vector2 b = QuadBezier( p1, p2, p3, t );
			return Lerp( a, b, t );
		}

		private static Vector2 QuadBezier( Vector2 p0, Vector2 p1, Vector2 p2, float t ) {
			float tsq = t * t;
			float t2 = t * 2;
			return p0 * ( tsq - t2 + 1 ) + p1 * ( t2 - 2 * tsq ) + p2 * tsq;

		}

		private static Vector2 Lerp( Vector2 v0, Vector2 v1, float t ) {
			return ( v0 * ( 1f - t ) + t * v1 );
		}









		public static void DrawStyledConnection( SF_Editor editor, Vector2 a, Vector2 b, int cc, Color col ) {
			switch( SF_Settings.ConnectionLineStyle ) {
				case ConnectionLineStyle.Bezier:
					DrawMultiBezierConnection( editor, a, b, cc, col );
					break;
				case ConnectionLineStyle.Linear:
					DrawMultiLinearConnection( editor, a, b, cc, col );
					break;
				case ConnectionLineStyle.Rectilinear:
					DrawMultiRectilinearConnection( editor, a, b, cc, col );
					break;
			}
		}

		const float partOffsetFactor = 10f;
		const float connectionWidth = 2f;

		public static void DrawMultiRectilinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, int count, Color col ) {
			float partOffset = partOffsetFactor / count;
			float mainOffset = -( count - 1 ) * 0.5f * partOffset;

			for( int i = 0; i < count; i++ ) {
				float offset = mainOffset + partOffset * i;
				DrawRectilinearConnection( editor, p0, p1, offset, col );
			}
		}

		public static void DrawMultiLinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, int count, Color col ) {
			float partOffset = partOffsetFactor / count;
			float mainOffset = -( count - 1 ) * 0.5f * partOffset;

			for( int i = 0; i < count; i++ ) {
				float offset = mainOffset + partOffset * i;
				DrawLinearConnection( editor, p0, p1, offset, col );
			}
		}


		public static void DrawMultiBezierConnection( SF_Editor editor, Vector2 p0, Vector2 p1, int count, Color col ) {
			float partOffset = partOffsetFactor / count;
			float mainOffset = -( count - 1 ) * 0.5f * partOffset;

			for( int i = 0; i < count; i++ ) {
				float offset = mainOffset + partOffset * i;
				DrawBezierConnection( editor, p0, p1, offset, col );
			}
		}

		public static void DrawRectilinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col ) {

			p0 = editor.nodeView.AddNodeWindowOffset( p0 );
			p1 = editor.nodeView.AddNodeWindowOffset( p1 );

			p0 += new Vector2( 0f, offset );
			p1 += new Vector2( 0f, offset );


			Vector2 p0t = new Vector2( ( p0.x + p1.x ) / 2f + ( p0.y < p1.y ? -offset : offset ), p0.y );
			Vector2 p1t = new Vector2( p0t.x, p1.y );


			GUILines.DrawLine( p0, p0t, col, connectionWidth, true );
			GUILines.DrawLine( p0t, p1t, col, connectionWidth, true );
			GUILines.DrawLine( p1t, p1, col, connectionWidth, true );

		}

		public static void DrawLinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col ) {
			p0 = editor.nodeView.AddNodeWindowOffset( p0 );
			p1 = editor.nodeView.AddNodeWindowOffset( p1 );

			p0 += new Vector2( 0f, offset );
			p1 += new Vector2( 0f, offset );

			GUILines.DrawLine( p0, p1, col, connectionWidth, true );
		}

		public static void DrawBezierConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col ) {

			p0 = editor.nodeView.AddNodeWindowOffset( p0 );
			p1 = editor.nodeView.AddNodeWindowOffset( p1 );

			Vector2 p0t = p0;
			Vector2 p1t = p1;

			bool reversed = p0.x > p1.x;

			float hDist = Mathf.Max( 20f, Mathf.Abs( p0.x - p1.x ) * 0.5f );

			p0t.x = p0.x + hDist;
			p1t.x = p1.x - hDist;

			int segments = 25;

			if( !reversed ) {
				if( offset == 0 )
					GUILines.DrawCubicBezier( p0, p0t, p1t, p1, col, connectionWidth, true, segments );
				else
					GUILines.DrawCubicBezierOffset( offset, p0, p0t, p1t, p1, col, connectionWidth, true, segments );
			} else {
				Vector2 mid = ( p0 + p1 ) * 0.5f;
				Vector2 mid0t = new Vector2( p0t.x, mid.y );
				Vector2 mid1t = new Vector2( p1t.x, mid.y );
				if( offset == 0 ) {
					GUILines.DrawCubicBezier( p0, p0t, mid0t, mid, col, connectionWidth, true, segments );
					GUILines.DrawCubicBezier( mid, mid1t, p1t, p1, col, connectionWidth, true, segments );
				} else {
					GUILines.DrawCubicBezierOffset( offset, p0, p0t, mid0t, mid, col, connectionWidth, true, segments );
					GUILines.DrawCubicBezierOffset( offset, mid, mid1t, p1t, p1, col, connectionWidth, true, segments );
				}

			}
		}












	}
}
