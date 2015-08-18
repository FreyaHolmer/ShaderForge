using UnityEngine;
using UnityEditor;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ArcTan2 : SF_Node_Arithmetic {

		public enum ArcTan2Type { NegPiToPi, NegOneToOne, ZeroToOne, ZeroToOneWrapped };
		public static string[] atanTypeStr = new string[] { "-\u03C0 to \u03C0", "-1 to 1", "0 to 1", "0 to 1 Wrapped" };

		public ArcTan2Type arcTanType = ArcTan2Type.NegPiToPi;

		public SFN_ArcTan2() {
		}

		public override void Initialize() {
			base.Initialize( "ArcTan2" );
			base.UseLowerPropertyBox( true, true );
			base.PrepareArithmetic(2);
			connectors[1].label = "y";
			connectors[2].label = "x";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string aStr = GetConnectorByStringID( "A" ).TryEvaluate();
			string bStr = GetConnectorByStringID( "B" ).TryEvaluate();


			if( arcTanType == ArcTan2Type.NegOneToOne )
				return "(atan2(" + aStr + "," + bStr + ")/3.14159265359)";
			if( arcTanType == ArcTan2Type.ZeroToOne )
				return "((atan2(" + aStr + "," + bStr + ")/6.28318530718)+0.5)";
			if( arcTanType == ArcTan2Type.ZeroToOneWrapped )
				return "(1-abs(atan2(" + aStr + "," + bStr + ")/3.14159265359))";
			//if( arcTanType == ArcTan2Type.NegPiToPi )
			return "atan2(" + aStr + "," + bStr + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {

			float a = GetInputData( "A", x, y, c );
			float b = GetInputData( "B", x, y, c );

			if( arcTanType == ArcTan2Type.NegOneToOne )
				return Mathf.Atan2( a, b ) / Mathf.PI;
			if( arcTanType == ArcTan2Type.ZeroToOne )
				return (Mathf.Atan2( a, b ) / (2*Mathf.PI)) + 0.5f;
			if( arcTanType == ArcTan2Type.ZeroToOneWrapped )
				return 1f-(Mathf.Abs(Mathf.Atan2( a, b ) / Mathf.PI));
			//if( arcTanType == ArcTan2Type.NegPiToPi )
			return Mathf.Atan2( a, b );
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			arcTanType = (ArcTan2Type)UndoableEnumPopupNamed( lowerRect, (int)arcTanType, atanTypeStr, "ArcTan2 type" );
			if( EditorGUI.EndChangeCheck() )
				OnUpdateNode();
		}

		public override string SerializeSpecialData() {
			return "attp:" + (int)arcTanType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "attp":
					arcTanType = (ArcTan2Type)int.Parse( value );
					break;
			}
		}

	}
}