using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class HealthChangeViewModel: BaseViewModel
    {
        public enum HealthChangeType {
            Damage = -1,
            Healing = 1,
            Temporary = 0
        }
        public HealthChangeViewModel() : base()
        {
            
        }

        private HealthChangeType _type;

        public int Amount { get; set; }
        public HealthChangeType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        
        public string ChangeTypeDescription
        {
            get
            {
                switch (_type)
                {
                    case HealthChangeType.Damage:
                        return "Damage Amount:";
                    case HealthChangeType.Healing:
                        return "Healing Amount:";
                    case HealthChangeType.Temporary:
                        return "Amount of THP:";
                    default:
                        return "Amount:";
                }
            }
        }

    }
}
