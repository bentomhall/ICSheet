using System.Runtime.Serialization;


namespace ICSheetCore
{
    /// <summary>
    /// A magical spell that a player can cast.
    /// </summary>
    [DataContract]
    public class Spell
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Range { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Components { get; set; }

        /// <summary>
        /// Minimum spell slot level needed.
        /// </summary>
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// Only really matters for wizards.
        /// </summary>
        [DataMember]
        public string School { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CastTime { get; set; }

        /// <summary>
        /// Does the character have the spell prepared?
        /// </summary>
        [DataMember]
        public bool IsPrepared { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Duration { get; set; }

        /// <summary>
        /// The spellbook (class type) to which the spell belongs. Different spellbooks may have the same spell in them. This property distinguishes the spells.
        /// </summary>
        [DataMember]
        public string InSpellbook { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsBonusSpell { get; set; }

        /// <summary>
        /// Reports the preparation status:
        /// Prepared = checkmark
        /// Bonus spell (domain, circle, etc) = D
        /// Unprepared = --
        /// </summary>
        public string PreparationStatus
        {
            get
            {
                if (IsBonusSpell) { return "D"; }
                if (IsPrepared) { return "\u2714"; }
                else { return "--"; }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullDescription
        {
            get
            {
                return $"Level: {Level}\r\nCast Time: {CastTime}\r\nRange: {Range}\r\nComponents: {Components}\r\n{Description}";
            }
        }

        /// <summary>
        /// Creates a copy that shares all the same details except for the associate spellbook (InSpellbook) and the preparation status flags (IsBonusSpell and IsPrepared)
        /// </summary>
        /// <returns></returns>
        public Spell DeepCopyInvariants()
        {
            var s = (Spell)MemberwiseClone();
            s.InSpellbook = "";
            s.IsBonusSpell = false;
            s.IsPrepared = false;
            return s;
        }
    }
}
