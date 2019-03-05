using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShaderForge {


	public enum CustomValueType{
		Float, 
		Float2, 
		Float3, 
		Float4,
		Half,
		Half2,
		Half3,
		Half4,
		Fixed,
		Fixed2,
		Fixed3,
		Fixed4,
		Sampler2D,
		Matrix4x4
		/*, Texture*/ };

	[System.Serializable]
	public class SFN_Code : SF_Node_Resizeable {


		public string code = "";
		public string functionName = "Function_node_";

		private bool isEditing = false;

		public SFN_Code() {
		}

		public override void Initialize() {
			base.Initialize( "Code" );
			functionName = "Function_node_" + base.id;
			base.minWidth = (int)(NODE_WIDTH * 2.5f);
			base.minHeight = NODE_HEIGHT;
			base.ClampSize();
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","Out",ConType.cOutput,ValueType.VTvPending)
			};
			controlName = base.id + "_codeArea";
			connectors[0].CustomValueType = CustomValueType.Float3;
			//Debug.Log("Creating thing " + base.id);
		}

		public string GetFunctionName(){
			return functionName;
			//return "CustomCode_" + id;
		}

		public override int GetEvaluatedComponentCount (){
			return SF_Tools.ComponentCountOf(connectors[0].CustomValueType);
		}

		public override string GetPrepareUniformsAndFunctions(){
			return GetFunctionHeader() + "\n" + code + "\n}\n";
		}

		public string GetFunctionHeader(){
			string outputType = ToCodeType(connectors[0].CustomValueType); // Output type
			string inputs = "(";
			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput)
					continue;
				inputs += " " + ToCodeType(con.CustomValueType) + " " + con.label + " ";

				if(con != connectors[connectors.Length-1]) // Add comma if it's not the last one
					inputs += ",";
			}
			inputs += "){";
			return outputType + " " + GetFunctionName() + inputs;
		}

		private string ToCodeType(CustomValueType cvt){
			if(cvt == CustomValueType.Sampler2D)
				return "sampler2D"; // Uppercase D
			if( cvt == CustomValueType.Matrix4x4 )
				return "float4x4";
			return cvt.ToString().ToLower();
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string s = GetFunctionName();
			s += "(";
			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput)
					continue;
				s += " " + con.TryEvaluate() + " ";
				
				if(con != connectors[connectors.Length-1]) // Add comma if it's not the last one
					s += ",";
			}

			s += ")";
			return s;
		}

		public override float EvalCPU( int c ) {
			return 1f;
		}

		string controlName;


		float targetSideButtonWidth;
		float currentSideButtonWidth;

		//CustomValueType outType = CustomValueType.Float3;

		bool hoveringNode = false;

		int guiIncID = 0;

		bool justFocused = false;
		int pressedTabLastFrameCounter = 0;
		int pressedEditLastFrameCounter = 0;
		int savedCaretPosition;

		TextEditor txtEditor;

		public override void DrawInner(Rect r){



			//Debug.Log("GUI THREAD: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());

			//if(Event.current.type == EventType.layout)
				//return;

			if(Event.current.type == EventType.Repaint)
				guiIncID++;

			//if(Event.current.type == EventType.repaint)
				//if(hoveringNode){
					//hoveringNode = r.Margin(128).Contains(Event.current.mousePosition);
				//} else {
					hoveringNode = r.Contains(Event.current.mousePosition);
				//}


			if(!isEditing) // Don't resize while editing
				targetSideButtonWidth = (selected) ? 70f : 0f;


			int sideButtonHeight = 16;
			int buttonTextMargin = 4;

			int sideButtonWidth = Mathf.RoundToInt(currentSideButtonWidth);
			if(Event.current.type == EventType.Repaint){
				currentSideButtonWidth = Mathf.Lerp(currentSideButtonWidth, targetSideButtonWidth, 0.6f);
			}

			Rect txtRect = r;



			txtRect = txtRect.PadRight(/*(int)sideButtonWidth +*/ buttonTextMargin);
			txtRect = txtRect.PadLeft((int)sideButtonWidth*2 + buttonTextMargin);
			txtRect = txtRect.PadBottom(buttonTextMargin);


			// BUTTONS
			if(sideButtonWidth > 12f){
				
				Rect btnOutput = txtRect;
				Rect btnInput = txtRect;
				btnOutput.width = sideButtonWidth;
				btnInput.width = sideButtonWidth*2;
				btnOutput.height = btnInput.height = sideButtonHeight;
				btnOutput.x += txtRect.width - sideButtonWidth;
				btnInput.x += - buttonTextMargin / 2 - sideButtonWidth*2;
				
				DrawTypecastButtons( btnOutput, btnInput );
				
			}


			txtRect = txtRect.PadTop((int)(sideButtonWidth*0.32f));



			if(isEditing && !justFocused && Event.current.type == EventType.Repaint){
				//Debug.Log("GUI THREAD " + Event.current.type + " LOWER");
				if(GUI.GetNameOfFocusedControl() != controlName){
					//Debug.Log("DEFOCUS - " + Event.current.type + " fc: " + GUI.GetNameOfFocusedControl() );
					isEditing = false;
					isEditingAnyNodeTextField = false;
				}
			}


			
			if(Event.current.type == EventType.Repaint){
				justFocused = false;
			}

			//Debug.Log("GUI THREAD B: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());

			if(isEditing){

				controlName = base.id + "_codeArea";

				GUI.SetNextControlName(controlName);

				string codeBefore = code;
				//code = GUI.TextArea(txtRect,code,SF_Styles.CodeTextArea);
				code = UndoableTextArea(txtRect, code, "code", SF_Styles.CodeTextArea);

				SF_GUI.AssignCursor( txtRect , MouseCursor.Text );

				//if(copied){
				//	code = codeBefore;
				//	txtEditor.pos += copyLength-1;
				//}


				txtEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				savedCaretPosition = txtEditor.cursorIndex;
				//txtEditor.selectPos = 4;




				//if(SF_GUI.HoldingControl() && Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.C)


				if(Event.current.keyCode == KeyCode.Tab && Event.current.type == EventType.KeyDown){
					//Debug.Log("Tab");
					UndoRecord("insert tab in " + functionName + " code");
					code = code.Insert( txtEditor.cursorIndex, "\t" );
					//Debug.Log("Caret position = " + txtEditor.pos);
					savedCaretPosition = txtEditor.cursorIndex;
					pressedTabLastFrameCounter = 5; // Force it for five GUI frames
					Event.current.Use();
					GUI.FocusControl(controlName);
				}

				if(pressedTabLastFrameCounter > 0 /*&& GUI.GetNameOfFocusedControl() != controlName*/){
					GUI.FocusControl(controlName);
					txtEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
					txtEditor.cursorIndex = savedCaretPosition + 1;
					txtEditor.selectIndex = savedCaretPosition+1;
					pressedTabLastFrameCounter--;
				}

				/*
				if(Event.current.keyCode == KeyCode.Tab && Event.current.type == EventType.keyUp){
					GUI.FocusControl(controlName);
					Event.current.Use();
					GUI.FocusControl(controlName);
				}

				if(Event.current.Equals( Event.KeyboardEvent("tab") )){
					GUI.FocusControl(controlName);
					Event.current.Use();
					GUI.FocusControl(controlName);
				}*/



				if(code != codeBefore){
					OnUpdateNode(NodeUpdateType.Soft, false);
				}
				//if(focusBefore != string.Empty && GUI.GetNameOfFocusedControl() != focusBefore){
				//	GUI.FocusControl(focusBefore);
				//}
				//Debug.Log("GUI THREAD B_A_1: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());

			}else{
				//Debug.Log("GUI THREAD " + Event.current.type + " UPPER");
				//Debug.Log("GUI THREAD B_B_0: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());
				GUI.Box(txtRect.PadBottom(1),code,SF_Styles.CodeTextArea);
				if(hoveringNode){

					bool doubleClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2;

					Rect btnRect = new Rect(txtRect.xMax,txtRect.yMax,46,16).MovedUp().MovedLeft();
					btnRect.x -= 3;
					btnRect.y -= 4;

					btnRect.xMin -= 3;
					btnRect.yMin -= 4;

					// Workaround for a weird issue
					//bool clickedBtn = btnRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseUp && Event.current.button == 0;

					//Debug.Log("GUI THREAD B_B_1: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());
					if(GUI.Button(btnRect,"Edit",EditorStyles.miniButton) || doubleClicked && Event.current.type == EventType.Repaint){
						isEditing = true;
						//Debug.Log("FOCUS - " + Event.current.type + " fc: " + GUI.GetNameOfFocusedControl() );
						pressedEditLastFrameCounter = 5;
						isEditingAnyNodeTextField = true;
						GUI.FocusControl(controlName);
					//	forceFocusCodeField = true;
						Event.current.Use();
						justFocused = true;
					}
					//Debug.Log("GUI THREAD B_B_2: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());
				}
			}

			if(pressedEditLastFrameCounter > 0){
				GUI.FocusControl(controlName);
				//Debug.Log("REFOCUSING " + controlName + " fc: " + GUI.GetNameOfFocusedControl() );
				pressedEditLastFrameCounter--;
			}

			//Debug.Log("GUI THREAD C: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());



			/*
			if (forceFocusCodeField) {
				//GUI.SetNextControlName("focusChange");
				if (GUI.GetNameOfFocusedControl() != controlName) {
					GUI.FocusControl(controlName);
				} else {
					forceFocusCodeField = false;
				}
				
			}*/




			//Debug.Log("GUI THREAD END: " + Event.current.type + " - " + GUI.GetNameOfFocusedControl());
		}
		
		//bool forceFocusCodeField = false;

		public void DrawTypecastButtons(Rect btnL, Rect btnR){


			// OUTPUT
			DrawTypecastButton(btnL, connectors[0]);


			// INPUTS
			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput)
					continue;
				DrawTypecastButton(btnR, con, isInput:true);
				btnR.y += btnR.height+4;
			}

			// ADD INPUT BUTTON
			GUI.color = new Color(0.7f,1f,0.7f,1f);
			if(GUI.Button(btnR,"Add input", EditorStyles.miniButton)){
				UndoRecord("add input to " + functionName);
				AddInput();
			}
			GUI.color = Color.white;


		}




		public void AddInput(){


			SF_NodeConnector[] savedCons = new SF_NodeConnector[connectors.Length+1];
			for(int i=0;i<connectors.Length;i++){
				savedCons[i] = connectors[i];
			}

			int id = savedCons.Length - 1;
			savedCons[id] = SF_NodeConnector.Create(this,SF_Tools.alphabetUpper[id-1].ToString(),SF_Tools.alphabetUpper[id-1].ToString(),ConType.cInput,ValueType.VTv3).SetRequired(true); // Last one, the new one 
			savedCons[id].CustomValueType = CustomValueType.Float3;
			connectors = savedCons;

			RefreshConnectorStringIDs();

			UpdateMinHeight();

			OnUpdateNode(NodeUpdateType.Hard, false);
		}

		public void UpdateMinHeight(){
			base.minHeight = Mathf.Max(NODE_HEIGHT, (connectors.Length - 1) * 20 + 48);
			base.ClampSize();
		}

		public void RemoveConnector(SF_NodeConnector con, bool undoRecord){

			string undoString = "";
			if(undoRecord){
				undoString = "remove input from " + functionName;
				Undo.RecordObject(con, undoString);
				UndoRecord(undoString);
			}

			con.Disconnect();

			List<SF_NodeConnector> conList = new List<SF_NodeConnector>();
			foreach(SF_NodeConnector c in connectors){
				if(c != con){
					conList.Add(c);
				}
			}

			if(undoRecord)
				Undo.DestroyObjectImmediate(con);
			else
				DestroyImmediate(con);
			connectors = conList.ToArray();

			RefreshConnectorStringIDs();
			UpdateMinHeight();

			OnUpdateNode(NodeUpdateType.Hard, false);
		}

		public void RefreshConnectorStringIDs(){
			int nameIndex = 0;
			foreach(SF_NodeConnector c in connectors){
				if(c.conType == ConType.cInput){
					c.strID = SF_Tools.alphabetUpper[nameIndex].ToString();
					nameIndex++;
				}
			}
		}


		string EncodeCode(){
			return SF_Tools.StringToBase64String( code );
		}

		string DecodeCode(string encoded){
			return SF_Tools.Base64StringToString( encoded );
		}

		public class SF_Serializer{

			List<string> keys;
			List<string> values;

			public SF_Serializer(){
				keys = new List<string>();
				values = new List<string>();
			}

			public SF_Serializer Add(string key, string value){
				keys.Add(key);
				values.Add(value);
				return this;
			}

			public SF_Serializer Add(string key, int value){
				return Add(key, value.ToString());
			}

			public SF_Serializer Add(string key, float value){
				return Add(key, value.ToString());
			}

			public SF_Serializer Add(string key, bool value){
				return Add(key, value.ToString());
			}

			public override string ToString(){
				string s = "";
				for(int i=0;i<keys.Count;i++){
					if(i > 0)
						s += ",";
					s += keys[i] + ":" + values[i];
				}
				return s;
			}

		}


		public override string SerializeSpecialData() {
			/*
			string s = "";
			s += "code:" + EncodeCode() + ",";

			s += "output:" + (int)connectors[0].CustomValueType + ",";

			s += "fnme:" + functionName + ",";

			for(int i=1;i<connectors.Length;i++){
				s += ",";
				s += "input:" + (int)connectors[i].CustomValueType;
			}*/

			//return s;

			SF_Serializer s = new SF_Serializer();

			s.Add(	"code",		EncodeCode()						);
			s.Add(	"output",	(int)connectors[0].CustomValueType	);
			s.Add(	"fname",	functionName						);
			s.Add(	"width",	(int)rect.width						);
			s.Add(	"height",	(int)rect.height					);

			for(int i=1;i<connectors.Length;i++){
				s.Add(	"input",	(int)connectors[i].CustomValueType	);
			}

			for(int i=1;i<connectors.Length;i++){
				s.Add(	"input_" + i + "_label", connectors[i].label );
			}

			return s.ToString();
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "code":
				code = DecodeCode(value);
				break;
			case "output":
				connectors[0].CustomValueType = (CustomValueType)(int.Parse(value));
				break;
			case "input":
				AddInput();
				connectors[connectors.Length-1].CustomValueType = (CustomValueType)(int.Parse(value));
				break;
			case "fname":
				functionName = value;
				break;
			case "width":
				rect.width = int.Parse(value);
				break;
			case "height":
				rect.height = int.Parse(value);
				break;
			}

			if(key.StartsWith("input_") && key.EndsWith("_label")){
				int i = int.Parse(key.Split('_')[1]);
				connectors[i].label = value;
			}

			UpdateExtraInputWidth();
		}


		public void DrawTypecastButton(Rect r, SF_NodeConnector con, bool isInput = false){

			Rect closeRect = r.ClampWidth(0,(int)r.height);

			if(isInput){
				GUI.color = new Color(1f,0.7f,0.7f,1f);
				if(GUI.Button(closeRect,"-")){
					RemoveConnector(con, undoRecord:true);
					return;
				}
				GUI.color = Color.white;
				r.xMin += closeRect.width;
				Rect dropdownRect = r;
				dropdownRect.xMax -= 50;
				r.xMin += dropdownRect.width;
				int cvtccBef = SF_Tools.ComponentCountOf(con.CustomValueType );
				//con.CustomValueType = (CustomValueType)EditorGUI.EnumPopup(dropdownRect, con.CustomValueType);
				con.CustomValueType = (CustomValueType)UndoableEnumPopup(dropdownRect, con.CustomValueType, "set input " + con.label + " value type");
				if(cvtccBef != SF_Tools.ComponentCountOf(con.CustomValueType)){
					con.Disconnect();
				}
				string before = con.label;
				con.label = EditorGUI.TextField(r,con.label);
				if(con.label != before && con.label.Length > 0){
					con.label = SF_ShaderProperty.FormatInternalName(con.label);
					UpdateExtraInputWidth();
					OnUpdateNode(NodeUpdateType.Soft);
				}
			} else {
				int cvtccBef = SF_Tools.ComponentCountOf(con.CustomValueType );
				//con.CustomValueType = (CustomValueType)EditorGUI.EnumPopup(r, con.CustomValueType);
				con.CustomValueType = (CustomValueType)UndoableEnumPopup(r, con.CustomValueType, "set output value type");
				if(cvtccBef != SF_Tools.ComponentCountOf(con.CustomValueType)){
					con.Disconnect();
				}
			}
				 
		}

		public void UpdateExtraInputWidth(){

			int widest = SF_NodeConnector.defaultConnectorWidth;

			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput)
					continue;

				widest = Mathf.Max( SF_GUI.WidthOf(con.label, SF_Styles.MiniLabelOverflow)+2, widest);
			}

			extraWidthInput = widest - SF_NodeConnector.defaultConnectorWidth;



		}






	}
}