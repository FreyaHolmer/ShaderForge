using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

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

		public string GetLabelString() {
			return expanded ? labelExpanded : labelContracted;
		}

		public virtual void DrawExtraTitleContent( Rect r ) {
			// Override. Currently only used by Console
		}



		public bool StartExpanderChangeCheck(Rect r, ref bool foldVar, string labelContracted, string labelExpanded ) {
			
			// TOOD: COLOR RECT BEHIND
			Color prev = GUI.color;
			GUI.color = new Color(0,0,0,0);
			if( GUI.Button( r, string.Empty , EditorStyles.foldout ) ){
				Event.current.Use();
				Undo.RecordObject(this, foldVar ? "collapse " + labelExpanded : "expand " + labelExpanded);
				foldVar = !foldVar;
			}
			GUI.color = prev;
			//EditorGUI.Foldout( r, foldVar, foldVar ? smoothHeight + " " + labelExpanded : smoothHeight + " " + labelContracted );
			EditorGUI.Foldout( r, foldVar, GetLabelString() );
			DrawExtraTitleContent( r );
			
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















		public int UndoableContentScaledToolbar(Rect r, string label, int selected, string[] labels, string undoInfix){
			int newValue = SF_GUI.ContentScaledToolbar( r, label, selected, labels );
			if(newValue != selected){
				string undoName = "set " + undoInfix + " to " + labels[newValue];
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return selected;
		}


		public void UndoableConditionalToggle(Rect r, ref bool value, bool usableIf, bool disabledDisplayValue, string label, string undoSuffix){
			bool nextValue = value;
			SF_GUI.ConditionalToggle(r,ref nextValue, usableIf,disabledDisplayValue,label);
			if(nextValue != value){	
				string undoName = (nextValue ? "enable" : "disable") + " " + undoSuffix;
				Undo.RecordObject(this,undoName);
				value = nextValue;
			}
		}


		public bool UndoableToggle(Rect r, bool boolVar, string label, string undoActionName, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.toggle;
			bool newValue = GUI.Toggle(r, boolVar, label, style);
			if(newValue != boolVar){
				string undoName = (newValue ? "enable" : "disable") + " " + undoActionName;
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return boolVar;
		}

		public bool UndoableToggle(Rect r, bool boolVar, string undoActionName, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.toggle;
			bool newValue = GUI.Toggle(r, boolVar, new GUIContent(""));
			if(newValue != boolVar){
				string undoName = (newValue ? "enable" : "disable") + " " + undoActionName;
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return boolVar;
		}


		public Enum UndoableEnumPopup(Rect r, Enum enumValue, string undoInfix){
			Enum nextEnum = EditorGUI.EnumPopup( r, enumValue );

			if(nextEnum.ToString() != enumValue.ToString()){
				string undoName = "set " + undoInfix + " to " + nextEnum;
				Undo.RecordObject(this,undoName);
				enumValue = nextEnum;
			}
			return enumValue;
		}


		public Enum UndoableLabeledEnumPopup(Rect r, string label, Enum enumValue, string undoInfix){
			Enum nextEnum = SF_GUI.LabeledEnumField( r, label, enumValue, EditorStyles.miniLabel );
			if(nextEnum.ToString() != enumValue.ToString()){
				string undoName = "set " + undoInfix + " to " + nextEnum;
				Undo.RecordObject(this,undoName);
				enumValue = nextEnum;
			}
			return enumValue;
		}


		public int UndoableEnumPopupNamed(Rect r, Enum enumValue, string[] displayedOptions, string undoInfix){
			int nextEnum = EditorGUI.Popup( r, (int)((object)enumValue), displayedOptions);
			if(nextEnum != ((int)((object)enumValue))){
				string undoName = "set " + undoInfix + " to " + displayedOptions[nextEnum];
				Undo.RecordObject(this,undoName);
				return nextEnum;
			}
			return (int)((object)enumValue);
		}

		public int UndoableLabeledEnumPopupNamed(Rect r, string label, Enum enumValue, string[] displayedOptions, string undoInfix){
			int nextEnum = SF_GUI.LabeledEnumFieldNamed( r, displayedOptions, new GUIContent(label), (int)((object)enumValue), EditorStyles.miniLabel);
			if(nextEnum != ((int)((object)enumValue))){
				string undoName = "set " + undoInfix + " to " + displayedOptions[nextEnum];
				Undo.RecordObject(this,undoName);
				return nextEnum;
			}
			return (int)((object)enumValue);
		}


		//UndoablePopup

		public float UndoableFloatField(Rect r, float value, string undoInfix, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.textField;
			float newValue = EditorGUI.FloatField( r, value, style );
			if(newValue != value){
				string undoName = "set " + undoInfix + " to " + newValue;
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return value;
		}

		public int UndoableIntField(Rect r, int value, string undoInfix, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.textField;
			int newValue = EditorGUI.IntField( r, value, style );
			if(newValue != value){
				string undoName = "set " + undoInfix + " to " + newValue;
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return value;
		}




		public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.textField;
			string newValue = EditorGUI.TextField( r, value, style );
			if(newValue != value){
				string undoName = "change " + undoInfix + " to " + newValue;
				Undo.RecordObject(this,undoName);
				return newValue;
			}
			return value;
		}


		public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style = null, UnityEngine.Object extra = null, bool showContent = true){
			if(style == null)
				style = EditorStyles.textField;
			string newValue = EditorGUI.TextField( r, value, style );
			if(newValue != value){
				string undoName = "change " + undoInfix;
				if(showContent)
					undoName += " to " + newValue;
				Undo.RecordObject(this, undoName);
				if(extra != null)
					Undo.RecordObject(extra, undoName);
				return newValue;
			}
			return value;
		}


		public void UndoableEnterableNodeTextField(SF_Node node, Rect r, ref string value, string undoMsg, bool update = true, UnityEngine.Object extra = null){
			string nextValue = value;
			SF_GUI.EnterableTextField(node, r, ref nextValue, EditorStyles.textField, update );
			if(nextValue != value){
				Undo.RecordObject(this, undoMsg );
				if(extra != null)
					Undo.RecordObject(extra, undoMsg);
				value = nextValue;
			}
		}






















	}
}