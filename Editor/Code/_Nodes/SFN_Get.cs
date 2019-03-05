using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Get : SF_Node_Arithmetic {

		public SFN_Get() {

		}

		public SF_NodeConnector[] FindInConnectors() {
			return SF_Editor.instance.nodes.Where( x => x is SFN_Set ).Select( x => x.connectors[0] ).ToArray();
		}

		public string[] GetInConnectorNames( SF_NodeConnector[] connectors ) {
			return connectors.Select( x => x.node.variableName ).ToArray();
		}

		public override void Initialize() {
			node_height = 20;
			node_width = 120;
			base.Initialize( "Get" );
			lowerRect.y -= 8;
			lowerRect.height = 28;
			base.showColor = false;
			base.discreteTitle = true;
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			base.lockedVariableName = false;
			extraWidthInput = -9;
			extraWidthOutput = -9;
			//base.texture.uniform = true;
			//base.texture.CompCount = 1;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
			};

			connectors[1].enableState = EnableState.Hidden;

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);

			editor.nodeView.RefreshRelaySources();

		}

		public override string[] GetBlitOutputLines() {
			return new string[] { "_in" };
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return GetConnectorByStringID( "IN" ).TryEvaluate();
		}

		public override float EvalCPU( int c ) {
			return GetInputData( "IN", c );
		}


		public override void DrawLowerPropertyBox() {
			Rect r = new Rect( lowerRect );
			r.yMin += 4;
			r.yMax -= 2;
			r.xMin += 2;
			Rect[] splitRects = r.SplitHorizontal( 0.25f, 2 );

			int selectedID = -1;
			if( connectors[1].inputCon != null){
				selectedID = editor.nodeView.NodeIdToRelayId( connectors[1].inputCon.node.id );
			}

			EditorGUI.BeginChangeCheck();
			int newID = UndoableEnumPopupNamed( splitRects[1], selectedID, editor.nodeView.relayInNames, "select Get option" );
			if( EditorGUI.EndChangeCheck() ) {
				// Changed input, let's hook it up!
				SF_NodeConnector con = editor.nodeView.relayInSources[newID].con;
				connectors[1].LinkTo( con );
			}


			Rect texCoords = new Rect( splitRects[0] );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( splitRects[0], SF_GUI.Handle_drag, texCoords, alphaBlend:true );
		}





	}
}