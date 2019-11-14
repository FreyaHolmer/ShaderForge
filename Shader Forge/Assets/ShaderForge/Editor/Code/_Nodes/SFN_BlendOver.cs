using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_BlendOver : SF_Node {

		public bool gammaCorrect = false;
		
		public SFN_BlendOver() {

		}

		public override void Initialize() {
			base.Initialize( "Blend Over" );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			alwaysDefineVariable = true;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",			ConType.cOutput,ValueType.VTv4,false),
				SF_NodeConnector.Create(this,"OUT_RGB","RGB",	ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"OUT_A","A",		ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A),
				SF_NodeConnector.Create(this,"SRC","Top",		ConType.cInput,ValueType.VTv4,false).WithUseCount(2).SetRequired(true),
				SF_NodeConnector.Create(this,"DST","Btm",		ConType.cInput,ValueType.VTv4,false).WithUseCount(2).SetRequired(true)
			};
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.xMin += 3;
			gammaCorrect = EditorGUI.Toggle( r, gammaCorrect );
			r.xMin += 17;
			GUI.Label( r, "Gamma Corr.", EditorStyles.miniLabel );
			if( EditorGUI.EndChangeCheck() ) {
				OnUpdateNode( NodeUpdateType.Hard );
			}
		}

		public override string SerializeSpecialData() => "gmcr:" + gammaCorrect.ToString();
		public override void DeserializeSpecialData( string key, string value ) {
			if( key == "gmcr" )
				gammaCorrect = bool.Parse( value );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() => ( GetInputData( "SRC" ).uniform && GetInputData( "DST" ).uniform );

		public override int GetEvaluatedComponentCount() => 4;

		public override string GetBlitShaderSuffix() => gammaCorrect ? "GammaCorr" : "Simple";

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string src = GetConnectorByStringID( "SRC" ).TryEvaluate();
			string dst = GetConnectorByStringID( "DST" ).TryEvaluate();

			float gamma = 2.2f;
			float gammaInv = 1f/gamma;

			string a = src + ".a + " + dst + ".a * (1.0-"+src+".a)";
			string rgb;
			if( gammaCorrect == false ) {
				rgb = "lerp( "+ dst + ".rgb * " + dst + ".a, "+src+".rgb, "+src+".a )";
			} else {
				rgb = string.Format( "pow((pow({0}.rgb," + gamma + ") * {0}.a + pow( {1}.rgb," + gamma+") * (1.0-{0}.a) ),"+gammaInv+")", src, dst );
			}
			return "float4( " + rgb + ", " + a + " )";
		}
		
		public override float EvalCPU( int c ) {
			return GetInputData( "SRC", c );
		}


	}
}