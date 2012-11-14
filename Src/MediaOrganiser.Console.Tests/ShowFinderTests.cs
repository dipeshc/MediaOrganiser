using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;
using MediaOrganiser.Console.Finders;

namespace MediaOrganiser.Console.Tests.Finders
{
	[TestFixture]
	public class ShowFinderTests
	{
		private IFileSystem _fileSystem;

		[TestFixtureSetUp]
		public void Setup()
		{
			var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
			fileSystem.AddFile("/Input/ShowName1 - S01E01.mp4", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E02.avi", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E03.mkv", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E04.m4v", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E05.MP4", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E06.AVI", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E07.MKV", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E08.M4V", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E09.mP4", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E10.aVi", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E11.mKV", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E12.m4V", null);

			fileSystem.AddFile("/Input/ShowName1 - S01E13.other", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E14.OTHER", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E15.OtHeR", null);
			fileSystem.AddFile("/Input/ShowName1 - S01E16.OtHeR$", null);

			fileSystem.AddFile("/Input/Subfolder1/ShowName2 - S01E01.mp4", null);
			fileSystem.AddFile("/Input/Subfolder1/ShowName2 - S01E02.avi", null);
			fileSystem.AddFile("/Input/Subfolder1/ShowName2 - S01E03.mkv", null);
			fileSystem.AddFile("/Input/Subfolder1/ShowName2 - S01E04.m4v", null);

			fileSystem.AddFile("/Input/ShowName3 - S01E01.mp4", null);
			fileSystem.AddFile("/Input/ShowName3 - S01E02.avi", null);
			fileSystem.AddFile("/Input/ShowName3 - S01E03.mkv", null);
			fileSystem.AddFile("/Input/ShowName3 - S01E04.m4v", null);

			fileSystem.AddFile("/Exclude/ShowName3 - S01E01.mp4", null);
			fileSystem.AddFile("/Exclude/ShowName3 - S01E02.mp4", null);
			fileSystem.AddFile("/Exclude/ShowName3 - S01E03.mp4", null);
			fileSystem.AddFile("/Exclude/ShowName3 - S01E04.mp4", null);

			_fileSystem = fileSystem;
		}

		[Test]
		public void IntialiseTest()
		{
			var inputPaths = new List<string> {"/Input/"};
			var excludePaths = new List<string> {"/Exclude/"};
			var finder = new ShowFinder(_fileSystem, inputPaths, excludePaths);

			Assert.AreSame(inputPaths, finder.InputPaths);
			Assert.AreSame(excludePaths, finder.ExcludedPaths);
			Assert.AreSame(_fileSystem, finder.FileSystem);
		}

		[Test]
		public void ScanTest()
		{
			var inputPaths = new List<string> {"/Input/"};
			var ouputPaths = new List<string> {"/Exclude/"};
			var finder = new ShowFinder(_fileSystem, inputPaths, ouputPaths);
			finder.Scan();
		}
	}
}

