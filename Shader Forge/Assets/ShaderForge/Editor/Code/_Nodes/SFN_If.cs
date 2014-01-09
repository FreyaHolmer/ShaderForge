using UnityEngine;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_If : SF_Node_Arithmetic {

		public SFN_If() {
		}

		public override void Initialize() {
			base.Initialize( "If" );
			base.PrepareArithmetic(0);
			base.showLowerReadonlyValues = false;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this,"OUT", "", ConType.cOutput, ValueType.VTvPending, false ),
				SF_NodeConnector.Create( this,"A", "A", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this,"B", "B", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this,"GT", "A>B", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this,"EQ", "A=B", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this,"LT", "A<B", ConType.cInput, ValueType.VTvPending, false ).SetRequired( true )};
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5] );

		}

		
		public string StA() {
			return GetVariableName() + "_if_leA";
		}
		public string StB() {
			return GetVariableName() + "_if_leB";
		}


		public override string[] GetPreDefineRows() {
			return new string[] {
				"float " + StA() + " = step(" + this["A"].TryEvaluate() + "," + this["B"].TryEvaluate() + ");",
				"float " + StB() + " = step(" + this["B"].TryEvaluate() + "," + this["A"].TryEvaluate() + ");"
			};
		}

		public override bool IsUniformOutput() {
			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput)
					continue;
				if(con.IsConnectedAndEnabled())
					if(!con.inputCon.node.IsUniformOutput())
						return false;
			}
			return true;
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			if(!ShouldDefineVariable()) // TODO: Isn't this already handled on the node level?
				this.PreDefine();

			string less = "(" + StA() + "*" + GetInputCon( "LT" ).Evaluate() + ")";
			string larger = "(" + StB() + "*" + GetInputCon( "GT" ).Evaluate() + ")";
			string lela = less + "+" + larger;

			return "lerp(" + lela + "," + GetInputCon( "EQ" ).Evaluate() + "," + StA() + "*" + StB() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {
			float a = GetInputData( "A", x, y, c );
			float b = GetInputData( "B", x, y, c );

			float sta = ( ( a <= b ) ? 1.0f : 0.0f );
			float stb = ( ( b <= a ) ? 1.0f : 0.0f );

			float less = sta * GetInputData( "LT", x, y, c );
			float larger = stb * GetInputData( "GT", x, y, c );
			float lela = ( less + larger );

			return Mathf.Lerp( lela, GetInputData( "EQ", x, y, c ), sta * stb );
		}

	}
}