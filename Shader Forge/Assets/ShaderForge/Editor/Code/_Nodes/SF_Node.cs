using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace ShaderForge {

	public enum NodeUpdateType { Soft, Hard };

	[System.Serializable]
	public class SF_Node : ScriptableObject {

		public const int NODE_SIZE = 96;
		public const int NODE_WIDTH = NODE_SIZE + 3;	// This fits a NODE_SIZE texture inside
		public const int NODE_HEIGHT = NODE_SIZE + 16;	// This fits a NODE_SIZE texture inside

		public int node_width = NODE_WIDTH;
		public int node_height = NODE_HEIGHT;

		public int depth = 0; // Used when deserializing and updating

		public bool isGhost = false;

		public bool selected = false;

		public bool varDefined = false; // Whether or not this node has had its variable defined already.
		public bool varPreDefined = false; // Whether or not this variable has done its prefefs
		public bool alwaysDefineVariable = false;

		public static Color colorExposed = new Color( 0.8f, 1f, 0.9f );
		public static Color colorExposedDark = new Color( 0.24f, 0.32f, 0.30f ) * 1.25f;
		public static Color colorExposedDarker = new Color( 0.24f, 0.32f, 0.30f ) * 0.75f;


		

		public Color colorDefault{
			get{
				if(SF_GUI.ProSkin)
					return new Color( 0.8f, 0.8f, 0.8f);
				else
					return new Color( 1f, 1f, 1f );
			}
		}

		public bool showColor;
		public Color displayColor = Color.black;

		public SF_ShaderProperty property = null;

		public SF_NodeStatus status;
		
		public SF_Editor editor;

		public ShaderProgram program = ShaderProgram.Any;

		// User typed comment
		public string comment = "";
		//public bool hasComment;

		public bool showLowerPropertyBox;
		public bool showLowerPropertyBoxAlways;
		public bool showLowerReadonlyValues;
		public bool initialized = false;


		//public int depth = 0; // Used to sort variable initialization

		// public static bool DEBUG = false;


		public SF_NodePreview texture;
		//	public float[] vector;



		public int id;

		public string nodeName;
		private string nodeNameSearch;
		public string SearchName{
			get{
				if(string.IsNullOrEmpty(nodeNameSearch)){
					return nodeName;
				} else {
					return nodeNameSearch;
				}
			}
			set{
				nodeNameSearch = value;
			}
		}

		public Rect rect;
		public Rect rectInner;
		public Rect lowerRect;

		[SerializeField]
		public SF_NodeConnection[] connectors;

		public SF_NodeConnectionGroup conGroup;

		public float extraWidthOutput = 0f;
		public float extraWidthInput = 0f;

		public SF_Node() {
			//Debug.Log("NODE " + GetType());
		}

		// Quick retrieval of connectors
		public SF_NodeConnection this[string s] {
			get {
				return GetConnectorByStringID(s);
			}
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public bool IsProperty() {
			if( property == null )
				return false;
			if( string.IsNullOrEmpty( property.nameType ) ) {
				property = null;
				return false;
			}
			return true;
		}


		// TODO: Matrices & Samplers?
		// TODO: Precision
		public string GetVariableType() {
			if( texture.CompCount == 1 )
				return "float";
			return "float" + texture.CompCount;
		}

		public string GetVariableName() {
			if( IsProperty() && !ShouldDefineVariable())
				return property.nameInternal + "_var";
			return "node_" + id;
		}

		public virtual void Initialize() {
			// Override
		}

		public void Initialize( string name ) {
			editor = SF_Editor.instance; // TODO, pass in a better way
			status = ScriptableObject.CreateInstance<SF_NodeStatus>().Initialize(this);
			Vector2 pos = editor.mousePosition; // TODO: check where to spawn first
			AssignID();
			this.nodeName = name;
			if( SF_Debug.nodes )
				this.nodeName = ( "[" + id + "] " + this.nodeName );
			texture = ScriptableObject.CreateInstance<SF_NodePreview>().Initialize( this );
			texture.Fill( Color.black );


			// Try to find icon

			texture.LoadAndInitializeIcons(this.GetType());


			pos = editor.nodeView.SubtractNodeWindowOffset( pos );
			InitializeDefaultRect( pos );
		}

		public void AssignID() {
			this.id = ( editor.idIncrement++ );
		}

		public virtual void OnPreGetPreviewData() {
			// Override
		}

		

		public virtual void Update() {
			// Override
		}

		public void InitializeDefaultRect( Vector2 pos ) {


			this.rect = new Rect(
				pos.x - node_width / 2,
				pos.y - node_height / 2,
				node_width,
				( showLowerPropertyBox ? ( node_height ) : ( node_height + 20 ) ) ); // TODO: This seems a bit reversed...
			rectInner = rect;
			rectInner.x = 1;
			rectInner.y = 15;
			rectInner.width = node_width - 3;
			rectInner.height = node_height - 16;

			lowerRect = rectInner;
			lowerRect.y += rectInner.height;
			lowerRect.height = 20;


		}

		//	public virtual void OnConnectedNode(){
		//		Debug.Log("OnConnectedNode " + name);
		//	}

		public virtual void OnUpdateNode( NodeUpdateType updType = NodeUpdateType.Hard, bool cascade = true ) {
			//Debug.Log("Updating " + name);

			

			if( conGroup != null )
				conGroup.Refresh();

			if( !InputsConnected() ) {
				//Debug.Log("Detected missing input on obj " + name);
				texture.OnLostConnection();
			}

			RefreshValue(); // Refresh this value

			if( IsProperty() )
				editor.shaderEvaluator.ApplyProperty(this);

			if( cascade )
				if( connectors != null && connectors.Length > 0 )
					foreach( SF_NodeConnection mCon in connectors ) {
						if( mCon == null )
							continue;
						if( mCon.conType == ConType.cOutput ) {
							foreach( SF_NodeConnection mConOut in mCon.outputCons ) {
								mConOut.node.OnUpdateNode( updType ); // TODO Null ref
							}
						}
					}


			editor.OnShaderModified( NodeUpdateType.Soft );
			if(!SF_Parser.quickLoad && !isGhost)
				Repaint();

		}

		public float[] VectorCopy( float[] original ) {
			float[] retVec = new float[original.Length];
			for( int i = 0; i < original.Length; i++ ) {
				retVec[i] = original[i];
			}
			return retVec;
		}

		public bool VectorEqual( float[] a, float[] b ) {
			if( a.Length != b.Length )
				return false;
			for( int i = 0; i < a.Length; i++ )
				if( a[i] != b[i] )
					return false;
			return true;
		}



		public bool InputsConnected() {
			foreach( SF_NodeConnection con in connectors ) {
				if( con == null )
					break;
				if( con.conType == ConType.cInput )
					if( !con.IsConnected() && con.required )
						return false;
			}
			return true;
		}

		public bool GetInputIsConnected( string id ) {
			SF_NodeConnection con = GetConnectorByStringID(id);
			if( con == null ) {
				Debug.LogError("Tried to find invalid connector by string ID [" + id + "] in node [" + this.GetType().ToString() + "]");
				return false;
			}
			if( con.IsConnected() )
				return true;
			return false;
		}

		public virtual Color NodeOperator( int x, int y ) {
			return new Color(
				NodeOperator(x,y,0),
				NodeOperator(x,y,1),
				NodeOperator(x,y,2),
				NodeOperator(x,y,3)
			);
		}

		public virtual float NodeOperator( int x, int y, int c ) {
			return 0f; // Override this
		}

		public virtual void RefreshValue() {
			// Override this
		}

		public void RefreshValue( int ia, int ib ) {

			//Debug.Log("Refreshing value of " + name);

			if( connectors == null ) {
				Debug.LogError( "Refreshing node with null connector list on " + this.nodeName );
				return;
			} else if( connectors.Length == 0 ) {
				Debug.LogError( "Refreshing node without connectors on " + this.nodeName );
				return;
			} else if( connectors[0] == null ) {
				Debug.LogError( "Refreshing node with null connectors " + this.nodeName );
				return;
			}

			foreach( SF_NodeConnection nc in connectors ) {
				if( nc.required && nc.conType == ConType.cInput && !nc.IsConnected() ) { // Check only required inputs
					texture.OnLostConnection();
					return;
				}
			}

		
			
			texture.Combine();
			if(!SF_Parser.quickLoad && !isGhost)
				SF_Editor.instance.Repaint();
		}

		public virtual bool IsUniformOutput() {
			return false; // Override
		}


		public void PrepareEvaluation() {
			if(IsProperty()){
				editor.shaderEvaluator.ApplyProperty( this );
			}
			varDefined = false;
			varPreDefined = false;
		}

		public float GetInputData( string id, int x, int y, int c ) {

			switch( GetConnectorByStringID(id).inputCon.outputChannel ) {
				case OutChannel.R:
					c = 0;
					break;
				case OutChannel.G:
					c = 1;
					break;
				case OutChannel.B:
					c = 2;
					break;
				case OutChannel.A:
					c = 3;
					break;
			}

			return GetInputData( id )[x, y, c];
		}

		/*
		public SF_NodePreview GetInputData( int id ) {

			if( connectors[id].inputCon == null ) {
				Debug.LogWarning( "Attempt to find input node of connector " + id + " of " + this.nodeName );
			}

			return connectors[id].inputCon.node.texture;
		}*/

		public SF_NodePreview GetInputData( string id ) {

			SF_NodeConnection con = GetConnectorByStringID(id);
			//SF_Node n; // TODO: What was this? Quite recent too. Define and undefine ghosts?

			if( con.inputCon == null ) {

				List<SF_Node> tmpGhosts = new List<SF_Node>();
				con.DefineGhostIfNeeded(ref tmpGhosts);
				//n = tmpGhosts[0];
				tmpGhosts = null;

				Debug.LogWarning( "Attempt to find input node of connector " + id + " of " + this.nodeName );
			}

			//SF_NodePreview ret = con.inputCon.node.texture;




			return con.inputCon.node.texture;
		}

		/*
		public SF_NodeConnection GetInputCon( int id ) {
			if( connectors[id] == null ) {
				Debug.LogError("Failed attempt to find connector [" + id + "] in " + this.nodeName);
				return null;
			}
			if( connectors[id].inputCon == null ) {
				Debug.LogError( "Failed attempt to find node of connector [" + id + "] on " + this.nodeName );
				return null;
			}
			return connectors[id].inputCon;
		}*/

		public SF_NodeConnection GetInputCon( string id ) {
			SF_NodeConnection con = GetConnectorByStringID( id );
			
			if( con == null ) {
				Debug.LogError( "Failed attempt to find connector [" + id + "] in " + this.nodeName );
				return null;
			}
			if(con.inputCon == null) {
				Debug.LogError( "Failed attempt to find input connector of [" + id + "] in " + this.nodeName );
				return null;
			}
			return con.inputCon;
		}




		public virtual int GetEvaluatedComponentCount() {
			// Override
			return 0;
		}

		public bool CanEvaluate() {
			//Debug.Log("Checking if can evaluate " + nodeName);
			for( int i = 0; i < connectors.Length; i++ ) {
				if( connectors[i].required )
					if( !connectors[i].IsConnected() )
						return false;
			}
			return true;
		}


		public void CheckForBrokenConnections() {
			foreach( SF_NodeConnection con in connectors ) {
				if( con.IsConnected() && con.conType == ConType.cInput ) {
					if( con.inputCon.IsDeleted() )
						con.inputCon = null;
				}

			}
		}

		//	public MaterialNode MakeDotProductNode(){
		//		connectors = new MaterialNodeConnector[3]{
		//			new MaterialNodeConnector(this,"A",ConType.cInput),
		//			new MaterialNodeConnector(this,"B",ConType.cInput),
		//			new MaterialNodeConnector(this,"Out",ConType.cOutput)
		//		};
		//		return this;
		//	}


		public void DrawConnections() {
			foreach( SF_NodeConnection con in connectors )
				con.CheckConnection( editor );
		}

		public void Repaint() {
			//SF_Editor.instance.Repaint();
		}

		public bool IsFocused() {
			return rect.Contains( Event.current.mousePosition );
		}

		public bool CheckIfDeleted() {

			if( Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.keyDown && selected ) {
				Delete();
				return true;
			}
			return false;

		}

		public void PrepareWindowColor() {
			Color unselected = IsProperty() ? colorExposed : colorDefault;
			GUI.color = unselected;
		}

		public void ResetWindowColor() {
			GUI.color = colorDefault;
		}


		public void OnPress(){
			if( MouseOverNode( world: true ) && Event.current.isMouse ) {
				editor.ResetRunningOutdatedTimer();
				if( !selected && !SF_GUI.MultiSelectModifierHeld() )
					editor.nodeView.selection.DeselectAll();
				
				StartDragging();
				
				//if(!selected)
				Event.current.Use();
				//Select();
			}
				
		}

		public void OnRelease() {

			isDragging = false;

			if( SF_NodeConnection.pendingConnectionSource != null )
				return;

			if( MouseOverNode( world: true ) && dragDelta.sqrMagnitude < SF_Tools.stationaryCursorRadius ) { // If you released on the node without dragging

				if( SF_GUI.MultiSelectModifierHeld() ) {
					if( selected )
						Deselect();
					else
						Select();
					Event.current.Use();
				} else if(!selected) {
					editor.nodeView.selection.DeselectAll();
					Select();
					Event.current.Use();
				}


				
			}

		}


		bool isDragging = false;
		bool isEditingComment = false;

		public void ContextClick( object o ) {
			string picked = o as string;
			switch(picked){
			case "doc_open":
				SF_Web.OpenDocumentationForNode(this);
				break;
			case "cmt_edit":
				editor.Defocus(deselectNodes:true);
				GUI.FocusControl("node_comment_" + id);
				isEditingComment = true;
				break;
			}
		}

		public bool HasComment(){
			return !string.IsNullOrEmpty(comment);
		}


		public void DrawWindow() {


			

			//Vector2 prev = new Vector2( rect.x, rect.y );
			//int prevCont = GUIUtility.hotControl;



			GUI.Box( rect, nodeName, SF_Styles.NodeStyle );




			ResetWindowColor();
			//rect = GUI.Window( id, rect, NeatWindow, nodeName );
			NeatWindow();

			// If you didn't interact with anything inside...
			if( SF_GUI.PressedLMB() ) {
				OnPress();
			} else if( SF_GUI.ReleasedRawLMB() ) {
				OnRelease();
			} else if( Event.current.type == EventType.ContextClick ) {
				Vector2 mousePos = Event.current.mousePosition;
				if( rect.Contains( mousePos ) ) {
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu();
					editor.ResetRunningOutdatedTimer();
					menu.AddItem( new GUIContent("Edit Comment"), false, ContextClick, "cmt_edit" );
					menu.AddItem( new GUIContent("What does " + nodeName + " do?"), false, ContextClick, "doc_open" );
					menu.ShowAsContext();
					Event.current.Use();
				}
			}




			if( isDragging && Event.current.isMouse)
				OnDraggedWindow( Event.current.delta );



			string focusName = "namelabel" + this.id;
			if( Event.current.type == EventType.keyDown && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) && GUI.GetNameOfFocusedControl() == focusName ) {
				editor.Defocus();
			}

			if( IsProperty() ) {
				PrepareWindowColor();
				Rect nameRect = new Rect( rect );
				nameRect.height = 20;
				nameRect.y -= nameRect.height;
				nameRect.xMax -= 1; // Due to reasons
				//GUI.color = SF_Styles.nodeNameLabelBackgroundColor;
				GUI.Box( nameRect, "", EditorStyles.textField );
				GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
				string oldName = property.nameDisplay;
				
				GUI.SetNextControlName(focusName);
				//Debug.Log();
				string newName = GUI.TextField( nameRect, oldName, SF_Styles.GetNodeNameLabelText() );
				SF_Tools.FormatSerializable( ref newName );
				
				
				if( oldName != newName )
					property.SetName( newName );

				bool focusedField = GUI.GetNameOfFocusedControl() == focusName;

				bool mouseOver = nameRect.Contains( Event.current.mousePosition ) || rect.Contains( Event.current.mousePosition );

				if( focusedField )
					editor.nodeView.selection.DeselectAll();

				if( selected || focusedField || mouseOver ) {
					GUI.color = new Color(1f,1f,1f,0.6f);
					nameRect.x += nameRect.width;
					GUI.Label( nameRect, property.nameInternal, EditorStyles.boldLabel );
					nameRect.y -= 12;

					// Right:
					GUI.color = new Color( 1f, 1f, 1f, 0.3f );
					GUI.Label( nameRect, "Internal name:", EditorStyles.miniLabel);
					
					// Upper:
					nameRect = new Rect( rect );
					nameRect.height = 20;
					nameRect.y -= 33;
					GUI.color = new Color( 1f, 1f, 1f, 0.6f );
					GUI.Label( nameRect, "Property label:", EditorStyles.miniLabel );


					GUI.color = Color.white;
				}
				ResetWindowColor();
				
			}



			if(HasComment() || isEditingComment){
				GUI.color = Color.white;
				Rect cr = rect;
				cr.height = SF_Styles.GetNodeCommentLabelTextField().fontSize + 4;
				cr.width = 2048;
				cr.y -= cr.height + 2;
				if( IsProperty() ){
					cr.y -= 26;
				}

				if(isEditingComment){


					bool clicked = Event.current.rawType == EventType.mouseDown && Event.current.button == 0;
					bool clickedOutside = clicked && !cr.Contains(Event.current.mousePosition);
					bool pressedReturn = Event.current.rawType == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;

					bool defocus = pressedReturn || clickedOutside;

					if( defocus ){
						isEditingComment = false;
						editor.Defocus();
					}
					string fieldStr = "node_comment_" + id;
					GUI.SetNextControlName(fieldStr);
					Rect tmp = cr;
					tmp.width = 256;
					comment = GUI.TextField(tmp, comment, SF_Styles.GetNodeCommentLabelTextField());
					SF_Tools.FormatSerializableComment(ref comment);


					if(!defocus){
						GUI.FocusControl(fieldStr);
					}

				} else {
					GUI.Label(cr, "// " + comment, SF_Styles.GetNodeCommentLabelText());
				}



			}







			
			//GUI.Label( nameRect, "Test", EditorStyles.toolbarTextField );

		}


		public void StartDragging() {
			isDragging = true;
			dragStart = new Vector2( rect.x, rect.y );
			dragDelta = Vector2.zero;
		}


		public static int snapThreshold = 10;
		public static int snapDistance = 256;
		public static Color snapColor = new Color(1f,1f,1f,0.5f);
		public Vector2 dragStart;
		public Vector2 dragDelta;

		public void OnDraggedWindow( Vector2 delta ) {

			editor.ResetRunningOutdatedTimer();

			dragDelta += delta;
			Vector2 finalDelta = new Vector2( rect.x, rect.y );
			rect.x = dragStart.x + dragDelta.x;
			rect.y = dragStart.y + dragDelta.y;
			Event.current.Use();

			

			if(!editor.nodeView.hierarcyMove) // TODO: Snap toggle + make it work properly with hierarchal on
				foreach(SF_Node n in editor.nodes){
					if( n == this )
						continue;
					if( SF_Tools.DistChebyshev( rect.center, n.rect.center ) > snapDistance )
						continue;
					if( n.selected ) // Don't snap to selected nodes
						continue;
					if( Mathf.Abs( n.rect.x - rect.x ) < snapThreshold ) { // LEFT SIDE SNAP
						delta.x -= rect.x - n.rect.x;
						rect.x = n.rect.x;
					} else if( Mathf.Abs( n.rect.y - rect.y ) < snapThreshold ) { // TOP SIDE SNAP
						delta.y -= rect.y - n.rect.y;
						rect.y = n.rect.y;
					} else if( Mathf.Abs( n.rect.center.x - rect.center.x ) < snapThreshold ) { // CENTER HORIZONTAL SNAP
						delta.x -= rect.center.x - n.rect.center.x;
						Vector2 tmp = rect.center;
						tmp.x = n.rect.center.x;
						rect.center = tmp;

						//GUILines.DrawLine( rect.center, n.rect.center, snapColor, snapThreshold * 2, true );

					} else if( Mathf.Abs( n.rect.center.y - rect.center.y ) < snapThreshold ) { // CENTER VERTICAL SNAP
						delta.y -= rect.center.y - n.rect.center.y;
						Vector2 tmp = rect.center;
						tmp.y = n.rect.center.y;
						rect.center = tmp;

						//GUILines.DrawLine( editor.nodeView.AddNodeWindowOffset( rect.center ), editor.nodeView.AddNodeWindowOffset( n.rect.center ), Color.white, snapThreshold * 2, true );
			
					}
				}

			finalDelta =  new Vector2( rect.x, rect.y ) - finalDelta;
			
			editor.nodeView.selection.MoveSelection(finalDelta, ignore:this);

			if( delta != Vector2.zero && editor.nodeView.hierarcyMove && ( GetType() != typeof( SFN_Final ) ) ) {
				MoveUnselectedChildren( delta );
			}

		}

		public void MoveUnselectedChildren( Vector2 delta ) {
			// Find all child nodes
			// TODO: On click or on connect, not every frame
			List<SF_Node> children = new List<SF_Node>();
			children.AddRange( editor.nodeView.selection.Selection );
			children.Add( this );
			AppendUnselectedChildren( children );
			foreach( SF_Node n in editor.nodeView.selection.Selection ) {
				n.AppendUnselectedChildren( children );
			}
			foreach(SF_Node n in editor.nodeView.selection.Selection){
				children.Remove( n );
			}
			children.Remove( this );

			for( int i = 0; i < children.Count; i++ ) {
				children[i].rect.x += delta.x;
				children[i].rect.y += delta.y;
			}
		}

		public void AppendUnselectedChildren( List<SF_Node> list ) {

			// Search all connected
			for( int i = 0; i < connectors.Length; i++ ) {
				if( connectors[i].conType == ConType.cOutput )
					continue;
				if( connectors[i].IsConnected() && !list.Contains( connectors[i].inputCon.node ) ) {
					//if( connectors[i].inputCon.node.ConnectedOutputCount() > 1 )
					//	continue; // Only unique children
					//if( OutputsToAnyOutside( list ) )
					//	continue; // Only unique children
					if( !connectors[i].inputCon.node.selected )
						list.Add( connectors[i].inputCon.node );
					connectors[i].inputCon.node.AppendUnselectedChildren( list );
				}
			}
		}


		/*
		public bool OutputsToAnyOutside( List<SF_Node> list ) {
			foreach( SF_NodeConnection nc in connectors ) {
				if( nc.conType == ConType.cInput )
					continue;
				foreach(SF_NodeConnection con in nc.outputCons){
					if( !list.Contains( con.inputCon.node ) )
						return true;
				}
			}
			return false;
		}*/

		public int ConnectedOutputCount() {
			int count = 0;
			foreach( SF_NodeConnection nc in connectors ) {
				if(nc.conType == ConType.cInput)
					continue;
				count += nc.outputCons.Count;
			}
			return count;
		}

		public void Select() {
			if( !editor.nodeView.selection.Selection.Contains( this ) ) {
				editor.nodeView.selection.Add( this );
				selected = true;
			}
		}

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
		}

		/*
		public void OnSelectedWindow() {
			Debug.Log("Beep!");
		}*/


		public void ProcessInput() {
			/*
			if( IsFocused() )
				Debug.Log( "Mouse over " + nodeName + " rawType = " + Event.current.rawType );
			if( Event.current.rawType == EventType.mouseDown && Event.current.button == 0 ) {
				if( !selected ) {
					Debug.Log("SELECTED");
					Debug.Log("Rect: " + rect + " mPos: " + Event.current.mousePosition);
					editor.nodeView.selection.DeselectAll();
					Select();
				}
					
			}*/
		}


		public virtual bool Draw() {


			ProcessInput();


			DrawHighlight();

			//if(status != null)
			
			

			

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



		public virtual void NeatWindow( ) {
			GUI.BeginGroup( rect );
			GUI.color = Color.white;
			GUI.skin.box.clipping = TextClipping.Overflow;


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


			}

			if( showLowerPropertyBox ) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}

			//GUI.DragWindow();


			GUI.EndGroup( );
			

			//if(rect.center.x)
		}


		public Rect LocalRect() {
			Rect r = new Rect( rect );
			r.x = 0;
			r.y = 0;
			return r;
		}

		public bool MouseOverNode(bool world = false) {

			if( world ) {
				return rect.Contains( Event.current.mousePosition );
			}
				
			else
				return LocalRect().Contains( Event.current.mousePosition );
		}

		public void ColorPickerCorner( Rect r ) {
			//bool prevEnabledState = GUI.enabled;
			//GUI.enabled = MouseOverNode(false);

			//try {
			Rect pickRect = new Rect( r );
			pickRect.height = 15;
			pickRect.width = 45;
			pickRect.y -= pickRect.height + 1;
			pickRect.x += 1;
			Rect pickBorder = new Rect( pickRect );
			pickBorder.xMax -= 18;
			//pickBorder.xMin -= 1;
			//pickBorder.yMax += 1;
			//pickBorder.yMin -= 1;

			float grayscale = texture.dataUniform.grayscale;
			Color borderColor = Color.white - new Color( grayscale, grayscale, grayscale );
			borderColor.a = GUI.enabled ? 1f : 0.25f;
			GUI.color = borderColor;
			GUI.DrawTexture( pickBorder, EditorGUIUtility.whiteTexture );
			GUI.color = Color.white;

			
			
			Color pickedColor = EditorGUI.ColorField( pickRect, texture.ConvertToDisplayColor( texture.dataUniform ) );
			SetColor( pickedColor );
			
		}

		public void SetColor(Color c) {
			if( c != texture.dataUniform ) {
				texture.dataUniform = texture.ConvertToDisplayColor( c );
				if( IsProperty() ) {
					if( property is SFP_Color ) {
						( this as SFN_Color ).OnUpdateValue();
					}
				}
			}
		}


		public string FloatArrToString( float[] arr ) {
			string s = "";
			for( int i = 0; i < arr.Length; i++ )
				s += arr[i] + " ";
			return s;
		}

		public void UseLowerReadonlyValues( bool use ) {
			UseLowerPropertyBox( use );
			showLowerReadonlyValues = use;
		}

		public void UseLowerPropertyBox( bool use, bool always = false ) {
			rect.height = ( use ? ( node_height + 20 ) : ( node_height ) );
			showLowerPropertyBox = use;
			if( always )
				showLowerPropertyBoxAlways = use;
		}

		public virtual void DrawLowerPropertyBox() {
			if( showLowerReadonlyValues )
				DrawLowerReadonlyValues();
		}

		public void DrawLowerReadonlyValues() {

			if( !texture.uniform )
				return;

			if( !InputsConnected() || !texture.uniform ) {
				GUI.enabled = false;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label( lowerRect, "<Input missing>" );
				GUI.enabled = true;
				return;
			}

			Rect tmp = lowerRect;
			tmp.width /= texture.CompCount;
			GUI.enabled = false;
			for( int i = 0; i < texture.CompCount; i++ ) {
				GUI.Box( tmp, "" );
				EditorGUI.SelectableLabel( tmp, texture.dataUniform[i].ToString() );
				tmp.x += tmp.width;
			}
			GUI.enabled = true;
		}

		public virtual void OnDelete() {
			// Override
		}

		public void Delete() {

			if( this is SFN_Final )
				return;

			bool leadsToFinal = status.leadsToFinal;


			if(SF_Debug.nodeActions)
				Debug.Log("Deleting node " + nodeName);

			OnDelete();


			Deselect();
			editor.nodes.Remove( this );
			if( editor.nodeView.treeStatus.propertyList.Contains( this ) )
				editor.nodeView.treeStatus.propertyList.Remove( this );

			for( int i = 0; i < connectors.Length; i++ ) {
				connectors[i].Disconnect(true, false);
				connectors[i] = null;
			}
			connectors = null;


			//SF_Editor.instance.CheckForBrokenConnections();
			//SF_Editor.instance.Repaint();

			texture.DestroyTexture();
			DestroyImmediate( texture );
			ScriptableObject.DestroyImmediate( status );
			
			ScriptableObject.DestroyImmediate(this);

			//if(leadsToFinal){
			//	editor.ShaderOutdated = UpToDateState.OutdatedHard; // TODO: Only if connected
			//	editor.ps.fChecker.UpdateAvailability();
			//}
		}


		// TODO: Channels etc
		// Override if this node has unconnected, required inputs
		public virtual string Evaluate( OutChannel channel = OutChannel.All ) {
			return GetVariableName();
		}


		// Used to see if it's an already defined variable or not
		public string PreEvaluate() {

			if(varDefined)
				return GetVariableName();

			// If it shouldn't be defined, get raw value
			if( !ShouldDefineVariable() ) {
				return Evaluate();
			} else if( !varDefined ) { // If it's not defined yet, define it! Append a new row
				DefineVariable();
			}

			return GetVariableName();
			
		}

		public void DefineVariable() {

			//if(this is SFN_If)
				//Debug.Log("Defining variable");

			if( varDefined ) {
				//Debug.Log( "Already defined!" );
				return;
			}
			PreDefine();
			
			string s = GetVariableType() + " " + GetVariableName() + " = " + Evaluate() + ";";
			SF_Editor.instance.shaderEvaluator.App( s );
			varDefined = true;
		}


		public void DefineGhostsIfNeeded(ref List<SF_Node> ghosts) {

			//Debug.Log("Checking if ghosts should be defined on " + nodeName + "...");


			// Super duper ultra weird and shouldn't be here. Find real issue later // TODO
			if(this == null)
				return;
			
			// TODO: This will prevent multi-ghosting
			/*
			if( editor.shaderEvaluator.ghostNodes.Contains(this) ){
				if(SF_Debug.The(DebugType.GhostNodes))
					Debug.Log("Skipping ghost define for " + nodeName);
				return;
			}

			if(Connectors == null){
				Debug.Log("CHK. GHOST: [" + nodeName + "] Connector count = NULL");
				Debug.Log("WHAT? this = " + this);
				if(this == null)
					return;
			} else
				Debug.Log("CHK. GHOST: [" + nodeName + "] Connector count = " + Connectors.Length);
				*/

			foreach(SF_NodeConnection con in connectors){
				if( con.conType == ConType.cOutput) {
					//Debug.LogError("Ghost node defined on an output: "+nodeName+"[" + con.label + "]");
					continue;
				}
				con.DefineGhostIfNeeded( ref ghosts );
			}
		}


		public void PreDefine() {
			if( varDefined || varPreDefined )
				return;

			string[] preDefs = GetPreDefineRows();
			if( preDefs != null ) {
				foreach( string row in preDefs ) {
					SF_Editor.instance.shaderEvaluator.App( row );
				}
			}
			varPreDefined = true;
		}

		public virtual string[] GetPreDefineRows() {
			return null; // Override this
		}



		public bool ShouldDefineVariable() {
			return ((UsedMultipleTimes() || alwaysDefineVariable) /*&& !varDefined*/);
		}


		public bool UsedMultipleTimes() {
			return ( GetOutputCount() > 1 );
		}

		public int GetOutputCount() {
			int n = 0;
			foreach( SF_NodeConnection con in connectors ) {
				if( con.conType == ConType.cInput )
					continue;
				if( con.IsConnected() ) {
					n += con.outputCons.Count;
				}
			}
			return n;
		}


		public virtual string SerializeSpecialData() {
			return null; // Override!
		}

		public virtual void DeserializeSpecialData( string key, string value ) {
			return; // Override!
		}


		// n:type:SFN_Multiply,id:8,x:33794,y:32535|1-9-0,2-7-0;
		public string Serialize(bool skipExternalLinks = false, bool useSuffixPrefix = false) {
			
			string s = "";
			if(useSuffixPrefix)
				s = "n:";


			s += "type:" + this.GetType().ToString() + ",";
			s += "id:" + this.id + ",";
			s += "x:" + (int)rect.x + ",";
			s += "y:" + (int)rect.y;
			if(IsProperty())
				s += ",ptlb:" + property.nameDisplay;
			if(HasComment())
				s += ",cmnt:" + comment;
			
			
			//
			string specialData = SerializeSpecialData(); // <-- This is the unique data for each node
			if( !string.IsNullOrEmpty( specialData ) ) {
				s += "," + specialData;
			}
			//

			if( HasAnyInputConnected(skipExternalLinks) ) {
				s += "|";
				int linkCount = 0;
				int i = 0;
				foreach( SF_NodeConnection con in connectors ) { // List connections, connected inputs only
					if( con.conType == ConType.cOutput ) { i++; continue; }
					if( !con.IsConnected() ) { i++; continue; }
					
					if(skipExternalLinks)
						if(!con.inputCon.node.selected){ i++; continue; }


					string link = con.GetIndex() + "-" + connectors[i].inputCon.node.id + "-" + connectors[i].inputCon.GetIndex();

					if( linkCount > 0 )
						s += ",";
					s += link;

					linkCount++;
					i++;
				}
			}
			
			if(useSuffixPrefix)
				s += ";";

			return s;
		}

		public void TrySerialize( XmlWriter xml, string key, object val ) {
			if( val == null )
				return;
			string str = val.ToString();
			if( string.IsNullOrEmpty( str ) )
				return;
			xml.WriteElementString( key, str );
		}

		/*
		public virtual string SerializeCustomData() {
			return ""; // Override
		}*/

		public void DrawConnectors() {



			int yOut = 0;
			int yIn = 0;

			int spacing = 20;

			if( connectors != null ) {
				for( int i = 0; i < connectors.Length; i++ ) {
					Vector2 pos = new Vector2( rect.width + rect.x, 16 + rect.y );


					if( connectors[i].conType == ConType.cInput ) {
						pos.y += yIn * spacing;
						yIn++;
					} else {
						pos.y += yOut * spacing;
						yOut++;
					}

					connectors[i].Draw( pos );


				}
			}



			/*if( DEBUG ) {
				Rect tmp = new Rect( rect );
				tmp.height = 20;
				tmp.width = 250;
				tmp.y -= tmp.height;
				GUI.Box( tmp, depth.ToString(), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "cCons: " + CalcConnectionCount().ToString(), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Conctrs: " + connectors.Length, EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Editor: " + ( editor != null ), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Property: " + IsProperty(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				if( conGroup != null ) {
					GUI.Box( tmp, "C Group out: " + conGroup.output, EditorStyles.miniLabel );
					tmp.y -= tmp.height;
					GUI.Box( tmp, "C Group ins: " + conGroup.inputs.Length, EditorStyles.miniLabel );
					tmp.y -= tmp.height;
					GUI.Box( tmp, "C Group hash: " + conGroup.GetHashCode(), EditorStyles.miniLabel );
					tmp.y -= tmp.height;
				} else {
					GUI.Box( tmp, "C Group is NULL", EditorStyles.miniLabel );
					tmp.y -= tmp.height;
				}
				GUI.Box( tmp, "Type: " + GetType().ToString(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Hash: " + GetHashCode(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				if(texture != null)
					GUI.Box( tmp, "Unif: " + texture.uniform );
				tmp.y -= tmp.height;
			}*/

		}

		public SF_NodeConnection GetConnectorByID(string s) {
			int number;
			if( int.TryParse( s, out number ) ) {
				return connectors[number];
			} else {
				return GetConnectorByStringID(s);
			}
		}

		public SF_NodeConnection GetConnectorByStringID(string s) {
			foreach( SF_NodeConnection con in connectors ) {
				if( !con.HasID() )
					continue;
				if( s == con.strID )
					return con;
			}

			Debug.LogError("Unsuccessfully tried to find connector by string ID [" + s + "] in node " + nodeName);
			return null;
		}

		public bool HasAnyInputConnected(bool skipExternalLinks = false) {
			foreach( SF_NodeConnection con in connectors )
				if( con.IsConnected() && con.conType == ConType.cInput ){
					if(skipExternalLinks){
						if(con.inputCon.node.selected)
							return true;
					} else {
						return true;
					}
				}
					
			return false;
		}

		public int CalcConnectionCount() {
			int i = 0;
			foreach( SF_NodeConnection con in connectors ) {
				if( con.IsConnected() )
					i++;
			}
			return i;
		}
	}

}