using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Desaturate : SF_Node {

		// SF_Node tNode;

		public SFN_Desaturate() {

		}


		public override void Initialize() {
			base.Initialize( "Desaturate" );
			base.showColor = true;
			UseLowerReadonlyValues( true );

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"COL","Col",ConType.cInput,ValueType.VTvPending,false).SetRequired(true).TypecastTo(3),
				SF_NodeConnector.Create(this,"DES","Des",ConType.cInput,ValueType.VTv1,false).SetRequired(false)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );

		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			if(this["DES"].IsConnected())
				return ( GetInputData( "COL" ).uniform && GetInputData( "DES" ).uniform );
			return GetInputData( "COL" ).uniform;
		}

		public override int GetEvaluatedComponentCount() {
			return this["DES"].IsConnected() ? 3 : 1;
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string col = GetInputCon( "COL" ).Evaluate();
			string lum = "float3(0.3,0.59,0.11)";
			string dot = "dot("+col+","+lum+")";

			if( this["DES"].IsConnected() ) {
				string desat = GetInputCon( "DES" ).Evaluate();
				return "lerp(" + col + "," + dot + "," + desat + ")";
			} else {
				return dot; // Fully desaturated
			}
		}

		// TODO Expose more out here!
		public override Color NodeOperator( int x, int y ) {

			Color col = GetInputData( "COL" )[x,y];
			Color lum = new Color( 0.3f, 0.59f, 0.11f );
			Color dot = SF_Tools.FloatToColor(SF_Tools.Dot( col, lum ));

			float desat = 1f;
			if( this["DES"].IsConnected() ) {
				desat = GetInputData( "DES" )[x, y, 0];
			}

			return Color.Lerp(col, dot, desat);
		}


	}
}