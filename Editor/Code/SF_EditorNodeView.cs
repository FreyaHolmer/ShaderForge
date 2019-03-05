using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

namespace ShaderForge {

	public enum ConnectionLineStyle { Bezier, Linear, Rectilinear };

	[System.Serializable]
	public class SF_SetNodeSource {

		public SF_NodeConnector con;

		public SF_SetNodeSource( SF_Node node ) {
			con = node.connectors[0];
		}

		public int NodeID {
			get { return con.node.id; }
		}

		public string Name {
			get { return con.node.variableName; }
		}

	}

	[System.Serializable]
	public class SF_EditorNodeView : ScriptableObject {

		SF_Editor editor;

		const int TOOLBAR_HEIGHT = 18;
		[SerializeField]
		public Vector2 cameraPos = Vector3.zero;

		[SerializeField]
		bool panCamera = false;

		[SerializeField]
		Vector2 mousePosStart;
		public Rect rect;
		public GUIStyle toolbarStyle;

		public List<SF_SetNodeSource> relayInSources;
		public string[] relayInNames;

		public SF_SelectionManager selection;

		public SF_NodeTreeStatus treeStatus;

		

	

		public SF_EditorNodeView() {

		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public void RefreshRelaySources() {
			relayInSources = new List<SF_SetNodeSource>();
			for( int i = 0; i < editor.nodes.Count; i++ ) {
				if( editor.nodes[i] is SFN_Set ) {
					relayInSources.Add( new SF_SetNodeSource(editor.nodes[i]) );
				}
			}
			relayInSources.Sort( ( a, b ) => a.Name.CompareTo( b.Name ) );
			relayInNames = relayInSources.Select( x => x.Name ).ToArray();
		}

		// Only the node ID is serialized - this is used to ensure proper display in the GUI
		// Returns -1 if the relay ID is missing
		public int NodeIdToRelayId(int nodeId) {
			if( relayInSources != null ) {
				for( int i = 0; i < relayInSources.Count; i++ ) {
					if( relayInSources[i].NodeID == nodeId ) {
						return i;
					}
				}
			}
			return -1;
		}

		public SF_EditorNodeView Initialize( SF_Editor editor ) {
			this.editor = editor;
			selection = ScriptableObject.CreateInstance<SF_SelectionManager>().Initialize( editor );
			treeStatus = ScriptableObject.CreateInstance<SF_NodeTreeStatus>().Initialize(editor);
			rect = new Rect();
			cameraPos = new Vector2( 32768 - 400, 32768 - 300 );
			toolbarStyle = new GUIStyle( EditorStyles.toolbar );
			toolbarStyle.fixedHeight = TOOLBAR_HEIGHT;
			return this;
		}


		// Erasing nodes with cut line: (alt+RMB)

		public Vector2 cutStart = Vector3.zero;
		public bool isCutting = false;

		public void StartCutting(){
			isCutting = true;
			cutStart = editor.nodeView.GetNodeSpaceMousePos();
		}

		public void StopCutting(){
			List<SF_NodeConnector> disconnectors = new List<SF_NodeConnector>();
			for (int i = 0; i < editor.nodes.Count; i++) {
				SF_Node n = editor.nodes [i];
				for (int j = 0; j < n.connectors.Length; j++) {
					SF_NodeConnector con = n.connectors [j];
					if (con.IsConnected () && con.conType == ConType.cInput) {
						if (con.conLine.aboutToBeDeleted) {
							disconnectors.Add(con);
						}
					}
				}
			}

			if(disconnectors.Count == 0){
				isCutting = false;
				return;
			}

			UnmarkDeleteHighlights();

			//Undo.RecordObject((Object)con, "cut"
			string undoMsg = "cut ";
			if(disconnectors.Count > 1){
				undoMsg += disconnectors.Count + " ";
				undoMsg += "connections";
			} else {
				undoMsg += "connection: ";
				undoMsg += disconnectors[0].node.nodeName;
				undoMsg += "[" + disconnectors[0].label + "]";
				undoMsg += " <--- ";
				undoMsg += "[" + disconnectors[0].inputCon.label + "]";
				undoMsg += disconnectors[0].inputCon.node.nodeName;
			} // = disconnectors.Count > 1 ? "cut "+disconnectors.Count+" connections" : "cut connection " + disconnectors[i].node.name + "[" + 

			foreach(SF_NodeConnector con in disconnectors){
				Undo.RecordObject(con, undoMsg);
			}

			foreach(SF_NodeConnector con in disconnectors){
				con.Disconnect();
			}

			isCutting = false;

		}

		public void UnmarkDeleteHighlights(){
			foreach(SF_Node n in editor.nodes){
				foreach(SF_NodeConnector con in n.connectors){
					if(con.IsConnected() && con.conType == ConType.cInput){
						con.conLine.aboutToBeDeleted = false;
					}
				}
			}
		}


		public float zoom = 1f;
		public float zoomTarget = 1f;

		public void SetZoom(float setZoom){
			Vector2 oldWidth = new Vector2(rect.width,rect.height)/zoom;
			zoom = ClampZoom(setZoom);
			Vector2 newWidth = new Vector2(rect.width,rect.height)/zoom;
			Vector2 delta = newWidth - oldWidth;

			Vector2 normalizedMouseCoords = (Event.current.mousePosition - new Vector2(editor.separatorLeft.rect.xMax,editor.TabOffset));

			normalizedMouseCoords.x /= rect.width;
			normalizedMouseCoords.y /= rect.height;



			cameraPos -= Vector2.Scale(delta, normalizedMouseCoords);

			if(delta.sqrMagnitude != 0f){


				// Correct in here to prevent going outside the bounds
				BoundsAdjustCamera();
			}


			if(zoom == 1f)
				SnapCamera();

		}



		public void BoundsAdjustCamera(){
			/*
			Rect wrapped = GetNodeEncapsulationRect().Margin(256);
			Rect view = ScreenSpaceToZoomSpace(rect);

			Vector2 toCenter = (view.center - wrapped.center)*0.5f;

			float camBottom = cameraPos.y + rect.height/zoom - 22;
			float camTop = cameraPos.y;
			float camRight = cameraPos.x + rect.width/zoom - editor.separatorLeft.rect.xMax;
			float camLeft = cameraPos.x - editor.separatorLeft.rect.xMax;

			Vector2 deltaTotal = Vector2.zero; 

			if( camBottom > wrapped.yMax)
				deltaTotal -= new Vector2(0f,camBottom-wrapped.yMax);
			if(camTop < wrapped.yMin){
				deltaTotal -= new Vector2(0f,camTop-wrapped.yMin);
			}
			if(camRight > wrapped.xMax)
				deltaTotal -= new Vector2(camRight-wrapped.xMax,0f);
			if(camLeft < wrapped.xMin)
				deltaTotal -= new Vector2(camLeft-wrapped.xMin,0f);

			cameraPos += deltaTotal;

*/

		}


		public float ClampZoom(float in_zoom){
			return Mathf.Clamp(in_zoom,0.125f,1f);
		}


		public void OnLocalGUI( Rect r ) {


			//r = r.PadTop(Mathf.CeilToInt(22*zoom));





			editor.mousePosition = Event.current.mousePosition;
			rect = r;

			

			// TOOLBAR
			//DrawToolbar( new Rect( rect.x, rect.y, rect.width, TOOLBAR_HEIGHT ) );

			

			Rect localRect = new Rect( r );
			localRect.x = 0;
			localRect.y = 0;

			//rect.y += TOOLBAR_HEIGHT;
			//rect.height -= TOOLBAR_HEIGHT;




			// VIEW
			Rect rectInner = new Rect( rect );
			rectInner.width = float.MaxValue / 2f;
			rectInner.height = float.MaxValue / 2f;


			// TEMP:
//			Rect btn = rectInner;
//			btn.width = 64;
//			btn.height = 24;
//			if(SF_Debug.renderDataNodes){
//				if(selection.Selection.Count > 0){
//					if(GUI.Button(btn,"NSS")){
//						editor.TakeNodePreviewScreenshot();
//					}
//				}
//			}



			if(Event.current.type == EventType.Repaint){
				nodeSpaceMousePos = ScreenSpaceToZoomSpace( Event.current.mousePosition );

			}





			bool mouseOverNode = false;

			
			
			
			SF_ZoomArea.Begin(zoom,rect,cameraPos);
			{
				selection.OnGUI(); // To detect if you press things
				if(editor.nodeView != null)
					editor.nodeView.selection.DrawBoxSelection();

				if(Event.current.type == EventType.Repaint){
					viewSpaceMousePos = ZoomSpaceToScreenSpace( Event.current.mousePosition );
				}
				// NODES
				if( editor.nodes != null ) {

					// If we're repainting, draw in reverse to sort properly
					//if(Event.current.rawType == EventType.repaint){
						for (int i = editor.nodes.Count - 1; i >= 0; i--) {
							if( !editor.nodes[i].Draw() )
								break;
						}
					/*} else {
						for(int i=0;i<editor.nodes.Count;i++) {
							if( !editor.nodes[i].Draw() )
								break;
						}
					}*/

					if(!mouseOverNode){
						for(int i=0;i<editor.nodes.Count;i++){
							if(editor.nodes[i].MouseOverNode(world:true)){
								mouseOverNode = true;
							}
						}
					}

					if( Event.current.type == EventType.Repaint ) {
						for( int i=0; i < editor.nodes.Count; i++ )
							editor.nodes[i].DrawConnectors();
					}
					
				}


				UpdateCutLine();

				UpdateCameraPanning();

			}
			SF_ZoomArea.End(zoom);


			if(!SF_Node.isEditingAnyNodeTextField)
				SF_Editor.instance.UpdateKeyHoldEvents(mouseOverNode);


			if ( MouseInsideNodeView(false) && Event.current.type == EventType.ScrollWheel){
				zoomTarget = ClampZoom(zoomTarget * (1f-Event.current.delta.y*0.02f));
			}


















			SetZoom( Mathf.Lerp(zoom,zoomTarget, 0.2f ));





			if( Event.current.type == EventType.ContextClick && !SF_GUI.HoldingAlt() ) {
				Vector2 mousePos = Event.current.mousePosition;
				if( rect.Contains( mousePos ) ) {
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu();

					// First item is for creating a comment box
					//menu.AddItem( new GUIContent("Create comment box"), false, ContextClick, mousePos );

					//menu.AddSeparator("");

					for( int i = 0; i < editor.nodeTemplates.Count; i++ ) {
						if( editor.ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.Deferred && !editor.nodeTemplates[i].availableInDeferredPrePass)
							continue; // Skip forward nodes when in deferred
						menu.AddItem( new GUIContent( editor.nodeTemplates[i].fullPath ), false, ContextClick, editor.nodeTemplates[i] );
					}
					editor.ResetRunningOutdatedTimer();
					menu.ShowAsContext();
					Event.current.Use();
				}
			}

			if( Event.current.type == EventType.DragPerform ) {
				Object droppedObj = DragAndDrop.objectReferences[0];
				if( droppedObj is Texture2D || droppedObj is RenderTexture ) {
					SFN_Tex2d texNode = editor.nodeBrowser.OnStopDrag() as SFN_Tex2d;
					texNode.TextureAsset = droppedObj as Texture;
					texNode.OnAssignedTexture();
					Event.current.Use();
				}
			}

			if( Event.current.type == EventType.DragUpdated && Event.current.type != EventType.DragPerform ) {
				if( DragAndDrop.objectReferences.Length > 0 ) {
					Object dragObj = DragAndDrop.objectReferences[0];
					if( dragObj is Texture2D || dragObj is RenderTexture  ) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						if( !editor.nodeBrowser.IsPlacing() )
							editor.nodeBrowser.OnStartDrag( editor.GetTemplate<SFN_Tex2d>() );
						else
							editor.nodeBrowser.UpdateDrag();
					} else {
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					}
				} else {
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}



			


			// If release
			if( MouseInsideNodeView( false ) && Event.current.type == EventType.MouseUp) {
				bool ifCursorStayed = Vector2.SqrMagnitude( mousePosStart - Event.current.mousePosition ) < SF_Tools.stationaryCursorRadius;

				if( ifCursorStayed && !SF_GUI.MultiSelectModifierHeld() )
					selection.DeselectAll(registerUndo:true);


				//editor.Defocus( deselectNodes: ifCursorStayed );
			}

			if( SF_GUI.ReleasedRawLMB() ) {
				SF_NodeConnector.pendingConnectionSource = null;
			}

			// If press
			if( Event.current.type == EventType.MouseDown && MouseInsideNodeView( false ) ) {
				//bool ifNotHoldingModifier = !SF_GUI.MultiSelectModifierHeld();
				mousePosStart = Event.current.mousePosition;
				editor.Defocus();
			}


			if(!editor.screenshotInProgress){

				Rect logoRect = rect;
				logoRect.y -= 14;
				logoRect.x += 1;
				logoRect.width = SF_GUI.Logo.width;
				logoRect.height = SF_GUI.Logo.height;
				GUI.color = new Color(1f,1f,1f,0.5f);
				GUI.DrawTexture( logoRect, SF_GUI.Logo );

				logoRect.y += logoRect.height;
				logoRect.height = 16;
				
				GUI.Label(logoRect, "v"+SF_Tools.version, EditorStyles.boldLabel);
				GUI.color = Color.white;


			}


		}

		// For connecting procedural materials to the main node
		public SF_Node TryLinkIfExistsAndOpenSlotAvailable(Texture tex, string propertyName, SF_NodeConnector connector, string outChannel, SF_Node prevNode = null){

			if(tex){
				SFN_Tex2d tNode = editor.AddNode<SFN_Tex2d>();
				if(prevNode != null){
					Rect r = tNode.rect;
					r = r.MovedDown(1);
					r.y += 64;
					tNode.rect = r;
				}
				tNode.TextureAsset = tex;
				tNode.property.SetName(propertyName);
				tNode.OnAssignedTexture();
				if(connector.enableState == EnableState.Enabled && connector.availableState == AvailableState.Available && !connector.IsConnected()){
					connector.LinkTo(tNode[outChannel]);
				}
				return tNode;
			}
			return null;
		}

		public void UpdateCutLine(){

			if(SF_GUI.HoldingAlt() && Event.current.type == EventType.MouseDown && Event.current.button == 1){ // Alt + RMB drag
				StartCutting();
			} else if(SF_GUI.ReleasedRawRMB()){
				StopCutting();
			}
			
			if(isCutting){
				Vector2 cutEnd = GetNodeSpaceMousePos();

				GUILines.DrawDashedLine(editor, cutStart, cutEnd, Color.white, 5f);
				
				
				foreach(SF_Node n in editor.nodes){
					foreach(SF_NodeConnector con in n.connectors){
						if(con.IsConnected() && con.conType == ConType.cInput && con.enableState != EnableState.Hidden){
							Vector2 intersection = Vector2.zero;
							if(con.conLine.Intersects(cutStart, cutEnd, out intersection)){
								
								con.conLine.aboutToBeDeleted = true;
								
								Vector2 hit = editor.nodeView.ScreenSpaceToZoomSpace(intersection);
								
								float scale = 5f;
								float scaleDiff = 0.95f;
								//Vector2 rg, up, lf, dn;
								
								
								//Vector2 localRight = (cutStart-cutEnd).normalized;
								//Vector2 localUp = new Vector2(localRight.y,-localRight.x);
								
								//rg = hit + localRight * scale;
								//up = hit + localUp * scale;
								//lf = hit - localRight * scale;
								//dn = hit - localUp * scale;
								Color c0 = new Color(1f,0.1f,0.1f,0.9f);
								Color c1 = new Color(1f,0.1f,0.1f,0.7f);
								Color c2 = new Color(1f,0.1f,0.1f,0.5f);
								Color c3 = new Color(1f,0.1f,0.1f,0.3f);
								
								GUILines.DrawDisc(hit,scale,c0);
								GUILines.DrawDisc(hit,scale-scaleDiff,c1);
								GUILines.DrawDisc(hit,scale-scaleDiff*2,c2);
								GUILines.DrawDisc(hit,scale-scaleDiff*3,c3);
								
								//GUILines.DrawLine(rg,up,Color.red,2f,true);
								//GUILines.DrawLine(up,lf,Color.red,2f,true);
								//GUILines.DrawLine(lf,dn,Color.red,2f,true);
								//GUILines.DrawLine(dn,rg,Color.red,2f,true);
								
								
								
								
								
								continue;
							} else {
								con.conLine.aboutToBeDeleted = false;
							}
						}
					}
				}
				
				
			}

		}



		public Rect GetNodeEncapsulationRect(){

			Rect r = editor.nodes[0].rect; // No need for null check, there should always be a main node
			foreach( SF_Node n in editor.nodes ) {
				r = SF_Tools.Encapsulate( r, n.rect );
			}
			return r;

		}

		public void CenterCamera() {

			// Find midpoint of all nodes
			Rect r = GetNodeEncapsulationRect();
			
			// Move Camera
			cameraPos = r.center - new Vector2( 0f, Screen.height * 0.5f );
			SnapCamera();
		}




		public void ContextClick( object o ) {
			// Add node
			SF_EditorNodeData nodeData = o as SF_EditorNodeData;
			editor.AddNode( nodeData, true );
		}



		public void UpdateDebugInput() {

			if( Event.current.type != EventType.KeyDown )
				return;

			if( Event.current.keyCode == KeyCode.UpArrow ) {
				HierarchalRefresh();
			}


			if( Event.current.keyCode == KeyCode.DownArrow ) {
				Debug.Log( GetNodeDataSerialized() );
			}


		}


		public void AssignDepthValuesToNodes() {
			foreach( SF_Node n in editor.nodes ) {
				n.depth = 0;
			}
			// Recurse some depth!
			// TODO: Run this for disconnected islands of nodes too
			//Debug.Log("SFN_FINAL exists = " + (editor.materialOutput != null));
			AddDepthToChildrenOf( editor.mainNode, 0 );
		}

		void AddDepthToChildrenOf( SF_Node n, int carry ) {
			carry++;
			n.depth = Mathf.Max( carry, n.depth ); ;
			for( int i = 0; i < n.connectors.Length; i++ ) {
				if( n.connectors[i].conType == ConType.cOutput ) // Ignore outputs, we came from here!
					continue;
				if( !n.connectors[i].IsConnected() ) // Ignore unconnected inputs
					continue;
				AddDepthToChildrenOf( n.connectors[i].inputCon.node, carry );
			}
		}

		public void HierarchalRefresh() {

//			AssignDepthValuesToNodes();
//
//			int maxDepth = 0; // Deepest level
//			foreach( SF_Node n in editor.nodes ) {
//				if( maxDepth < n.depth )
//					maxDepth = n.depth;
//			}
//
//			
//			// Relink everything
//			int depth = maxDepth;
//			while( depth > 0 ) {
//				for(int i=0; i<editor.nodes.Count; i++){
//					SF_Node n = editor.nodes[i];
//					if( n.depth == depth ) {
//						n.OnUpdateNode(NodeUpdateType.Soft, cascade:true);
//					}
//				}
//				depth--;
//			}


			foreach(SF_Node n in editor.GetDepthSortedDependencyTreeForConnectedNodes(reverse:true)){
				n.OnUpdateNode(NodeUpdateType.Soft, cascade:false);
			}
			

			// Soft Update node previews
			/*
			depth = maxDepth;
			while( depth > 0 ) {
				foreach( SF_Node n in editor.nodes ) {
					if( n.depth == depth ) {
						//n.RefreshValue();
						//n.OnUpdateNode( NodeUpdateType.Soft );
					}
						
				}
				depth--;
			}
			 * */

		}


		public void ReconnectConnectedPending() {
			AssignDepthValuesToNodes();

			int maxDepth = 0; // Deepest level
			foreach( SF_Node n in editor.nodes ) {
				if( maxDepth < n.depth )
					maxDepth = n.depth;
			}


			int depth = maxDepth;
			while( depth > 0 ) {
				//foreach( SF_Node n in editor.nodes ) {
				for( int i = 0; i < editor.nodes.Count; i++ ) {
					SF_Node n = editor.nodes[i];
					if( n.depth == depth ) {
						foreach( SF_NodeConnector con in n.connectors ) {
							if( con.conType == ConType.cOutput )
								continue;
							if( !con.IsConnectedAndEnabled() )
								continue;
							if( con.valueType != ValueType.VTvPending )
								continue;
							con.inputCon.LinkTo( con, LinkingMethod.Default );
						}
					}
				}
				depth--;
			}
		}




		public string GetNodeDataSerialized() {

			// TODO; move parts of this to their respective places

			string header = "";
			header += "// Shader created with " + SF_Tools.versionString + " \n";
			header += "// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/\n";
			header += "// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/\n";
			header += "// Note: Manually altering this data may prevent you from opening it in Shader Forge\n";
			header += "/" + "*"; // Hurgh!

			string sData = "";
			sData += "SF_DATA;"; // TODO: Multi-pass, shader settings etc
			sData += "ver:" + SF_Tools.version + ";";
			sData += "sub:START;";
			sData += "pass:START;";
			sData += editor.ps.Serialize() + ";";

			foreach( SF_Node node in editor.nodes )
				sData += node.Serialize(false,useSuffixPrefix:true);

			if(editor.nodeView.treeStatus.propertyList.Count > 0)
				sData += editor.nodeView.treeStatus.SerializeProps() + ";";

			string footer = "pass:END;sub:END;";
			footer += "*" + "/";
			return ( header + sData + footer );
		}

		public float lastChangeTime;

		float GetTime(){
			return (float)EditorApplication.timeSinceStartup;
		}

		public float GetTimeSinceChanged(){
			return GetTime() - lastChangeTime;
		}
		
		public void DrawRecompileTimer(Rect r){

			if(!SF_Settings.autoCompile)
				return; // Don't draw recompile timer when autoRecompile is unchecked

			float delta = GetTimeSinceChanged();

			if(delta > 1.12f)
				return;

			r.width *= Mathf.Clamp01(delta);
			if(SF_GUI.ProSkin){
				GUI.Box(r,string.Empty);
				GUI.Box(r,string.Empty);
				GUI.Box(r,string.Empty);
			} else {
				GUI.color = new Color(1f,1f,1f,0.4f);
				GUI.Box(r,string.Empty);
				GUI.color = Color.white;
			}
		}

		void DrawToolbar( Rect r ) {
			
		}

		void UpdateCameraPanning() {


			if( SF_GUI.ReleasedCameraMove() ) {
				panCamera = false;
			}

			bool insideNodeView = MouseInsideNodeView( true );
			bool dragging = ( Event.current.type == EventType.MouseDrag && panCamera );
			bool connecting = SF_NodeConnector.IsConnecting();
			bool rotatingPreview = editor.preview.isDraggingLMB;
			bool placingNode = editor.nodeBrowser.IsPlacing();
			bool draggingSeparators = editor.DraggingAnySeparator();


			if(connecting){
				// Pan camera when cursor nears edges while making a connection
				Vector2 mousePosInNodeViewScreenSpace = ZoomSpaceToScreenSpace(Event.current.mousePosition) - Vector2.right*editor.separatorLeft.rect.xMax;

				float areaWidth;
				if(SF_Settings.showNodeSidebar)
					areaWidth = editor.separatorRight.rect.xMin - editor.separatorLeft.rect.xMax;
				else
					areaWidth = Screen.width - editor.separatorLeft.rect.xMax;
				float areaHeight = editor.nodeView.rect.height;
				float dragPanMargin = 32f;
				float panSpeed = 0.2f;
				float leftMag = Mathf.Clamp(-mousePosInNodeViewScreenSpace.x + dragPanMargin, 0f, dragPanMargin);
				float rightMag = Mathf.Clamp( mousePosInNodeViewScreenSpace.x - areaWidth + dragPanMargin, 0f, dragPanMargin);
				float topMag = Mathf.Clamp( -mousePosInNodeViewScreenSpace.y + dragPanMargin , 0f, dragPanMargin);
				float bottomMag = Mathf.Clamp( mousePosInNodeViewScreenSpace.y - areaHeight + dragPanMargin , 0f, dragPanMargin);
				cameraPos += new Vector2(rightMag-leftMag, bottomMag-topMag)*panSpeed;
			}


			bool doingSomethingElse = connecting || rotatingPreview || placingNode || draggingSeparators;
			bool dragInside = dragging && insideNodeView;

			if( dragInside && !doingSomethingElse ) {

				//if( !SF_GUI.MultiSelectModifierHeld() )
				//	selection.DeselectAll();
				//Debug.Log("Delta: " + Event.current.delta);
				cameraPos -= Event.current.delta;
				SnapCamera();

				BoundsAdjustCamera();
				editor.Defocus();
				//Debug.Log( "USING" );
				Event.current.Use();
			}


			if( SF_GUI.PressedCameraMove() ) {
				panCamera = true;
			}

		

		}

		public Vector2 nodeSpaceMousePos;
		public Vector2 viewSpaceMousePos;

		public Vector2 GetNodeSpaceMousePos() {
			return nodeSpaceMousePos;
		}


		public bool MouseInsideNodeView( bool offset = false ) {

			if( offset ) {
				return rect.Contains( viewSpaceMousePos/*ZoomSpaceToScreenSpace( Event.current.mousePosition )*/ );
			} else {
				return rect.Contains( Event.current.mousePosition );
			}

		} 

		void SnapCamera(){
			cameraPos.x = Mathf.Round(cameraPos.x);
			cameraPos.y = Mathf.Round(cameraPos.y);
		} 

		 
		public Vector2 ZoomSpaceToScreenSpace( Vector2 in_vec ) {
			return (in_vec - cameraPos + editor.separatorLeft.rect.TopRight() )*zoom + rect.TopLeft() + (Vector2.up * (editor.TabOffset))*(zoom-1);
		}
		public Rect ZoomSpaceToScreenSpace( Rect in_rect ) {
			Vector2 offset = ZoomSpaceToScreenSpace(in_rect.TopLeft());
			in_rect.x = offset.x;
			in_rect.y = offset.y;
			in_rect.width /= zoom;
			in_rect.height /= zoom;
			//in_rect.x += -cameraPos.x;
			//in_rect.y += -cameraPos.y;
			return in_rect;
		}
		public Vector2 ScreenSpaceToZoomSpace( Vector2 in_vec ) {
			return ( in_vec - (Vector2.up * (editor.TabOffset))*(zoom-1) - rect.TopLeft() ) / zoom - editor.separatorLeft.rect.TopRight() + cameraPos;
			//return in_vec + cameraPos;
		}

		// az + b + x(z-1)


		public Rect ScreenSpaceToZoomSpace( Rect in_rect ) {
			//in_rect.x -= -cameraPos.x;
			//in_rect.y -= -cameraPos.y;
			Vector2 offset = ScreenSpaceToZoomSpace(in_rect.TopLeft());
			in_rect.x = offset.x;
			in_rect.y = offset.y;
			in_rect.width *= zoom;
			in_rect.height *= zoom;

			return in_rect;
		}


	}

}

