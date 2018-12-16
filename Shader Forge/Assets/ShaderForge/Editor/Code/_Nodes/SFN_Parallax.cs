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
			base.shaderGenMode = ShaderGenerationMode.ManualModal;
			UseLowerReadonlyValues( false );
			texture.CompCount = 2;
			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false).Outputting(OutChannel.RG),
				SF_NodeConnector.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2,false).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnector.Create(this,"HEI","Hei",ConType.cInput,ValueType.VTv1,false).SetRequired(true),
				SF_NodeConnector.Create(this,"DEP","Dep",ConType.cInput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"REF","Ref",ConType.cInput,ValueType.VTv1,false)
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

		public override string GetCurrentModalMode() {

			
			bool uvCon = GetInputIsConnected( "UVIN" );
			bool refCon = GetInputIsConnected( "REF" );
			bool depCon = GetInputIsConnected( "DEP" );

			if( !uvCon && !refCon && !depCon ) {
				return "REQONLY";
			}

			if( uvCon && !refCon && !depCon ) {
				return "UV";
			}

			string s = "";
			if( refCon && depCon ) {
				s = "DEP_REF";
			} else {
				if( refCon )
					s = "REF";
				else
					s = "DEP";
			}


			if( GetInputIsConnected( "UVIN" ) ) {
				s = "UV_" + s;
			}

			return s;
			
		}

		public override string[] GetModalModes() {
			return new string[] {
				"REQONLY",
				"DEP",
				"REF",
				"DEP_REF",
				"UV",
				"UV_DEP",
				"UV_REF",
				"UV_DEP_REF",
			};
		}

		public override string[] GetBlitOutputLines( string mode ) {

			string uvStr = mode.Contains( "UV" ) ? "_uv.xy" : "i.uv0.xy";
			string depStr = mode.Contains( "DEP" ) ? "_dep.x" : "0.05";
			string refStr = mode.Contains( "REF" ) ? "_ref.x" : "0.5";
			string vDir = "mul(tangentTransform, viewDirection).xy";

			string line = string.Format( "({0}*({1} - {2})*{3} + {4})", depStr, "_hei", refStr, vDir, uvStr );
			return new string[] { line };

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
		public override float EvalCPU( int c ) {

			//return 1f;


			if( GetInputIsConnected( "UVIN" ) && GetInputIsConnected( "HEI" ) ) { // UV and height connected ?
				float hei = GetInputData( "HEI", c );
				float dep = GetInputIsConnected( "DEP" ) ? GetInputData( "DEP", c ) : 0.05f;
				float href = GetInputIsConnected( "REF" ) ? GetInputData( "REF", c ) : 0.5f;
				return GetInputData( "UVIN", c ) - ( dep * ( hei - href ) );
			}
			else
				return 0;
			//return Lerp( GetInputData( 1, x, y, c ), GetInputData( 2, x, y, c ), GetInputData( 3, x, y, c ) );
		}


	}
}