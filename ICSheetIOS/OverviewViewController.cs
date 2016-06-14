using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace ICSheetIOS
{
    public partial class OverviewViewController : UIViewController
    {
        public Models.OverviewModel Model { get; set; }

        public OverviewViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            if (Model != null)
            {
                loadFromModel();
            }
        }

        public void OnModelChanged(object sender, Interfaces.ModelChangedEventArgs e)
        {
            if (e.ModelChangedType == Interfaces.ModelType.All || e.ModelChangedType == Interfaces.ModelType.Overview)
            {
                Model = (TabBarController as MainTabBarViewController).OverviewViewModel;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void loadFromModel()
        {
            NameLabel.Text = Model.Name;
            AbilityScoreButton.SetTitle(formatAbilities(), UIControlState.Normal);
            HealthButton.SetTitle(Model.Health, UIControlState.Normal);
            DefenseButton.SetTitle(formatDefenses(), UIControlState.Normal);
        }

        private string formatDefenses()
        {
            var defenseNames = new List<string> { "AC", "STR", "DEX", "CON", "INT", "WIS", "CHA" };
            return string.Join(" / ", defenseNames.Select(x => Model.DefenseStringFor(x)));
        }

        private string formatAbilities()
        {
            var abilityNames = new List<string> { "STR", "DEX", "CON", "INT", "WIS", "CHA" };
            return string.Join(" / ", abilityNames.Select(x => Model.AbilityStringFor(x)));
        }
    }
}

