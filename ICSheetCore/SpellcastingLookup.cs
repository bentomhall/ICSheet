using System.Collections.Generic;

namespace ICSheetCore
{
    /// <summary>
    /// Contains static lookups for spellcasting features
    /// </summary>
    public static class SpellcastingLookup
    {
        /// <summary>
        /// Spell slots by level for full casters.
        /// </summary>
        public static Dictionary<int, List<int>> FullCaster = new Dictionary<int, List<int>>()
        {
            { 1, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 3, 3, 2, 1, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 3, 3, 2, 1, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 0, 0 }},
            { 14, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 0 }},
            { 16, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 0 }},
            { 17, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 1 }},
            { 18, new List<int>() { 4, 3, 3, 3, 3, 1, 1, 1, 1 }},
            { 19, new List<int>() { 4, 3, 3, 3, 3, 2, 1, 1, 1 }},
            { 20, new List<int>() { 4, 3, 3, 3, 3, 2, 2, 1, 1 }}
        };

        /// <summary>
        /// Spell slots by level for half casters
        /// </summary>
        public static Dictionary<int, List<int>> HalfCaster = new Dictionary<int, List<int>>()
        {
            { 1, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 14, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 16, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 17, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 18, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 19, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }},
            { 20, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }}
        };

        /// <summary>
        /// spell slots by level for warlocks
        /// </summary>
        public static Dictionary<int, List<int>> Warlock = new Dictionary<int, List<int>>()
        {
            { 1, new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 0, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 0, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 0, 0, 2, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 0, 0, 2, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 0, 0, 0, 2, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 0, 0, 0, 2, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 0, 0, 0, 0, 2, 0, 0, 0, 0 }},
            { 10, new List<int>() { 0, 0, 0, 0, 2, 0, 0, 0, 0 }},
            { 11, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 12, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 13, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 14, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 15, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 16, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 17, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 18, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 19, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 20, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }}
        };

        /// <summary>
        /// spell slots by level for martial casters
        /// </summary>
        public static Dictionary<int, List<int>> Martial = new Dictionary<int, List<int>>()
        {
            { 1, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 14, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 16, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 17, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 18, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 19, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 20, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }}
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnumerable<int> SpellSlotsFor(CastingType type, int level)
        {
            switch (type)
            {
                case CastingType.Full:
                    return FullCaster[level];
                case CastingType.Half:
                    return HalfCaster[level];
                case CastingType.Martial:
                    return Martial[level];
                case CastingType.Warlock:
                    return Warlock[level];
                case CastingType.None:
                    break;
            }
            return new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        }

        /// <summary>
        /// Types of casters
        /// </summary>
        public enum CastingType
        {
            /// <summary>
            /// full progression casters (ie Wizard, multiclass casters)
            /// </summary>
            Full,
            /// <summary>
            /// half-speed progression
            /// </summary>
            Half,
            /// <summary>
            /// 1/3 speed progression (eldritch knight, etc)
            /// </summary>
            Martial,
            /// <summary>
            /// warlock casting progression
            /// </summary>
            Warlock,
            /// <summary>
            /// unused.
            /// </summary>
            None
        }
        
    }
}
