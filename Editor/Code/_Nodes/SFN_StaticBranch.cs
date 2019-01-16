using UnityEngine;
using UnityEditor;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_StaticBranch : SF_Node_Arithmetic {

		public bool on = false;

		public SFN_StaticBranch() {
		}

		public override void Initialize() {
			base.Initialize( "Static Branch" );
			base.PrepareArithmetic(2);
			base.showLowerReadonlyValues = false;
			base.alwaysDefineVariable = true;
			base.onlyPreDefine = true;
			base.showLowerPropertyBox = true;
			base.showLowerPropertyBoxAlways = true;
			base.property = ScriptableObject.CreateInstance<SFP_Branch>().Initialize( this );
		}


		public override void DrawLowerPropertyBox() {
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			r.xMin += 3;
			on = EditorGUI.Toggle( r, on );
			r.xMin += 17;
			GUI.Label(r,"On");
			if( EditorGUI.EndChangeCheck() ) {
				OnUpdateNode();
				editor.shaderEvaluator.ApplyProperty( this );
			}
			
		}

		public override string[] GetPreDefineRows() {
			string indent = "    ";
			string[] rows = new string[]{
				"#ifdef STATIC_BRANCH",
				indent+"float" + GetEvaluatedComponentCount() + " " + GetVariableName() + " = " + this["B"].TryEvaluate() + ";",
				"#else",
				indent+"float" + GetEvaluatedComponentCount() + " " + GetVariableName() + " = " + this["A"].TryEvaluate() + ";",
				"#endif"
			};
			varDefined = true; // Hack
			return rows;

		}


		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			if(!ShouldDefineVariable()) // TODO: Isn't this already handled on the node level?
				this.PreDefine();

			return GetVariableName();
		}

		public override float EvalCPU( int c ) {
			return on ? GetInputData( "B", c ) : GetInputData( "B", c );
		}

		public override string SerializeSpecialData() {
			return "on:" + on;
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "on":
				on = bool.Parse( value );
				editor.shaderEvaluator.ApplyProperty( this );
				break;
			}
		}

	}
}