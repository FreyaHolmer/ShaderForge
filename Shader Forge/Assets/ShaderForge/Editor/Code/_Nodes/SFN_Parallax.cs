using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Parallax : SF_Node {

		// SF_Node tNode;

		public SFN_Parallax() {

		}


		public override void Initialize() {
			base.Initialize( "Parallax" );
			base.showColor = true;
			UseLowerReadonlyValues( false );
			texture.CompCount = 2;
			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false).Outputting(OutChannel.RG),
				SF_NodeConnection.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2,false).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnection.Create(this,"HEI","Hei",ConType.cInput,ValueType.VTv1,false).SetRequired(true),
				SF_NodeConnection.Create(this,"DEP","Dep",ConType.cInput,ValueType.VTv1,false),
				SF_NodeConnection.Create(this,"REF","Ref",ConType.cInput,ValueType.VTv1,false)
			};

			//base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
		}


		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		
		public override bool IsUniformOutput() {
			return false;
		}

		public override int GetEvaluatedComponentCount() {
			return 2;
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string uv = GetInputIsConnected( "UVIN" ) ? GetInputCon( "UVIN" ).Evaluate() : "i.uv0.xy";
			string hei = GetInputCon( "HEI" ).Evaluate();
			string dep = GetInputIsConnected( "DEP" ) ? GetInputCon( "DEP" ).Evaluate() : "0.05";
			string href = GetInputIsConnected( "REF" ) ? GetInputCon( "REF" ).Evaluate() : "0.5";
			string vDir = "mul(tangentTransform, viewDirection).xy";

			return "(" + dep + "*(" + hei + " - " + href + ")*" + vDir + " + " + uv + ")";
		}

		// TODO Expose more out here!
		public override float NodeOperator( int x, int y, int c ) {

			//return 1f;


			if( GetInputIsConnected( "UVIN" ) && GetInputIsConnected( "HEI" ) ) { // UV and height connected ?
				float hei = GetInputData( "HEI", x, y, c );
				float dep = GetInputIsConnected( "DEP" ) ? GetInputData( "DEP", x, y, c ) : 0.05f;
				float href = GetInputIsConnected( "REF" ) ? GetInputData( "REF", x, y, c ) : 0.5f;
				return GetInputData( "UVIN", x, y, c ) - ( dep * ( hei - href ) );
			}
			else
				return 0;
			//return Lerp( GetInputData( 1, x, y, c ), GetInputData( 2, x, y, c ), GetInputData( 3, x, y, c ) );
		}


	}
}