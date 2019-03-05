using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_InverseLerp : SF_Node {

		public SFN_InverseLerp() {

		}

		public override void Initialize() {
			base.Initialize( "Inverse Lerp" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","T",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"V","Val",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3] );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "((_v-_a)/(_b-_a))" };
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform && GetInputData( "V" ).uniform );
		}

		public override int GetEvaluatedComponentCount() {
			return Mathf.Max( this["A"].GetCompCount(), this["B"].GetCompCount(), this["V"].GetCompCount() );
		}
		
		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string a = GetConnectorByStringID( "A" ).TryEvaluateAs( GetEvaluatedComponentCount() );
			string b = GetConnectorByStringID( "B" ).TryEvaluateAs( GetEvaluatedComponentCount() );
			string v = GetConnectorByStringID( "V" ).TryEvaluateAs( GetEvaluatedComponentCount() );

			return "((" + v + "-" + a + ")/(" + b + "-" + a + "))";
		}
		
		public override float EvalCPU( int c ) {

			float a = GetInputData( "A", c );
			float b = GetInputData( "B", c );
			float v = GetInputData( "V", c );

			if( (b - a) == 0f )
				return 0;
			return ( v - a ) / ( b - a );
		}

		public float Lerp( float a, float b, float t ) {
			return ( ( 1f - t ) * a + t * b );
		}


	}
}