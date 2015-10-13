using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_TexCoord : SF_Node {



		public enum UV { uv0 = 0, uv1 = 1, uv2 = 2, uv3 = 3 };
		public UV currentUV = UV.uv0;

		public SFN_TexCoord() {

		}

		public override void Initialize() {
			base.Initialize( "UV Coord.", InitialPreviewRenderMode.BlitQuad );
			base.UseLowerPropertyBox( true, true );
			base.showColor = true;
			base.texture.uniform = false;
			base.texture.CompCount = 2;
			base.neverDefineVariable = true;

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2),
				SF_NodeConnector.Create(this,"U","U",ConType.cOutput,ValueType.VTv1).WithColor(Color.red).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"V","V",ConType.cOutput,ValueType.VTv1).WithColor(Color.green).Outputting(OutChannel.G)
			};

		}

		public override bool IsUniformOutput() {
			return false;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string s = SF_Evaluator.inTess ? "texcoord" + (int)currentUV : currentUV.ToString();
			
			return SF_Evaluator.WithProgramPrefix( s );
		}

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();

			currentUV = (UV)UndoableEnumPopup(lowerRect, currentUV, "switch UV channel");
			
			if(EditorGUI.EndChangeCheck())
				OnUpdateNode();
		}

		public override string SerializeSpecialData() {
			return "uv:" + (int)currentUV;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "uv":
					currentUV = (UV)int.Parse( value );
					break;
			}
		}


	}
}