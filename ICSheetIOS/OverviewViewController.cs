using System;

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
        }
    }
}

