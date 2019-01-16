using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShaderForge {


	[System.Serializable]
	public class SFN_CommentBox : SF_Node_Resizeable {


		public SFN_CommentBox() {
		}

		public override void Initialize() {
			base.Initialize( "Comment Box" );
			base.minWidth = (int)( NODE_WIDTH * 2.5f );
			base.minHeight = NODE_HEIGHT;
			base.ClampSize();
			connectors = new SF_NodeConnector[]{
				//SF_NodeConnector.Create(this,"OUT","Out",ConType.cOutput,ValueType.VTvPending)
			};
		}


		public override void DrawInner( Rect r ) {

			// Things
			UpdateMinHeight();

		}




		public void UpdateMinHeight() {
			base.minHeight = Mathf.Max( NODE_HEIGHT, ( connectors.Length - 1 ) * 20 + 48 );
			base.ClampSize();
		}

	}

}