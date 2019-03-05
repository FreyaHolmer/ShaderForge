using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShaderForge {
	
	
	[System.Serializable]
	public class SFPSC_Console : SFPS_Category {


		public SF_NodeTreeStatus treeStatus;

		/*
		public override string Serialize(){
			string s = "";
			s += Serialize( "f2p0", force2point0.ToString());
			return s;
		}

		public override void Deserialize(string key, string value){

			switch( key ) {
			case "f2p0":
				force2point0 = bool.Parse( value );
				break;
			}

		}*/


		public override void DrawExtraTitleContent( Rect r ) {

			if( treeStatus.Errors.Count > 0 && !expanded ) {
				r = r.MovedRight();
				r.width = SF_Styles.IconErrorSmall.width;
				r = r.MovedLeft();
				r.height = SF_Styles.IconErrorSmall.height;
				r.x -= 1;
				r.y += 1;

				bool hasError = false;
				for( int i = 0; i < treeStatus.Errors.Count; i++ ) {
					if( treeStatus.Errors[i].isWarning == false ) {
						hasError = true;
						break;
					}
				}

				GUI.DrawTexture( r, hasError ? SF_Styles.IconErrorSmall : SF_Styles.IconWarningSmall );
			}
			
		}
	

		public override float DrawInner(ref Rect r){

			float prevYpos = r.y;
			r.y = 0;

			

			
			r.xMin += 20;
			r.y += 20;
			//GUI.DrawTexture(r.ClampSize(0,SF_Styles.IconWarningSmall.width),SF_Styles.IconWarningSmall);
			//r.xMin += 20;
			//GUI.Label(r, "Experimental features may not work");
			//r.xMin -= 20;
		//	r.height += 20;

			r.height = 20;

			for( int i = 0; i < treeStatus.Errors.Count; i++ ) {

				bool isNode = treeStatus.Errors[i].node != null;

				Texture2D icon = treeStatus.Errors[i].icon;

				Rect blockRect = r;
				blockRect.height = treeStatus.Errors[i].rows * 14f + 6;


				Rect iconRect = blockRect;
				iconRect.width = icon.width;
				iconRect.height = icon.height;
				//iconRect.x += 1;
				//iconRect.y += 1;

				Rect textRect = blockRect;
				textRect.xMin += iconRect.width + 3;

				iconRect.center = new Vector2( iconRect.center.x, textRect.center.y );

				bool hasAction = treeStatus.Errors[i].action != null;

				if( isNode || hasAction ) {
					if( GUI.Button( iconRect.Pad( -2 ).PadHorizontal(-3), "" ) ) {
						if( hasAction ) {
							treeStatus.Errors[i].OnPress();
							break;
						} else if( isNode ) {
							editor.nodeView.selection.DeselectAll( true );
							treeStatus.Errors[i].node.Select( true );
						}
					}
				}
				
				GUI.DrawTexture( iconRect, icon );
				EditorGUI.SelectableLabel( textRect, treeStatus.Errors[i].error, SF_Styles.SmallTextArea );
				
				r.y += textRect.height;
			}
			

			r.y += prevYpos;

			return (int)r.yMax;
		}




	}
}