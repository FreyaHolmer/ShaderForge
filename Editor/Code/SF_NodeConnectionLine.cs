using UnityEngine;
using System.Collections;


namespace ShaderForge{


	[System.Serializable]
	public class SF_NodeConnectionLine : ScriptableObject {


		public SF_NodeConnector connector;
		public SF_Editor editor;

		public bool aboutToBeDeleted = false;

		public Vector2[] pointsBezier0;
		public Vector2[] pointsBezier1;
		public Vector2[] pointsBezier2;
		public Vector2[] pointsBezier3;

		public Vector2[] pointsLinear0;
		public Vector2[] pointsLinear1;
		public Vector2[] pointsLinear2;
		public Vector2[] pointsLinear3;

		public Vector2[] pointsRectilinear0;
		public Vector2[] pointsRectilinear1;
		public Vector2[] pointsRectilinear2;
		public Vector2[] pointsRectilinear3;

		public Vector2[] this[ConnectionLineStyle style, int id]{
			get{
				switch(style){
				case ConnectionLineStyle.Bezier:
					switch(id){
					case 0:
						return pointsBezier0;
					case 1:
						return pointsBezier1;
					case 2:
						return pointsBezier2;
					case 3:
						return pointsBezier3;
					}
					break;
				case ConnectionLineStyle.Linear:
					switch(id){
					case 0:
						return pointsLinear0;
					case 1:
						return pointsLinear1;
					case 2:
						return pointsLinear2;
					case 3:
						return pointsLinear3;
					}
					break;
				case ConnectionLineStyle.Rectilinear:
					switch(id){
					case 0:
						return pointsRectilinear0;
					case 1:
						return pointsRectilinear1;
					case 2:
						return pointsRectilinear2;
					case 3:
						return pointsRectilinear3;
					}
					break;
				}
				Debug.LogError("Invalid this[style,id] attempt on NodeConnectionLink = " + style + " and " + id);
				return null;
			}
			set{
				switch(style){
				case ConnectionLineStyle.Bezier:
					switch(id){
					case 0:
						pointsBezier0 = value;
						return;
					case 1:
						pointsBezier1 = value;
						return;
					case 2:
						pointsBezier2 = value;
						return;
					case 3:
						pointsBezier3 = value;
						return;
					}
					return;
				case ConnectionLineStyle.Linear:
					switch(id){
					case 0:
						pointsLinear0 = value;
						return;
					case 1:
						pointsLinear1 = value;
						return;
					case 2:
						pointsLinear2 = value;
						return;
					case 3:
						pointsLinear3 = value;
						return;
					}
					return;
				case ConnectionLineStyle.Rectilinear:
					switch(id){
					case 0:
						pointsRectilinear0 = value;
						return;
					case 1:
						pointsRectilinear1 = value;
						return;
					case 2:
						pointsRectilinear2 = value;
						return;
					case 3:
						pointsRectilinear3 = value;
						return;
					}
					return;
				}
				Debug.LogError("Invalid this[style,id] set attempt on NodeConnectionLink = " + style + " and " + id);
				//return null;
			}
		}



		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public SF_NodeConnectionLine Initialize(SF_Editor editor, SF_NodeConnector connector){
			this.connector = connector;
			this.editor = editor;
			return this;
		}

		public bool DeleteImminent(){

			bool thisIsPendingInput = (connector.conType == ConType.cInput) && (SF_NodeConnector.pendingConnectionSource == connector);

			return aboutToBeDeleted || connector.IsDeleteHovering(false) || connector.inputCon.IsDeleteHovering(false) || thisIsPendingInput;
		}


		public void Draw(){

			if(aboutToBeDeleted && !connector.IsConnected()){ // It's disconnected, don't mark it anymore
				aboutToBeDeleted = false;
			}

			if(!connector.IsConnected())
				return;

			if(Event.current.rawType != EventType.Repaint)
				return;

			//Vector2 a = connector.GetConnectionPoint();
			//Vector2 b = connector.inputCon.GetConnectionPoint();
			int cc = connector.GetCompCount();
			bool isMatrix4x4 = ( cc == 16 );
			if( isMatrix4x4 ) {
				cc = 1;
			}

			Color color = DeleteImminent() ? new Color(1f,0f,0f,0.7f) : connector.GetConnectionLineColor();

			// TEMP:
			ReconstructShapes();
			
			//GUILines.DrawStyledConnection( editor, a, b, cc,  color);


			//switch(SF_Settings.ConnectionLineStyle){
			//case ConnectionLineStyle.Bezier:
				if( isMatrix4x4 ) {
					//GUILines.DrawMatrixConnection( editor, connector.GetConnectionPoint(), connector.inputCon.GetConnectionPoint(), color );
					GUILines.DrawLines( editor, this[ConnectionLineStyle.Bezier, 0], color, GetConnectionWidth(), true );
					GUILines.DrawLines( editor, this[ConnectionLineStyle.Bezier, 1], color, GetConnectionWidth(), true, true );
					GUILines.DrawLines( editor, this[ConnectionLineStyle.Bezier, 2], color, GetConnectionWidth(), true );
				} else {
					for( int i=0; i < cc; i++ ) {
						GUILines.DrawLines( editor, this[ConnectionLineStyle.Bezier, i], color, GetConnectionWidth(), true );
					}
				}
				
				//break;
			//}

			

		}

