using System;
using System.Collections.Generic;
using System.Text;

namespace ICSheetIOS.Interfaces
{
    interface IModelDataSource
    {
        event EventHandler ModelChanged;
        Models.OverviewModel OverviewViewModel { get; set; }
        Models.InformationModel InformationViewModel {get;set;}
        Models.InventoryModel InventoryViewModel {get;set;}
        Models.SpellsModel SpellsViewModel {get;set;}
    }

    public enum ModelType
    {
        Overview,
        Information,
        Inventory,
        Spells,
        All
    }

    public class ModelChangedEventArgs : EventArgs
    {
        public ModelType ModelChangedType { get; private set; }
        public ModelChangedEventArgs(ModelType type)
        {
            ModelChangedType = type;
        }
    }

}
