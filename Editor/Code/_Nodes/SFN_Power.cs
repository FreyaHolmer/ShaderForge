using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Power : SF_Node {

		public SFN_Power() {

		}

		public override void Initialize() {
			base.Initialize( "Power" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"VAL","Val",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"EXP","Exp",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
		}


		public override int GetEvaluatedComponentCount() {
			return Mathf.Max( this["VAL"].GetCompCount(), this["EXP"].GetCompCount() );
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "VAL" ).uniform && GetInputData( "EXP" ).uniform );
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "pow(_val,_exp)" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "pow(" + GetInputCon( "VAL" ).Evaluate() + "," + GetInputCon( "EXP" ).Evaluate() + ")";
		}

		// New system
		public override void RefreshValue() {
			RefreshValue( 1, 2 );
		}

		public override float EvalCPU( int c ) {
			return Mathf.Pow( GetInputData( "VAL", c ), GetInputData( "EXP", c ) );
		}
	}
}