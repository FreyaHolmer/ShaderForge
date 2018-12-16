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

		public static void DrawDisc( Vector2 center, float radius, Color color) {
			Handles.BeginGUI();
			Handles.color = color;
			Handles.DrawWireDisc(center,Vector3.forward,radius);
			Handles.EndGUI();
		}





		public static void DrawLines( SF_Editor editor, Vector2[] points, Color color, float width, bool antiAlias, bool railway = false ) {
			Handles.BeginGUI();
			Handles.color = color;
		

			Vector3[] v3Pts = new Vector3[points.Length];
			for (int i = 0; i < points.Length; i++) {
				points[i] = editor.nodeView.ZoomSpaceToScreenSpace( points[i] );
				v3Pts[i] = new Vector3(points[i].x, points[i].y);
			}

			if( antiAlias ){
				if( railway ) {
					DrawPolyLineWithRail( width, v3Pts );
				} else {
					Handles.DrawAAPolyLine( width, v3Pts );
				}
			} else {
				Handles.DrawPolyLine( v3Pts );
			}
			Handles.EndGUI();
		}


		static void DrawPolyLineWithRail( float width, Vector3[] v3pts ) {

			Vector3[] pair = new Vector3[] { Vector3.zero, Vector3.zero };
			for( int i = 0; i < v3pts.Length - 1; i++ ) {

				Vector3 dir = (v3pts[i] - v3pts[i+1] ).normalized;
				dir = new Vector3(-dir.y, dir.x);
				Vector3 center = (v3pts[i] + v3pts[i+1] )*0.5f;
				pair[0] = center + dir * 3;
				pair[1] = center - dir * 3;
				Handles.DrawAAPolyLine( pair );
			}

			Handles.DrawAAPolyLine( width, v3pts );
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


		

		public static void DrawCubicBezier( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color color, float width, bool antiAlias, int segments, bool railway = false ) {
			Initialize();
			Vector2 lastV = CubicBezier( p0, p1, p2, p3, 0 );
			for( int i = 1; i <= segments; i++ ) {
				Vector2 v = CubicBezier( p0, p1, p2, p3, i / (float)segments );

				if( railway ) {
					Vector2 dir = ( lastV - v ).normalized;
					dir = new Vector2(-dir.y, dir.x)*2;
					Vector2 center = ( v + lastV ) * 0.5f;
					DrawLine( center + dir, center - dir, color, width, antiAlias );
				} else {
					DrawLine( lastV, v, color, width, antiAlias );
				}

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




		public static Vector2[] ConnectionBezierOffsetArray(float offset, SF_NodeConnector startCon, SF_NodeConnector endCon, int segments){


			Vector2 start = startCon.GetConnectionPoint();
			Vector2 end = endCon.GetConnectionPoint();

			bool reversed = (start.x < end.x);

			Vector2[] points;

			int pCount = (segments+1); // Point count per bezier

			if(reversed)
				points = new Vector2[pCount*2]; // Two curves
			else
				points = new Vector2[pCount];



			if(reversed){

				// Calculate new start/end positions!
				// We want an S shape, which essentially is two curves with a connected center
				// Let's define the new points!


				float midVert;

				if(startCon.node.rect.center.y > endCon.node.rect.center.y)
					midVert = (startCon.node.BoundsTop() + endCon.node.BoundsBottom())/2;
				else
					midVert = (startCon.node.BoundsBottom() + endCon.node.BoundsTop())/2;



				float deltaX = Mathf.Abs(start.x-end.x);
				float mul = Mathf.InverseLerp(0f,100f,deltaX);
				mul = SF_Tools.Smoother(mul) * 0.70710678118f;


				Vector2 bAp0 = start;						// Start Point
				Vector2 bAp3 = new Vector2(start.x, midVert); // End Point



				float tangentMag = Mathf.Abs(bAp0.y-bAp3.y)*mul; // TODO: Scale based on length if smaller than something
				Vector2 tangentVec = new Vector2(tangentMag, 0f);


				Vector2 bAp1 = bAp0 - tangentVec; 			// Start Tangent
				Vector2 bAp2 = bAp3 - tangentVec; 			// End Tangent


				for(int i=0;i<pCount;i++){
					float t = (float)i/(float)segments;
					points[i] = CubicBezierOffset(offset, bAp0, bAp1, bAp2, bAp3, t);
				}

				// Second line! Let's go

				Vector2 bBp0 = new Vector2(end.x, midVert);	// Start Point
				Vector2 bBp3 = end; 						// End Point

				tangentMag = Mathf.Abs(bBp0.y-bBp3.y)*mul; // TODO: Scale based on length if smaler than something
				tangentVec = new Vector2(tangentMag, 0f);

				Vector2 bBp1 = bBp0 + tangentVec; 			// Start Tangent
				Vector2 bBp2 = bBp3 + tangentVec; 			// End Tangent

				for(int i=0;i<pCount;i++){
					float t = (float)i/(float)segments;
					points[i+pCount] = CubicBezierOffset(offset, bBp0, bBp1, bBp2, bBp3, t);
				}



			} else {
				for(int i=0;i<pCount;i++){
					float t = (float)i/(float)segments;
					points[i] = ConnectionBezierOffset(offset, start, end, t);
				}
			}





			return points;
		}




		public static Vector2 ConnectionBezierOffset( float offset, Vector2 start, Vector2 end, float t){


			float mult = (start.x > end.x) ? 1f : 4f;

			float xHalfway = Mathf.Abs(end.x-start.x)*0.5f * mult;

			Vector2 p1 = new Vector2(start.x-xHalfway, start.y);
			Vector2 p2 = new Vector2(end.x+xHalfway, end.y);
			return CubicBezierOffset(offset, start, p1, p2, end, t);
		}

		public static Vector2 CubicBezierOffset( float offset, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t ) {
			Vector2 a = QuadBezier( p0, p1, p2, t );
			Vector2 b = QuadBezier( p1, p2, p3, t );
			Vector2 origin = Lerp( a, b, t );
			Vector2 tangent = ( b - a ).normalized;
			Vector2 normal = new Vector2( -tangent.y, tangent.x );
			return origin + normal * offset;
		}

		public static Vector2 CubicBezier( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t ) {
			float omt = 1f - t;
			float omt2 = omt * omt;
			float t2 = t * t;
			return p0 * ( omt2 * omt ) +
					p1 * ( 3f * omt2 * t ) +
					p2 * ( 3f * omt * t2 ) +
					p3 * ( t2 * t );
		}

		public static Vector2 QuadBezier( Vector2 p0, Vector2 p1, Vector2 p2, float t ) {
			float tsq = t * t;
			float t2 = t * 2;
			return p0 * ( tsq - t2 + 1 ) + p1 * ( t2 - 2 * tsq ) + p2 * tsq;

		}

		public static Vector2 Lerp( Vector2 v0, Vector2 v1, float t ) {
			return ( v0 * ( 1f - t ) + t * v1 );
		}




		public static void QuickBezier(Vector2 p0, Vector2 p1, Color color, int detail = 12, int width = 2){

			Vector2 prevPoint = p0;
			for(float i=1;i<detail-1;i++){
				float t = i/(detail-2);

				Vector2 nextPoint = new Vector2(
					Mathf.Lerp( p0.x, p1.x, t),
					Mathf.Lerp( p0.y, p1.y, SF_Tools.Smooth(t))
				);

				DrawLine(prevPoint, nextPoint, color, width, true);

				prevPoint = nextPoint;

			}

		}



		public static void DrawMatrixConnection( SF_Editor editor, Vector2 a, Vector2 b, Color col ) {
			DrawMultiBezierConnection( editor, a, b, 2, col, true );
		}


		public static void DrawStyledConnection( SF_Editor editor, Vector2 a, Vector2 b, int cc, Color col ) {
			//switch( SF_Settings.ConnectionLineStyle ) {
				//case ConnectionLineStyle.Bezier:
					DrawMultiBezierConnection( editor, a, b, cc, col );
				//	break;
				//case ConnectionLineStyle.Linear:
				//	DrawMultiLinearConnection( editor, a, b, cc, col );
				//	break;
				//case ConnectionLineStyle.Rectilinear:
				//	DrawMultiRectilinearConnection( editor, a, b, cc, col );
				//	break;
			//}
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


		public static void DrawMultiBezierConnection( SF_Editor editor, Vector2 p0, Vector2 p1, int count, Color col, bool railway = false ) {
			float partOffset = partOffsetFactor / count;
			float mainOffset = -( count - 1 ) * 0.5f * partOffset;

			for( int i = 0; i < count; i++ ) {
				float offset = mainOffset + partOffset * i;
				DrawBezierConnection( editor, p0, p1, offset, col );
			}

			if( railway ) {
				DrawBezierConnection( editor, p0, p1, 0, col, true );
			}
		}

		public static void DrawRectilinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col ) {

			p0 = editor.nodeView.ZoomSpaceToScreenSpace( p0 );
			p1 = editor.nodeView.ZoomSpaceToScreenSpace( p1 );

			p0 += new Vector2( 0f, offset );
			p1 += new Vector2( 0f, offset );


			Vector2 p0t = new Vector2( ( p0.x + p1.x ) / 2f + ( p0.y < p1.y ? -offset : offset ), p0.y );
			Vector2 p1t = new Vector2( p0t.x, p1.y );


			GUILines.DrawLine( p0, p0t, col, connectionWidth, true );
			GUILines.DrawLine( p0t, p1t, col, connectionWidth, true );
			GUILines.DrawLine( p1t, p1, col, connectionWidth, true );

		}

		public static void DrawLinearConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col ) {
			p0 = editor.nodeView.ZoomSpaceToScreenSpace( p0 );
			p1 = editor.nodeView.ZoomSpaceToScreenSpace( p1 );

			p0 += new Vector2( 0f, offset );
			p1 += new Vector2( 0f, offset );

			GUILines.DrawLine( p0, p1, col, connectionWidth, true );
		}

		public static void DrawDashedLine(SF_Editor editor, Vector2 p0, Vector2 p1, Color col, float dashLength ){
//			p0 = editor.nodeView.ZoomSpaceToScreenSpace( p0 );
//			p1 = editor.nodeView.ZoomSpaceToScreenSpace( p1 );

			float frac = dashLength/(p0-p1).magnitude;

			//int segcount = Mathf.Max(1, Mathf.RoundToInt(1f/frac));

			for(float t = 0; t < 1; t += frac*2f){

				float tNext = Mathf.Min(1f,t + frac);

				GUILines.DrawLine( Vector2.Lerp (p0,p1,t), Vector2.Lerp (p0,p1,tNext), col, connectionWidth, true );
			}


		}

		public static void DrawBezierConnection( SF_Editor editor, Vector2 p0, Vector2 p1, float offset, Color col, bool railway = false) {

			p0 = editor.nodeView.ZoomSpaceToScreenSpace( p0 );
			p1 = editor.nodeView.ZoomSpaceToScreenSpace( p1 );

			Vector2 p0t = p0;
			Vector2 p1t = p1;

			bool reversed = p0.x < p1.x;

			float hDist = Mathf.Max( 20f, Mathf.Abs( p0.x - p1.x ) * 0.5f );

			p0t.x = p0.x - hDist;
			p1t.x = p1.x + hDist;

			int segments = 25;

			if( !reversed ) {
				if( offset == 0 )
					GUILines.DrawCubicBezier( p0, p0t, p1t, p1, col, connectionWidth, true, segments, railway );
				else
					GUILines.DrawCubicBezierOffset( offset, p0, p0t, p1t, p1, col, connectionWidth, true, segments );
			} else {
				Vector2 mid = ( p0 + p1 ) * 0.5f;
				Vector2 mid0t = new Vector2( p0t.x, mid.y );
				Vector2 mid1t = new Vector2( p1t.x, mid.y );
				if( offset == 0 ) {
					GUILines.DrawCubicBezier( p0, p0t, mid0t, mid, col, connectionWidth, true, segments, railway );
					GUILines.DrawCubicBezier( mid, mid1t, p1t, p1, col, connectionWidth, true, segments, railway );
				} else {
					GUILines.DrawCubicBezierOffset( offset, p0, p0t, mid0t, mid, col, connectionWidth, true, segments );
					GUILines.DrawCubicBezierOffset( offset, mid, mid1t, p1t, p1, col, connectionWidth, true, segments );
				}
			}
		}












	}
}
