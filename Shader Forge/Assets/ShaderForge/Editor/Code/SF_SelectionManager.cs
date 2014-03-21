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
			foreach(SF_Node n in selection){
				if( n == ignore )
					continue;
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
					DeselectAll();

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

			if( SF_GUI.PressedDelete() ) {
				DeleteSelected();
				Event.current.Use();
			}
		}
		
		


		public void ExecuteBoxSelect() {
			boxSelecting = false;
			foreach( SF_Node n in editor.nodes ) {
				if( SF_Tools.Intersects( n.rect, selectionBox ) ){
					n.Select();
				}
			}
			Event.current.Use();
		}



		public void DeleteSelected() {
			for( int i = editor.nodes.Count - 1; i >= 0; i-- ) {
				if( editor.nodes[i].selected ) {
					editor.nodes[i].Delete();
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
			
			InstantiateNodes(CbNodes, new Vector2(64f,64f));
			CbNodes = GetSelectionSerialized(); // This is now the new clipboard!
		}
		
		
		public void DuplicateSelection(){
			string[] selectionSerialized = GetSelectionSerialized();
			if(selectionSerialized.Length == 0)
				return;
			
			//Rect selBounds = GetSelectionBounds();
			Vector2 posOffset = new Vector2(64,64);
			
			InstantiateNodes(selectionSerialized, posOffset);
			
		}
		
		
		void InstantiateNodes(string[] serializedNodes, Vector2 posOffset){
			// Make sure it knows about the editor
			SF_Parser.editor = editor;
			
			List<SF_Node> newNodes = new List<SF_Node>();					// List of all new nodes
			List<SF_Parser.SF_Link> links = new List<SF_Parser.SF_Link>(); 	// Used for multi-clone
			
			int[] idOld = new int[serializedNodes.Length];
			int[] idNew = new int[serializedNodes.Length];
			
			for(int i=0;i<serializedNodes.Length;i++){
				SF_Node node = SF_Parser.DeserializeNode(serializedNodes[i], ref links);
				if( node.IsProperty())
					if(editor.PropertyNameTaken(node.property))
						node.property.SetName(node.property.GetClonedName()); // Rename if needed
				idOld[i] = node.id;
				node.id = editor.idIncrement++; // Increment IDs
				idNew[i] = node.id;
				node.rect.x += posOffset.x;
				node.rect.y += posOffset.y;
				newNodes.Add(node);
			}
			
			// Establish all links
			foreach(SF_Parser.SF_Link link in links){
				link.Remap(idOld, idNew);
				link.Establish(editor,LinkingMethod.Default);
			}
				
			DeselectAll();
			Event.current.Use();
			foreach(SF_Node n in newNodes)
				n.Select();
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
				
		public List<SF_Parser.SF_Link> CloneNodeAndGetLinks(){
			List<SF_Parser.SF_Link> links = new List<SF_Parser.SF_Link>();	
				
				
				
				
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

		public void DeselectAll() {
			editor.ResetRunningOutdatedTimer();
			foreach( SF_Node n in editor.nodes ) {
				n.Deselect();
			}
		}

		
	}
}

