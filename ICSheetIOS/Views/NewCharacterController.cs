using System;
using ICSheetCore.Data;
using ICSheetCore;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace ICSheetIOS.Views
{
    public partial class NewCharacterController : UIViewController, IUIPickerViewDataSource, IUIPickerViewDelegate
    {
        private class PickerDataSource : UIPickerViewDataSource
        {
            public PickerDataSource(List<string> contents)
            {
                Contents = contents;
            }

            public List<string> Contents { get; set; }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return Contents.Count;
            }
        }

        private class PickerDelegate : UIPickerViewDelegate
        {
            private List<string> _contents;
            public PickerDelegate(List<string> contents)
            {
                _contents = contents;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return _contents[(int)row];
            }
        }


        private Utility.CharacterCreationManager _factory = new Utility.CharacterCreationManager();
        private List<string> classNames;
        private List<string> races = new List<string>();

        public Action<PlayerCharacter> delegateAction { get; set; }

        public NewCharacterController(IntPtr handle): base(handle)
        {

        }


        public NewCharacterController() : base("NewCharacterController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var _featureFactory = new XMLFeatureFactory(Utility.FileManager.GetResourceString(Utility.ResourceType.RaceFeatures), Utility.FileManager.GetResourceString(Utility.ResourceType.ClassFeatures));
            classNames = _featureFactory.ExtractClassNames().ToList();
            var baseRaces = _featureFactory.ExtractRaceNames();
            foreach (var r in baseRaces)
            {
                if (r == "Human") { races.Add(r); } //This one you can have without the subrace (variant human). It's wierd, because humans are wierd.
                var subraces = _featureFactory.ExtractSubraceNames(r);
                if (subraces.Count() > 0) { races.AddRange(subraces); }
                else { races.Add(r); }
            }
            ClassPicker.DataSource = new PickerDataSource(classNames);
            ClassPicker.Delegate = new PickerDelegate(classNames);
            ClassPicker.ReloadAllComponents();

            RacePicker.DataSource = new PickerDataSource(races);
            RacePicker.Delegate = new PickerDelegate(races);
            RacePicker.ReloadAllComponents();

            // Perform any additional setup after loading the view, typically from a nib.

            CreateButton.TouchUpInside += CharacterCreationExecuted;
        }

        public void CharacterCreationExecuted(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameField.Text)) { return; }
            var race = races[(int)RacePicker.SelectedRowInComponent(0)];
            var raceDetails = parseRace(race);
            var c = _factory.CreateNewCharacter(NameField.Text, raceDetails.Item1, raceDetails.Item2, classNames[(int)ClassPicker.SelectedRowInComponent(0)]);
            Utility.FileManager.SaveCharacterWith(c.Name, c.ToCharacterData());
            delegateAction?.Invoke(c);
            NavigationController.PopToRootViewController(true);
            
        }

        private Tuple<string, string> parseRace(string race)
        {
            var raceData = race.Split(' ');
            if (raceData.GetLength(0) > 1)
            {
                return new Tuple<string, string>(raceData[1], race);
            }
            else
            {
                return new Tuple<string, string>(race, null);
            }
        }

        public nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (pickerView == RacePicker) { return races.Count; }
            else { return classNames.Count; }
        }

        public string GetTitle(UIPickerView pickerView, int row, int component)
        {
            if (pickerView == RacePicker) { return races[row]; }
            else { return classNames[row]; }
        }
    }
}