﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class SpellViewModel : BaseViewModel
    {
        private bool isSpellKnown = false;
        private Spell _spell;

        public bool SpellKnown
        {
            get { return isSpellKnown; }
            set
            {
                isSpellKnown = value;
                NotifyPropertyChanged();
            }
        }

        public SpellViewModel(Spell spell)
        {
            _spell = spell;
        }

        public Spell Spell { get { return _spell; } }

        public string Name { get { return _spell.Name; } }
        public bool IsPrepared { get { return _spell.IsPrepared; } }
        public string Level { get { return $"Level {_spell.Level}"; } }
        public string Text { get { return _spell.Description; } }
        public string School { get { return _spell.School; } }
        public string Components { get { return _spell.Components; } }
        public string CastTime { get { return _spell.CastTime; } }
        public string Duration { get { return _spell.Duration; } }
        public string Range { get { return _spell.Range; } }

        public void PrepareSpell()
        {
            //_spell.IsPrepared = (!_spell.IsPrepared);
            NotifyPropertyChanged("IsPrepared");
        }

        
        
    }
}
