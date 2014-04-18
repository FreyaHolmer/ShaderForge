using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_SwitchProperty : SF_Node_Arithmetic {

		public SFN_SwitchProperty() {

		}

		[SerializeField]
		public bool on = false;

		public override void Initialize() {
			base.Initialize( "Switch" );
			base.showColor = true;
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","Off",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","On",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			property = ScriptableObject.CreateInstance<SFP_SwitchProperty>().Initialize( this );
			
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2]);
			
		}
		
		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r = r.PadLeft(32).PadRight(32);
			//r.xMin += 3;



			bool prevVal = on;

			GUI.color = SF_Node.colorExposed;
			bool newVal = GUI.Button(r,string.Empty) ? !prevVal : prevVal;


			if(newVal){
				Rect chkRect = r;
				chkRect.width = SF_GUI.Toggle_check_icon.width;
				chkRect.height = SF_GUI.Toggle_check_icon.height;
				chkRect.x += (r.width-chkRect.width)*0.5f;
				chkRect.y += 2;
				GUI.DrawTexture(chkRect,SF_GUI.Toggle_check_icon);
			}
			
			GUI.color = Color.white;
			
			if(prevVal != newVal){
				string dir = on ? "on" : "off";
				UndoRecord("switch " + dir + " " + property.nameDisplay);
				on = newVal;
				OnUpdateNode(NodeUpdateType.Soft, true);
				editor.shaderEvaluator.ApplyProperty( this );
			}
			
		}
		
		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lerp( " + GetConnectorByStringID( "A" ).TryEvaluate() + ", " + GetConnectorByStringID( "B" ).TryEvaluate() + ", "+ property.GetVariable() + " )";
		}
		
		public override float NodeOperator( int x, int y, int c ) {
			if(on){
				return GetInputData("B",x,y,c);
			} else {
				return GetInputData("A",x,y,c);
			}
		}


		public override string SerializeSpecialData() {
			return "on:" + on;
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "on":
				on = bool.Parse( value );
				break;
			}
		}
	}
}