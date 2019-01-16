using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_TexCoord : SF_Node {



		public enum UV { uv0 = 0, uv1 = 1, uv2 = 2, uv3 = 3 };
		public UV currentUV = UV.uv0;
		public bool useAsFloat4 = false;

		public SFN_TexCoord() {

		}

		public override void Initialize() {
			base.Initialize( "UV Coord.", InitialPreviewRenderMode.BlitQuad );
			base.UseLowerPropertyBox( true, true );
			base.showColor = true;
			base.texture.uniform = false;
			base.texture.CompCount = 4;
			base.neverDefineVariable = true;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2),
				SF_NodeConnector.Create(this,"U","U",ConType.cOutput,ValueType.VTv1).WithColor(Color.red).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"V","V",ConType.cOutput,ValueType.VTv1).WithColor(Color.green).Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1).WithColor(Color.blue).Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"W","W",ConType.cOutput,ValueType.VTv1).Outputting(OutChannel.A)
			};

			UpdateConnectorVisibility();


		}

		public override int GetEvaluatedComponentCount() {
			return useAsFloat4 ? 4 : 2;
		}

		public override bool IsUniformOutput() {
			return false;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string s = SF_Evaluator.inTess ? "texcoord" + (int)currentUV : currentUV.ToString();
			
			return SF_Evaluator.WithProgramPrefix( s );
		}

		static string[] float4Names = new string[] { "uv", "uvzw" };
		const string undoCompCountSwitch = "uv component count";
		const string undoSwitchUvChannel = "switch UV channel";

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();

			Rect[] rects = lowerRect.SplitHorizontal( 0.5f );

			currentUV = (UV)UndoableEnumPopup( rects[0], currentUV, undoSwitchUvChannel );
			int curVal = useAsFloat4 ? 1 : 0;
			int newVal = UndoableEnumPopupNamed( rects[1], curVal, float4Names, undoCompCountSwitch );
			useAsFloat4 = newVal == 1;

			if( EditorGUI.EndChangeCheck() ) {
				UpdateConnectorVisibility();
				OnUpdateNode();
			}
		}

		void UpdateConnectorVisibility() {
			SF_NodeConnector z = GetConnectorByID( "Z" );
			SF_NodeConnector w = GetConnectorByID( "W" );
			if( !useAsFloat4 ) {
				if(z.IsConnected()){
					for( int i = 0; i < z.outputCons.Count; i++ ) {
						Undo.RecordObject( z.outputCons[i], "disconnect" );
					}
					Undo.RecordObject( z, "disconnect" );
					z.Disconnect();
				}
				if( w.IsConnected() ) {
					for( int i = 0; i < w.outputCons.Count; i++ ) {
						Undo.RecordObject( w.outputCons[i], "disconnect" );
					}
					Undo.RecordObject( w, "disconnect" );
					w.Disconnect();
				}
			}
			EnableState enableState = useAsFloat4 ? EnableState.Enabled : EnableState.Disabled;
			z.enableState = enableState;
			w.enableState = enableState;
		}

		public override string SerializeSpecialData() {
			string s = "";
			s += "uv:" + (int)currentUV + ",";
			s += "uaff:" + useAsFloat4.ToString();
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "uv":
					currentUV = (UV)int.Parse( value );
					break;
				case "uaff":
					useAsFloat4 = (bool)bool.Parse( value );
					UpdateConnectorVisibility();
					break;
			}
		}


	}
}