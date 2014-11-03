
using UnityEngine;

// OLD: [Source][Con] <---- [Con][Target]
// NEW: [Target][Con] ----> [Con][Source]

namespace ShaderForge{
	public struct SF_Link {
		public int sNode; // Source
		string sCon;
		public int tNode; // Target
		string tCon;
		
		public SF_Link( int sNode, string linkData ) {
			this.sNode = sNode;
			string[] split = linkData.Split( '-' );
			if(split.Length != 3){
				Debug.Log("Invalid link on node " + sNode + ". Expected 3 entries, found " + split.Length + ". Link Data = [" + linkData + "]");
			}
			sCon = split[0];
			tNode = int.Parse( split[1] );
			tCon = split[2];
		}
		
		public void Establish( SF_Editor editor, LinkingMethod linkMethod = LinkingMethod.NoUpdate ) {
			SF_Node source = editor.GetNodeByID( sNode );
			SF_Node target = editor.GetNodeByID( tNode );
			// Debug.Log( "Linking " + target.nodeName + " <- " + source.nodeName );
			
			target.GetConnectorByID(tCon).LinkTo( source.GetConnectorByID(sCon), linkMethod );
		}
		
		
		
		public void Remap(int[] oldIDs, int[] newIDs){
			// Source id switching
			for(int i=0; i<oldIDs.Length; i++){
				if(sNode == oldIDs[i]){
					sNode = newIDs[i];
					break;
				}
			}
			
			// Target id switching
			for(int i=0; i<newIDs.Length; i++){
				if(tNode == oldIDs[i]){
					tNode = newIDs[i];
					break;
				}
			}
			
		}

	}
}