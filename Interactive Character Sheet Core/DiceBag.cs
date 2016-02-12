using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interactive_Character_Sheet_Core
{
    public enum DiceSize
    {
        d2 = 2,
        d4 = 4,
        d6 = 6,
        d8 = 8,
        d10 = 10,
        d12 = 12,
        d20 = 20,
        d100 = 100
    };

    public interface IRollable
    {
        public int roll();
        public int roll(int withModifier);
    }

    public class DiceBag
    {
        private Random generator = new Random();

        public int rollOne(DiceSize sides)
        {
            return generator.Next(1, (int)sides + 1);
        }

        public int rollMany(DiceSet dice)
        {
            int sum = 0;
            for (int i = 0; i < dice.number; i++) { sum += rollOne(dice.sides); }
            return sum;
        }
    }

    public struct DiceSet
    {
        public int number { get; set; }
        public DiceSize sides { get; set; }

        public DiceSet(int number, DiceSize diceType)
        {
            this.number = number;
            this.sides = diceType;
        }
    }

    public class DiceCollection: IRollable
    {
        private DiceBag bag = new DiceBag();
        public List<DiceSet> dice = new List<DiceSet>();
        public int modifiers { get; set; }

        public int roll()
        {
            var sum = 0;
            foreach (DiceSet die in dice)
            {
                sum += bag.rollMany(die);
            }
            return sum + modifiers;
        }

        public int rollMaximum(Nullable<DiceSet> additional)
        {
            var sum = 0;
            foreach (DiceSet die in dice)
            {
                sum += (int)die.sides;
            }
            if (additional.HasValue) {
                sum += bag.rollMany(additional.Value);
            }
            return sum + modifiers;
        }

        public int roll(int withModifier)
        {
            return roll() + withModifier;
        }
    }

    static class FixedSizeDice
    {
        public static int rolld20()
        {
            Random r = new Random();
            return r.Next(1, 21);
        }

        public static int rollPercentile()
        {
            Random r = new Random();
            return r.Next(1, 101);
        }
    }
}
