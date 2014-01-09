using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Exp : SF_Node_Arithmetic {


		public enum ExpType { Exp, Exp2 };
		public ExpType expType = ExpType.Exp;

		public SFN_Exp() {

		}

		public override void Initialize() {
			base.Initialize( "Exp" );
			base.UseLowerPropertyBox( true, true );
			base.PrepareArithmetic( 1, ValueType.VTvPending, ValueType.VTvPending );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string inner = GetConnectorByStringID( "IN" ).TryEvaluate();
			switch( expType ) {
				case ExpType.Exp:
					return "exp(" + inner + ")";
				case ExpType.Exp2:
					return "exp2(" + inner + ")";
			}

			return inner;
		}

		public override float NodeOperator( int x, int y, int c ) {

			float inpDt = GetInputData( "IN", x, y, c );

			switch( expType ) {
				case ExpType.Exp:
					inpDt = Mathf.Pow( 2.718281828459f, inpDt );
					break;
				case ExpType.Exp2:
					inpDt = Mathf.Pow( 2f, inpDt );
					break;
			}

			return inpDt;
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			expType = (ExpType)EditorGUI.EnumPopup( lowerRect, expType );
			if( EditorGUI.EndChangeCheck() )
				OnUpdateNode();
		}

		public override string SerializeSpecialData() {
			return "et:" + (int)expType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "et":
					expType = (ExpType)int.Parse( value );
					break;
			}
		}



	}
}