
using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading;
using System.Collections.Generic;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MediaOrganiser;


namespace MediaOrganiserApp
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
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
		#endregion

		#region MainWindow
		// Strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}
		#endregion

		#region Handlers
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
			DoOrganise();
		}
		#endregion

		#region CodeBody
		private IEnumerable<IPath> InputPaths;
		private IDirectory OutputDirectory;

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
					IPath Path = new Path(Url.AbsoluteUrl.Path);

					InputPathsString += Path.Name + ";";
					InputPathsList.Add(Path);
				}

				// Assign.
				InputDisplayLabel.StringValue = InputPathsString;
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
				IDirectory Directory = new Directory(Dialog.Url.AbsoluteUrl.Path);

				// Assign.
				OutputDisplayLabel.StringValue = Directory.Name;
				OutputDirectory = Directory;
			}
		}

		public void DoOrganise()
		{
			// Get values.
			Boolean AddToiTunes = AddToiTunesCheckbox.State==NSCellStateValue.On;
			Boolean ExcludeiTunesMedia = ExcludeiTunesMediaCheckbox.State==NSCellStateValue.On;

			// Check values.
			if(InputPaths==null || InputPaths.Count()==0)
			{
				InputDisplayLabel.StringValue = "Please select input";
				return;
			}

			// UI update before work.
			OrganiseButton.Enabled = false;

			Organiser Organiser = new Organiser(InputPaths, OutputDirectory, new List<IPath>(), false, AddToiTunes, ExcludeiTunesMedia);
			Organiser.Organise();

			// Do organisation. Main work in another thread to prevent UI lockup.
			/**
			(new Thread(() =>
			{
				try
				{
					Organiser Organiser = new Organiser(InputPaths, OutputDirectory, new List<IPath>(), false, AddToiTunes, ExcludeiTunesMedia);
					Organiser.Organise();
				}
				finally
				{
					OrganiseButton.Enabled = true;
				}
			})
			{
				Name = "Organiser Thread",
				Priority = ThreadPriority.BelowNormal
			}).Start();
			**/
		}
		#endregion
	}
}

