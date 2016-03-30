using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveCharacterSheetCore
{
    public enum DiceSize
    {
        None = 0,
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
        int Roll();
        int Roll(int withModifier);
    }

    public class DiceBag
    {
        private Random generator = new Random();

        public int RollOne(DiceSize sides)
        {
            return generator.Next(1, (int)sides + 1);
        }

        public int RollMany(DiceSet dice)
        {
            int sum = 0;
            for (int i = 0; i < dice.Number; i++) { sum += RollOne(dice.Sides); }
            return sum;
        }
    }

    public struct DiceSet
    {
        public int Number { get; set; }
        public DiceSize Sides { get; set; }

        public DiceSet(int number, DiceSize diceType) :this()
        {

            Number = number;
            Sides = diceType;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiceSet)) return false;
            var other = (DiceSet)obj;
            return (other.Number == Number && other.Sides == Sides);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(DiceSet d1, DiceSet d2)
        {
            return d1.Equals(d2);
        }
        
        public static bool operator !=(DiceSet d1, DiceSet d2)
        {
            return !d1.Equals(d2);
        }
    }

    public class Dice: IRollable
    {
        private DiceBag bag = new DiceBag();
        private List<DiceSet> activeDice = new List<DiceSet>();
        public int Modifiers { get; set; }

        public Dice (IEnumerable<DiceSet> dice)
        {
            activeDice.AddRange(dice);
        }

        public int Roll()
        {
            var sum = 0;
            foreach (DiceSet die in activeDice)
            {
                sum += bag.RollMany(die);
            }
            return sum + Modifiers;
        }

        public int RollMaximum(DiceSet? additional)
        {
            var sum = 0;
            foreach (DiceSet die in activeDice)
            {
                sum += (int)die.Sides;
            }
            if (additional.HasValue) {
                sum += bag.RollMany(additional.Value);
            }
            return sum + Modifiers;
        }

        public int Roll(int withModifier)
        {
            return Roll() + withModifier;
        }
    }
}
