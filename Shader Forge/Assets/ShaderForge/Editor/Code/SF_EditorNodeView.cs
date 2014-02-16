using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.IO;

namespace ShaderForge {

	public enum ConnectionLineStyle { Bezier, Linear, Rectilinear };

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



		public SF_SelectionManager selection;

		public SF_NodeTreeStatus treeStatus;

	

		public SF_EditorNodeView() {

		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
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
			for (int i = 0; i < editor.nodes.Count; i++) {
				SF_Node n = editor.nodes [i];
				for (int j = 0; j < n.connectors.Length; j++) {
					SF_NodeConnector con = n.connectors [j];
					if (con.IsConnected () && con.conType == ConType.cInput) {
						if (con.conLine.aboutToBeDeleted) {
							con.Disconnect ();
						}
					}
				}
			}
			isCutting = false;
			UnmarkDeleteHighlights();
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

			Vector2 normalizedMouseCoords = (Event.current.mousePosition - new Vector2(editor.separatorLeft.rect.xMax,22));

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

			selection.OnGUI(); // To detect if you press things

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


			
			if ( MouseInsideNodeView(false) && Event.current.type == EventType.ScrollWheel){
				zoomTarget = ClampZoom(zoomTarget * (1f-Event.current.delta.y*0.02f));
			}

			SetZoom( Mathf.Lerp(zoom,zoomTarget, 0.2f ));




			SF_ZoomArea.Begin(zoom,rect,cameraPos);
			{
				// NODES
				if( editor.nodes != null ) {
					for(int i=0;i<editor.nodes.Count;i++) {
						if( !editor.nodes[i].Draw() )
							break;
					}
					for(int i=0;i<editor.nodes.Count;i++)
						editor.nodes[i].DrawConnectors();
				}


				UpdateCutLine();

				UpdateCameraPanning();

			}
			SF_ZoomArea.End(zoom);








			if( Event.current.type == EventType.ContextClick && !SF_GUI.HoldingAlt() ) {
				Vector2 mousePos = Event.current.mousePosition;
				if( rect.Contains( mousePos ) ) {
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu();
					for( int i = 0; i < editor.nodeTemplates.Count; i++ ) {
						menu.AddItem( new GUIContent( editor.nodeTemplates[i].fullPath ), false, ContextClick, editor.nodeTemplates[i] );
					}
					editor.ResetRunningOutdatedTimer();
					menu.ShowAsContext();
					Event.current.Use();
				}
			}






			if( Event.current.type == EventType.DragPerform ) {
				if( DragAndDrop.objectReferences[0] is Texture2D ) {
					SFN_Tex2d texNode = editor.nodeBrowser.OnStopDrag() as SFN_Tex2d;
					texNode.TextureAsset = DragAndDrop.objectReferences[0] as Texture2D;
					texNode.OnAssignedTexture();
					Event.current.Use();
				}
			}

			if( Event.current.type == EventType.dragUpdated && Event.current.type != EventType.DragPerform ) {
				if( DragAndDrop.objectReferences.Length > 0 ) {
					if( DragAndDrop.objectReferences[0].GetType() == typeof( Texture2D ) ) {
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
			if( MouseInsideNodeView( false ) && Event.current.type == EventType.mouseUp) {
				bool ifCursorStayed = Vector2.SqrMagnitude( mousePosStart - Event.current.mousePosition ) < SF_Tools.stationaryCursorRadius;

				if( ifCursorStayed && !SF_GUI.MultiSelectModifierHeld() )
					selection.DeselectAll();


				//editor.Defocus( deselectNodes: ifCursorStayed );
			}

			if( SF_GUI.ReleasedRawLMB() ) {
				SF_NodeConnector.pendingConnectionSource = null;
			}

			// If press
			if( Event.current.type == EventType.mouseDown && MouseInsideNodeView( false ) ) {
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
				
				GUI.Label(logoRect, SF_Tools.versionStage+" "+SF_Tools.version, EditorStyles.boldLabel);
				GUI.color = Color.white;


			}


		}




		public void UpdateCutLine(){

			if(SF_GUI.HoldingAlt() && Event.current.type == EventType.mouseDown && Event.current.button == 1){ // Alt + RMB drag
				StartCutting();
			} else if(SF_GUI.ReleasedRawRMB()){
				StopCutting();
			}
			
			if(isCutting){
				
				Vector2 cutEnd = GetNodeSpaceMousePos();
				
				GUILines.DrawDashedLine(editor, cutStart, GetNodeSpaceMousePos(), Color.white, 5f);
				
				
				foreach(SF_Node n in editor.nodes){
					foreach(SF_NodeConnector con in n.connectors){
						if(con.IsConnected() && con.conType == ConType.cInput){
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
			SF_EditorNodeData nodeData = o as SF_EditorNodeData;
			editor.AddNode( nodeData );
		}



		public void UpdateDebugInput() {

			if( Event.current.type != EventType.keyDown )
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
			AddDepthToChildrenOf( editor.materialOutput, 0 );
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

			AssignDepthValuesToNodes();

			int maxDepth = 0; // Deepest level
			foreach( SF_Node n in editor.nodes ) {
				if( maxDepth < n.depth )
					maxDepth = n.depth;
			}

			
			// Relink everything
			int depth = maxDepth;
			while( depth > 0 ) {
				for(int i=0; i<editor.nodes.Count; i++){
					SF_Node n = editor.nodes[i];
					if( n.depth == depth ) {
						n.OnUpdateNode(NodeUpdateType.Soft, cascade:true);
					}
				}
				depth--;
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
			header += "// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/\n";
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

			if(!SF_Settings.AutoRecompile)
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
			/*
			GUI.color = Color.white;
			GUI.Box( r, "", toolbarStyle );
			r.x += 6;

			r.width = 108;
*/
		/*
			GUI.color = SF_GUI.outdatedStateColors[(int)editor.ShaderOutdated];
			if( GUI.Button( r, "Compile shader", EditorStyles.toolbarButton ) ) {
				if(treeStatus.CheckCanCompile())
					editor.shaderEvaluator.Evaluate();
			}
			GUI.color = Color.white;

			DrawRecompileTimer(r);

			r.x += r.width + 4;
			r.width = 100;
			SF_Settings.AutoRecompile = GUI.Toggle( r, SF_Settings.AutoRecompile, "Auto-compile" );
			*/
			/*
			r.x += r.width + 20;
			r.width = 140;
			SF_Settings.HierarcyMove = GUI.Toggle( r, SF_Settings.HierarcyMove, "Hierarchal Node Move" );
			r.x += r.width + 20;
			r.width = 60;
			*/
			//GUI.Label( r, "Con. style:", EditorStyles.miniLabel );
			//r.x += r.width + 2;
			//SF_Settings.ConnectionLineStyle = (ConnectionLineStyle)EditorGUI.EnumPopup( r, SF_Settings.ConnectionLineStyle, EditorStyles.toolbarPopup);


			//GUILayout.FlexibleSpace();

			//r.x += r.width + 20;
			/*
			GUI.color = new Color(0.8f,1f,0.8f,1f);
			r.width = 110;
			SF_Tools.LinkButton( r, SF_Tools.manualLabel, SF_Tools.manualURL, EditorStyles.toolbarButton );
			r.x += r.width + 2;
			r.width = 120;
			SF_Tools.LinkButton( r, SF_Tools.bugReportLabel, SF_Tools.bugReportURL, EditorStyles.toolbarButton );
			r.x += r.width + 2;
			r.width = 80;
			SF_Tools.LinkButton( r, SF_Tools.featureListLabel, SF_Tools.featureListURL, EditorStyles.toolbarButton );
			GUI.color = Color.white;
			*/

		}

		void UpdateCameraPanning() {



			if( SF_GUI.ReleasedCameraMove() ) {
				panCamera = false;
			}

			bool insideNodeView = MouseInsideNodeView( true );
			bool dragging = ( Event.current.type == EventType.MouseDrag && panCamera );
			bool connecting = SF_NodeConnector.IsConnecting();
			bool rotatingPreview = editor.preview.isDragging;
			bool placingNode = editor.nodeBrowser.IsPlacing();
			bool draggingSeparators = editor.DraggingAnySeparator();


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

		public Vector2 GetNodeSpaceMousePos() {
			return ScreenSpaceToZoomSpace( Event.current.mousePosition );
		}


		public bool MouseInsideNodeView( bool offset = false ) {

			if( offset ) {
				return rect.Contains( ZoomSpaceToScreenSpace( Event.current.mousePosition ) );
			} else {
				return rect.Contains( Event.current.mousePosition );
			}

		} 

		void SnapCamera(){
			cameraPos.x = Mathf.Round(cameraPos.x);
			cameraPos.y = Mathf.Round(cameraPos.y);
		} 

		 
		public Vector2 ZoomSpaceToScreenSpace( Vector2 in_vec ) {
			return (in_vec - cameraPos + editor.separatorLeft.rect.TopRight() )*zoom + rect.TopLeft() + (Vector2.up * (22f))*(zoom-1);
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
			return ( in_vec - (Vector2.up * (22f))*(zoom-1) - rect.TopLeft() ) / zoom - editor.separatorLeft.rect.TopRight() + cameraPos;
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


		/*
		float zoomAbsolute = 1f;
		float zoom = 1f;
		public void UpdateCameraZoomInput() {
			if( Event.current.type == EventType.ScrollWheel ) {
				zoomAbsolute = Mathf.Clamp(zoomAbsolute - Event.current.delta.y*0.025f, 0.1f,1f );
			}
		}

		public void UpdateCameraZoomValue() {
			zoom = Mathf.Lerp( zoom, zoomAbsolute, 0.05f);
		}
		*/


	}

}

