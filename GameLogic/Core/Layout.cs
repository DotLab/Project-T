using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameLogic.Core {
	public struct Layout {
		public static readonly Layout INIT = new Layout(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), new Vector3(1, 1, 1));

		public Vector3 pos;
		public Quaternion rot;
		public Vector3 sca;

		public Layout(Vector3 pos, Quaternion rot, Vector3 sca) {
			this.pos = pos;
			this.rot = rot;
			this.sca = sca;
		}
	}

	public struct Quaternion {
		public float X, Y, Z, W;

		public Quaternion(float x, float y, float z, float w) {
			X = x; Y = y; Z = z; W = w;
		}
	}

	public struct Vector3 {
		public float X, Y, Z;

		public Vector3(float x, float y, float z) {
			X = x; Y = y; Z = z;
		}
	}
}
