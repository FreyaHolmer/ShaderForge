using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Properties : SFPS_Category {


		const float propertyHeight = 60f;



		public SF_Node draggingProperty = null;
		public float dragStartOffsetY = 0;
		public int dragStartIndex;
		public float startMouseY;
		
		public float DragRectPosY {
			get {
				return Event.current.mousePosition.y + dragStartOffsetY;
			}
		}




		public override string Serialize(){
			return "";
		}

		public override void Deserialize(string key, string value){
		}


		public override void PreDraw(Rect r){

			int propCount = editor.nodeView.treeStatus.propertyList.Count;
			
			GUI.color = SF_GUI.ProSkin ? SF_Node.colorExposedDarker : SF_Node.colorExposedDim;
			if( expanded ) {
				Rect fullArea = new Rect( r );
				fullArea.height += 23 + propertyHeight * propCount;
				GUI.DrawTexture( fullArea, EditorGUIUtility.whiteTexture );
			} else {
				GUI.DrawTexture( r, EditorGUIUtility.whiteTexture );
			}
			GUI.color = SF_Node.colorExposed;
		}



		public override float DrawInner(ref Rect r){

			Restart:
			//int propCount = editor.nodeView.treeStatus.propertyList.Count;

			List<SF_Node> propertyList = editor.nodeView.treeStatus.propertyList;

			//GUI.Label( r.MovedUp(), "propertyList.Count = " + propertyList.Count );

			int propCount = propertyList.Count;

			bool multiple = propCount > 1;

			
			float prevYpos = r.y;
			r.y = 0;
			
			
			if( propCount == 0 ) {
				r.y += 16;
				GUI.enabled = false;
				GUI.Label( r, "No properties in this shader yet" );
				GUI.enabled = true;
				r.y -= 16;
			}
			
			
			r.y += 23;
			r.xMin += 20; // Indent
			r.xMax -= 3;
			
			
			
			r.height = propertyHeight;

			

			
			// On drop...
			if( draggingProperty != null && SF_GUI.ReleasedRawLMB()) {
				
				
				int moveDist = Mathf.RoundToInt( ( Event.current.mousePosition.y - startMouseY ) / propertyHeight );
				
				// Execute reordering!
				if( moveDist != 0 ) { // See if it actually moved to another slot
					int newIndex = Mathf.Clamp( dragStartIndex + moveDist, 0, propCount - 1 );
					Undo.RecordObject(editor.nodeView.treeStatus,"property reorder");
					editor.nodeView.treeStatus.propertyList.RemoveAt( dragStartIndex );
					//if( newIndex > dragStartIndex )
					//	newIndex--;
					editor.nodeView.treeStatus.propertyList.Insert( newIndex, draggingProperty );
				}
				
				draggingProperty = null;
				
				
			}
			
			float yStart = r.y;
			
			
			int i = 0;

			
			for(int j=0;j<propertyList.Count;j++){


				SF_Node prop = propertyList[j];
				
				
				if( prop.property == null ) { // Due to a weird bug - remove these nodes

					// Disconnect
					foreach( SF_NodeConnector con in prop.connectors ) {
						if( con.conType == ConType.cOutput ) {
							con.Disconnect();
						}
					}
					prop.Deselect( registerUndo: false );
					propertyList.Remove( prop );
					editor.nodeView.treeStatus.propertyList.Remove( prop );
					editor.nodes.Remove( prop );
					//Debug.Log("Removing broken property...");
					DestroyImmediate( prop );
					goto Restart;
				}
					

				bool draggingThis = ( draggingProperty == prop );
				bool dragging = (draggingProperty != null);

				r.y = yStart + propertyHeight * i;
				
				if( draggingThis ) {
					r.x -= 5;
					r.y = Mathf.Clamp(Event.current.mousePosition.y + dragStartOffsetY, yStart, yStart+propertyHeight*(propCount-1));
				} else if( dragging ) {
					if( i < dragStartIndex ){
						float offset = propertyHeight + SF_Tools.Smoother( Mathf.Clamp( r.y - DragRectPosY, -propertyHeight, 0 ) / -propertyHeight ) * -propertyHeight;
						r.y += offset;
					} else if( i > dragStartIndex) {
						r.y -= propertyHeight - SF_Tools.Smoother( Mathf.Clamp( r.y - DragRectPosY, 0, propertyHeight ) / propertyHeight ) * propertyHeight;
					}
				}
				
				
				
				
				
				GUI.Box( r, string.Empty, draggingThis ? SF_Styles.HighlightStyle : SF_Styles.NodeStyle );
				bool mouseOver = r.Contains( Event.current.mousePosition );
				
				
				
				
				
				// We're now in the property box
				// We need: Grabber, Text field, Internal label
				
				
				
				bool imagePreview = (prop.property is SFP_Tex2d || prop.property is SFP_Cubemap);
				bool colorInput = ( prop.property is SFP_Color );
				bool checkboxInput = (prop.property is SFP_ToggleProperty || prop.property is SFP_SwitchProperty);
				
				
				// GRABBER
				Rect gRect = SF_Tools.GetExpanded( r, -6);
				gRect.width = gRect.height/2f;
				
				gRect.yMin += 8;
				
				Rect gRectCoords = new Rect( gRect );
				
				gRectCoords.x = 0;
				gRectCoords.y = 0;
				gRectCoords.width /= SF_GUI.Handle_drag.width;
				gRectCoords.height /= SF_GUI.Handle_drag.height;
				if(multiple)
					GUI.DrawTextureWithTexCoords( gRect, SF_GUI.Handle_drag, gRectCoords );
				gRect.yMin -= 8;
				/*
				if( propCount > 1 ) {
					if( gRect.Contains( Event.current.mousePosition ) && SF_GUI.PressedLMB() && !dragging ) {
						dragStartOffsetY = r.y - Event.current.mousePosition.y;
						draggingProperty = prop;
						dragStartIndex = i;
						startMouseY = Event.current.mousePosition.y;
					}	
					SF_GUI.AssignCursor( gRect,MouseCursor.Pan);
					GUI.DrawTextureWithTexCoords(gRect, SF_GUI.Handle_drag, gRectCoords );
				}
				*/
				
				
				
				
				// Property type name
				Color c = GUI.color;
				c.a = 0.5f;
				GUI.color = c;
				Rect propTypeNameRect = new Rect( gRect );
				//propTypeNameRect.x += propTypeNameRect.width + 8;
				propTypeNameRect.y -= 5;
				if( imagePreview || colorInput || checkboxInput )
					propTypeNameRect.width = r.width - r.height - 38;
				else
					propTypeNameRect.width = r.width - 48;
				propTypeNameRect.height = 16;
				//if( prop.property != null )
				GUI.Label( propTypeNameRect, prop.property.nameType, EditorStyles.miniLabel );
				propTypeNameRect.x += gRect.width + 8;
				c.a = 1f;
				GUI.color = c;
				//else
				//return (int)r.yMax;
				
				
				// INTERNAL NAME
				
				if( mouseOver ) {
					c.a = 0.5f;
					GUI.color = c;
					Rect intRect = new Rect( propTypeNameRect );
					intRect.xMin += intRect.width - SF_GUI.WidthOf( prop.property.nameInternal, EditorStyles.label );
					//SF_GUI.AssignCursor( intRect, MouseCursor.Text );
					GUI.Label( intRect, prop.property.nameInternal, EditorStyles.label );
					c.a = 1f;
					GUI.color = c;
				}
				
				
				
				// DISPLAY NAME
				Rect dispNameRect = new Rect( propTypeNameRect );
				dispNameRect.y += 18;
				//dispNameRect.x += dispNameRect.width + 4;
				//dispNameRect.height = 16;
				//dispNameRect.y += 10;
				//dispNameRect.width = ( r.width - dispNameRect.width - texRect.width - 20 ) * 0.5f;
				
				ps.StartIgnoreChangeCheck();
				string bef = prop.property.nameDisplay;
				SF_GUI.AssignCursor( dispNameRect, MouseCursor.Text );
				//if( mouseOver )
				UndoableEnterableNodeTextField(prop.property.node, dispNameRect, ref prop.property.nameDisplay, "change property name", update:false, extra:prop.property);
				//else
				//GUI.Label( dispNameRect, prop.property.nameDisplay, EditorStyles.boldLabel );
				if( prop.property.nameDisplay != bef ) { // Changed
					prop.property.UpdateInternalName();
				}
				ps.EndIgnoreChangeCheck();
				
				
				
				
				
				
				// Texture preview
				Rect texRect = new Rect( 0, 0, 0, 0 );
				c = GUI.color;
				if( imagePreview ) {
					texRect = SF_Tools.GetExpanded(new Rect( r ), -4);
					texRect.xMin += texRect.width - texRect.height;
					//texRect.x += gRect.width + 4;
					//texRect.width = texRect.height;
					GUI.Box( SF_Tools.GetExpanded( texRect, 1f ), string.Empty, SF_Styles.NodeStyle );
					GUI.color = Color.white;
					GUI.DrawTexture( texRect, prop.texture.texture );
					GUI.color = c;
				}
				
				
				if( prop.property is SFP_Slider ) {
					
					SFN_Slider slider = ( prop as SFN_Slider );
					
					ps.StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height+5;
					sR.width = 28;
					GUI.Label( sR, "Min" );
					//sR.x += sR.width;
					sR = sR.MovedRight();
					prop.UndoableEnterableFloatField(sR, ref slider.min, "min value",null);


					sR = sR.MovedRight();
					
					sR.width = r.width - 164;
					
					float beforeSlider = slider.current;
					
					string sliderName = "slider" + slider.id;
					GUI.SetNextControlName( sliderName );

					sR.xMin += 4;
					sR.xMax -= 4;

					slider.current = prop.UndoableHorizontalSlider(sR, slider.current, slider.min, slider.max, "value");
					if( beforeSlider != slider.current ) {
						GUI.FocusControl( sliderName );
						slider.OnValueChanged();
					}
					//SF_GUI.AssignCursor( sR, MouseCursor.Arrow );
					
					sR.x += sR.width+4;
					sR.width = 32;
					prop.UndoableEnterableFloatField(sR, ref slider.max, "max value",null);
					sR.x += sR.width;
					GUI.Label( sR, "Max" );
					
					ps.EndIgnoreChangeCheck();
					
				} else if( colorInput ) {
					
					
					SFN_Color colNode = ( prop as SFN_Color );
					
					texRect = SF_Tools.GetExpanded( new Rect( r ), -4 );
					texRect.xMin += texRect.width - texRect.height;
					//GUI.Box( SF_Tools.GetExpanded( texRect, 1f ), string.Empty, SF_Styles.NodeStyle );
					GUI.color = Color.white;
					texRect.yMax -= 21;
					texRect.yMin += 15;
					texRect.xMin += 2;
					//texRect.xMax -= 2;
					
					SF_GUI.AssignCursor( texRect, MouseCursor.Arrow );
					
					ps.StartIgnoreChangeCheck();
					//Color col = EditorGUI.ColorField( texRect, colNode.texture.dataUniform );
					Color col = colNode.UndoableColorField(texRect, colNode.texture.dataUniform, "set color of " + colNode.property.nameDisplay);
					ps.EndIgnoreChangeCheck();
					colNode.SetColor( col );
					GUI.color = c;
				} else if( prop.property is SFP_Vector4Property ) {
					
					SFN_Vector4Property vec4 = ( prop as SFN_Vector4Property );
					
					ps.StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height + 5;
					sR.width = 20;
					
					int lbWidth = 12;


					//string channelStr = "XYZW";



					sR.width = lbWidth;
					GUI.Label( sR, "X", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					prop.UndoableEnterableFloatField(sR, ref vec4.texture.dataUniform.x, "X channel", EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width + 3;
					
					
					sR.width = lbWidth;
					GUI.Label( sR, "Y", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					prop.UndoableEnterableFloatField(sR, ref vec4.texture.dataUniform.y, "Y channel", EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width+3;
					
					
					sR.width = lbWidth;
					GUI.Label( sR, "Z", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					prop.UndoableEnterableFloatField(sR, ref vec4.texture.dataUniform.z, "Z channel", EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					sR.x += sR.width + 3;
					
					
					sR.width = lbWidth;
					GUI.Label( sR, "W", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 32;
					prop.UndoableEnterableFloatField(sR, ref vec4.texture.dataUniform.w, "W channel", EditorStyles.textField );
					SF_GUI.AssignCursor( sR, MouseCursor.Text );

					
					
					
					ps.EndIgnoreChangeCheck();
					
				} else if( prop.property is SFP_ValueProperty ) {
					
					SFN_ValueProperty val = ( prop as SFN_ValueProperty );
					
					ps.StartIgnoreChangeCheck();
					Rect sR = new Rect( dispNameRect );
					sR.y += sR.height + 5;
					sR.width = 20;
					
					sR.width = 35;
					GUI.Label( sR, "Value", EditorStyles.miniLabel );
					sR.x += sR.width;
					sR.width = 55;
					//SF_GUI.EnterableFloatField( prop, sR, ref val.texture.dataUniform.r, EditorStyles.textField );
					prop.UndoableEnterableFloatField(sR, ref val.texture.dataUniform.x, "value", EditorStyles.textField);
					SF_GUI.AssignCursor( sR, MouseCursor.Text );
					ps.EndIgnoreChangeCheck();
				} else if (checkboxInput){

					bool isToggle = (prop.property is SFP_ToggleProperty);

					bool prevValue = isToggle ? (prop.property.node as SFN_ToggleProperty).on : (prop.property.node as SFN_SwitchProperty).on;


					

					ps.StartIgnoreChangeCheck();

					texRect = SF_Tools.GetExpanded( new Rect( r ), -4 );
					texRect.xMin += texRect.width - texRect.height;
					//GUI.Box( SF_Tools.GetExpanded( texRect, 1f ), string.Empty, SF_Styles.NodeStyle );

					texRect.yMax -= 21;
					texRect.yMin += 15;
					texRect.xMin += 2;
					//texRect.xMax -= 2;

					SF_GUI.AssignCursor( texRect, MouseCursor.Arrow );

					bool newValue = prevValue;

					if(isToggle){
						prop.property.node.UndoableToggle(texRect, ref (prop.property.node as SFN_ToggleProperty).on, "", "property checkbox", EditorStyles.toggle);
						newValue = (prop.property.node as SFN_ToggleProperty).on;
					} else {
						prop.property.node.UndoableToggle(texRect, ref (prop.property.node as SFN_SwitchProperty).on, "", "property checkbox", EditorStyles.toggle);
						newValue = (prop.property.node as SFN_SwitchProperty).on;
					}

					if(newValue != prevValue){
						//if(isToggle){
						//	(prop.property.node as SFN_ToggleProperty).on = newValue;
						//} else {
						//	(prop.property.node as SFN_SwitchProperty).on = newValue;
						////}
						if(isToggle){
							prop.property.node.texture.dataUniform = Color.white * (newValue ? 1f : 0f);
						} else {
							//prop.property.node.texture.UpdateColorPreview("",true);
						}
						prop.property.node.OnUpdateNode(NodeUpdateType.Soft);
					}
					ps.EndIgnoreChangeCheck();

				}
				
				

				
				
				if( r.Contains( Event.current.mousePosition ) && SF_GUI.PressedLMB() && !dragging && multiple) {
					dragStartOffsetY = r.y - Event.current.mousePosition.y;
					draggingProperty = prop;
					dragStartIndex = i;
					startMouseY = Event.current.mousePosition.y;
					editor.Defocus();
				}
				if(multiple)
					SF_GUI.AssignCursor( r, MouseCursor.Pan );
				
				
				
				
				
				
				
				
				if( draggingThis )
					r.x += 5;

				//GUI.Label( r, "prop.property.nameType = " + prop.property.nameType );

				r.y += propertyHeight;
				i++;
			}

			
			
			r.y = yStart + propCount * propertyHeight;
			r.height = 20;

			r.y += prevYpos;

			return r.yMax;
		}

	



	}
}