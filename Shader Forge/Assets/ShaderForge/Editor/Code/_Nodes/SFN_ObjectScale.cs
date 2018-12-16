using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_ObjectScale : SF_Node {


		public bool reciprocal;

		public SFN_ObjectScale() {

		}

		public override void Initialize() {
			base.Initialize( "Object Scale", InitialPreviewRenderMode.BlitQuad );
			base.showColor = true;
			base.UseLowerPropertyBox( true, true );
			base.texture.CompCount = 3;
			base.neverDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3,false),
				SF_NodeConnector.Create(this,"X","X",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R).WithColor(Color.red),
				SF_NodeConnector.Create(this,"Y","Y",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G).WithColor(Color.green),
				SF_NodeConnector.Create(this,"Z","Z",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B).WithColor(Color.blue)
			};
		}

		public override Vector4 EvalCPU() {
			return new Color( 1f, 1f, 1f, 1f );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return reciprocal ? "recipObjScale" : "objScale";
		}

		public override string GetBlitShaderSuffix() {
			if( reciprocal )
				return "Reciprocal";
			return "Default";
		}

		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.xMin += 3;
			reciprocal = EditorGUI.Toggle( r, reciprocal );
			r.xMin += 17;
			GUI.Label( r, "Reciprocal" );
			if( EditorGUI.EndChangeCheck() ) {
				OnUpdateNode();
			}
		}

		public override string SerializeSpecialData() {
			return "rcp:" + reciprocal;
		}

		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
				case "rcp":
					reciprocal = bool.Parse( value );
					break;
			}
		}

	}
}