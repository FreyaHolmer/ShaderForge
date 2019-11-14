using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace ShaderForge {

	[System.Serializable]
	public class SF_Node_Resizeable : SF_Node {


		public int minWidth = NODE_WIDTH;
		public int minHeight = NODE_HEIGHT;

		/*
		public void Deselect() {
			editor.nodeView.selection.Remove( this );
			selected = false;
		}

		public void DrawHighlight() {
			
			//if( Event.current.type == EventType.repaint )
			if( selected ) {

				Rect r = new Rect( rect );
				r.xMax -= 1;
				if( IsProperty() )
					r.yMin -= 20;
				GUILines.Highlight( r, offset: 1, strength: 2 );
			}
		}*/



		public override bool Draw() {


			ProcessInput();


			DrawHighlight();


			

			PrepareWindowColor();

			if( showLowerPropertyBox )
				if( showLowerPropertyBoxAlways || ( showLowerPropertyBox && CanEvaluate() && IsUniformOutput() ) ) {
					rect.height = ( node_height + 20 );
				} else {
					rect.height = node_height;
				}


			DrawWindow();

			ResetWindowColor();

			return true;
		}


		bool resizing = false;
		int xDrag = 0;
		int yDrag = 0;

		public override void NeatWindow( ) {
			GUI.BeginGroup( rect );
			GUI.color = Color.white;
			GUI.skin.box.clipping = TextClipping.Overflow;





			// Resize handle
			int size = 10;




			Rect topLeft = LocalRect().GetBorder(RectBorder.TopLeft,size);
			//Rect lowerRight = LocalRect().GetBorder(RectBorder.BottomRight,size);
			Rect topRight = LocalRect().GetBorder(RectBorder.TopRight,size);

			Rect left = LocalRect().GetBorder(RectBorder.Left,size);
			//Rect lowerRight = LocalRect().GetBorder(RectBorder.Center,size);
			Rect right = LocalRect().GetBorder(RectBorder.Right,size);


			Rect lowerLeft = LocalRect().GetBorder(RectBorder.BottomLeft,size); //new Rect(rect.width - size, rect.height-size,size,size);
			Rect lower = LocalRect().GetBorder(RectBorder.Bottom,size);
			Rect lowerRight = LocalRect().GetBorder(RectBorder.BottomRight,size);


			/*
			if(!resizing)
				SF_GUI.AssignCursor(lowerRight,MouseCursor.ResizeUpLeft);
			else
				SF_GUI.AssignCursor(new Rect(0,0,Screen.width,Screen.height),MouseCursor.ResizeUpLeft);*/



			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.TopLeft		,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			//SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.Top			,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.TopRight		,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.Left			,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.Right			,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.BottomLeft		,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.Bottom			,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );
			SF_GUI.DrawTextureTiled(LocalRect().GetBorder(RectBorder.BottomRight	,size, 	showResizeCursor:true), SF_GUI.Handle_drag, local:true );


			// -1 = left / top
			//  0 = static
			//  1 = right / bottom



			bool clicked = Event.current.type == EventType.MouseDown && Event.current.button == 0;




			if(clicked){

				xDrag = 0;
				yDrag = 0;
				Vector3 mPos = Event.current.mousePosition;
				/*
				bool[,] dragGrid = new bool[3,3]{
					{topLeft.Contains(mPos),		false,				topRight.Contains(mPos)},
					{left.Contains(mPos),		false,					right.Contains(mPos)},
					{lowerLeft.Contains(mPos),	lower.Contains(mPos),	lowerRight.Contains(mPos)}
				};*/

				bool[,] dragGrid = new bool[3,3]{
					{topLeft.Contains(mPos),left.Contains(mPos),lowerLeft.Contains(mPos)},
					{false,false,lower.Contains(mPos)},
					{topRight.Contains(mPos),right.Contains(mPos),lowerRight.Contains(mPos)}
				};



				bool leftSide = dragGrid[0,0] || dragGrid[0,1] || dragGrid[0,2];
				bool rightSide = dragGrid[2,0] || dragGrid[2,1] || dragGrid[2,2];
				bool topSide = dragGrid[0,0] || dragGrid[1,0] || dragGrid[2,0];
				bool bottomSide = dragGrid[0,2] || dragGrid[1,2] || dragGrid[2,2];


				if(leftSide)
					xDrag = -1;
				else if(rightSide)
					xDrag = 1;

				if(topSide)
					yDrag = -1;
				else if(bottomSide)
					yDrag = 1;


				bool contained = xDrag != 0 || yDrag != 0;


				if( contained ){
					resizing = true;
					Event.current.Use();
				}

			}




			if(resizing && Event.current.type == EventType.MouseDrag){

				if(Event.current.delta.sqrMagnitude > 0){
					UndoRecord("resize node");
				}

				if(xDrag == 1)
					rect.width += Event.current.delta.x;
				else if(xDrag == -1)
					rect.xMin += Event.current.delta.x;

				if(yDrag == 1)
					rect.height += Event.current.delta.y;
				if(yDrag == -1)
					rect.yMin += Event.current.delta.y;

				//Debug.Log("RESIZING X " + xDrag + " Y " + yDrag);

				ClampSize();

				Event.current.Use();
			}

			if(resizing && SF_GUI.ReleasedRawLMB()){
				resizing = false;
				xDrag = 0;
				yDrag = 0;
				if(base.isDragging)
					base.OnRelease();
				Event.current.Use();
			}

			Rect insideHandleRect = LocalRect().PadLeft(size).PadRight(size).PadBottom(size).PadTop(Mathf.Max(15,size));
			DrawInner(insideHandleRect);

			/*
			if( showColor ) {

				texture.Draw( rectInner );

				if( SF_Debug.nodes ) {
					Rect r = new Rect( 0, 16, 96, 20 );
					GUI.color = Color.white;
					GUI.skin.box.normal.textColor = Color.white;
					GUI.Box( r, "ID: " + id );
					r.y += r.height;
					//GUI.Box( r, "Cmps: " + texture.CompCount );
					//r.y += r.height;
					//GUI.Box( r, "Unif: " + texture.dataUniform );

				}


			}*/

			if( showLowerPropertyBox ) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}

			//GUI.DragWindow();


			GUI.EndGroup( );
			

			//if(rect.center.x)
		}


		public void ClampSize(){
			rect = rect.ClampMinSize(minWidth, minHeight);
		}


		public virtual void DrawInner(Rect r){
			// Override
		}




	}

}