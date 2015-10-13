using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_HsvToRgb : SF_Node {

		public SFN_HsvToRgb() {
		}

		public override void Initialize() {
			base.Initialize( "HSV to RGB" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues( true );
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this, "OUT", "", ConType.cOutput, ValueType.VTv3, false ),
				SF_NodeConnector.Create( this, "H", "Hue", ConType.cInput, ValueType.VTv1, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "S", "Sat", ConType.cInput, ValueType.VTv1, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "V", "Val", ConType.cInput, ValueType.VTv1, false ).SetRequired( true )};
		}

		public override int GetEvaluatedComponentCount() {
			return 3;
		}

		public override bool IsUniformOutput() {
			if( InputsConnected() ) {
				return GetInputData( "H" ).uniform && GetInputData( "S" ).uniform && GetInputData( "V" ).uniform;
			}
			return true;
		}

		public override string[] GetBlitOutputLines() {
			return new string[] { 
				"float4((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_h.x+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_s.x)*_v.x),0)"
			};
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string h = GetConnectorByStringID( "H" ).TryEvaluate();
			string s = GetConnectorByStringID( "S" ).TryEvaluate();
			string v = GetConnectorByStringID( "V" ).TryEvaluate();
			return "(lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac("+h+"+float3(0.0,-1.0/3.0,1.0/3.0)))-1),"+s+")*" + v + ")";
		}

		static Vector3 offsets = new Vector3(0f,-1f/3f, 1f/3f);

		public override float EvalCPU( int c ) {
			if(c == 3)
				return 1f;
			float h = GetInputData( "H", c );
			float s = GetInputData( "S", c );
			float v = GetInputData( "V", c );
			float o = offsets[c];
			return Mathf.Lerp(1,Mathf.Clamp01(3 * Mathf.Abs(1-2*Frac(h+o)) - 1),s)*v;
		}

		float Frac( float x ) {
			return x - Mathf.Floor( x );
			
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			base.OnUpdateNode( updType );
		}

	}
}