using UnityEngine;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_NodeStatus : ScriptableObject {

		public SF_Node node;
		public SF_NodeStatus Initialize( SF_Node node ) {
			this.node = node;
			Reset();
			return this;
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}


		public bool leadsToFinal; // Whether or not this is in the end, used

		public void Reset() {
			leadsToFinal = false;
		}




		public void SetLeadsToFinalRecursively(bool all = false, bool passDependent = true) {
			leadsToFinal = true;
			//Debug.Log("Checking if " + node.nodeName + " leads to final...");
			foreach( SF_NodeConnector con in node.connectors ) {
				if( con.conType == ConType.cOutput )
					continue;
				if( !con.IsConnected() )
					continue;
				//if( !con.IsConnectedEnabledAndAvailable() ) // Don't process data in disabled inputs, although maybe we should?
					//continue;
				if( passDependent && con.SkipPasses.Contains( SF_Evaluator.currentPass ) && !all ) // So it's enabled and all - But does this pass even use it?
					continue;
				con.inputCon.node.status.SetLeadsToFinalRecursively();
			}
			//Debug.Log("Yep, " + node.nodeName + " leads to final!");
		}








	}

}
