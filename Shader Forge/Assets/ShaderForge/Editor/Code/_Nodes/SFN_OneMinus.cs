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
			UseLowerReadonlyValues( true );

			connectors = new SF_NodeConnection[]{
			SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
			SF_NodeConnection.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
		};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1] );
		}

		public override int GetEvaluatedComponentCount() {
			return this["IN"].GetCompCount();
		}

		public override bool IsUniformOutput() {
			return GetInputData( "IN" ).uniform;
		}


		// New system
		public override void RefreshValue() {
			RefreshValue( 1, 1 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(1.0 - " + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			return 1f - GetInputData( "IN", x, y, c );
		}


	}
}