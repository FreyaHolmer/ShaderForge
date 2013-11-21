using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ConstantLerp : SF_Node {

		// SF_Node tNode;

		float lerp_a = 0f;
		float lerp_b = 1f;


		public SFN_ConstantLerp() {

		}


		public override void Initialize() {
			base.Initialize( "Const. Lerp" );
			base.showColor = true;
			UseLowerReadonlyValues( true );
			base.UseLowerPropertyBox( true, true );

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnection[]{
				SF_NodeConnection.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnection.Create(this,"IN","T",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1] );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 1 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			return GetInputData( "IN" ).uniform;
		}

		public override int GetEvaluatedComponentCount() {
			return Mathf.Max( this["IN"].GetCompCount() );
		}



		

		public override void DrawLowerPropertyBox() {

			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.width /= 8;
			GUI.Label( r, "A" );
			r.x += r.width;
			r.width *= 3;
			SF_GUI.EnterableFloatField(this, r, ref lerp_a, null );
			r.x += r.width;
			r.width /= 3;
			GUI.Label( r, "B" );
			r.x += r.width;
			r.width *= 3;
			SF_GUI.EnterableFloatField( this, r, ref lerp_b, null );

		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "lerp(" + lerp_a + "," + lerp_b + "," + GetConnectorByStringID( "IN" ).TryEvaluate() + ")";
		}

		// TODO Expose more out here!
		public override float NodeOperator( int x, int y, int c ) {
			return Lerp( lerp_a, lerp_b, GetInputData( "IN", x, y, c ) );
		}

		public float Lerp( float a, float b, float t ) {
			return ( ( 1f - t ) * a + t * b );
		}

		public override string SerializeSpecialData() {
			string s = "a:" + lerp_a + ",";
			s += "b:" + lerp_b;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "a":
					lerp_a = float.Parse( value );
					break;
				case "b":
					lerp_b = float.Parse( value );
					break;
			}
		}


	}
}