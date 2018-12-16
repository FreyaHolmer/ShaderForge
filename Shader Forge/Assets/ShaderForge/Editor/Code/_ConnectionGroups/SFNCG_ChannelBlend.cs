using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	// Used to detect types based on input
	[System.Serializable]
	public class SFNCG_ChannelBlend : SFNCG_Arithmetic {
		

		public SFNCG_ChannelBlend() {

		}

		public new SFNCG_ChannelBlend Initialize( SF_NodeConnector output, params SF_NodeConnector[] inputs ) {
			SF_NodeConnector[] inputsWithoutFirst = new SF_NodeConnector[inputs.Length-1];
			for(int i=1;i<inputs.Length;i++){
				inputsWithoutFirst[i-1] = inputs[i];
			}
			base.Initialize(output,inputsWithoutFirst);
			return this;
		}

	}
}