using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_RemapRange : SF_Node_Arithmetic {

		// SF_Node tNode;

		[SerializeField]
		Vector2 from = new Vector2(0,1);
		[SerializeField]
		Vector2 to = new Vector2(-1,1);
		[SerializeField]
		float multiplier = 2f;
		float offset = -1f;


		public SFN_RemapRange() {

		}


		public override void Initialize() {
			base.Initialize( "Remap (Simple)" );
			base.SearchName = "Remap Simple";
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			base.PrepareArithmetic(1);
			base.node_height += 15;
			base.shaderGenMode = ShaderGenerationMode.ValuePassing;
			UpdateMultOffset();

		}


		// n-p*m = x

		public override string[] ExtraPassedFloatProperties() {
			return new string[]{
				"Multiplier",
				"Offset"
			};
		}

		public override void PrepareRendering( Material mat ) {
			UpdateMultOffset();
			mat.SetFloat( "_multiplier", multiplier );
			mat.SetFloat( "_offset", offset );
		}

		public override string[] GetBlitOutputLines() {
			return new string[]{
				"_in*_multiplier+_offset"
			};
		}


		public override void DrawLowerPropertyBox() {
			//EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.height = 15;
			r.y += 2;
			r.width /= 3;

			Vector2 befFrom = from;
			Vector2 befTo = to;
			DrawRemapLine(ref r, "From", ref from);
			DrawRemapLine(ref r, "To", ref to);

			if( (from != befFrom) || (to != befTo) ){
				UpdateMultOffset();
			}


		}

		// x = n/p

		public void UpdateMultOffset(){
			float oldRange = from.y - from.x;
			float newRange = to.y - to.x;
			multiplier = newRange/oldRange; // Might need to warn on division by zero
			offset = to.x - from.x * multiplier;
		}

		public void DrawRemapLine(ref Rect r, string label, ref Vector2 target){
			GUI.Label(r.PadRight(4),label,SF_Styles.MiniLabelRight);
			r = r.MovedRight();
			//SF_GUI.EnterableFloatField( this, r, ref target.x, EditorStyles.textField );
			UndoableEnterableFloatField(r, ref target.x, "lower '" + label.ToLower() + "' value", EditorStyles.textField);
			r = r.MovedRight();
			//SF_GUI.EnterableFloatField( this, r, ref target.y, EditorStyles.textField );
			UndoableEnterableFloatField(r, ref target.y, "upper '" + label.ToLower() + "' value", EditorStyles.textField);
			r = r.MovedDown().MovedLeft(2);
		}


		public override void OnUpdateNode( NodeUpdateType updType = NodeUpdateType.Hard, bool cascade = true ) {
			UpdateMultOffset();
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string inVal = GetInputCon( "IN" ).Evaluate();
			return "(" + inVal + "*" + multiplier.ToString( "0.0###########" ) + "+" + offset.ToString( "0.0###########" ) + ")";
		}

		// TODO Expose more out here!
		public override float EvalCPU( int c ) {
			return GetInputData( "IN", c ) * multiplier + offset;
		}


		public override string SerializeSpecialData() {
			string s = "";
			s += "frmn:" + from.x + ",";
			s += "frmx:" + from.y + ",";
			s += "tomn:" + to.x + ",";
			s += "tomx:" + to.y;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "frmn":
					from.x = float.Parse( value );
					break;
				case "frmx":
					from.y = float.Parse( value );
					break;
				case "tomn":
					to.x = float.Parse( value );
					break;
				case "tomx":
					to.y = float.Parse( value );
					break;
			}
		}





	}
}