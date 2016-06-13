
using System;
using System.Collections.Generic;
using System.Drawing;

using Foundation;
using UIKit;
using ICSheetCore;
using System.Linq;

namespace ICSheetIOS
{
    public partial class CharacterTableViewController : UITableViewController
    {
        private class CharacterTableSource : UITableViewSource
        {
            private List<string> _characterNames;
            private Action<string> _onAccessoryTapped;
            public CharacterTableSource(List<string> characterNames, Action<string> createCharacterAction)
            {
                _characterNames = characterNames;
                _onAccessoryTapped = createCharacterAction;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return _characterNames.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell("CharacterCell");
                var name = _characterNames[indexPath.Row];

                if (cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, "CharacterCell");
                }

                cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;

                cell.TextLabel.Text = name;
                return cell;
            }

            public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
            {
                var characterName = _characterNames[indexPath.Row];
                tableView.DeselectRow(indexPath, true);
                _onAccessoryTapped(characterName);
            }

            public void Add(UITableView tableView, string item)
            {
                _characterNames.Add(item);
                tableView.ReloadData();
            }
        }

        private CharacterTableSource _dataSource;

        public CharacterTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var names = Utility.FileManager.GetAllCharacterNames();
            _dataSource = new CharacterTableSource(names.ToList(), LoadExistingCharacter);
            TableView.Source = _dataSource;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "CreateCharacterSegue")
            {
                var destination = segue.DestinationViewController as Views.NewCharacterController;
                destination.delegateAction = ReceiveNewCharacterInformation;
            }
        }

        public void ReceiveNewCharacterInformation(PlayerCharacter c)
        {
            _dataSource.Add(TableView, c.Name);
            var tabBar = TabBarController as MainTabBarViewController;
            tabBar.SetCharacter(c);
        }

        public void LoadExistingCharacter(string name)
        {
            var tabBar = TabBarController as MainTabBarViewController;
            var data = Utility.FileManager.ReadCharacterWith(name);
            tabBar.CreateCharacterFrom(data);
        }
    }
}