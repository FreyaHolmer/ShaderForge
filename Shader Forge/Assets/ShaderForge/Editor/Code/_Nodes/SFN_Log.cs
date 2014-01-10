using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Log : SF_Node_Arithmetic {


		public enum LogType { Natural, Base2, Base10 };
		public LogType logType = LogType.Natural;

		public SFN_Log() {

		}

		public override void Initialize() {
			base.Initialize( "Log" );
			base.UseLowerPropertyBox( true, true );
			base.PrepareArithmetic( 1, ValueType.VTvPending, ValueType.VTvPending );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string inner = GetConnectorByStringID( "IN" ).TryEvaluate();
			switch( logType ) {
				case LogType.Natural:
					return "log(" + inner + ")";
				case LogType.Base2:
					return "log2(" + inner + ")";
				case LogType.Base10:
					return "log10(" + inner + ")";
			}

			return inner;
		}

		public override float NodeOperator( int x, int y, int c ) {

			float inpDt = GetInputData( "IN", x, y, c );

			switch( logType ) {
				case LogType.Natural:
					inpDt = Mathf.Log( inpDt );
					break;
				case LogType.Base2:
					inpDt = Mathf.Log( inpDt ) / Mathf.Log( 2f );
					break;
				case LogType.Base10:
					inpDt = Mathf.Log10( inpDt );
					break;
			}

			return inpDt;
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			logType = (LogType)EditorGUI.EnumPopup( lowerRect, logType );
			if( EditorGUI.EndChangeCheck() )
				OnUpdateNode();
		}

		public override string SerializeSpecialData() {
			return "lt:" + (int)logType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "lt":
					logType = (LogType)int.Parse( value );
					break;
			}
		}



	}
}