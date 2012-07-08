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
		MonoMac.AppKit.NSTextField InputDisplayLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField OutputDisplayLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton AddToiTunesCheckbox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton ExcludeiTunesMediaCheckbox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton OrganiseButton { get; set; }

		[Action ("InputBrowseButtonPressed:")]
		partial void InputBrowseButtonPressed (MonoMac.Foundation.NSObject sender);

		[Action ("OutputBrowseButtonPressed:")]
		partial void OutputBrowseButtonPressed (MonoMac.Foundation.NSObject sender);

		[Action ("OrganiseButtonPressed:")]
		partial void OrganiseButtonPressed (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (InputDisplayLabel != null) {
				InputDisplayLabel.Dispose ();
				InputDisplayLabel = null;
			}

			if (OutputDisplayLabel != null) {
				OutputDisplayLabel.Dispose ();
				OutputDisplayLabel = null;
			}

			if (AddToiTunesCheckbox != null) {
				AddToiTunesCheckbox.Dispose ();
				AddToiTunesCheckbox = null;
			}

			if (ExcludeiTunesMediaCheckbox != null) {
				ExcludeiTunesMediaCheckbox.Dispose ();
				ExcludeiTunesMediaCheckbox = null;
			}

			if (OrganiseButton != null) {
				OrganiseButton.Dispose ();
				OrganiseButton = null;
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
