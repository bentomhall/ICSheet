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

    public class DiceBag
    {
        private Random generator = new Random();

        public int rollOne(DiceSize sides)
        {
            return generator.Next(1, (int)sides + 1);
        }
    }
}
