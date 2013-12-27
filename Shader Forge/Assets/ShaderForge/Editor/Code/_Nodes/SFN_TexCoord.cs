using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_TexCoord : SF_Node {



		public enum UV { uv0 = 0, uv1 = 1 };
		public UV currentUV = UV.uv0;

		public SFN_TexCoord() {

		}

		public override void Initialize() {
			base.Initialize( "UV Coord." );
			base.UseLowerPropertyBox( true, true );
			base.showColor = true;
			base.texture.uniform = false;
			base.texture.CompCount = 2;

			texture.GenerateTexcoord();

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"UVOUT","UV",ConType.cOutput,ValueType.VTv2).Outputting(OutChannel.RG),
				SF_NodeConnector.Create(this,"U","U",ConType.cOutput,ValueType.VTv1).WithColor(Color.red).Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"V","V",ConType.cOutput,ValueType.VTv1).WithColor(Color.green).Outputting(OutChannel.G)
			};
		}

		public override bool IsUniformOutput() {
			return false;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return SF_Evaluator.WithProgramPrefix(currentUV.ToString());
		}

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			currentUV = (UV)EditorGUI.EnumPopup( lowerRect, currentUV );
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