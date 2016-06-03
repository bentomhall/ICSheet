using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class ShapechangeViewModel : BaseViewModel
    {
        private CharacterViewModel _baseModel;

        public ShapechangeViewModel(CharacterViewModel baseFormModel, string newFormName)
        {
            _baseModel = baseFormModel;

        }
    }
}
