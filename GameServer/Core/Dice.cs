using System;

namespace GameServer.Core {
	public struct DicePoint {
		public int value;
		public double weight;

		public DicePoint(int v, double w) {
			this.value = v;
			this.weight = w;
		}
	}

	public struct DiceType {
		public DicePoint[] range;

		public DiceType(DicePoint[] range) {
			this.range = range;
		}

		public static DiceType Create(int min, int max) {
			DiceType ret;
			int count = max - min + 1;
			double weight = 1.0 / count;
			ret.range = new DicePoint[count];
			for (int i = 0; i < count; ++i) {
				ret.range[i] = new DicePoint(min + i, weight);
			}
			return ret;
		}

		public static DiceType Create(int[] points) {
			DiceType ret;
			double weight = 1.0 / points.Length;
			ret.range = new DicePoint[points.Length];
			for (int i = 0; i < points.Length; ++i) {
				ret.range[i] = new DicePoint(points[i], weight);
			}
			return ret;
		}
	}

	public sealed class Dice {
		private Random _generator;
		private DiceType _diceType;

		public DiceType DiceType { get => _diceType; set => _diceType = value; }

		public Dice(DiceType diceType) {
			this._generator = new Random();
			this._diceType = diceType;
		}

		public Dice(DicePoint[] points) : this(new DiceType(points)) {

		}

		public int Roll(int number) {
			int ret = 0;
			for (int c = 1; c <= number; ++c) {
				double rand = this._generator.NextDouble();
				double step = 0.0;
				int value = this._diceType.range[this._diceType.range.Length - 1].value;
				for (int i = 0; i < this._diceType.range.Length; ++i) {
					double weight = this._diceType.range[i].weight;
					if (rand >= step && rand < weight + step) {
						value = this._diceType.range[i].value;
						break;
					}
					step += weight;
				}
				ret += value;
			}
			return ret;
		}
	}

	public static class FateDice {
		private static Dice dice = new Dice(DiceType.Create(-1, 1));

		public static int[] Roll() {
			int[] ret = new int[4];
			for (int i = 0; i < 3; ++i) {
				ret[i] = dice.Roll(1);
			}
			return ret;
		}
	}
}
