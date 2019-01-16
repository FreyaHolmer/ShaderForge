using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	public class SFN_Vector4Property : SF_Node {


		public SFN_Vector4Property() {

		}

		public override void Initialize() {
			base.Initialize( "Vector 4" );
			base.showColor = true;
			base.UseLowerPropertyBox( true );
			base.neverDefineVariable = true;
			base.texture.uniform = true;
			base.texture.CompCount = 4;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;
			lowerRect.width /= 4;

			property = ScriptableObject.CreateInstance<SFP_Vector4Property>().Initialize( this );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"X","X",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"Y","Y",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"W","W",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return property.GetVariable();
		}


		public override void DrawLowerPropertyBox() {

			Vector4 vecPrev = texture.dataUniform;

			if( !IsGlobalProperty() && selected && !SF_GUI.MultiSelectModifierHeld() )
				ColorPickerCorner( lowerRect );

			PrepareWindowColor();
			if(IsGlobalProperty()){
				texture.dataUniform[0] = texture.dataUniform[1] = texture.dataUniform[2] = 0.5f;
				texture.dataUniform[3] = 1f;
				GUI.enabled = false;
			}
			Rect tRect = lowerRect;
			texture.dataUniform[0] = UndoableFloatField(tRect, texture.dataUniform[0], "R channel");
			tRect.x += tRect.width;
			texture.dataUniform[1] = UndoableFloatField(tRect, texture.dataUniform[1], "G channel");
			tRect.x += tRect.width;
			texture.dataUniform[2] = UndoableFloatField(tRect, texture.dataUniform[2], "B channel");
			tRect.x += tRect.width;
			texture.dataUniform[3] = UndoableFloatField(tRect, texture.dataUniform[3], "A channel");
			if(IsGlobalProperty()){
				GUI.enabled = true;
			}
			ResetWindowColor();
			if( texture.dataUniform != vecPrev ) {
				OnUpdateNode( NodeUpdateType.Soft );
				editor.shaderEvaluator.ApplyProperty( this );
			}
				
		}

		public override string SerializeSpecialData() {
			string s = property.Serialize() + ",";
			s += "v1:" + texture.dataUniform[0] + ",";
			s += "v2:" + texture.dataUniform[1] + ",";
			s += "v3:" + texture.dataUniform[2] + ",";
			s += "v4:" + texture.dataUniform[3];
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize(key,value);
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