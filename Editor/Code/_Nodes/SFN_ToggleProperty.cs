using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ToggleProperty : SF_Node {


		public SFN_ToggleProperty() {

		}

		[SerializeField]
		public bool on = false;

		public override void Initialize() {
			node_height = 24;
			//node_width = (int)(NODE_WIDTH*1.25f);
			base.Initialize( "Toggle" );
			lowerRect.y -= 8;
			lowerRect.height = 28;
			base.showColor = false;
			base.neverDefineVariable = true;
			base.UseLowerPropertyBox( true );
			base.texture.uniform = true;
			base.texture.CompCount = 1;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;

			property = ScriptableObject.CreateInstance<SFP_ToggleProperty>().Initialize( this );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1,false)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return property.GetVariable();
		}


		public override void DrawLowerPropertyBox() {
			PrepareWindowColor();
			float vecPrev = texture.dataUniform[0];
			//int strWidth = (int)SF_Styles.GetLargeTextField().CalcSize( new GUIContent( texture.dataUniform[0].ToString() ) ).x;
			//lowerRect.width = Mathf.Max( 32, strWidth );
			Rect r = new Rect( lowerRect );
			r.width -= 75;
			r.width *= 2;
			r.yMin += 4;
			r.yMax -= 2;
			r.xMin += 2;
			float fVal = texture.dataUniform[0];;

			//GUI.enabled = false;
			//fVal = EditorGUI.FloatField(r, texture.dataUniform[0], SF_Styles.LargeTextField);
			//GUI.enabled = true;

			//r.x += r.width + 6;



			bool prevVal = on;

			GUI.enabled = false;
			r = r.PadTop(2);
			GUI.Label(r,prevVal ? "1": "0", SF_Styles.LargeTextFieldNoFrame);
			r = r.PadTop(-2);
			GUI.enabled = true;

			r.x += 18;

			r.width = r.height + 2;
			bool newVal = GUI.Button(r,string.Empty) ? !prevVal : prevVal;

			if(newVal){
				Rect chkRect = r;
				chkRect.width = SF_GUI.Toggle_check_icon.width;
				chkRect.height = SF_GUI.Toggle_check_icon.height;
				chkRect.x += (r.width-chkRect.width)*0.5f;
				chkRect.y += 2;
				GUI.DrawTexture(chkRect,SF_GUI.Toggle_check_icon);
			}



			if(prevVal != newVal){
				UndoRecord("set toggle of " + property.nameDisplay + " to " + newVal.ToString());
				fVal = newVal ? 1f : 0f;
				connectors[0].label = "";
				//Debug.Log("Setting it to " + newVal.ToString());
			}

			r.x += r.width + 6;
			r.width = r.height + 18;
			Rect texCoords = new Rect( r );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( r, SF_GUI.Handle_drag, texCoords, alphaBlend:true );
			on = newVal;
			texture.dataUniform = new Color( fVal, fVal, fVal, fVal );
			if( texture.dataUniform[0] != vecPrev ) {
				OnUpdateNode( NodeUpdateType.Soft );
				editor.shaderEvaluator.ApplyProperty( this );
			}

			ResetWindowColor();
				
		}

		public override float EvalCPU( int c ) {
			if(on){
				return 1f;
			} else {
				return 0f;
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
				float fVal = on ? 1f : 0f;
				texture.dataUniform = new Color( fVal, fVal, fVal, fVal );
				break;
			}
		}



	}
}