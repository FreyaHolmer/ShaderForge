using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Noise : SF_Node {
		
		public SFN_Noise() {

		}

		public override void Initialize() {
			base.Initialize( "Noise" );
			base.UseLowerPropertyBox(false);
			base.showColor = true;
			base.alwaysDefineVariable = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","Rnd",ConType.cOutput,ValueType.VTv1,false),
				SF_NodeConnector.Create(this,"XY","XY",ConType.cInput,ValueType.VTv2,false).SetRequired(false).TypecastTo(2).WithUseCount(3).SetGhostNodeLink(typeof(SFN_TexCoord),"UVOUT")
			};
		}

		public override int GetEvaluatedComponentCount (){
			return 1;
		}


		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 1 );
			base.OnUpdateNode( updType );
		}



		public string Skew() {
			return GetVariableName() + "_skew";
		}

		public string Rnd() {
			return GetVariableName() + "_rnd";
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { 
				"float2 s = _xy + 0.2127+_xy.x*0.3713*_xy.y;",
				"float2 r = 4.789*sin(489.123*s);",
				"frac(r.x*r.y*(1+s.x))"
			};
		}

		public override string[] GetPreDefineRows (){

			string p = this["XY"].TryEvaluate();
			string r = Rnd();
			string s = Skew();

			return new string[]{
				"float2 "+s+" = "+p+" + 0.2127+"+p+".x*0.3713*"+p+".y;",
				"float2 "+r+" = 4.789*sin(489.123*("+s+"));"
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string r = Rnd();
			string s = Skew();
			return "frac("+r+".x*"+r+".y*(1+"+s+".x))";
		}

		public override Vector4 EvalCPU() {
			
			Vector2 p = GetInputIsConnected( "XY" ) ? GetInputData( "XY" ).dataUniform : Vector4.one;

			float tmp = 0.2127f+p.x*0.3713f*p.y;
			Vector2 s = p + new Vector2(tmp,tmp);

			Vector2 r = Vector2.Scale (new Vector2(4.789f,4.789f), new Vector2(Mathf.Sin(489.123f*s.x),Mathf.Sin(489.123f*s.y)));

			return SF_Tools.Frac(r.x*r.y*(1f+s.x)) * Vector4.one;
		}

	}
}