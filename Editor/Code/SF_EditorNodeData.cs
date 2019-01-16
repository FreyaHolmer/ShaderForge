using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SF_EditorNodeData : ScriptableObject {

		[SerializeField]
		KeyCode key;
		[SerializeField]
		public bool holding = false; 
		[SerializeField]
		public string nodeName;
		[SerializeField]
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

		[SerializeField]
		public string type;
		[SerializeField]
		public bool isNew = false;
		[SerializeField]
		public bool isUnstable = false;
		[SerializeField]
		public string fullPath;
		[SerializeField]
		public string category;
		[SerializeField]
		public bool isProperty = false;
		[SerializeField]
		public bool availableInDeferredPrePass = true;



		public SF_EditorNodeData() {

		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_EditorNodeData Initialize( string type, string fullPath, KeyCode key = KeyCode.None ) {
			holding = false;
			this.type = type;
			ParseCategoryAndName( fullPath );
			this.key = key;

			if( type.Contains( "SFN_Color" ) ||
				type.Contains( "SFN_Cubemap" ) ||
				type.Contains( "SFN_Slider" ) ||
				type.Contains( "SFN_Tex2d" ) ||
				type.Contains( "SFN_Tex2dAsset" ) ||
				type.Contains( "SFN_Vector4Property" ) ||
				type.Contains( "SFN_ValueProperty" ) ||
			   	type.Contains( "SFN_ToggleProperty" ) ||
			   	type.Contains( "SFN_SwitchProperty" ) ||
				type.Contains( "SFN_Matrix4x4Property" )
				)
					isProperty = true;

			return this;
		}

		public void ParseCategoryAndName(string fullPath) {

			this.fullPath = fullPath;

			string[] split = fullPath.Split( '/' );
			if( split.Length > 1 ) {
				this.category = split[0];
				this.nodeName = split[1];
			} else {
				this.nodeName = fullPath;
			}

		} 


		public SF_Node CreateInstance() {

			Type fType = Type.GetType( type );

			// Might be dynamic...
			if( fType == null ) {
				if(SF_Debug.dynamicNodeLoad)
					Debug.Log( "CreateInstance couldn't use GetType, attempting dynamic load..." );
				fType = SF_Editor.GetNodeType( type );
				if( SF_Debug.dynamicNodeLoad && fType == null )
					Debug.Log( "Failed to load dynamic load fType is null" );
			}


			SF_Node node = (SF_Node)ScriptableObject.CreateInstance( fType );
			node.Initialize();
			return node;
		}

		public SF_EditorNodeData MarkAsNewNode() {
			isNew = true;
			return this;
		}

		public SF_EditorNodeData MarkAsUnstableNode() {
			isUnstable = true;
			return this;
		}

		public SF_EditorNodeData UavailableInDeferredPrePass(){
			availableInDeferredPrePass = false;
			return this;
		}

		public float smoothHotkeySelectorIndex = 0f;
		public int defaultHotkeySelectorIndex = 0;
		public int hotkeySelectorIndex = 0;
		[SerializeField]
		private List<SF_EditorNodeData> hotkeyFriends;
		public List<SF_EditorNodeData> HotkeyFriends{
			get{
				if(hotkeyFriends == null){
					hotkeyFriends = new List<SF_EditorNodeData>();
				}

				if(hotkeyFriends.Count == 0){
					int i=0;
					foreach( SF_EditorNodeData node in SF_Editor.instance.nodeTemplates){
						if(node == this)
							smoothHotkeySelectorIndex = hotkeySelectorIndex = defaultHotkeySelectorIndex = i;
						if(node.key == key || KeyCodeToChar(key) == char.ToUpper(node.nodeName[0])){
							hotkeyFriends.Add(node);
							i++;
						}

					}
				}
				return hotkeyFriends;
			}
		}


		public char KeyCodeToChar(KeyCode kc){
			string s = kc.ToString();
			if(s.StartsWith("Alpha"))	 // Numbers 0 to 9 are called "Alpha5" etc. Extract just the numeral as the returned character
				return s[5];
			return s[0];
		}

		[SerializeField]
		private static GUIStyle popupButtonStyle;
		public static GUIStyle PopupButtonStyle{
			get{
				if(popupButtonStyle == null){
					popupButtonStyle = new GUIStyle(SF_Styles.NodeStyle);
					popupButtonStyle.alignment = TextAnchor.UpperLeft;
					RectOffset ro = popupButtonStyle.padding;
					ro.left = 4;
					popupButtonStyle.padding = ro;
				}
				return popupButtonStyle;
			}
		}
		 
		public Vector2 quickpickerStartPosition = Vector2.zero;

		public SF_EditorNodeData CheckHotkeyInput(bool mouseOverSomeNode) {
			
			bool mouseInNodeView = SF_Editor.instance.nodeView.MouseInsideNodeView(false);


			if(Event.current.type == EventType.Repaint){
				smoothHotkeySelectorIndex = Mathf.Lerp(smoothHotkeySelectorIndex, hotkeySelectorIndex, 0.5f);
			}

			bool useScroll = SF_Settings.quickPickScrollWheel;

			if(holding && Event.current.type == EventType.ScrollWheel && HotkeyFriends.Count > 0 && mouseInNodeView){

				if(useScroll){
					hotkeySelectorIndex += (int)Mathf.Sign(Event.current.delta.y);
					hotkeySelectorIndex = Mathf.Clamp(hotkeySelectorIndex, 0, HotkeyFriends.Count-1);
				}


				// hotkeySelectorIndex = ( hotkeySelectorIndex + HotkeyFriends.Count ) % HotkeyFriends.Count; // Wrap
				Event.current.Use();
			}

			if( key == KeyCode.None )
				return null;

			if( Event.current.keyCode == key ) {
				if( Event.current.type == EventType.KeyDown && !SF_GUI.HoldingControl() && holding == false && mouseInNodeView ){

					hotkeySelectorIndex = defaultHotkeySelectorIndex;
					smoothHotkeySelectorIndex = defaultHotkeySelectorIndex;

					quickpickerStartPosition = Event.current.mousePosition;

					holding = true;
				}
				if( Event.current.rawType == EventType.KeyUp ){
					holding = false;
				}
			}



			if(holding && !mouseOverSomeNode){



				
				float width = 166f; // nodeName.Length*8 + 10;
				Rect dispPos = new Rect(0, 0, width, 36);

				Vector2 centerPos = useScroll ? Event.current.mousePosition : quickpickerStartPosition;

				dispPos.center = centerPos;
				dispPos.y -= dispPos.height*0.3333f;

				//
				//GUI.Box(dispPos, nodeName, GUI.skin.button);
				//
			


				// Draw hotkey node picker
				//if(Event.current.type == EventType.keyDown){
				//Debug.Log(Event.current.keyCode);
				Rect nRect = dispPos; //new Rect(0,0,128,32);
				nRect.center = centerPos - Vector2.up*nRect.height*0.3333f;
				//nRect = nRect.MovedRight();
				if(useScroll)
					nRect.y -= nRect.height * smoothHotkeySelectorIndex;
				else
					nRect.y -= nRect.height * defaultHotkeySelectorIndex;
				//if(Event.current.keyCode != KeyCode.None){

				Color prevCol = GUI.color;



				int i = 0;
				foreach( SF_EditorNodeData node in HotkeyFriends){
					//float dist = Mathf.Abs(smoothHotkeySelectorIndex - i);
					//float alpha = Mathf.Clamp(1f-Mathf.Clamp01(dist*0.25f), 0.2f, 0.8f);


					float offset = 0f;//(dist*dist)/3f;




					//if(i == hotkeySelectorIndex){
						//alpha = 1;
						//offset -= 8f;
						//GUI.Box(nRect, node.nodeName, PopupButtonStyle);
					//}
					Rect newNRect = nRect;
					newNRect.x += offset;


					if(!useScroll && newNRect.Contains(Event.current.mousePosition)){
						hotkeySelectorIndex = i;
					}

					bool selected = (i == hotkeySelectorIndex);

					if( selected )
						GUI.color = new Color(1f,1f,1f,1f);
					else
						GUI.color = new Color(0.6f,0.6f,0.6f,0.5f);
					
					if(node.isProperty){
						GUI.color *= SF_Node.colorExposed;
					}


					Texture2D icon = SF_Resources.LoadNodeIcon( node.type.Split('.')[1].ToLower() );

					if(icon != null){
						newNRect.width -= newNRect.height;
					}

					//if(useScroll){
						GUI.Box(newNRect, node.nodeName, PopupButtonStyle);
					//} else {
						//if(GUI.Button(newNRect, node.nodeName, PopupButtonStyle)){
							//hotkeySelectorIndex = i;
						//}
					//}



					
					if(icon != null){
						Rect iconRect = newNRect;
						iconRect = iconRect.MovedRight();
						iconRect.width = iconRect.height;
						GUI.color = selected ? Color.white : new Color(1f,1f,1f,0.4f);
						GUI.DrawTexture(iconRect, icon);
						
					}




					nRect = nRect.MovedDown();

					i++;
				}
				GUI.color = prevCol;



				//}
				if(Event.current.type == EventType.KeyDown/* && Event.current.type == EventType.layout*/ /*&& GUI.GetNameOfFocusedControl() == "defocus"*/){
					Event.current.Use();
				}
				//}
				
				//}

				//GUI.Label(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 256,32),"currentindex = " + hotkeySelectorIndex);
			}




			bool clicked = Event.current.type == EventType.MouseDown;
			if(holding && clicked){
				return HotkeyFriends[hotkeySelectorIndex];
			} else {
				return null;
			}
		}
	}
}