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
			base.shaderGenMode = ShaderGenerationMode.Modal;
		}

		
		public override string[] GetModalModes() {
			return new string[] {
				"LOG",
				"LOG2",
				"LOG10"
			};
		}

		public override string GetCurrentModalMode() {
			switch( logType ) {
				case LogType.Base10:
					return "LOG10";
				case LogType.Base2:
					return "LOG2";
				default:
					return "LOG";
			}
		}

		public override string[] GetBlitOutputLines( string mode ) {
			if( mode == "LOG2" )
				return new string[] { "log(_in);" };
			if( mode == "LOG10" )
				return new string[] { "log10(_in);" };
			return new string[] { "log(_in)" };
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

		public override float EvalCPU( int c ) {

			float inpDt = GetInputData( "IN", c );

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
			logType = (LogType)UndoableEnumPopup( lowerRect, logType, "switch log type");
			//logType = (LogType)EditorGUI.EnumPopup( lowerRect, logType );
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