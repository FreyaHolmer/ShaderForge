using UnityEngine;
using System.Collections;


namespace ShaderForge {
	[System.Serializable]
	public class SF_ErrorEntry : ScriptableObject {

		public SF_Node node;
		public SF_NodeConnector con;
		public string error;
		public bool isWarning;
		public int rows = 1;
		public System.Action action;

		void OnEnable() {
			hideFlags = HideFlags.HideAndDontSave;
		}

		
		public static SF_ErrorEntry Create( string error, bool isWarning ) {
			SF_ErrorEntry entry = ScriptableObject.CreateInstance<SF_ErrorEntry>();
			entry.isWarning = isWarning;
			entry.error = error;
			entry.InitializeRows();
			return entry;
		}

		public static SF_ErrorEntry Create( string error, SF_Node target, bool isWarning ) {
			SF_ErrorEntry entry = ScriptableObject.CreateInstance<SF_ErrorEntry>();
			entry.isWarning = isWarning;
			entry.node = target;
			entry.error = error;
			entry.InitializeRows();
			return entry;
		}

		public static SF_ErrorEntry Create( string error, SF_NodeConnector target, bool isWarning ) {
			SF_ErrorEntry entry = ScriptableObject.CreateInstance<SF_ErrorEntry>();
			entry.isWarning = isWarning;
			entry.con = target;
			entry.node = target.node;
			entry.error = error;
			entry.InitializeRows();
			return entry;
		}

		void InitializeRows() {
			rows = Mathf.CeilToInt( error.Length / 50f );
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
