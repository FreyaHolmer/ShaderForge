using UnityEngine;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_PreviewSettings {
		
		//public SF_PreviewWindow preview;

		// TODO: Load/Save default settings

		public bool previewAutoRotate = true;
		public Color colorBg = SF_GUI.ProSkin ? new Color( 0.2f, 0.2f, 0.2f, 1f ) : new Color( 0.6f, 0.6f, 0.6f, 1f );


		public SF_PreviewSettings( SF_PreviewWindow preview ) {
			//this.preview = preview;
		}


	}
}