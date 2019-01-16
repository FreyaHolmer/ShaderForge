using UnityEngine;
using UnityEditor;
using System.Collections;
//using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Panner : SF_Node {

		// SF_Node tNode;


		public Vector2 speed = new Vector2(1,1);

		public SFN_Panner() {

		}


		public override void Initialize() {
			base.Initialize( "Panner" );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.Modal;
			texture.CompCount = 2;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2,false),
				SF_NodeConnector.Create(this,"UVIN","UV",ConType.cInput,ValueType.VTv2,false).SetRequired(true)/*.SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT")*/,
				SF_NodeConnector.Create(this,"DIST","Dist",ConType.cInput,ValueType.VTv1,false).SetRequired(false).SetGhostNodeLink(typeof(SFN_Time),"T")
			};

		}


		public override void DrawLowerPropertyBox() {
			//EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.width /= 8;
			GUI.Label(r,"U");
			r.x += r.width;
			r.width *= 3;
			//SF_GUI.EnterableFloatField( this, r, ref speed.x, EditorStyles.textField );
			UndoableEnterableFloatField( r, ref speed.x, "U speed", EditorStyles.textField );
			//speed.x = EditorGUI.FloatField( r, speed.x );
			r.x += r.width;
			r.width /= 3;
			GUI.Label( r, "V" );
			r.x += r.width;
			r.width *= 3;
			//SF_GUI.EnterableFloatField( this, r, ref speed.y, EditorStyles.textField );
			UndoableEnterableFloatField( r, ref speed.y, "V speed", EditorStyles.textField );
			//speed.y = EditorGUI.FloatField( r, speed.y );

			//if( EditorGUI.EndChangeCheck() ) {
			//	OnUpdateNode();
			//}

		}

		public override string[] ExtraPassedFloatProperties() {
			return new string[]{
				"Uspeed",
				"Vspeed"
			};
		}

		public override string[] GetModalModes() {
			return new string[]{
				"REQONLY",
				"DIST"
			};
		}

		public override string GetCurrentModalMode() {
			if( this["DIST"].IsConnectedAndEnabled() )
				return "DIST";
			return "REQONLY";
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_uspeed", speed.x );
			mat.SetFloat( "_vspeed", speed.y );
		}

		public override string[] GetBlitOutputLines( string mode ) {
			string distStr = mode == "DIST" ? "_dist.x" : "0";
			return new string[]{
				"float4((_uvin.xy+" + distStr + "*float2(_uspeed,_vspeed)),0,0)"
			};
		}

		public override void OnUpdateNode( NodeUpdateType updType = NodeUpdateType.Hard, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			if(this["UVIN"].IsConnectedAndEnabled() && this["DIST"].IsConnectedAndEnabled()){
				return ( GetInputData( "UVIN" ).uniform && GetInputData( "DIST" ).uniform );
			}
			return false;
		}

		public override int GetEvaluatedComponentCount() {
			return 2;
		}

		public override bool UpdatesOverTime() {
			return true; //GetInputIsConnected( "DIST" );
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string distEval = this["DIST"].TryEvaluate();
			return "(" + GetInputCon( "UVIN" ).Evaluate() + "+" + distEval + "*float2(" + speed.x + "," + speed.y + "))";
		}

		// TODO Expose more out here!
		public override Vector4 EvalCPU() {

			Vector2 inputVec = Vector2.one;

			if(GetInputIsConnected("UVIN")){
				inputVec = new Vector2( GetInputData( "UVIN", 0 ), GetInputData( "UVIN", 1 ) );
			} else {
				//inputVec = new Vector2( x/((float)SF_Node.NODE_SIZE), y/SF_NodeData.RESf ); // TODO: should use ghost nodes... 
			}


			float distance = GetInputIsConnected( "DIST" ) ? GetInputData( "DIST", 0 ) : 0f;
			return (Vector4)( inputVec + speed * distance );
		}


		public override string SerializeSpecialData() {
			string s = "spu:" + speed.x + ",";
			s += "spv:" + speed.y;
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "spu":
					float fVal1 = float.Parse( value );
					speed.x = fVal1;
					break;
				case "spv":
					float fVal2 = float.Parse( value );
					speed.y = fVal2;
					break;
			}
		}





	}
}