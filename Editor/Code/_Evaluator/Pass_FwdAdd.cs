using UnityEngine;
using System.Collections;

namespace ShaderForge{
	public class Pass_FrwAdd : MonoBehaviour {

		public SF_Evaluator eval;


		public Pass_FrwAdd( SF_Evaluator eval ) {
			this.eval = eval;
		}


		public void ForwardAddPass() {
			eval.ResetDefinedState();

			

		}

	

		////////////////////////////////////////////////////////////


		public void StartPass() {
			App( "Pass {" );
			eval.scope++;
		}





		public void EndPass() {
			eval.scope--;
			App( "}" );
		}

		////////////////////////////////////////////////////////////

		public void App( string s ) {
			eval.App( s );
		}
		

	}
}

