using UnityEngine;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_NodeData : ScriptableObject {

		public const int RES = 96; // For node previews
		public const float RESf = (float)RES;

		public SF_Node node;
		public Color[] data;

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_NodeData Initialize( SF_Node node ) {
			this.node = node;
			data = new Color[RES * RES];
			return this;
		}

		public float this[int x, int y, int c] {
			get {
				return data[x + RES * y][c];
			}
			set {
				data[x + RES * y][c] = value;
			}
		}

		public Color this[int x, int y] {
			get {
				return new Color(data[x + RES * y][0],data[x + RES * y][1],data[x + RES * y][2],data[x + RES * y][3]);
			}
			set {
				data[x + RES * y] = value;
			}
		}

	}
}