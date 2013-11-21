using UnityEngine;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_NodeData : ScriptableObject {

		public SF_Node node;
		public Color[] data;

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_NodeData Initialize( SF_Node node ) {
			this.node = node;
			data = new Color[128 * 128];
			return this;
		}

		public float this[int x, int y, int c] {
			get {
				return data[x + 128 * y][c];
			}
			set {
				data[x + 128 * y][c] = value;
			}
		}

		public Color this[int x, int y] {
			get {
				return new Color(data[x + 128 * y][0],data[x + 128 * y][1],data[x + 128 * y][2],data[x + 128 * y][3]);
			}
			set {
				data[x + 128 * y] = value;
			}
		}

	}
}