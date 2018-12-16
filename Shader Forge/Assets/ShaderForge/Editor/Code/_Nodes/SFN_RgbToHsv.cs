using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_RgbToHsv : SF_Node {

		public SFN_RgbToHsv() {

		}

		public override void Initialize() {
			base.Initialize( "RGB to HSV" );
			base.UseLowerPropertyBox(false);
			base.showColor = true;
			base.alwaysDefineVariable = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"HOUT","Hue",ConType.cOutput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"SOUT","Sat",ConType.cOutput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"VOUT","Val",ConType.cOutput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTv3,false).SetRequired(true).TypecastTo(3).WithUseCount(3)
			};

			connectors[0].outputChannel = OutChannel.R;
			connectors[1].outputChannel = OutChannel.G;
			connectors[2].outputChannel = OutChannel.B;
		}

		public override int GetEvaluatedComponentCount (){
			return 3;
		}


		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 1 );
			base.OnUpdateNode( updType );
		}



		public string K() {
			return GetVariableName() + "_k";
		}
		public string P() {
			return GetVariableName() + "_p";
		}
		public string Q() {
			return GetVariableName() + "_q";
		}
		public string D() {
			return GetVariableName() + "_d";
		}
		public string E() {
			return GetVariableName() + "_e";
		}


		public override string[] GetBlitOutputLines() {
			return new string[]{
				"float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);",
				"float4 p = lerp(float4(_in.zy, k.wz), float4(_in.yz, k.xy), step(_in.z, _in.y));",
				"float4 q = lerp(float4(p.xyw, _in.x), float4(_in.x, p.yzx), step(p.x, _in.x));",
				"float d = q.x - min(q.w, q.y);",
				"float e = 1.0e-10;",
				"float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, 0);"
			};
		}


		public override string[] GetPreDefineRows (){

			string c = this["IN"].TryEvaluateAs(4);
			string k = K();
			string p = P();
			string q = Q();
			string d = D();
			string e = E();

			return new string[]{
				"float4 "+k+" = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);",
				"float4 "+p+" = lerp(float4("+c+".zy, "+k+".wz), float4("+c+".yz, "+k+".xy), step("+c+".z, "+c+".y));",
				"float4 "+q+" = lerp(float4("+p+".xyw, "+c+".x), float4("+c+".x, "+p+".yzx), step("+p+".x, "+c+".x));",
				"float "+d+" = "+q+".x - min("+q+".w, "+q+".y);",
				"float "+e+" = 1.0e-10;",
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string q = Q();
			string d = D();
			string e = E();
			return "float3(abs(" + q + ".z + (" + q + ".w - " + q + ".y) / (6.0 * " + d + " + " + e + ")), " + d + " / (" + q + ".x + " + e + "), " + q + ".x);";
		}

		public override Vector4 EvalCPU() {

			if( !GetInputIsConnected( "IN" ) )
				return Color.black;

			
			Vector4 c = GetInputData( "IN" ).dataUniform;
			Vector4 k = new Vector4( 0, -1f/3f, 2f/3f, -1f );
			Vector4 p = Vector4.Lerp( new Vector4( c.z, c.y, k.w, k.z ), new Vector4( c.y, c.z, k.x, k.y ), Step( c.z, c.y ));
			Vector4 q = Vector4.Lerp( new Vector4( p.x, p.y, p.w, c.x ), new Vector4( c.x, p.y, p.z, p.x ), Step( p.x, c.x ) );
			float d = q.x - Mathf.Min(q.w, q.y);
			float e = Mathf.Epsilon;

			Vector3 rgb = new Vector3();

			rgb.x = Mathf.Abs(q.z + (q.w - q.y) / (6f * d + e));
			rgb.y = d / ( q.x + e );
			rgb.z = q.x;


			return SF_Tools.VectorToColor( rgb );
		}

		public float Step( float a, float b ) {
			if( a <= b )
				return 1f;
			return 0f;
		}

	}
}