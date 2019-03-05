using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Vector2 : SF_Node {


		public SFN_Vector2() {

		}

		public override void Initialize() {
			node_height /= 2;
			base.Initialize( "Vector 2" );
			base.showColor = true;
			base.UseLowerPropertyBox( true );
			base.texture.uniform = true;
			base.canAlwaysSetPrecision = true;
			base.texture.CompCount = 2;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;
			lowerRect.width /= 2;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv2,false)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return precision.ToCode() + "2(" + texture.dataUniform[0] + "," + texture.dataUniform[1] + ")";
		}

		public override void DrawLowerPropertyBox() {

			if( selected && !SF_GUI.MultiSelectModifierHeld() )
				ColorPickerCorner(lowerRect);

			Vector4 cPrev = texture.dataUniform;
			Rect tRect = lowerRect;
			//SF_GUI.EnterableFloatField( this, tRect, ref texture.dataUniform.r, null );
			UndoableEnterableFloatField(tRect,ref texture.dataUniform.x, "R channel", null);
			tRect.x += tRect.width;
			//SF_GUI.EnterableFloatField( this, tRect, ref texture.dataUniform.g, null );
			UndoableEnterableFloatField(tRect,ref texture.dataUniform.y, "G channel", null);
			if( texture.dataUniform != cPrev )
				OnUpdateNode();
			 
		}


		public override string SerializeSpecialData() {
			string s = "v1:" + texture.dataUniform[0] + ",";
			s += "v2:" + texture.dataUniform[1];
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "v1":
					float fVal1 = float.Parse( value );
					texture.dataUniform[0] = fVal1;
					break;
				case "v2":
					float fVal2 = float.Parse( value );
					texture.dataUniform[1] = fVal2;
					break;
			}
		}




	}
}