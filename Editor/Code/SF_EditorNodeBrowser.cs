using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {

	[System.Serializable]
	public class SF_EditorNodeBrowser : ScriptableObject {


		[SerializeField]
		public SF_Editor editor;
		[SerializeField]
		public Vector2 scrollPos;

		[SerializeField]
		GUIStyle styleToolbar;
		[SerializeField]
		GUIStyle styleSearchField;
		[SerializeField]
		GUIStyle styleSearchCancel;
		[SerializeField]
		GUIStyle styleCategory;
		[SerializeField]
		GUIStyle styleButton;

		[SerializeField]
		bool showFiltered = false;
		//[SerializeField] SerializableDictionary<SF_EditorNodeData, Func<SF_Node>> unfiltered;
		//[SerializeField] SerializableDictionary<SF_EditorNodeData, Func<SF_Node>> filtered;
		[SerializeField]
		List<SF_EditorNodeData> unfiltered;
		[SerializeField]
		List<SF_EditorNodeData> filtered;

		bool initializedStyles = false;

		[SerializeField]
		SF_EditorNodeData dragNode = null;

		[SerializeField]
		public string searchString = "";


		public SF_EditorNodeBrowser() {
			initializedStyles = false;
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}




		public SF_EditorNodeBrowser Initialize( SF_Editor editor ) {
			this.editor = editor;
			unfiltered = editor.nodeTemplates;
			filtered = new List<SF_EditorNodeData>();
			dragNode = null;
			return this;
		}


		public void CheckInitializeStyles() {
			if( initializedStyles && styleCategory.fixedHeight == 24 && styleButton.fixedHeight == 24 )
				return;
			InitializeStyles();
		}

		private void InitializeStyles() {
			styleToolbar = new GUIStyle( GUI.skin.FindStyle( "Toolbar" ) );
			styleSearchField = new GUIStyle( GUI.skin.FindStyle( "ToolbarSeachTextField" ) );
			styleSearchCancel = new GUIStyle( GUI.skin.FindStyle( "ToolbarSeachCancelButton" ) );
			styleCategory = new GUIStyle( EditorStyles.toolbarButton );
			styleCategory.alignment = TextAnchor.MiddleLeft;
			styleCategory.fixedHeight = 24;
			styleCategory.fontStyle = FontStyle.Bold;
			styleCategory.fontSize = 9;
			styleCategory.margin.top = 0;
			styleCategory.margin.bottom = 0;

			styleButton = new GUIStyle( GUI.skin.textField );
			styleButton.alignment = TextAnchor.MiddleLeft;
			styleButton.normal.textColor = SF_GUI.ProSkin ? new Color( 0.8f, 0.8f, 0.8f ) : new Color( 0.2f, 0.2f, 0.2f );
			styleButton.fontSize = 10;
			styleButton.fixedHeight = 24;
			styleButton.fontSize = 10;
			styleButton.margin.top = 0;
			styleButton.margin.bottom = 0;

			initializedStyles = true;
		}



		[SerializeField]
		string prevString;

		[SerializeField]
		string prevCategory;

		[SerializeField]
		float innerHeight = 256;

		const string searchBoxName = "sf_search_box";

		public void OnLocalGUI( Rect rect ) {

			if( IsPlacing() && Event.current.type == EventType.MouseUp && Event.current.button == 1 ) {
				CancelDrag();
				Event.current.Use();
			}

			CheckInitializeStyles();
			//EditorGUIUtility.LookLikeInspector();

			if( styleCategory.alignment != TextAnchor.MiddleLeft )
				InitializeStyles();


			Rect toolbarRect = new Rect( rect );
			toolbarRect.height = 19;


			Rect searchRect = new Rect( toolbarRect );
			searchRect.width -= 19;
			searchRect.y += 1;

			Rect searchCancelRect = new Rect( searchRect );
			searchCancelRect.x += searchCancelRect.width;
			searchCancelRect.width = 19;

			// Command/ctrl + F // TODO
			/*
			if( SF_GUI.HoldingControl() &&
			    Event.current.keyCode == KeyCode.F && 
			    Event.current.type == EventType.keyDown && 
			    GUI.GetNameOfFocusedControl() != searchBoxName){

				Event.current.character = (char)0; // We're done using F now
				Event.current.Use();
				GUI.FocusControl(searchBoxName); // Focus search field
				Event.current.character = (char)0; // Stop! No more characters! Please!
			}
			*/


			// Draw Toolbar
			GUI.Box( toolbarRect, "", styleToolbar );

			prevString = searchString.Trim();
			GUI.SetNextControlName( searchBoxName );
			searchString = EditorGUI.TextField( searchRect, searchString, styleSearchField );
			if( GUI.Button(searchCancelRect, "", styleSearchCancel ) ) {
				searchString = "";
				GUI.FocusControl( null );
			}
			if( searchString.Trim() != prevString )
				OnSearchStringChanged();





			// Scroll view stuff
			Rect panelRect = new Rect( rect );
			panelRect.yMin += toolbarRect.height - 1;
			panelRect.height -= toolbarRect.height;

			Rect scrollRect = new Rect( panelRect );
			scrollRect.y = scrollPos.y;

			

			// Calc insides height
			//Debug.Log(panelRect.height);
			scrollRect.height = Mathf.Max( panelRect.height, innerHeight );
			scrollRect.width -= 15;

			Rect btnRect = new Rect( panelRect.x, panelRect.y - toolbarRect.height, rect.width - 16, styleCategory.fixedHeight );
			innerHeight = 0;
			float innerStartY = 0f;

			scrollPos = GUI.BeginScrollView( panelRect, scrollPos, scrollRect, false, true /*GUILayout.Width( rect.wi )*/ );
			{
				if(Event.current.type == EventType.Layout)
					innerStartY = btnRect.y;
				if( GetNodeList().Count > 0 ) {
					foreach( SF_EditorNodeData entry in GetNodeList() ) {

						if( entry.category != prevCategory ) {
							DrawCategory(entry.category, ref btnRect );
							prevCategory = entry.category;
						}

						DrawButton( entry, ref btnRect );
					}
				} else {
					GUI.color = Color.gray;
					GUI.Label(btnRect, "No nodes matched" );
					GUI.color = Color.white;
				}

				if(Event.current.type == EventType.Layout){
					innerHeight = btnRect.yMax - innerStartY;
					//Debug.Log ("Inner: " + innerHeight + ", Panel: " + panelRect.height);
				}

			}
			GUI.EndScrollView();




			UpdateDrag();

		}

		public void DrawCategory( string label, ref Rect btnRect ) {
			GUI.Label( btnRect, label + ":", styleCategory );
			btnRect.y += btnRect.height;
		}

		public void DrawButton( SF_EditorNodeData entry, ref Rect btnRect ) {
			GUI.color = entry.isProperty ? SF_Node.colorExposed : Color.white;

			bool usable = !(!entry.availableInDeferredPrePass && editor.ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Deferred);

			if(!usable){
				//GUI.color = Color.red;
				GUI.enabled = false;
			}

			bool mouseOver = btnRect.Contains(Event.current.mousePosition);

			
			if(usable){
				if( dragNode == entry )
					GUI.color = SF_GUI.selectionColorBright;
				else if( mouseOver && dragNode == null )
					GUI.color = SF_GUI.selectionColorBrighter;
			}


			GUI.Label( btnRect, (usable ? string.Empty : "    ") + entry.nodeName, styleButton );


			if( mouseOver && Event.current.type == EventType.MouseDown && Event.current.button == 0 && usable) {
				OnStartDrag( entry );
			} else if( Event.current.type == EventType.ContextClick ) {
				Vector2 mousePos = Event.current.mousePosition;
				if( btnRect.Contains( mousePos ) ) {
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu();
					editor.ResetRunningOutdatedTimer();
					//menu.AddItem( new GUIContent("Edit Comment"), false, ContextClick, "cmt_edit" );
					menu.AddItem( new GUIContent("What does " + entry.nodeName + " do?"), false, ContextClick, entry );
					menu.ShowAsContext();
					Event.current.Use();
				}
			}



			GUI.color = Color.white;
			if( entry.isNew || entry.isUnstable) {
				GUIStyle miniStyle = new GUIStyle( EditorStyles.miniBoldLabel );
				miniStyle.alignment = TextAnchor.UpperRight;
				miniStyle.normal.textColor = Color.red;
				GUI.Label( btnRect, entry.isNew ? "New" : "Unstable", miniStyle );
			}

			if(usable){
				SF_GUI.AssignCursor( btnRect, MouseCursor.Pan );
			} else {
				if(Event.current.type == EventType.Repaint){
					GUI.enabled = true;
					SF_GUI.DrawLock(btnRect.PadTop(4),"Forward rendering only", TextAlignment.Right);
					//Draw(btnRect.PadTop(4), false, true, true, false); // Draw lock
					GUI.enabled = false;
				}
			}
			GUI.enabled = true;
			btnRect.y += btnRect.height;
		}

		public void ContextClick( object o ) {
			SF_EditorNodeData entry = o as SF_EditorNodeData;
			SF_Web.OpenDocumentationForNode(entry);
		}



		public void OnStartDrag( SF_EditorNodeData nodeData ) {
			//if( IsPlacing() )
			//	return;
			//Debug.Log( "DRAG BUTTON: " + nodeData.name );
			dragNode = nodeData;
		}

		public SF_Node OnStopDrag() {
			if( !IsPlacing() )
				return null;
			SF_Node newNode = null;
			if(editor.nodeView.rect.Contains(Event.current.mousePosition))
				newNode = editor.AddNode( dragNode, registerUndo:true );
			dragNode = null;
			return newNode;
		}

		public void UpdateDrag() {
			if( !IsPlacing() )
				return;

			editor.Repaint();

			//Debug.Log( "Drag exists: " + ( dragNode != null ) + "\nDrag name: " + dragNode.name + "\nDrag type: " + dragNode.type.ToString() );

			//float preScale = (float)(editor.separatorRight.rect.x - Event.current.mousePosition.x);
			//preScale /= 48f; // Distance to animate in
			//preScale = Mathf.Clamp01(preScale);

			//Rect boxRect = new Rect( 0, 0, SF_Node.NODE_SIZE, SF_Node.NODE_SIZE ).ScaleSizeBy(preScale).ClampSize((int)styleCategory.fixedHeight,SF_Node.NODE_SIZE);
			Rect boxRect = new Rect( 0, 0, SF_Node.NODE_SIZE, SF_Node.NODE_SIZE );
			boxRect.center = Event.current.mousePosition;

			GUI.Box( boxRect, dragNode.nodeName );
			//	Debug.Log( Event.current.type.ToString()); 
			if( Event.current.rawType == EventType.MouseUp )
				OnStopDrag();


		}

		public void CancelDrag() {
			dragNode = null;
		}

		public bool IsPlacing() {
			if( dragNode == null )
				return false;
			if( string.IsNullOrEmpty( dragNode.nodeName ) ) {
				dragNode = null;
				return false;
			}
			return true;
		}


		public bool DragButton( Rect r, string label, GUIStyle style ) {
			bool clicked = ( Event.current.type == EventType.MouseDown && Event.current.button == 0 );
			GUI.Button( r, label, style );
			bool hover = r.Contains( Event.current.mousePosition );
			return ( hover && clicked );
		}



		public List<SF_EditorNodeData> GetNodeList() {
			return showFiltered ? filtered : unfiltered;
		}


		public void OnSearchStringChanged() {
			if( string.IsNullOrEmpty( searchString ) ) {
				OnSearchStringCleared();
				return;
			}
			showFiltered = true;

			RefreshFilter();

		}

		public void OnSearchStringCleared() {
			showFiltered = false;
		}



		public void RefreshFilter() {
			filtered.Clear();

			/*foreach( KeyValuePair<SF_EditorNodeData, Func<SF_Node>> entry in unfiltered ) {
				if( Match(entry.Key.name, searchString) ) {
					filtered.Add(entry.Key,entry.Value);
				}
			}*/

			foreach( SF_EditorNodeData entry in unfiltered ) {
				if( Match( entry.nodeName, searchString ) ) {
					filtered.Add( entry );
				}
			}

		}

		private bool Match( string a, string b ) {
			return Clean( a ).Contains( Clean( b ) );
		}

		private string Clean( string s ) {
			return s.Trim().Replace( " ", string.Empty ).ToLower();
		}



	}
}