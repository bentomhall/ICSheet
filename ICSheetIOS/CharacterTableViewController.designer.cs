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
	[Register ("CharacterTableViewController")]
	partial class CharacterTableViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem CreateCharacterButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CreateCharacterButton != null) {
				CreateCharacterButton.Dispose ();
				CreateCharacterButton = null;
			}
		}
	}
}
