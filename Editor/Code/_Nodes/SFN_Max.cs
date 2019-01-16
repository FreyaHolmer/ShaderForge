using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Max : SF_Node_Arithmetic {

		public SFN_Max() {

		}

		public override void Initialize() {
			base.Initialize( "Max" );
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
			prefix = "max(";
			infix  = ", ";
			suffix = ")";
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string retStr = "max(" + TryEvalInput("A") + "," + TryEvalInput("B") + ")";
			
			// Loop through all chain childs
			foreach(SF_NodeConnector con in connectors){
				if(con.IsConnected() && con.IsChild()){
					retStr = "max(" + retStr + "," + con.TryEvaluate() + ")";
				}
			}
			
			return retStr;
		}

		string TryEvalInput(string s){
			return GetConnectorByStringID(s).TryEvaluate();
		}

		public override float EvalCPU( int c ) {

			float maximum = Mathf.Max( GetInputData( "A", c ), GetInputData( "B", c ) );
			
			// Loop through all chain childs
			foreach(SF_NodeConnector con in connectors){
				if(con.IsConnected() && con.IsChild()){
					maximum = Mathf.Max(maximum, GetInputData( con.strID, c ) );
				}
			}
			
			
			return maximum;
		} 

	}
}