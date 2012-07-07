using System;
using System.Linq;
using System.Files;
using System.Collections.Generic;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MediaOrganiser;

namespace MediaOrganiserApp
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private IEnumerable<IPath> InputPaths;
		private IDirectory OutputDirectory;

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}

		// Strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}

		#region handlers

		partial void InputBrowseButtonPressed (NSObject sender)
		{
			OpenInputFileBrowser();
		}

		partial void OutputBrowseButtonPressed (NSObject sender)
		{
			OpenOutputFileBrowser();
		}

		partial void OrganiseButtonPressed (NSObject sender)
		{
			Organise();
		}

		#endregion handlers

		private void OpenInputFileBrowser()
		{
			NSOpenPanel Dialog = new NSOpenPanel();

			Dialog.CanChooseFiles = true;
			Dialog.CanChooseDirectories = true;
			Dialog.AllowsMultipleSelection = true;


			if(Dialog.RunModal()!=0)
			{
				// Temps
				String InputPathsString = "";
				List<IPath> InputPathsList = new List<IPath>();

				foreach(NSUrl Url in Dialog.Urls)
				{
					InputPathsString += Url.AbsoluteUrl.Path + ";";
					InputPathsList.Add(new Path(Url.AbsoluteUrl.Path));
				}

				// Assign.
				InputPathsTextField.StringValue = InputPathsString;
				InputPaths = InputPathsList;
			}
		}

		private void OpenOutputFileBrowser()
		{
			NSOpenPanel Dialog = new NSOpenPanel();

			Dialog.CanChooseFiles = false;
			Dialog.CanChooseDirectories = true;
			Dialog.AllowsMultipleSelection = false;


			if(Dialog.RunModal()!=0)
			{
				// Assign.
				OutputPathTextField.StringValue = Dialog.Url.AbsoluteUrl.Path;
				OutputDirectory = new Directory(Dialog.Url.AbsoluteUrl.Path);
			}
		}

		// Do organisation.
		public void Organise()
		{
			Boolean AddToiTunes = AddToiTunesCheckbox.State==NSCellStateValue.On;
			Boolean ExcludeiTunesMedia = ExcludeiTunesMediaCheckbox.State==NSCellStateValue.On;

			Organiser Organiser = new Organiser(InputPaths, OutputDirectory, new List<IPath>(), false, AddToiTunes, ExcludeiTunesMedia);
			Organiser.Organise();
		}
	}
}

