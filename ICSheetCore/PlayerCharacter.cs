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

        private AbilityAggregate _abilityAggregate; //can construct
        private DefenseAggregate _defenseAggregate; //can construct
        private IRace _race; //should be passed in
        private PlayerClassAggregate _classAggregate; //should be passed in
        private SkillAggregate _skillAggregate; //can construct
        //private Inventory _inventory;

        internal PlayerCharacter(string name, IRace race, PlayerClassAggregate classesAndLevels)
        {
            _race = race;
            _name = name;
            _classAggregate = classesAndLevels;
            _abilityAggregate = new AbilityAggregate();
            _defenseAggregate = new DefenseAggregate(_abilityAggregate, _classAggregate.ProficiencyForDefenses);
            _skillAggregate = new SkillAggregate();
        }
    }

}