		const float partOffsetFactor = 10f;
		const float connectionWidth = 2f;
		const int bezierSegments = 25;


		private float GetConnectionWidth(){
			return DeleteImminent() ? 4f : connectionWidth;
		}



		//Vector2 lastUpdatePosStart = Vector2.zero;
		//Vector2 lastUpdatePosEnd = Vector2.zero;
		//float lastUpdateCC = 0;


		public void ReconstructShapes(){


	
			//if( Event.current.type != EventType.repaint ) // To trigger it before painting!
			//	return;


			float cc = connector.GetCompCount();
			if( cc == 16 )
				cc = 3;
			

			

			//if( cc != lastUpdateCC ) {
			//	lastUpdateCC = cc;
			//	needsUpdate = true;
			//}

			//Vector2 curPosStart = connector.inputCon.node.rect.position;
			//Vector2 curPosEnd = connector.node.rect.position;

			//if( lastUpdatePosStart != curPosStart ) {
			//	lastUpdatePosStart = curPosStart;
			//	needsUpdate = true;
			//}

			//if( lastUpdatePosEnd != curPosEnd ) {
			//	lastUpdatePosEnd = curPosEnd;
			//	needsUpdate = true;
			//}


			//if( needsUpdate ) {
				//Debug.Log("Updating");
				float partOffset = partOffsetFactor / cc;
				float mainOffset = -( cc - 1 ) * 0.5f * partOffset;

				float offset = 0;

				for( int i=0; i < cc; i++ ) {
					offset = mainOffset + partOffset * i;


					// TODO: Style branching
					ReconstructBezier( offset, i );


				}
			//}
			

			//Debug.Log(this[ConnectionLineStyle.Bezier,0][0].ToString());




		}



		private void ReconstructBezier(float offset, int id){
			this[ConnectionLineStyle.Bezier,id] = GUILines.ConnectionBezierOffsetArray(
				offset, 
				connector, 
				connector.inputCon, 
				bezierSegments
			);
		}

		public bool Intersects(Vector2 p0, Vector2 p1, out Vector2 intersection){
			intersection = Vector2.zero;

//			p0 = editor.nodeView.ZoomSpaceToScreenSpace(p0);
//			p1 = editor.nodeView.ZoomSpaceToScreenSpace(p1);
			p0 = editor.nodeView.ZoomSpaceToScreenSpace(p0);
			p1 = editor.nodeView.ZoomSpaceToScreenSpace(p1); // Double, for whatever reason

			float cc = connector.GetCompCount();
			if( cc == 16 || cc == 0 ) // Matrices
				cc = 1;

			if(cc == 1){
				if(SF_Tools.LineIntersection(p0, p1, this[0, 0], out intersection)){
					return true;
				}
			} else if( cc == 2){

				Vector2 intA = Vector2.zero;
				Vector2 intB = Vector2.zero;

				bool hitA = SF_Tools.LineIntersection(p0, p1, this[0, 0], out intA);
				bool hitB = SF_Tools.LineIntersection(p0, p1, this[0, 1], out intB);

				if(hitA && hitB){
					intersection = (intA + intB)/2;
					return true;
				}

			} else if(cc == 3){
				if(SF_Tools.LineIntersection(p0, p1, this[0, 1], out intersection)){
					return true;
				}
			}else if( cc == 4){
				
				Vector2 intA = Vector2.zero;
				Vector2 intB = Vector2.zero;
				
				bool hitA = SF_Tools.LineIntersection(p0, p1, this[0, 1], out intA);
				bool hitB = SF_Tools.LineIntersection(p0, p1, this[0, 2], out intB);
				
				if(hitA && hitB){
					intersection = (intA + intB)/2;
					return true;
				}
			}
		
			return false;


		}





	}

}
