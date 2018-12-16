using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_DraggableSeparator : ScriptableObject {


		[SerializeField]
		public bool dragging = false;

		[SerializeField]
		public Rect rect;

		[SerializeField]
		public bool initialized = false;

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		[SerializeField]
		int minX;
		public int MinX {
			get {
				return minX;
			}
			set {
				minX = value;
				ClampX();
			}
		}

		[SerializeField]
		int maxX;
		public int MaxX {
			get {
				return maxX;
			}
			set {
				maxX = value;
				ClampX();
			}
		}

		public void Draw( int yPos, int height ) {

			rect.y = yPos;
			rect.height = height;
			rect.width = 7;

			GUI.Box( rect, "", EditorStyles.textField );
			Rect rHandle = new Rect( rect );
			rHandle.xMin += 0;
			rHandle.xMax -= 0;
			Rect uv = new Rect( rect );
			uv.x = 0;
			uv.y = 0;
			uv.width = 1;
			uv.height /= SF_GUI.Handle_drag.height;
			GUI.DrawTextureWithTexCoords( rHandle, SF_GUI.Handle_drag, uv );

			if( rect.Contains( Event.current.mousePosition ) || dragging ) {
				SF_GUI.AssignCursor( rect, MouseCursor.ResizeHorizontal );
			}

			if(Event.current.isMouse){

				if( SF_GUI.ReleasedRawLMB() ) {
					StopDrag();
				}
				if( dragging ) {
					UpdateDrag();
				}
				if( SF_GUI.PressedLMB( rect ) ) {
					StartDrag();
				}
			}
		}


		void ClampX(){
			rect.x = Mathf.Clamp( rect.x, minX, maxX );
		}
		int startDragOffset = 0;
		void StartDrag() {
			dragging = true;
			startDragOffset = (int)(Event.current.mousePosition.x - rect.x);
		}
		void UpdateDrag() {
			rect.x = Event.current.mousePosition.x - startDragOffset;
			ClampX();
		}
		void StopDrag() {
			dragging = false;
		}


	}
}

