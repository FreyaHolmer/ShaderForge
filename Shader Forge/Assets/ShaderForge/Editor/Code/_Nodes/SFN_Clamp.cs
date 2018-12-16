using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Clamp : SF_Node {

		// SF_Node tNode;

		public SFN_Clamp() {

		}


		public override void Initialize() {
			base.Initialize( "Clamp" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
			UseLowerReadonlyValues( true );

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"MIN","Min",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"MAX","Max",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			if( InputsConnected() )
				return ( GetInputData( "IN" ).uniform && GetInputData( "MIN" ).uniform && GetInputData( "MAX" ).uniform );
			return true;
		}

		public override int GetEvaluatedComponentCount() {
			return Mathf.Max( connectors[1].GetCompCount(), connectors[2].GetCompCount() );
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "clamp(" + GetConnectorByStringID( "IN" ).TryEvaluate() + "," + GetInputCon( "MIN" ).Evaluate() + "," + GetInputCon( "MAX" ).Evaluate() + ")";
		}


		public override float EvalCPU( int c ) {
			//if( c + 1 > GetEvaluatedComponentCount() && GetEvaluatedComponentCount() > 1 ) // Why was this needed before?
			//	return 0f;
			return Mathf.Clamp( GetInputData( "IN", c ), GetInputData( "MIN", c ), GetInputData( "MAX", c ) );
		}



	}
}