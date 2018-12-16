using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Dot : SF_Node_Arithmetic {


		public enum DotType { Standard, Positive, Negative, Abs, Normalized };
		public DotType dotType = DotType.Standard;

		public SFN_Dot() {

		}

		public override void Initialize() {
			base.Initialize( "Dot" );
			base.texture.CompCount = 1;
			base.UseLowerPropertyBox( true, true );
			base.PrepareArithmetic(2, ValueType.VTvPending, ValueType.VTv1);
			base.shaderGenMode = ShaderGenerationMode.Modal;
			( base.conGroup as SFNCG_Arithmetic ).LockOutType();
		}

		public override int GetEvaluatedComponentCount() {
			return 1;
		}

		public override string[] GetModalModes() {
			return new string[]{
				"STD",
				"POS",
				"NEG",
				"ABS",
				"NRM"
			};
		}

		public override string GetCurrentModalMode() {
			if( dotType == DotType.Positive )
				return "POS";
			if( dotType == DotType.Negative )
				return "NEG";
			if( dotType == DotType.Abs )
				return "ABS";
			if( dotType == DotType.Normalized )
				return "NRM";
			//if( dotType == DotType.Standard )
			return "STD";
		}

		public override string[] GetBlitOutputLines( string mode ) {
			string dotStr = "dot(_a, _b)";
			switch( mode ) {
				case "POS":
					dotStr = "max(0," + dotStr + ")";
					break;
				case "NEG":
					dotStr = "min(0," + dotStr + ")";
					break;
				case "ABS":
					dotStr = "abs(" + dotStr + ")";
					break;
				case "NRM":
					dotStr = "0.5*" + dotStr + "+0.5";
					break;
			}
			return new string[]{dotStr};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string dotStr = "dot(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
			switch( dotType ) {
				case DotType.Positive:
					return "max(0," + dotStr + ")";
				case DotType.Negative:
					return "min(0," + dotStr + ")";
				case DotType.Abs:
					return "abs(" + dotStr + ")";
				case DotType.Normalized:
					return "0.5*" + dotStr + "+0.5";
			}
			return dotStr;
		}

		public override Vector4 EvalCPU() {


			int cc = Mathf.Max(GetInputCon("A").GetCompCount(), GetInputCon("B").GetCompCount());

			float dot = SF_Tools.Dot( GetInputData( "A" ).dataUniform, GetInputData( "B" ).dataUniform,  cc );

			switch( dotType ) {
				case DotType.Positive:
					dot = Mathf.Max(0f,dot);
					break;
				case DotType.Negative:
					dot = Mathf.Min(0f,dot);
					break;
				case DotType.Abs:
					dot = Mathf.Abs(dot);
					break;
				case DotType.Normalized:
					dot = 0.5f*dot+0.5f;
					break;
			}

			return dot * Vector4.one;
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			dotType = (DotType)UndoableEnumPopup( lowerRect, dotType, "dot product type" );
			if( EditorGUI.EndChangeCheck() )
				OnUpdateNode();
		}

		public override string SerializeSpecialData() {
			return "dt:" + (int)dotType;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "dt":
					dotType = (DotType)int.Parse( value );
					break;
			}
		}



	}
}