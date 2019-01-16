using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	public class SFN_Vector4 : SF_Node {


		public SFN_Vector4() {

		}

		public override void Initialize() {
			node_height /= 2;
			base.Initialize( "Vector 4" );
			base.showColor = true;
			base.UseLowerPropertyBox( true );
			base.texture.uniform = true;
			base.texture.CompCount = 4;
			base.canAlwaysSetPrecision = true;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;
			lowerRect.width /= 4;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv4,false)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return precision.ToCode() + "4(" + texture.dataUniform[0] + "," + texture.dataUniform[1] + "," + texture.dataUniform[2] + "," + texture.dataUniform[3] + ")";
		}


		public override void DrawLowerPropertyBox() {

			if( selected && !SF_GUI.MultiSelectModifierHeld() )
				ColorPickerCorner( lowerRect );

			//Color vecPrev = texture.dataUniform;
			Rect tRect = lowerRect;
			UndoableEnterableFloatField( tRect, ref texture.dataUniform.x, "R channel", null);
			tRect.x += tRect.width;
			UndoableEnterableFloatField( tRect, ref texture.dataUniform.y, "G channel", null);
			tRect.x += tRect.width;
			UndoableEnterableFloatField( tRect, ref texture.dataUniform.z, "B channel", null);
			tRect.x += tRect.width;
			UndoableEnterableFloatField( tRect, ref texture.dataUniform.w, "A channel", null);

		}

		public override string SerializeSpecialData() {
			string s = "v1:" + texture.dataUniform[0] + ",";
			s += "v2:" + texture.dataUniform[1] + ",";
			s += "v3:" + texture.dataUniform[2] + ",";
			s += "v4:" + texture.dataUniform[3];
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
				case "v3":
					float fVal3 = float.Parse( value );
					texture.dataUniform[2] = fVal3;
					break;
				case "v4":
					float fVal4 = float.Parse( value );
					texture.dataUniform[3] = fVal4;
					break;
			}
		}





	}
}