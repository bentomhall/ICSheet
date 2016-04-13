using System;
using System.Collections.Generic;
namespace ICSheetCore
{
    internal class AbilityAggregate
    {
        private Dictionary<AbilityType, Ability> _abilities;

        public int AbilityModifierFor(AbilityType ability)
        {
            return _abilities[ability].Modifier;
        }

        public int AbilityScoreFor(AbilityType ability)
        {
            return _abilities[ability].Score;
        }

        public void Modify(AbilityType ability, int newScore)
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