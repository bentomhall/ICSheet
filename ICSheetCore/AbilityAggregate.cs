using System;
using System.Collections.Generic;
namespace ICSheetCore
{
    internal class AbilityAggregate : IAbilityDataSource
    {
        private Dictionary<AbilityType, Ability> _abilities =  new Dictionary<AbilityType, Ability>();

        internal AbilityAggregate()
        {
            _abilities[AbilityType.Strength] = new Ability(10);
            _abilities[AbilityType.Dexterity] = new Ability(10);
            _abilities[AbilityType.Constitution] = new Ability(10);
            _abilities[AbilityType.Intelligence] = new Ability(10);
            _abilities[AbilityType.Wisdom] = new Ability(10);
            _abilities[AbilityType.Charisma] = new Ability(10);
        }

        int IAbilityDataSource.AbilityModifierFor(AbilityType ability)
        {
            return _abilities[ability].Modifier;
        }

        int IAbilityDataSource.AbilityScoreFor(AbilityType ability)
        {
            return _abilities[ability].Score;
        }

        internal void Modify(AbilityType ability, int newScore)
        {
            var oldAbilityModifier = _abilities[ability].Modifier;
            _abilities[ability] = new Ability(newScore);
            if (oldAbilityModifier != _abilities[ability].Modifier)
            {
                var args = new AbilityModifiedEventArgs();
                args.Modifier = (newScore - 10) / 2;
                args.ModifiedAbility = ability;
                OnAbilityModified(args);
            }
        }

        internal void OnAbilityModified(AbilityModifiedEventArgs e)
        {
            AbilityModified?.Invoke(this, e);
        }

        internal event EventHandler<AbilityModifiedEventArgs> AbilityModified;
    }

    internal class AbilityModifiedEventArgs : EventArgs
    {
        internal int Modifier { get; set; }
        internal AbilityType ModifiedAbility { get; set; }
    }
}