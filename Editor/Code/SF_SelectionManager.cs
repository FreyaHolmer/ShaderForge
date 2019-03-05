using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {
	[System.Serializable]
	public class SF_SelectionManager : ScriptableObject {

		[SerializeField]
		SF_Editor editor;
		[SerializeField]
		List<SF_Node> selection;
		public List<SF_Node> Selection {
			get {
				if( selection == null )
					selection = new List<SF_Node>();
				return selection;
			}
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		[SerializeField]
		Rect selectionBox = new Rect(256,32,32,64);

		[SerializeField]
		public bool boxSelecting = false;

		public void MoveSelection(Vector2 delta, SF_Node ignore) {
			foreach(SF_Node n in Selection){
				if( n == ignore )
					continue;
				//if(selection.Count > 1){
					//Debug.Log("Selection count = " + selection.Count + " thus nodes");
					Undo.RecordObject(n,"move nodes");
				//}
				n.rect.x += delta.x;
				n.rect.y += delta.y;
			}
		}



		public void DrawBoxSelection() {
			if( boxSelecting ) {
				Rect r = new Rect( selectionBox );
				SF_Tools.FlipNegative( ref r );
				GUI.Box( r, string.Empty, SF_Styles.SelectionStyle );
				//GUI.Label(r,selectionBox.ToString().Replace(' ','\n'));
			}
		}

		public void OnGUI() {

			/*
			selectionBox.x = Event.current.mousePosition.x;
			selectionBox.y = Event.current.mousePosition.y;
			selectionBox.width = 128;
			selectionBox.height = 128;
			*/


			if( SF_GUI.ReleasedRawLMB() && boxSelecting) {
				ExecuteBoxSelect();
			}


			if( SF_GUI.PressedLMB() && SF_GUI.HoldingBoxSelect() ) {
				boxSelecting = true;

				if( !SF_GUI.MultiSelectModifierHeld() )
					DeselectAll(registerUndo:true);

				selectionBox.x = Event.current.mousePosition.x;
				selectionBox.y = Event.current.mousePosition.y;
				Event.current.Use();
			}
			
			
			// Duplicate, copy, cut, paste
			EventType et = Application.platform == RuntimePlatform.OSXEditor ? EventType.KeyDown : EventType.KeyUp; // TODO: Use KeyDown for Windows too



			if( SF_GUI.HoldingControl() && Event.current.type == et && !SF_Node.isEditingAnyNodeTextField ) {

				switch(Event.current.keyCode){
					case(KeyCode.D):
						DuplicateSelection();
						break;
					case(KeyCode.C):
						CopySelection();
						break;
					case(KeyCode.X):
						CutSelection();
						break;
					case(KeyCode.V):
						PasteFromClipboard();
						break;
				}
				
			}

			// Selection box
			if( boxSelecting ) {

				selectionBox.width = Event.current.mousePosition.x - selectionBox.x;
				selectionBox.height = Event.current.mousePosition.y - selectionBox.y;

				if(Event.current.isMouse)
					Event.current.Use();
			}

			if( SF_GUI.PressedDelete() && !SF_Node.isEditingAnyNodeTextField ) {
				DeleteSelected();
				Event.current.Use();
			}
		}
		
		


		public void ExecuteBoxSelect() {
			boxSelecting = false;
			foreach( SF_Node n in editor.nodes ) {
				if( SF_Tools.Intersects( n.rect, selectionBox ) ){
					n.Select(registerUndo:true);
				}
			}
			Event.current.Use();
		}



		public void DeleteSelected() {


			if(Selection.Contains(editor.mainNode)){
				editor.mainNode.Deselect(registerUndo:false); // Deselect main node if you press delete
			}

			int selCount = Selection.Count;

//			Debug.Log("Delete selected, count = " + selCount);

			if(selCount == 0)
				return;

			string undoMsg = "";

			if(selCount == 1)
				undoMsg = "delete " + Selection[0].nodeName;
			else
				undoMsg = "delete " + selCount + " nodes";
			//Debug.Log("Selection delete initiated - " + undoMsg );

			Undo.RecordObject(editor,undoMsg);
			Undo.RecordObject(editor.nodeView.treeStatus, undoMsg);

			foreach(SF_Node node in editor.nodes){
				node.UndoRecord(undoMsg);
				// Undo.RecordObject(node, undoMsg);
			}

			Undo.RecordObject(this,undoMsg);



			// Undo recording is weird :(






			for( int i = editor.nodes.Count - 1; i >= 0; i-- ) {
				SF_Node n = editor.nodes[i];
				if( n.selected ) {

					if(n is SFN_Relay){
						SF_NodeConnector inCon = n["IN"];
						SF_NodeConnector outCon = n["OUT"];
						if(inCon.IsConnected() && outCon.IsConnected() ){
							// Relink all outputs to the incoming connectors
							for (int ir = outCon.outputCons.Count - 1; ir >= 0; ir--) {
								outCon.outputCons[ir].LinkTo(inCon.inputCon);
							}
							inCon.Disconnect();
						}
					}

					foreach(SF_NodeConnector con in editor.nodes[i].connectors){
						if(con.conType == ConType.cOutput){
							con.Disconnect();
						}
					}
					if( editor.nodeView.treeStatus.propertyList.Contains( editor.nodes[i] ) )
						editor.nodeView.treeStatus.propertyList.Remove( editor.nodes[i] );
					editor.nodes[i].Deselect(registerUndo:false);
					editor.nodes.RemoveAt(i);


					//editor.nodes[i].Delete(registerUndo:false, undoMsg:undoMsg);
				}
			}
		}
		
		
		// Clipboard
		public string[] CbNodes{
			get{
				string s = EditorPrefs.GetString("shaderforge_clipboard", "");
				return s.Split('\n');
			}
			set{
				string s = "";
				for( int i=0;i<value.Length;i++){
					if(i != 0)
						s += '\n';
					s += value[i];
				}
				EditorPrefs.SetString("shaderforge_clipboard",s);
			}
		}
		
		public void CutSelection(){
			if(selection.Count == 0)
				return;
			//if(selection.Count == 1)
			//	Undo.RecordObject(editor,"cut node");
			//else
				//Undo.RecordObject(editor,"cut nodes");
			CbNodes = GetSelectionSerialized();
			DeleteSelected();
			Event.current.Use();
		}
		
		public void CopySelection(){
			if(selection.Count == 0)
				return;
			CbNodes = GetSelectionSerialized();
			Event.current.Use();
		}
		
		public void PasteFromClipboard(){
			if(CbNodes.Length == 0)
				return;
			
			//Rect selBounds = GetSelectionBounds();
			//Vector2 posOffset = ;

			string undoMsg = "paste ";

			if(CbNodes.Length == 1)
				undoMsg += "node";
			else
				undoMsg += CbNodes.Length + " nodes";

			RecordUndoNodeCreationAndSelectionStates(undoMsg);
			
			InstantiateNodes(CbNodes, new Vector2(64f,64f), undoMsg); // TODO
			CbNodes = GetSelectionSerialized(); // This is now the new clipboard!
		}

		public void RecordUndoNodeCreationAndSelectionStates(string undoMsg){
			Undo.RecordObject(editor,undoMsg);
			Undo.RecordObject(editor.nodeView.treeStatus,undoMsg);
			Undo.RecordObject(editor.nodeView.selection,undoMsg);
		}
		
		public void DuplicateSelection(){
			DeselectMainNode();
			string[] selectionSerialized = GetSelectionSerialized();
			if(selectionSerialized.Length == 0)
				return;

			string undoMsg = "duplicate ";

			if(Selection.Count > 1)
				undoMsg += "nodes";
			else
				undoMsg += Selection[0].nodeName;

			RecordUndoNodeCreationAndSelectionStates(undoMsg);
			
			//Rect selBounds = GetSelectionBounds();
			Vector2 posOffset = new Vector2(64,64);
			
			InstantiateNodes(selectionSerialized, posOffset, undoMsg);
			
		}
		
		
		void InstantiateNodes(string[] serializedNodes, Vector2 posOffset, string undoMsg){
			// Make sure it knows about the editor
			SF_Parser.editor = editor;
			
			List<SF_Node> newNodes = new List<SF_Node>();					// List of all new nodes
			List<SF_Link> links = new List<SF_Link>(); 	// Used for multi-clone
			
			int[] idOld = new int[serializedNodes.Length];
			int[] idNew = new int[serializedNodes.Length];
			
			for(int i=0;i<serializedNodes.Length;i++){
				SF_Node node = SF_Node.Deserialize(serializedNodes[i], ref links);
				if( node.IsProperty() ) {
					if( editor.PropertyNameTaken( node.property ) ) {
						node.property.SetName( node.property.GetClonedName() ); // Rename if needed
						node.variableName = node.property.nameInternal;
					}
				}
						
				idOld[i] = node.id;
				node.AssignID(); // Increment IDs
				if( !node.IsProperty() )
					node.ResetVariableName();
				idNew[i] = node.id;
				node.rect.x += posOffset.x;
				node.rect.y += posOffset.y;
				newNodes.Add(node);
			}
			
			// Establish all links
			foreach(SF_Link link in links){
				link.Remap(idOld, idNew);
				link.Establish(editor,LinkingMethod.Default);
			}
			Undo.IncrementCurrentGroup();
			DeselectAll(registerUndo:true, undoMsg:undoMsg);
			Undo.CollapseUndoOperations(Undo.GetCurrentGroup() - 1);
			Event.current.Use();
			foreach(SF_Node n in newNodes)
				n.Select(registerUndo:false);
		}
		
		public void DeselectMainNode(){
			for(int i=0;i<selection.Count;i++){
				if(selection[i] is SFN_Final){
					selection[i].Deselect(registerUndo:false);
				}
			}
		}
		
		// Get serialized selection, ignore SFN_Final
		public string[] GetSelectionSerialized(){
			List<string> srzdSel = new List<string>();
			
			for(int i=0;i<selection.Count;i++){
				if(selection[i] is SFN_Final)
					continue;
				srzdSel.Add(selection[i].Serialize(skipExternalLinks:true, useSuffixPrefix:false));
			}
			
			return srzdSel.ToArray();
		}
		
		
		private Rect GetSelectionBounds(){
			
			Rect nullRect = new Rect(0,0,0,0);
			Rect r = nullRect;
			
			for(int i=0;i<selection.Count;i++){
				if(selection[i] is SFN_Final)
					continue;
				if(r == nullRect)
					r = new Rect(selection[i].rect);
				else
					r = SF_Tools.Encapsulate(r,selection[i].rect);
			}
			
			return r;
			
		}
				
		public List<SF_Link> CloneNodeAndGetLinks(){
			List<SF_Link> links = new List<SF_Link>();	
				
				
				
				
			return links;
		}
		

		public SF_SelectionManager Initialize( SF_Editor editor ) {
			this.editor = editor;
			return this;
		}

		public void Add(SF_Node n) {
			Selection.Add( n );
		}

		public void Remove( SF_Node n ) {
			Selection.Remove( n );
		}

		public void DeselectAll(bool registerUndo, string undoMsg = null) {
			editor.ResetRunningOutdatedTimer();
			foreach( SF_Node n in editor.nodes ) {
				n.Deselect(registerUndo, undoMsg);
			}
		}

		
	}
}

