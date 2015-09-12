using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_UVTile : SF_Node {


		public SFN_UVTile() {

		}


		public override void Initialize() {
			base.Initialize( "UV Tile" );
			base.showColor = true;
			UseLowerReadonlyValues( false );
			base.alwaysDefineVariable = true;
			texture.CompCount = 2;
			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false),
				SF_NodeConnector.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2,false).SetRequired(false).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnector.Create(this,"WDT","Wid",ConType.cInput,ValueType.VTv1,false).SetRequired(true).WithUseCount(2),
				SF_NodeConnector.Create(this,"HGT","Hei",ConType.cInput,ValueType.VTv1,false).SetRequired(true).SetGhostNodeLink(typeof(SFN_Time),"T"),
				SF_NodeConnector.Create(this,"TILE","Tile",ConType.cInput,ValueType.VTv1,false).SetRequired(true).WithUseCount(2),
			};

			//base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
		}




		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			if( GetInputIsConnected( "UVIN" ) && !GetInputData( "UVIN" ).uniform )
				return false;
			if( GetInputIsConnected( "WDT" ) && !GetInputData( "WDT" ).uniform )
				return false;
			if( GetInputIsConnected( "HGT" ) && !GetInputData( "HGT" ).uniform )
				return false;
			if( GetInputIsConnected( "TILE" ) && !GetInputData( "TILE" ).uniform )
				return false;
			return false;
		}

		public override int GetEvaluatedComponentCount() {
			return 2;
		}

		public string TileCountRecip() {
			return GetVariableName() + "_tc_rcp";
		}

		public string TileX() {
			return GetVariableName() + "_tx";
		}

		public string TileY() {
			return GetVariableName() + "_ty";
		}

		public override string[] GetPreDefineRows() {
			return new string[] {
				"float2 " + TileCountRecip() + " = float2(1.0,1.0)/float2( " + this["WDT"].TryEvaluate() + ", "  + this["HGT"].TryEvaluate() + " );",
				"float " + TileY() + " = floor(" + this["TILE"].TryEvaluate() + " * " + TileCountRecip() + ".x);",
				"float " + TileX() + " = " + this["TILE"].TryEvaluate() + " - " + this["WDT"].TryEvaluate() + " * " + TileY() + ";",
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(" + this["UVIN"].TryEvaluate() + " + float2(" + TileX() + ", " + TileY() + ")) * " + TileCountRecip();
		}

		// TODO Expose more out here!
		public override Color NodeOperator( int x, int y ) {

			// GetInputData( "ANG", x, y, 0 )

			Vector2 uv;
			if( GetInputIsConnected( "UVIN" ) ) {
				uv = new Vector2( GetInputData( "UVIN", x, y, 0 ), GetInputData( "UVIN", x, y, 1 ) );
			} else {
				uv = new Vector2( x / SF_NodeData.RESf, y / SF_NodeData.RESf ); // TODO: should use ghost nodes... 
			}
			float tile = GetInputData( "TILE", x, y, 0 );
			float w = GetInputData( "WDT", x, y, 0 );
			float h = GetInputData( "HGT", x, y, 0 );

			float ty = Mathf.Floor( tile / w );
			float tx = tile - w * ty;

			uv.x += tx;
			uv.y += ty;

			uv.x /= w;
			uv.y /= h;


			return new Color( uv.x, uv.y, 0f, 0f );
		}

		float Frac( float x ) {
			return x - Mathf.Floor( x );
		}


	}
}