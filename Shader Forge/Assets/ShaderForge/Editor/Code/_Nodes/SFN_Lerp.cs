using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Lerp : SF_Node {
		
		public SFN_Lerp() {

		}

		public override void Initialize() {
			base.Initialize( "Lerp" );
			base.showColor = true;
			UseLowerReadonlyValues( true );

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"T","T",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3] );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform && GetInputData( "T" ).uniform );
		}

		public override int GetEvaluatedComponentCount() {
			return Mathf.Max( this["A"].GetCompCount(), this["B"].GetCompCount(), this["T"].GetCompCount() );
		}
		
		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lerp(" + GetConnectorByStringID( "A" ).TryEvaluateAs(GetEvaluatedComponentCount()) + "," + GetConnectorByStringID( "B" ).TryEvaluateAs(GetEvaluatedComponentCount()) + "," + GetInputCon( "T" ).Evaluate() + ")";
		}
		
		public override float EvalCPU( int c ) {
			return Lerp( GetInputData( "B", c ), GetInputData( "B", c ), GetInputData( "T", c ) );
		}

		public float Lerp( float a, float b, float t ) {
			return ( ( 1f - t ) * a + t * b );
		}


	}
}