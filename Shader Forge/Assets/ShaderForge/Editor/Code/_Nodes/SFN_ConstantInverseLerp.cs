using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ConstantInverseLerp : SF_Node {

		// SF_Node tNode;

		public float invlerp_a = 0f;
		public float invlerp_b = 1f;


		public SFN_ConstantInverseLerp() {

		}


		public override void Initialize() {
			base.Initialize( "InvLerp (Simple)" );
			base.SearchName = "Inverse Lerp Simple";
			base.showColor = true;
			UseLowerReadonlyValues( true );
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.ValuePassing;

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","V",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
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



		public override string[] ExtraPassedFloatProperties() {
			return new string[]{
				"InvLerpA",
				"InvLerpB"
			};
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_invlerpa", invlerp_a );
			mat.SetFloat( "_invlerpb", invlerp_b );
		}

		public override string[] GetBlitOutputLines( ) {
			return new string[] { "lerp(_invlerpa, _invlerpb, _in)" };
		}



		public override void DrawLowerPropertyBox() {

			//EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.width /= 8;
			GUI.Label( r, "A" );
			r.x += r.width;
			r.width *= 3;
			//SF_GUI.EnterableFloatField(this, r, ref lerp_a, null );
			UndoableEnterableFloatField(r, ref invlerp_a, "A value",null);
			r.x += r.width;
			r.width /= 3;
			GUI.Label( r, "B" );
			r.x += r.width;
			r.width *= 3;
			//SF_GUI.EnterableFloatField( this, r, ref lerp_b, null );
			UndoableEnterableFloatField(r, ref invlerp_b, "B value",null);

		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string v = GetConnectorByStringID( "IN" ).TryEvaluate();
			return $"({v}-{invlerp_a})/({invlerp_b}-{invlerp_a})";
		}

		public override float EvalCPU( int c ) {
			return InverseLerp( invlerp_a, invlerp_b, GetInputData( "IN", c ) );
		}

		float InverseLerp( float a, float b, float v ) => ( v - a ) / ( b - a );

		public override string SerializeSpecialData() {
			string s = "a:" + invlerp_a + ",";
			s += "b:" + invlerp_b;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "a":
					invlerp_a = float.Parse( value );
					break;
				case "b":
					invlerp_b = float.Parse( value );
					break;
			}
		}


	}
}