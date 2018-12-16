using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ComponentMask : SF_Node {

		public enum CompChannel {off = -1, r = 0, g = 1, b = 2, a = 3  };
		const string R = "R";
		const string G = "G";
		const string B = "B";
		const string A = "A";
		const string OFF = "-";
		public string[][] compLabels = new string[][]{
			new string[] { OFF, R },
			new string[] { OFF, R, G },
			new string[] { OFF, R, G, B },
			new string[] { OFF, R, G, B, A }
		};
		public string[][] compLabelsFirst = new string[][]{
			new string[] { R },
			new string[] { R, G },
			new string[] { R, G, B },
			new string[] { R, G, B, A }
		};

		public const float colDesat = 0.6f;

		public static Color[] chanColors = new Color[]{
			Color.white,
			new Color( 1f,			colDesat,	colDesat),
			new Color( colDesat,	1f,			colDesat),
			new Color( colDesat*1.1f,	colDesat*1.1f,	1f		),
			Color.white
		};

		public static Color[] outputChanColors = new Color[]{
			Color.red,
			Color.green,
			Color.blue,
			Color.white
		};



		public GUIStyle popupStyle;

		public CompChannel[] components = new CompChannel[] {
			CompChannel.r,
			CompChannel.off,
			CompChannel.off,
			CompChannel.off
		};

		public SFN_ComponentMask() {
			/*
			Initialize("Comp. Mask");
			base.showColor = true;
			UseLowerReadonlyValues(true);
			UseLowerPropertyBox( true, true );

			popupStyle = new GUIStyle( EditorStyles.miniButton );
			popupStyle.alignment = TextAnchor.MiddleCenter;
			popupStyle.fontSize = 12;
			popupStyle.fontStyle = FontStyle.Bold;
		
			connectors = new SF_NodeConnection[]{
				new SF_NodeConnection(this,"-",ConType.cOutput,ValueType.VTvPending,false).Outputting(OutChannel.All),
				new SF_NodeConnection(this,"In",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};
			*/
			//base.conGroup = new SFNCG_Append( connectors[0], connectors[1], connectors[2] );
		}

		public override void Initialize() {
			base.Initialize( "Comp. Mask" );
			base.SearchName = "ComponentMask";
			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.Modal;
			UseLowerReadonlyValues( true );
			UseLowerPropertyBox( true, true );
			SFN_ComponentMask.outputChanColors[3] = SF_NodeConnector.colorEnabledDefault;
			

			popupStyle = new GUIStyle( EditorStyles.miniButton );
			popupStyle.alignment = TextAnchor.MiddleCenter;
			popupStyle.fontSize = 12;
			popupStyle.fontStyle = FontStyle.Bold;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT",	"-",	ConType.cOutput,	ValueType.VTvPending,false)	.Outputting(OutChannel.All),
				SF_NodeConnector.Create(this,"IN",	"In",	ConType.cInput,		ValueType.VTvPending,false)	.SetRequired(true),
				SF_NodeConnector.Create(this,"R",	"R",	ConType.cOutput,	ValueType.VTv1)				.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G",	"G",	ConType.cOutput,	ValueType.VTv1)				.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B",	"B",	ConType.cOutput,	ValueType.VTv1)				.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"A",	"A",	ConType.cOutput,	ValueType.VTv1)										.Outputting(OutChannel.A)
			};
			outCompCount = 1;
			UpdateOutput();
		}


		public override int GetEvaluatedComponentCount() {
			return outCompCount;
		}

		public override bool IsUniformOutput() {
			if( GetInputIsConnected("IN") && !GetInputData( "IN" ).uniform )
				return false;
			return true;
		}


		// New system
		public override void RefreshValue() {
			RefreshValue( 1, 1 );
		}

		public override string[] ExtraPassedFloatProperties() {
			return new string[]{
				"ChR",
				"ChG",
				"ChB",
				"ChA"
			};
		}

		public override string[] GetModalModes() {
			return new string[]{
				"CC1",
				"CC2",
				"CC3",
				"CC4"
			};
		}

		public override void PrepareRendering( Material mat ) {
			mat.SetFloat( "_chr", (int)components[0] );
			mat.SetFloat( "_chg", (int)components[1] );
			mat.SetFloat( "_chb", (int)components[2] );
			mat.SetFloat( "_cha", (int)components[3] );
		}

		public override string GetCurrentModalMode() {
			if( components[1] == CompChannel.off )
				return "CC1";
			if( components[2] == CompChannel.off )
				return "CC2";
			if( components[3] == CompChannel.off )
				return "CC3";
			return "CC4";
		}

		public override string[] GetBlitOutputLines( string mode ) {
			
			string s = "";
			if( mode == "CC1" )
				s = "_in[_chr].xxxx";
			if( mode == "CC2" )
				s = "float4(_in[_chr],_in[_chg],0,0)";
			if( mode == "CC3" )
				s = "float4(_in[_chr],_in[_chg],_in[_chb],0)";
			if( mode == "CC4" )
				s = "float4(_in[_chr],_in[_chg],_in[_chb],_in[_cha])";

			return new string[]{ s };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			//if( outCompCount == inCompCount )
			//	return GetConnectorByStringID( "IN" ).TryEvaluate(); // Unchanged // No!


			string componentString = ".";

			for( int i = 0; i < outCompCount; i++ ) { // Build component string
				componentString += components[i].ToString();
			}

			return GetConnectorByStringID( "IN" ).TryEvaluate() + componentString;
		}

		static string[] writtenNumber = new string[]{"first", "second", "third", "fourth"};

		int outCompCount = 1;
		int inCompCount = 4;

		public override void DrawLowerPropertyBox() {

			inCompCount = 4;
			if( !connectors[1].IsConnected() ) {
				GUI.enabled = false;
			} else {
				inCompCount = connectors[1].GetCompCount();
				if( inCompCount > 4 )
					inCompCount = 4;
			}

			Rect r = lowerRect;
			r.width /= 4;

			bool changed = false;

			EditorGUI.BeginChangeCheck();
			for( int i = 0; i < 4; i++ ) {
				
				
				// Make sure they are valid
				if((int)components[i] >= inCompCount){
					components[i] = (CompChannel)(inCompCount-1);
					changed = true;
				}
				
				
				string[][] labels = (i == 0) ? compLabelsFirst : compLabels;
				int labelOffset = (i == 0) ? 0 : -1; // When skipping OFF
				
				if(!GUI.enabled && i != 0)
					components[i] = CompChannel.off;

				if(connectors[1].IsConnected()){

					int curDisplayIndex = (int)components[i]-labelOffset;
					string[] dispLabels = labels[inCompCount-1];
					GUI.color = chanColors[Mathf.Clamp((int)components[i]+1,0,4)];

					components[i] = (CompChannel)(UndoablePopup( r, curDisplayIndex, dispLabels, "set "+writtenNumber[i]+" component mask channel", popupStyle )+labelOffset);

					//components[i] = (CompChannel)(EditorGUI.Popup( r, curDisplayIndex, dispLabels, popupStyle )+labelOffset);
				}


				if( components[i] == CompChannel.off )
					GUI.enabled = false; // Disable following buttons

				r.x += r.width;
			}

			bool changedCompCount = UpdateOutCompCount();
			UpdateOutput();
			if( EditorGUI.EndChangeCheck() || changedCompCount || changed ) {

				OnUpdateNode();
			}


			GUI.enabled = true;

		}

		bool UpdateOutCompCount() { // returns true if changed
			int prev = outCompCount;
			outCompCount = 0;
			for( int i = 0; i < 4; i++ ) {
				if( components[i] != CompChannel.off )
					outCompCount++;
			}
			if( outCompCount == 0 ) {
				outCompCount = 1;
			}

			if(outCompCount != prev)
				return true;
			return false;
				
		}

		void UpdateOutput() {

			// Set proper value types and component count
			UpdateOutCompCount();
			texture.CompCount = outCompCount;
			switch( outCompCount ) {
				case 1:
					connectors[0].valueType = ValueType.VTv1;
					break;
				case 2:
					connectors[0].valueType = ValueType.VTv2;
					break;
				case 3:
					connectors[0].valueType = ValueType.VTv3;
					break;
				case 4:
					connectors[0].valueType = ValueType.VTv4;
					break;
				default:
					connectors[0].valueType = ValueType.VTvPending;
					texture.CompCount = 4;
					break;
			}



			// Rename the label

			string label = "";

			if( connectors[0].valueType == ValueType.VTvPending ) {
				label = "-";
			} else {
				for( int i = 0; i < outCompCount; i++ ) { // Build component string
					int id = (int)components[i];
					label += compLabels[3][id+1];
				}
			}
			connectors[0].label = label;

			connectors[0].color = outCompCount == 1 ? outputChanColors[Mathf.Clamp((int)components[0],0,3)] : SF_NodeConnector.colorEnabledDefault;


			SF_NodeConnector inCon = GetConnectorByStringID( "IN" );
			if(inCon.IsConnected()){
				for(int i=0;i<4;i++){

					if(i < outCompCount && outCompCount > 1){
						connectors[i+2].enableState = EnableState.Enabled;
					} else {
						connectors[i+2].enableState = EnableState.Hidden;
						connectors[i+2].Disconnect();

					}


					int id = (int)components[i];
					connectors[i+2].label = compLabels[3][id+1];

					connectors[i+2].color = outputChanColors[Mathf.Clamp(id,0,3)];
				}

			}
		}


		public override float EvalCPU( int c ) {
			CompChannel channel = components[c]; // Get the channel the user selected for component i
			if( channel == CompChannel.off ) {
				if(outCompCount > 1)
					return 0f; // Make remaining channels black if using more than one component
				return GetInputData( "IN", (int)components[0] ); // Repeat same value when using one component
			}
			return GetInputData( "IN", (int)channel);
		}

		public override string SerializeSpecialData() {
			string s = "";
			s += "cc1:" + (int)components[0] + ",";
			s += "cc2:" + (int)components[1] + ",";
			s += "cc3:" + (int)components[2] + ",";
			s += "cc4:" + (int)components[3];
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			
			switch( key ) {
				case ( "cc1" ):
					if(value == "4")
						value = "-1";
					components[0] = (CompChannel)int.Parse( value );
					break;
				case ( "cc2" ):
					if(value == "4")
						value = "-1";
					components[1] = (CompChannel)int.Parse( value );
					break;
				case ( "cc3" ):
					if(value == "4")
						value = "-1";
					components[2] = (CompChannel)int.Parse( value );
					break;
				case ( "cc4" ):
					if(value == "4")
						value = "-1";
					components[3] = (CompChannel)int.Parse( value );
					break;
			}
			
			UpdateOutput();
			OnUpdateNode();
		}

	}
}