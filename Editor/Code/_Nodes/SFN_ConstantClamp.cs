using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ConstantClamp : SF_Node {

		// SF_Node tNode;

		public float min = 0f;
		public float max = 1f;


		public SFN_ConstantClamp() {

		}


		public override void Initialize() {
			base.Initialize( "Clamp (Simple)" );
			base.SearchName = "Clamp Simple";
			base.showColor = true;
			UseLowerReadonlyValues( true );
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.ValuePassing;

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
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
				"ClampMin",
				"ClampMax"
			};
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_clampmin", min );
			mat.SetFloat( "_clampmax", max );
		}

		public override string[] GetBlitOutputLines( ) {
			return new string[] { "clamp( _in, _clampmin, _clampmax )" };
		}

		/*
		public void UndoableEnterableFloatField(Rect r, ref float value, string undoMessage, GUIStyle style){
			SF_GUI.EnterableFloatField(this, r, ref value, null );
		}
*/

		

		public override void DrawLowerPropertyBox() {

			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.width /= 4;
			GUI.Label( r, "Min", EditorStyles.miniLabel );
			r.x += r.width;
			//SF_GUI.EnterableFloatField(this, r, ref min, null );
			UndoableEnterableFloatField(r, ref min, "min value", null);
			r.x += r.width;
			GUI.Label( r, "Max", EditorStyles.miniLabel );
			r.x += r.width;
			//SF_GUI.EnterableFloatField( this, r, ref max, null );
			UndoableEnterableFloatField(r, ref max, "max value", null);

		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return "clamp(" + GetConnectorByStringID( "IN" ).TryEvaluate() + "," + min + "," + max + ")";
		}

		// TODO Expose more out here!
		public override float EvalCPU( int c ) {
			if( GetEvaluatedComponentCount() != 1 )
				if( c + 1 > GetEvaluatedComponentCount() )
					return 0f;
			return Mathf.Clamp( GetInputData( "IN", c ), min, max );
		}

		public override string SerializeSpecialData() {
			string s = "min:" + min + ",";
			s += "max:" + max;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "min":
					min = float.Parse( value );
					break;
				case "max":
					max = float.Parse( value );
					break;
			}
		}


	}
}