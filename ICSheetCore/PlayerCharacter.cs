using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    public class PlayerCharacter
    {
        private string _name;
        private string _alignment;
        private int _experience;
        private string _notes;
        private string _background;

        private AbilityAggregate _abilityAggregate;
        private DefenseAggregate _defenseAggregate;
        private Race _race;
        private PlayerClassAggregate _classAggregate;
        private SkillAggregate _skillAggregate;
        //private Inventory _inventory; 
    }

}
