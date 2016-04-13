using System;
using System.Collections.Generic;
namespace ICSheetCore
{
    internal class AbilityAggregate
    {
        private Dictionary<AbilityType, Ability> _abilities;

        internal AbilityAggregate()
        {
            _abilities[AbilityType.Strength] = new Ability(10);
            _abilities[AbilityType.Dexterity] = new Ability(10);
            _abilities[AbilityType.Constitution] = new Ability(10);
            _abilities[AbilityType.Intelligence] = new Ability(10);
            _abilities[AbilityType.Wisdom] = new Ability(10);
            _abilities[AbilityType.Charisma] = new Ability(10);
        }

        internal int AbilityModifierFor(AbilityType ability)
        {
            return _abilities[ability].Modifier;
        }

        internal int AbilityScoreFor(AbilityType ability)
        {
            return _abilities[ability].Score;
        }

        internal void Modify(AbilityType ability, int newScore)
        {
            _abilities[ability] = new Ability(newScore);
            var args = new AbilityModifiedEventArgs();
            args.Score = newScore;
            args.Modifier = (newScore - 10) / 2;
            OnAbilityModified(args);
        }

        internal void OnAbilityModified(AbilityModifiedEventArgs e)
        {
            EventHandler<AbilityModifiedEventArgs> handler = AbilityModified;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal event EventHandler<AbilityModifiedEventArgs> AbilityModified;
    }

    internal class AbilityModifiedEventArgs : EventArgs
    {
        internal int Score { get; set; }
        internal int Modifier { get; set; }
    }
}