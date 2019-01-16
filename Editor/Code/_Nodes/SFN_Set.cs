using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Set : SF_Node_Arithmetic {

		public SFN_Set() {

		}

		public override void Initialize() {
			node_height = 20;
			node_width = 120;
			base.Initialize( "Set" );
			lowerRect.y -= 8;
			lowerRect.height = 28;
			base.showColor = false;
			base.discreteTitle = true;
			base.alwaysDefineVariable = true;
			base.UseLowerPropertyBox( true, true );
			base.lockedVariableName = false; // In order for it to serialize
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			extraWidthInput = -9;
			extraWidthOutput = -9;
			//base.texture.uniform = true;
			//base.texture.CompCount = 1;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
			};

			connectors[0].enableState = EnableState.Hidden;

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);

			editor.nodeView.RefreshRelaySources();
		}

		public override bool CanCustomizeVariable() {
			return false; // Never allow using the dropdown varname editor
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
			Rect[] splitRects = r.SplitHorizontal( 0.75f, 2 );
			EditorGUI.BeginChangeCheck();
			variableName = UndoableTextField( splitRects[0], variableName, "Set variable name", null );
			if( EditorGUI.EndChangeCheck() ) {
				editor.nodeView.RefreshRelaySources();
			}
			Rect texCoords = new Rect( splitRects[1] );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( splitRects[1], SF_GUI.Handle_drag, texCoords, alphaBlend: true );
		}





	}
}