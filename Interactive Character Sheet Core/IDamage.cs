using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace InteractiveCharacterSheetCore
{
    public interface IDamage
    {
        int Amount { get; set; }
        DamageType Type { get; }
        bool IsResistedBy(ResistanceType type);
        bool TriggersVulnerability(ResistanceType type);
    }

    public enum DamageType
    {
        Untyped,
        Fire,
        Frost,
        Lightning,
        Thunder,
        Acid,
        Poison,
        Psychic,
        Radiant,
        Necrotic,
        Slashing,
        Piercing,
        Bludgeoning
    }

    public class DamageBase : IDamage
    {
        public int Amount
        {
            get;
            set;
        }

        public DamageType Type { get { return DamageType.Untyped; } }

        public bool IsResistedBy(ResistanceType type)
        {
            return false;
        }

        public bool TriggersVulnerability(ResistanceType type)
        {
            return false;
        }
    }

}
