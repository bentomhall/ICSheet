using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Interactive_Character_Sheet_Core
{
    public interface IDamage
    {
        int Amount { get; set; }
        DamageType type { get; }
        bool isResistedBy(ResistanceType type);
        bool triggersVulnerability(ResistanceType type);
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

        public DamageType type { get { return DamageType.Untyped; } }

        public bool isResistedBy(ResistanceType type)
        {
            return false;
        }

        public bool triggersVulnerability(ResistanceType type)
        {
            return false;
        }
    }

}
