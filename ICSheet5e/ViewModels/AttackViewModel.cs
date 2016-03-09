using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class AttackViewModel:BaseViewModel
    {
        int attackBonus = 0;
        int staticDamageBonus = 0;
        string name = "";

        public int StaticBonus { get { return staticDamageBonus; } set { staticDamageBonus = value; NotifyPropertyChanged("Damage"); } }
        public int AttackBonus { get { return attackBonus; } set { attackBonus = value; NotifyPropertyChanged(); } }
        public string Damage { get { return String.Format("{0} + {1}", BaseDamage, staticDamageBonus); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged(); } }
        public string BaseDamage { get; set; }

        static public AttackViewModel DefaultModel(int strength)
        {
            var vm = new AttackViewModel();
            vm.Name = "Unarmed Strike";
            vm.BaseDamage = "1";
            vm.StaticBonus = 0;
            vm.AttackBonus = strength;
            return vm;
        }
    }
}
