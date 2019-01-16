using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_OneMinus : SF_Node {

		public SFN_OneMinus() {

		}

		public override void Initialize() {
			base.Initialize( "One Minus" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );

			connectors = new SF_NodeConnector[]{
			SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
			SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
		};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1] );
		}

		public override int GetEvaluatedComponentCount() {
			return this["IN"].GetCompCount();
		}

		public override bool IsUniformOutput() {
			return GetInputData( "IN" ).uniform;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "1.0 - _in" };
		}


		// New system
		public override void RefreshValue() {
			RefreshValue( 1, 1 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(1.0 - " + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float EvalCPU( int c ) {
			return 1f - GetInputData( "IN", c );
		}


	}
}