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
			base.node_height -= 20;
			//base.lowerRect.height += 4;
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.ValuePassing;

			base.UseLowerPropertyBox( true, true );



			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","Off",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","On",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			property = ScriptableObject.CreateInstance<SFP_SwitchProperty>().Initialize( this );
			
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2]);
			
		}

		float smoothConnectorHeight = 23;
		float targetConnectorHeight = 23;
		Color conLineBg = Color.black;
		Color conLineFg = Color.white;
	//	Color conLineBgTrns = new Color(0f,0f,0f,0.3f);
	//	Color conLineFgTrns = new Color(1f,1f,1f,0.3f);

		public override string[] ExtraPassedFloatProperties(){
			return new string[] { "On" };
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_on", on ? 1.0f : 0.0f );
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "lerp(_a,_b,_on)" };
		}

		
		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.height = 24;
			r.width = 26;
			r.y -= 26;

			if(Event.current.type == EventType.Repaint){
				smoothConnectorHeight = Mathf.Lerp(smoothConnectorHeight, targetConnectorHeight, 0.6f);
			}

			r = r.PadTop(1).PadBottom(1).PadLeft(2);

			r.width = r.height + 2;
			//r.xMin += 3;

			//Handles.BeginGUI(rect);

			bool hovering = rect.Contains(Event.current.mousePosition + rect.TopLeft());



			if(hovering){
				targetConnectorHeight = on ? 43 : 23;
				Vector2 p0 = new Vector2(rect.width,23);
				Vector2 p1 = new Vector2(0, smoothConnectorHeight);
				GUILines.QuickBezier( p0, p1, conLineBg, 12, 5 );
				GUILines.QuickBezier( p0, p1, conLineFg, 12, 3 );
				GUILines.QuickBezier( p0, p1, conLineFg, 12, 3 );
				bool prevVal = on;
				GUI.color = new Color(SF_Node.colorExposed.r,SF_Node.colorExposed.g,SF_Node.colorExposed.b,GUI.color.a);
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
			

			//GUILines.DrawMultiBezierConnection(editor,,GetEvaluatedComponentCount(),Color.white);

			//Handles.DrawLine(new Vector3(0,0),new Vector3(32,32));
			//
			//Handles.EndGUI();





			//GUI.enabled = true;



			
		}
		
		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lerp( " + GetConnectorByStringID( "A" ).TryEvaluate() + ", " + GetConnectorByStringID( "B" ).TryEvaluate() + ", "+ property.GetVariable() + " )";
		}
		
		public override float EvalCPU( int c ) {
			if(on){
				return GetInputData("B", c);
			} else {
				return GetInputData("A", c);
			}
		}


		public override string SerializeSpecialData() {
			string s = property.Serialize() + ",";
			s += "on:" + on;
			return s;
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize( key, value );
			switch( key ) {
			case "on":
				on = bool.Parse( value );
				break;
			}
		}
	}
}