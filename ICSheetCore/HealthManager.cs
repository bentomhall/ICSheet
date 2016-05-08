using System;

namespace ICSheetCore
{
    internal class HealthManager
    {
        internal int CurrentHealth { get; private set; }
        internal int MaxHealth { get; set; }
        internal int TemporaryHP { get; private set; }

        internal void TakeDamage(int amount)
        {
            if (TemporaryHP > Math.Abs(amount))
            {
                TemporaryHP -= Math.Abs(amount);
            }
            else
            {
                CurrentHealth -= (Math.Abs(amount) - TemporaryHP);
                TemporaryHP = 0;
            }
            if (CurrentHealth < 0) { CurrentHealth = 0; }
        }

        internal void HealDamage(int amount)
        {
            CurrentHealth = Math.Min(CurrentHealth + Math.Abs(amount), MaxHealth);
        }

        internal void AddTHP(int amount)
        {
            TemporaryHP = Math.Max(Math.Abs(amount), TemporaryHP);
        }
    }
}