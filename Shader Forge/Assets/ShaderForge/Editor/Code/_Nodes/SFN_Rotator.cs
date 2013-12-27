using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Rotator : SF_Node {

		// SF_Node tNode;

		public SFN_Rotator() {

		}


		public override void Initialize() {
			base.Initialize( "Rotator" );
			base.showColor = true;
			UseLowerReadonlyValues( false );
			base.alwaysDefineVariable = true;
			texture.CompCount = 2;
			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false),
				SF_NodeConnector.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2,false).SetRequired(false).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT"),
				SF_NodeConnector.Create(this,"PIV","Piv",ConType.cInput,ValueType.VTv2,false,"float2(0.5,0.5)").SetRequired(false),
				SF_NodeConnector.Create(this,"ANG","Ang",ConType.cInput,ValueType.VTv1,false).SetRequired(false).SetGhostNodeLink(typeof(SFN_Time),"T"),
				SF_NodeConnector.Create(this,"SPD","Spd",ConType.cInput,ValueType.VTv2,false,"1.0").SetRequired(false),
			};

			//base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2] );
		}




		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "UVIN" ).uniform && GetInputData( "PIV" ).uniform && GetInputData( "ANG" ).uniform && GetInputData( "SPD" ).uniform );
		}

		public override int GetEvaluatedComponentCount() {
			return 2;
		}

		public string Sin() {
			return GetVariableName() + "_sin";
		}
		public string Cos() {
			return GetVariableName() + "_cos";
		}
		public string Spd() {
			return GetVariableName() + "_spd";
		}
		public string Ang() {
			return GetVariableName() + "_ang";
		}
		public string Piv() {
			return GetVariableName() + "_piv";
		}
		public string RotMatrix() {
			return "float2x2( " + Cos() + ", -" + Sin() + ", " + Sin() + ", " + Cos() + ")";
		}

		

		public override string[] GetPreDefineRows() {
			return new string[] {
				"float " + Ang() + " = " + this["ANG"].TryEvaluate() + ";",
				"float " + Spd() + " = " + this["SPD"].TryEvaluate() + ";",
				"float " + Cos() + " = cos("+ Spd() + "*" + Ang() + ");",
				"float " + Sin() + " = sin("+ Spd() + "*" + Ang() + ");",
				"float2 " + Piv() + " = " + this["PIV"].TryEvaluate() + ";"
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "(mul(" + GetInputCon( "UVIN" ).Evaluate() + "-" + Piv() + "," + RotMatrix() + ")+" + Piv() + ")";
		}

		// TODO Expose more out here!
		public override Color NodeOperator( int x, int y ) {

			//return GetInputData( 1 )[x, y];

			float angle = connectors[3].IsConnected() ? GetInputData( "ANG", x, y, 0 ) : Mathf.PI / 8f;
			Vector2 pivot = connectors[2].IsConnected() ? new Vector2( GetInputData( "PIV", x, y, 0 ), GetInputData( "PIV", x, y, 1 ) ) : new Vector2( 0.5f, 0.5f );
			Vector2 vec = new Vector2( GetInputData( "UVIN" )[x, y, 0], GetInputData( "UVIN" )[x, y, 1] ) - pivot;

			float cos = Mathf.Cos( angle );
			float sin = Mathf.Sin( angle );

			Vector4 mtx = new Vector4(
				cos, -sin,
				sin, cos
			);


			Vector2 retVec = new Vector2(
				mtx.x * vec.x + mtx.y * vec.y,
				mtx.z * vec.x + mtx.w * vec.y
			);

			retVec += pivot;

			return new Color( retVec.x, retVec.y, 0f, 0f );//Lerp( GetInputData( 1, x, y, c ), GetInputData( 2, x, y, c ), GetInputData( 3, x, y, c ) );
		}




	}
}