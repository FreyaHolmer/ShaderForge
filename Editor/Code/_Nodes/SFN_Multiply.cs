using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Multiply : SF_Node_Arithmetic {

		public SFN_Multiply() {

		}

		public override void Initialize() {
			base.Initialize( "Multiply" );
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.ModularInput;
			UseLowerReadonlyValues( true );
			
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"C","C",ConType.cInput,ValueType.VTvPending,false).SetRequired(false),
				SF_NodeConnector.Create(this,"D","D",ConType.cInput,ValueType.VTvPending,false).SetRequired(false),
				SF_NodeConnector.Create(this,"E","E",ConType.cInput,ValueType.VTvPending,false).SetRequired(false)
			};
			
			
			SetExtensionConnectorChain("B", "C", "D", "E");
			
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5] );
			
		}

		public override void GetModularShaderFixes( out string prefix, out string infix, out string suffix ) {
			prefix = "";
			infix = " * ";
			suffix = "";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			
			
			string evalStr = "";
			
			evalStr += GetConnectorByStringID( "A" ).TryEvaluate() + "*" + GetConnectorByStringID( "B" ).TryEvaluate();

			ChainAppendIfConnected(ref evalStr, "*", "C", "D", "E");
			
			return "(" + evalStr + ")";
		}
		
		public override float EvalCPU( int c ) {
			
			float result = GetInputData( "A", c ) * GetInputData( "B", c );
			
			if(GetInputIsConnected("C")){
				result *= GetInputData( "C", c );
			}
			if(GetInputIsConnected("D")){
				result *= GetInputData( "D", c );
			}
			if(GetInputIsConnected("E")){
				result *= GetInputData( "E", c );
			}
			
			return result;
		}
	}
}