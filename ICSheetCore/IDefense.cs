using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// Represents one of the defenses (AC and saving throws) of a character.
    /// </summary>
    [DataContract]
    public class Defense
    {
        /// <summary>
        /// Legacy constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public Defense(int value, DefenseType type)
        {
            _baseValue = value;
            _type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="proficiency"></param>
        /// <param name="adjustment"></param>
        /// <param name="type"></param>
        public Defense(int baseValue, int proficiency, int adjustment, DefenseType type)
        {
            _baseValue = baseValue;
            _proficiency = proficiency;
            _type = type;
            _defenseAdjustment = adjustment;
        }

        [DataMember]
        private DefenseType _type;
        [DataMember]
        private int _baseValue = 10;
        [DataMember]
        private int _proficiency = 0;
        [DataMember]
        private int _defenseAdjustment = 0;

        /// <summary>
        /// The total value of the defense including all variables.
        /// </summary>
        public int Value { get { return _baseValue + _proficiency + _defenseAdjustment; } }
        /// <summary>
        ///
        /// </summary>
        public DefenseType TypeOfDefense { get { return _type; } }
        /// <summary></summary>
        public int BaseValue { get { return _baseValue; } }
        /// <summary></summary>
        public int Proficiency { get { return _proficiency; } }
        /// <summary></summary>
        public int Adjustment { get { return _defenseAdjustment; } }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DefenseType
    {
        /// <summary>AC</summary>
        Armor,
        /// <summary>STR Saves</summary>
        Strength,
        /// <summary>DEX Saves</summary>
        Dexterity,
        /// <summary>CON Saves</summary>
        Constitution,
        /// <summary>INT Saves</summary>
        Intelligence,
        /// <summary>WIS Saves</summary>
        Wisdom,
        /// <summary>CHA Saves</summary>
        Charisma
    }
}
