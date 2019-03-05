using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Color : SF_Node {


		public SFN_Color() {

		}

		public override void Initialize() {
			//node_height /= 2;
			base.Initialize( "Color" );
			base.showColor = true;
			base.UseLowerPropertyBox( true );
			base.property = ScriptableObject.CreateInstance<SFP_Color>().Initialize( this );
			base.texture.uniform = true;
			base.neverDefineVariable = true;
			base.texture.dataUniform = new Color( 0.5f, 0.5f, 0.5f, 1.0f );
			base.texture.CompCount = 4;
			base.shaderGenMode = ShaderGenerationMode.OffUniform;
			lowerRect.width /= 4;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)							.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B),
				SF_NodeConnector.Create(this,"A","A",ConType.cOutput,ValueType.VTv1)							.Outputting(OutChannel.A)
			};
		}

		public void OnUpdateValue() {
			editor.shaderEvaluator.ApplyProperty( this );
			OnUpdateNode( NodeUpdateType.Soft );
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return property.GetVariable();
		}

		public override void DrawLowerPropertyBox() {

			if(selected && !SF_GUI.MultiSelectModifierHeld() && !IsGlobalProperty())
				ColorPickerCorner( lowerRect );

			Vector4 vecPrev = texture.dataUniform;
			PrepareWindowColor();
			Rect tRect = lowerRect;

			if(IsGlobalProperty()){
				texture.dataUniform[0] = texture.dataUniform[1] = texture.dataUniform[2] = 0.5f;
				texture.dataUniform[3] = 1f;
				GUI.enabled = false;
			}

			texture.dataUniform[0] = UndoableFloatField(tRect, texture.dataUniform[0], "R channel");
			tRect.x += tRect.width;
			texture.dataUniform[1] = UndoableFloatField(tRect, texture.dataUniform[1], "G channel");
			tRect.x += tRect.width;
			texture.dataUniform[2] = UndoableFloatField(tRect, texture.dataUniform[2], "B channel");
			tRect.x += tRect.width;
			texture.dataUniform[3] = UndoableFloatField(tRect, texture.dataUniform[3], "A channel");
			ResetWindowColor();
			if( texture.dataUniform != vecPrev ) {
				OnUpdateValue();
				OnUpdateNode();
			}

			if(IsGlobalProperty()){
				GUI.enabled = true;
			}
				
		}

		public Color GetColor() {
			return texture.dataUniform;
		}

		public override string SerializeSpecialData() {
			string s = property.Serialize() + ",";
			s += "c1:" + texture.dataUniform[0] + ",";
			s += "c2:" + texture.dataUniform[1] + ",";
			s += "c3:" + texture.dataUniform[2] + ",";
			s += "c4:" + texture.dataUniform[3];
			return s;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			property.Deserialize(key,value);
			switch( key ) {
				case "c1":
					float fVal1 = float.Parse( value );
					texture.dataUniform[0] = fVal1;
					break;
				case "c2":
					float fVal2 = float.Parse( value );
					texture.dataUniform[1] = fVal2;
					break;
				case "c3":
					float fVal3 = float.Parse( value );
					texture.dataUniform[2] = fVal3;
					break;
				case "c4":
					float fVal4 = float.Parse( value );
					texture.dataUniform[3] = fVal4;
					break;
			}
		}





	}
}