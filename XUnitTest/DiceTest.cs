using System;
using Xunit;
using GameServer.Core;

namespace XUnitTest {
	public class DiceTest {
		[Theory]
		[InlineData(new int[] { 1, 2, 3 })]
		[InlineData(new int[] { 1, 3, 5 })]
		[InlineData(new int[] { 1, 5, 2, 4 })]
		[InlineData(new int[] { -2, 0, 3, 6, -7 })]
		public void CreateDiceFromPointsAndRoll_RangeTest(int[] points, int diceCount = 4, int testCount = 10000) {
			/*
            DicePoint[] dicePoints = new DicePoint[2];
            dicePoints[0]= new DicePoint(0, 0.8);
            dicePoints[1]= new DicePoint(1, 0.8);
            Dice dice = Dice.Create(dicePoints);
            */
			Assert.True(points.Length > 0);

			int min_p = points[0];
			int max_p = points[0];

			for (int j = 0; j < points.Length; ++j) {
				min_p = points[j] < min_p ? points[j] : min_p;
				max_p = points[j] > max_p ? points[j] : max_p;
			}

			Dice dice = new Dice(DiceType.Create(points));
			int i = 0;
			while (i++ < testCount) {
				int dice_p = dice.Roll(diceCount);
				Assert.InRange(dice_p, min_p * diceCount, max_p * diceCount);

			}

		}

	}
}
