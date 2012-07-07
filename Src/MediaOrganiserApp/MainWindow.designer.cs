// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MediaOrganiserApp
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField InputPathsTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField OutputPathTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton ExcludeiTunesMediaCheckbox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton AddToiTunesCheckbox { get; set; }

		[Action ("InputBrowseButtonPressed:")]
		partial void InputBrowseButtonPressed (MonoMac.Foundation.NSObject sender);

		[Action ("OutputBrowseButtonPressed:")]
		partial void OutputBrowseButtonPressed (MonoMac.Foundation.NSObject sender);

		[Action ("OrganiseButtonPressed:")]
		partial void OrganiseButtonPressed (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (InputPathsTextField != null) {
				InputPathsTextField.Dispose ();
				InputPathsTextField = null;
			}

			if (OutputPathTextField != null) {
				OutputPathTextField.Dispose ();
				OutputPathTextField = null;
			}

			if (ExcludeiTunesMediaCheckbox != null) {
				ExcludeiTunesMediaCheckbox.Dispose ();
				ExcludeiTunesMediaCheckbox = null;
			}

			if (AddToiTunesCheckbox != null) {
				AddToiTunesCheckbox.Dispose ();
				AddToiTunesCheckbox = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
