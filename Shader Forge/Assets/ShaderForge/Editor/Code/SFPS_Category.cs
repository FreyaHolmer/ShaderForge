using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPS_Category : ScriptableObject {

		public SF_Editor editor;
		public SF_PassSettings ps;

		public string labelExpanded;
		public string labelContracted;

		public bool expanded = false;

		public float targetHeight = 0f;
		public float smoothHeight = 0f;


		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public SFPS_Category Initialize( SF_Editor editor, SF_PassSettings ps, string label ) {
			this.editor = editor;
			this.ps = ps;
			this.labelExpanded = label;
			this.labelContracted = label + "...";
			return PostInitialize();
		}

		public virtual SFPS_Category PostInitialize(){
			return this; // Override
		}

		public virtual string Serialize(){
			return "";
		}

		protected string Serialize( string key, string value, bool last = false ) {
			return key + ":" + value + (last ? "" : ",");
		}

		public virtual void Deserialize(string key, string value){
		}

		public virtual void ToggleExpansion(){

		}

		public virtual void PreDraw(Rect r){

		}


		public int Draw(float yOffset){

			if(Event.current.type == EventType.Repaint){
				if(Mathf.Abs(smoothHeight-targetHeight) > 0.1f)
					smoothHeight = Mathf.Lerp(smoothHeight,targetHeight,0.5f);
				else
					smoothHeight = targetHeight;
			}

			Rect topRect = new Rect( 0f, yOffset, ps.maxWidth, 20 );
			Rect r = new Rect( topRect );

			PreDraw(r);

			if( !StartExpanderChangeCheck( r, ref expanded, labelContracted, labelExpanded ) ) {
				//ps.guiChanged = EndExpanderChangeCheck();
				//GUI.color = Color.white;
				targetHeight = 0f;
				//return (int)(topRect.yMax+smoothHeight);
			}
			Rect gRect = r;
			gRect.height = smoothHeight + 20;
			GUI.BeginGroup(gRect);
			yOffset = DrawInner(ref r );
			GUI.EndGroup();
			if(expanded)
				targetHeight = yOffset-topRect.yMax;
			if( EndExpanderChangeCheck() )
				ps.guiChanged = true;

			GUI.color = Color.white;
			return (int)(topRect.yMax+smoothHeight);
		}

		public virtual float DrawInner(ref Rect r){

			return 0f;
		}



		public bool StartExpanderChangeCheck(Rect r, ref bool foldVar, string labelContracted, string labelExpanded ) {
			
			// TOOD: COLOR RECT BEHIND
			Color prev = GUI.color;
			GUI.color = new Color(0,0,0,0);
			if( GUI.Button( r, string.Empty , EditorStyles.foldout ) ){
				Event.current.Use();
				foldVar = !foldVar;
			}
			GUI.color = prev;
			//EditorGUI.Foldout( r, foldVar, foldVar ? smoothHeight + " " + labelExpanded : smoothHeight + " " + labelContracted );
			EditorGUI.Foldout( r, foldVar, foldVar ? labelExpanded : labelContracted );
			
			EditorGUI.BeginChangeCheck();
			if( !foldVar )
				return false;
			return true;
		}
		
		public bool EndExpanderChangeCheck() {
			return EditorGUI.EndChangeCheck();
		}


		public void CheckboxEnableLine(ref bool b, ref Rect r){
			Rect rCopy = r;
			rCopy.width = r.height;
			b = GUI.Toggle(rCopy,b,string.Empty);
			GUI.enabled = b;
			r.xMin += 20;
		}
		
		public void CheckboxEnableLineEnd(ref Rect r){
			r.y += 20;
			r.xMin -= 20;
			GUI.enabled = true;
		}



	}
}