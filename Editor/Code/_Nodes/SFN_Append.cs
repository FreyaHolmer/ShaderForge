using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Append : SF_Node {

		public override void Initialize() {
			base.Initialize( "Append" );
			base.showColor = true;
			UseLowerReadonlyValues( true );
			SFN_Append.channelColors[3] = SF_NodeConnector.colorEnabledDefault;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"C","",ConType.cInput,ValueType.VTvPending,false).SetRequired(false),
				SF_NodeConnector.Create(this,"D","",ConType.cInput,ValueType.VTvPending,false).SetRequired(false)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Append>();
			(base.conGroup as SFNCG_Append).Initialize( connectors[0], connectors[1], connectors[2], connectors[3], connectors[4] );


			SetExtensionConnectorChain( "B", "C", "D" );
		}

		public override int GetEvaluatedComponentCount() {
			return ( (SFNCG_Append)conGroup ).GetOutputComponentCount();
		}

		public override bool IsUniformOutput() {

			bool a = GetInputIsConnected( "A" );
			bool b = GetInputIsConnected( "B" );
			bool c = GetInputIsConnected( "C" );
			bool d = GetInputIsConnected( "D" );

			if( a && b && c && d )
				return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform && GetInputData( "C" ).uniform && GetInputData( "D" ).uniform );
			else if( a && b && c )
				return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform && GetInputData( "C" ).uniform);

			return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform );
		}


		// New system
		public override void RefreshValue() {
			UpdateInputLabels();
			RefreshValue( 1, 2 );
		}

		public override bool ExhaustedOptionalInputs() {
			return GetEvaluatedComponentCount() >= 4;
		}





		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string varName = "float";
			int compCount = GetEvaluatedComponentCount();
			if( compCount > 1 )
				varName += compCount;

			bool a = GetInputIsConnected( "A" );
			bool b = GetInputIsConnected( "B" );
			bool c = GetInputIsConnected( "C" );
			bool d = GetInputIsConnected( "D" );

			string line = varName + "(";

			if( a && b && c && d )
				line += GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + "," + GetConnectorByStringID( "C" ).TryEvaluate() + "," + GetConnectorByStringID( "D" ).TryEvaluate();
			else if( a && b && c )
				line += GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + "," + GetConnectorByStringID( "C" ).TryEvaluate();
			else
				line += GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate();

			return line + ")";
		}

		public int GetAmountOfConnectedInputs() {
			bool a = GetInputIsConnected( "A" );
			bool b = GetInputIsConnected( "B" );
			bool c = GetInputIsConnected( "C" );
			bool d = GetInputIsConnected( "D" );

			     if(  a &&  b &&  c &&  d )
				return 4;
			else if(  a &&  b &&  c && !d )
				return 3;
			else if(  a &&  b && !c && !d )
				return 2;
			else if(  a && !b && !c && !d )
				return 1;
			else if( !a && !b && !c && !d )
				return 0;

			return 0;
		}

		public override float EvalCPU( int c ) {

			int conCount = GetAmountOfConnectedInputs();

			int cSub = 0;
			for( int i = 0; i < conCount; i++ ) {
				int cc = connectors[i+1].GetCompCount();
				if(c < cc + cSub){
					return GetInputData( connectors[i+1].strID, c - cSub );
				} else {
					cSub += cc;
					continue;
				}
			}
			return 0;
		}


		static Color[] channelColors = new Color[4] { Color.red, Color.green, Color.blue, Color.white };

		public override void PrepareRendering( Material mat ) {

			Vector4[] masks = new Vector4[] {
				Vector4.zero,
				Vector4.zero,
				Vector4.zero,
				Vector4.zero
			};
			Vector4 offsets = Vector4.zero;
			int head = 0;
			for( int i = 0; i < GetAmountOfConnectedInputs(); i++ ) {
				SF_NodeConnector con = connectors[i + 1];
				if( GetInputIsConnected( con.strID ) ) {
					int cc = con.GetCompCount();
					for( int j = head; j < cc + head; j++ ) {
						masks[i][j] = 1f;
					}
					offsets[i] = head;
					head += cc;
				}
			}

			//for( int i = 0; i < 4; i++ ) {
			//	Debug.Log("Masks: " + masks[i]);
			//}

			mat.SetVector( "_A_mask", masks[0] );
			mat.SetVector( "_B_mask", masks[1] );
			mat.SetVector( "_C_mask", masks[2] );
			mat.SetVector( "_D_mask", masks[3] );
			mat.SetVector( "_offsets", offsets );

		}

		public void UpdateInputLabels() {
			
			string rgba = "RGBA";

			int conCount = 4;
			int cSub = 0;
			for( int i = 0; i < conCount; i++ ) {
				SF_NodeConnector con = connectors[i + 1];
				if( GetInputIsConnected( con.strID ) ) {

					int cc = con.GetCompCount();
					con.label = rgba.Substring( cSub, cc );
					if( cc == 1 )
						con.color = channelColors[cSub];
					cSub += cc;

				} else {
					con.label = "";
					con.color = SF_NodeConnector.colorEnabledDefault;
					cSub++;
				}
			}
		}


	}
}