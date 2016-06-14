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

namespace ICSheetIOS
{
	[Register ("OverviewViewController")]
	partial class OverviewViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AbilityScoreButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton DefenseButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton HealthButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AbilityScoreButton != null) {
				AbilityScoreButton.Dispose ();
				AbilityScoreButton = null;
			}
			if (DefenseButton != null) {
				DefenseButton.Dispose ();
				DefenseButton = null;
			}
			if (HealthButton != null) {
				HealthButton.Dispose ();
				HealthButton = null;
			}
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}
		}
	}
}
