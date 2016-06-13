// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ICSheetIOS.Views
{
	[Register ("NewCharacterController")]
	partial class NewCharacterController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView ClassPicker { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CreateButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField NameField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView RacePicker { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ClassPicker != null) {
				ClassPicker.Dispose ();
				ClassPicker = null;
			}
			if (CreateButton != null) {
				CreateButton.Dispose ();
				CreateButton = null;
			}
			if (NameField != null) {
				NameField.Dispose ();
				NameField = null;
			}
			if (RacePicker != null) {
				RacePicker.Dispose ();
				RacePicker = null;
			}
		}
	}
}
