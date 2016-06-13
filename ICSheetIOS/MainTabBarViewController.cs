using System;
using System.Drawing;
using CoreFoundation;
using UIKit;
using Foundation;
using ICSheetCore;
using ICSheetIOS.Models;
using ICSheetCore.Data;

namespace ICSheetIOS
{

    [Register("MainTabBarViewController")]
    public class MainTabBarViewController : UITabBarController, Interfaces.IModelDataSource
    {
        public OverviewModel OverviewViewModel { get; set; }
        public InformationModel InformationViewModel { get; set; }
        public SpellsModel SpellsViewModel { get; set; }

        public InventoryModel InventoryViewModel
        {
            get; set;
        }

        public MainTabBarViewController(IntPtr handle) : base(handle)
        {
        }

        public event EventHandler ModelChanged;

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _setTabBarItemStatus(false);
            // Perform any additional setup after loading the view
        }

        public void SetCharacter(PlayerCharacter c)
        {
            OverviewViewModel = new OverviewModel(c);
            InformationViewModel = new InformationModel(c);
            InventoryViewModel = new InventoryModel(c);
            SpellsViewModel = new SpellsModel(c);
            OnModelChanged(new Interfaces.ModelChangedEventArgs(Interfaces.ModelType.All));
            _setTabBarItemStatus(true);
            //TabBar.SelectedItem = TabBar.Items[1];
        }

        private void OnModelChanged(Interfaces.ModelChangedEventArgs e)
        {
            ModelChanged?.Invoke(this, e);
        }

        private void _setTabBarItemStatus(bool enabled)
        {
            var n = TabBar.Items.Length;
            for (var i = 1; i < n; i++)
            {
                TabBar.Items[i].Enabled = enabled;
            }
        }

        private Utility.CharacterCreationManager characterManager = new Utility.CharacterCreationManager();

        internal void CreateCharacterFrom(CharacterData characterData)
        {
            var character = characterManager.CreateCharacter(characterData);
            SetCharacter(character);
        }
    }
}