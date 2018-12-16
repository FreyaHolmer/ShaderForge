using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ChannelBlend : SF_Node_Arithmetic {

		// SF_Node tNode;
		public enum ChannelBlendType{ Summed, Layered };

		public ChannelBlendType channelBlendType = ChannelBlendType.Summed;

		public SFN_ChannelBlend() {

		}


		public override void Initialize() {
			base.Initialize( "Channel Blend" );
			base.PrepareArithmetic(6);
			base.extraWidthInput = 3;
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.Manual;

			//SF_NodeConnection lerpCon;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create( this, "OUT", "", 		ConType.cOutput, 	ValueType.VTvPending, false ),
				SF_NodeConnector.Create( this, "M", "Mask", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( true ).WithUseCount(4),
				SF_NodeConnector.Create( this, "R", "Rcol", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "G", "Gcol", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "B", "Bcol", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "A", "Acol", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( true ),
				SF_NodeConnector.Create( this, "BTM", "Btm", 	ConType.cInput, 	ValueType.VTvPending, false ).SetRequired( false )
			};
			this["BTM"].enableState = EnableState.Disabled;
			base.conGroup = ScriptableObject.CreateInstance<SFNCG_ChannelBlend>().Initialize( connectors[0], connectors[1], connectors[2], connectors[3], connectors[4], connectors[5], connectors[6] );
		}

		public override void OnUpdateNode( NodeUpdateType updType, bool cascade = true ) {
			if( InputsConnected() )
				RefreshValue( 1, 2 );
			if(this["M"].IsConnected()){
				UpdateMaskCompCountInputs();
			}
			//base.OnUpdateNode( updType );
		}

		public override bool IsUniformOutput() {
			foreach(SF_NodeConnector con in connectors){
				if(con.conType == ConType.cOutput || !con.IsConnected())
					continue;
				if(!con.inputCon.node.texture.uniform)
					return false;
			}
			return true;
		}

		public override int GetEvaluatedComponentCount() {
			return this["R"].GetCompCount();
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_Type", channelBlendType == ChannelBlendType.Summed ? 0 : 1 );
		}
		 

		private void UpdateMaskCompCountInputs(){

			int cc = this["M"].GetCompCount();
			base.texture.CompCount = cc;

			bool summed = channelBlendType == ChannelBlendType.Summed;
			//int enableInputCount = summed ? 4 : 5;

			for(int i = 0;i<4;i++){

				SF_NodeConnector con = connectors[i+2];

				//if(con.IsConnected() && con.inputCon.GetCompCount() != cc)
				//	connectors[i+2].Disconnect();

				bool use = i < cc;

				con.SetRequired( use );
				con.enableState = use ? EnableState.Enabled : EnableState.Disabled;

				// Disconnect if going hidden while connected, but not during load, as it might connect an unevaluated cc
				//if(!SF_Parser.quickLoad && !SF_Parser.settingUp){
				if(con.IsConnected() && con.enableState == EnableState.Disabled){
					//connectors[i+2].Disconnect();
					//Debug.Log("Disconnecting thing due to things!");
				}
				//}

			}

			this["BTM"].SetRequired(!summed);
			this["BTM"].enableState = summed ? EnableState.Disabled : EnableState.Enabled;
		}

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			channelBlendType = (ChannelBlendType)UndoableEnumPopup(lowerRect, channelBlendType, "switch channel blend type");
			//currentUV = (UV)EditorGUI.EnumPopup( lowerRect, currentUV );
			if(EditorGUI.EndChangeCheck()){
				UpdateMaskCompCountInputs();
				OnUpdateNode(NodeUpdateType.Hard);
			}
		}

		public override string SerializeSpecialData() {
			return "chbt:" + (int)channelBlendType;
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "chbt":
				channelBlendType = (ChannelBlendType)int.Parse( value );
				break;
			}
		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			int cc = this["M"].GetCompCount();

			string m = this["M"].TryEvaluate();

			string[] chStr = new string[]{"r","g","b","a"};


			

			string str = "(";


			if(channelBlendType == ChannelBlendType.Summed){
				for(int i=0;i<cc;i++){
					string col = connectors[i+2].TryEvaluate();
					str += m + "." + chStr[i] + "*" + col;
					if(i != cc-1) // not last
						str += " + ";
				}
			} else if(channelBlendType == ChannelBlendType.Layered){
				string inner = this["BTM"].TryEvaluateAs(GetEvaluatedComponentCount());
				for(int i=0;i<cc;i++){
					string col = connectors[i+2].TryEvaluate();
					string mask = m + "." + chStr[i];
					inner = "lerp( " + inner + ", " + col + ", " + mask + " )";
				}
				str += inner;
			}



			str += ")";

			return str;

		}


		public override Vector4 EvalCPU() {

			int cc = this["M"].GetCompCount();
			Color m = GetInputData("M").dataUniform;
			string[] chStr = new string[]{"R","G","B","A"};
			Color retCol = Color.black;

			if(channelBlendType == ChannelBlendType.Summed){
				for(int i=0;i<cc;i++){
					Color col = GetInputData( chStr[i] ).dataUniform;
					retCol += m[i] * col;
				}
			} else if(channelBlendType == ChannelBlendType.Layered){
				retCol = GetInputData( "BTM" ).dataUniform;
				for(int i=0;i<cc;i++){
					Color col = GetInputData( chStr[i] ).dataUniform;
					retCol = Color.Lerp(retCol, col, m[i]);
				}
			}

			return retCol;
		}



	}
}