using UnityEngine;
using System.Collections;


namespace ShaderForge {
	[System.Serializable]
	public class SF_ErrorEntry {

		public SF_Node node;
		public SF_NodeConnector con;
		public string error;
		public bool isWarning;
		public int rows = 1;
		public System.Action action;

		public SF_ErrorEntry( string error, bool isWarning ) {
			this.isWarning = isWarning;
			node = null;
			con = null;
			this.error = error;
			InitializeRows();
		}

		public SF_ErrorEntry( string error, SF_Node target, bool isWarning ) {
			this.isWarning = isWarning;
			node = target;
			con = null;
			this.error = error;
			InitializeRows();
		}

		public SF_ErrorEntry( string error, SF_NodeConnector target, bool isWarning ) {
			this.isWarning = isWarning;
			con = target;
			node = target.node;
			this.error = error;
			InitializeRows();
		}

		void InitializeRows() {
			rows = Mathf.CeilToInt( error.Length / 40f);
		}

		public void OnPress() {
			if( action != null ) {
				action.Invoke();
			}
		}

		public Texture2D icon {
			get {
				return isWarning ? SF_Styles.IconWarningSmall : SF_Styles.IconErrorSmall;
			}
		}

	}

}
