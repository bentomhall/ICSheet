using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// The 6 basic abilities of D&amp;D
    /// </summary>
    public enum AbilityType
    {
        /// <summary></summary>
        Strength,
        /// <summary></summary>
        Dexterity,
        /// <summary></summary>
        Constitution,
        /// <summary></summary>
        Intelligence,
        /// <summary></summary>
        Wisdom,
        /// <summary></summary>
        Charisma,
        /// <summary>Not used.</summary>
        None
    };

    /// <summary>
    /// Represents an ability with its numerical values.
    /// </summary>
    [DataContract]
    public class Ability
    {
        [DataMember] private int value;

        /// <summary>
        /// Returns the ability modifier: (score - 10)/2 rounded down
        /// </summary>
        public int Modifier
        {
            get { return (value - 10)/2; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="score"></param>
        public Ability(int score)
        {
            value = score;
        }

        /// <summary>
        /// Returns the raw score. Used almost entirely for display.
        /// </summary>
        public int Score
        {
            get { return value; }
        }
    }
}
