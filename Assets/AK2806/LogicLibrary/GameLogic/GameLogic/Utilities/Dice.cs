using System;

namespace GameLogic.Utilities
{
    public struct DicePoint
    {
        public int value;
        public double weight;
        
        public DicePoint(int v, double w)
        {
            this.value = v;
            this.weight = w;
        }
    }

    public struct DiceType
    {
        public DicePoint[] range;

        public static DiceType Create(int min, int max)
        {
            DiceType ret;
            int count = max - min + 1;
            double weight = 1.0 / count;
            ret.range = new DicePoint[count];
            for (int i = 0; i < count; ++i)
            {
                ret.range[i] = new DicePoint(min + i, weight);
            }
            return ret;
        }

        public static DiceType Create(int[] points)
        {
            DiceType ret;
            double weight = 1.0 / points.Length;
            ret.range = new DicePoint[points.Length];
            for (int i = 0; i < points.Length; ++i)
            {
                ret.range[i] = new DicePoint(points[i], weight);
            }
            return ret;
        }
    }

    public class Dice
    {
        public static Dice Create(DicePoint[] points)
        {
            Dice ret = new Dice();
            ret.CreateDiceType(points);
            return ret;
        }

        public static Dice Create(DiceType diceType)
        {
            Dice ret = new Dice();
            ret.diceType = diceType;
            return ret;
        }

        private Random generator;
        private DiceType diceType;

        public DiceType DiceType
        {
            get
            {
                return this.diceType;
            }
            set
            {
                this.diceType = value;
            }
        }

        public Dice()
        {
            this.generator = new Random();
        }

        public int Roll(int number)
        {
            int ret = 0;
            for (int c = 1; c <= number; ++c)
            {
                double rand = this.generator.NextDouble();
                double step = 0.0;
                int value = this.diceType.range[this.diceType.range.Length - 1].value;
                for (int i = 0; i < this.diceType.range.Length; ++i)
                {
                    double weight = this.diceType.range[i].weight;
                    if (rand >= step && rand < weight + step)
                    {
                        value = this.diceType.range[i].value;
                        break;
                    }
                    step += weight;
                }
                ret += value;
            }
            return ret;
        }

        public void CreateDiceType(DicePoint[] points)
        {
            this.diceType.range = points;
        }

    }
}
