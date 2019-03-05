using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Relay : SF_Node_Arithmetic {


		public SFN_Relay() {

		}

		public override void Initialize() {
			node_height = 24;
			node_width = 40;
			base.Initialize( "Relay" );
			lowerRect.y -= 8;
			lowerRect.height = 28;
			base.showColor = false;
			base.discreteTitle = true;
			base.UseLowerPropertyBox( true, true );
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			extraWidthInput = -9;
			extraWidthOutput = -9;
			//base.texture.uniform = true;
			//base.texture.CompCount = 1;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"IN","",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize( connectors[0], connectors[1]);

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
			Rect texCoords = new Rect( r );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( r, SF_GUI.Handle_drag, texCoords, alphaBlend:true );
		}





	}
}