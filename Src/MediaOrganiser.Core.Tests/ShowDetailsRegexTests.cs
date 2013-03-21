using MediaOrganiser.Media.Shows.Details;
using NUnit.Framework;

namespace MediaOrganiser.Tests
{
	[TestFixture]
	public class ShowDetailsRegexTests
	{
		[TestCase("Community.S01E01")]
		public void HasDetailsTrueTest(string SearchInput)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.True(ShowDetailsRegex.HasDetails);
		}

		[TestCase("Community")]
		public void HasDetailsFalseTest(string SearchInput)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.False(ShowDetailsRegex.HasDetails);
		}

		[TestCase("Community.S01E01", "Community")]
		public void ShowNameAssignedCorrectlyTest(string SearchInput, string ShowName)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(ShowName, ShowDetailsRegex.ShowName);
		}

		[TestCase("Community.E01", null)]
		[TestCase("Community.S0E01", 0)]
		[TestCase("Community.S00E01", 0)]
		[TestCase("Community.S01E01", 1)]
		[TestCase("Community.S02E01", 2)]
		[TestCase("Community.S11E01", 11)]
		[TestCase("Community.S12E01", 12)]
		[TestCase("Community.S99E01", 99)]
		[TestCase("Community.S100E01", 100)]
		[TestCase("Community.S501E01", 501)]
		[TestCase("Community.S00001E01", 1)]
		[TestCase("Community.S10001E01", 10001)]
		[TestCase("Community.S10001E001", 10001)]
		[TestCase("Community.S10001E12311", 10001)]
		public void SeasonNumberAssignedCorrectlyTest(string SearchInput, int? SeasonNumber)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(SeasonNumber, ShowDetailsRegex.SeasonNumber);
		}

		[TestCase("Community.E01", 1)]
		[TestCase("Community.S0E01", 1)]
		[TestCase("Community.S00E01", 1)]
		[TestCase("Community.S01E01", 1)]
		[TestCase("Community.S02E01", 1)]
		[TestCase("Community.S11E01", 1)]
		[TestCase("Community.S12E01", 1)]
		[TestCase("Community.S99E01", 1)]
		[TestCase("Community.S100E01", 1)]
		[TestCase("Community.S501E01", 1)]
		[TestCase("Community.S00001E01", 1)]
		[TestCase("Community.S10001E01", 1)]
		[TestCase("Community.S10001E001", 1)]
		[TestCase("Community.S10001E12311", 12311)]
		[TestCase("Community.000", 0)]
		[TestCase("Community.001", 1)]
		[TestCase("Community.002", 2)]
		[TestCase("Community.009", 9)]
		[TestCase("Community.109", 109)]
		[TestCase("Community.159", 159)]
		[TestCase("Community.999", 999)]
		[TestCase("Community.123999", 123999)]
		public void EpisodeNumberAssignedCorrectlyTest(string SearchInput, int EpisodeNumber)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(EpisodeNumber, ShowDetailsRegex.EpisodeNumber);
		}

		[TestCase("Community.S03E16.HDTV.x264-LOL", "Community", 3, 16)]
		[TestCase("Community.S03E16.HDTV.x264-LOL.mp4", "Community", 3, 16)]
		[TestCase("Community - S03E16 - Virtual Systems Analysis.mp4", "Community", 3, 16)]
		[TestCase("Family.Guy.S10E20.HDTV.x264-LOL.mp4", "Family Guy", 10, 20)]
		[TestCase("How.I.Met.Your.Mother.S07E21.HDTV.x264-LOL.[VTV].mp4", "How I Met Your Mother", 7, 21)]
		[TestCase("Mad Men S01E01 DVDRIP MPEG-4 AVC 856x480 AAC", "Mad Men", 1, 1)]
		[TestCase("[HorribleSubs] Naruto Shippuden - 260 [480p].mp4", "Naruto Shippuden", null, 260)]
		[TestCase("[Narutoverse]_NARUTO_Shippuden_260", "Naruto Shippuden", null, 260)]
		[TestCase("Naruto Shippuuden - E263 - Sai and Shin.mp4", "Naruto Shippuuden", null, 263)]
		[TestCase("The.Big.Bang.Theory.S05E21.HDTV.x264-LOL.mp4", "The Big Bang Theory", 5, 21)]
		[TestCase("The Walking Dead - S01E06 - TS-19.mp4", "The Walking Dead", 1, 6)]
		[TestCase("The Walking Dead - S01E04 - Vatos.mp4", "The Walking Dead", 1, 4)]
		[TestCase("The Walking Dead - S02E07 - Pretty Much Dead Already.mp4", "The Walking Dead", 2, 7)]
		[TestCase("The Walking Dead - S02E10 - 18 Miles Out.mp4", "The Walking Dead", 2, 10)]
		[TestCase("Top.Gear.18x07.HDTV.x264-FoV.mp4", "Top Gear", 18, 7)]
		[TestCase("Top.Gear.S18E08.Best.Of.Complation.1.HDTV.x264-TLA.mp4", "Top Gear", 18, 8)]
		[TestCase("Ugly.Americans.S02E12.Any.Given.Workday.HDTV.XviD-FQM.avi", "Ugly Americans", 2, 12)]
		[TestCase("Ugly.Americans.S02E13.HDTV.x264-2HD.mp4", "Ugly Americans", 2, 13)]
		[TestCase("Ugly.Americans.S02E14.HDTV.x264-ASAP.mp4", "Ugly Americans", 2, 14)]
		[TestCase("Ugly.Americans.S02E15.HDTV.x264-2HD.mp4", "Ugly Americans", 2, 15)]
		[TestCase("Ugly.Americans.S02E16.HDTV.x264-LOL.mp4", "Ugly Americans", 2, 16)]
		[TestCase("Ugly.Americans.S03E02.HDTV.x264-ASAP.mp4", "Ugly Americans", 3, 2)]
		[TestCase("Community S03E22 HDTV x264-LOL", "Community", 3, 22)]
		[TestCase("Community 3x06 (HDTV-LOL) [VTV]", "Community", 3, 6)]
		public void ParseTest(string SearchInput, string ShowName, int? SeasonNumber, int EpisodeNumber)
		{
			var ShowDetailsRegex = new ShowDetailsRegex();
			ShowDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(ShowName, ShowDetailsRegex.ShowName);
			Assert.AreEqual(SeasonNumber, ShowDetailsRegex.SeasonNumber);
			Assert.AreEqual(EpisodeNumber, ShowDetailsRegex.EpisodeNumber);
		}
	}
}
