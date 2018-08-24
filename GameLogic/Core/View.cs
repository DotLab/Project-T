namespace GameLogic.Core {
	public sealed class CharacterView {
		public string id;
		public string battle;
		public string story;
	}

	public struct CameraEffect {
		public enum AnimateType {
			None, Shake
		}

		public static readonly CameraEffect INIT = new CameraEffect(AnimateType.None);

		public AnimateType animation;

		public CameraEffect(AnimateType animation) {
			this.animation = animation;
		}
	}

	public struct CharacterViewEffect {
		public enum AnimateType {
			None, Shake
		}

		public static readonly CharacterViewEffect INIT = new CharacterViewEffect(new Vector4(1, 1, 1, 1), AnimateType.None);

		public Vector4 tint;
		public AnimateType animation;

		public CharacterViewEffect(Vector4 tint, AnimateType animation) {
			this.tint = tint;
			this.animation = animation;
		}
	}

	public struct Vector4 {
		public float X, Y, Z, W;

		public Vector4(float x, float y, float z, float w) {
			X = x; Y = y; Z = z; W = w;
		}
	}

	public struct PortraitStyle {
		public static readonly PortraitStyle INIT = new PortraitStyle(0, 0);

		public int action;
		public int emotion;

		public PortraitStyle(int action, int emotion) {
			this.action = action;
			this.emotion = emotion;
		}
	}
}
