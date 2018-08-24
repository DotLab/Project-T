namespace GameLogic.Core {
	public struct Range {
		public bool lowOpen;
		public float low;

		public bool highOpen;
		public float high;

		public Range(float greaterEqual, float less) {
			lowOpen = false;
			highOpen = true;
			low = greaterEqual;
			high = less;
		}

		public bool InRange(float num) {
			bool ret = true;
			ret &= num > low || (!lowOpen && num == low);
			ret &= num < high || (!highOpen && num == high);
			return ret;
		}

		public bool OutOfRange(float num) {
			return !this.InRange(num);
		}
	}
}
