using MediaOrganiser.Media.Movies.Details;
using NUnit.Framework;

namespace MediaOrganiser.Tests
{
	[TestFixture]
	public class MovieDetailsRegexTests
	{
		private MovieDetailsRegex _movieDetailsRegex;

		[SetUp]
		public void Setup()
		{
			_movieDetailsRegex = new MovieDetailsRegex();
		}

		[TestCase("SomeMovieName (2000)")]
		public void HasDetailsTrueTest(string SearchInput)
		{
			_movieDetailsRegex.ExtractDetails(SearchInput);
			Assert.True(_movieDetailsRegex.HasDetails);
		}

		[TestCase("SomeMovieName")]
		public void HasDetailsFalseTest(string SearchInput)
		{
			_movieDetailsRegex.ExtractDetails(SearchInput);
			Assert.False(_movieDetailsRegex.HasDetails);
		}

		[TestCase("SomeMovieName (2000)", "SomeMovieName")]
		public void NameAssignedCorrectlyTest(string SearchInput, string ShowName)
		{
			_movieDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(ShowName, _movieDetailsRegex.Name);
		}

		[TestCase("SomeMovieName (2000)", 2000)]
		public void YearAssignedCorrectlyTest(string SearchInput, int Year)
		{
			_movieDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(Year, _movieDetailsRegex.Year);
		}

		[TestCase("The.Lion.King.1994.704p.x264.BRRip.GokU61", "The Lion King", 1994)]
		[TestCase("The.Hunger.Games.2012.TS2DVD.DD2.0.NL.Subs.avi", "The Hunger Games", 2012)]
		[TestCase("Men in Black 3 2012 TS XViD UNiQUE.avi", "Men in Black 3", 2012)]
		[TestCase("Men in Black III 2012 BRRip XViD-UNiQUE", "Men in Black III", 2012)]
		[TestCase("Men in Black III (2012) DVDRip XviD- MAX", "Men in Black III", 2012)]
		[TestCase("Men in Black III.2012.TS.XVID-WBZ", "Men in Black III", 2012)]
		[TestCase("American Reunion[2012]UNRATED BRRip XviD-ETRG", "American Reunion", 2012)]
		[TestCase("American.Reunion.2012.UNRATED.720p.BluRay.X264-BLOW [PublicHD]", "American Reunion", 2012)]
		[TestCase("American Reunion 2012 PROPER TS V2 Xvid Read NFO UnKnOwN(Repack)", "American Reunion", 2012)]
		[TestCase("Prometheus 2012 Cam Xvid [OSR]", "Prometheus", 2012)]
		[TestCase("[ www.Torrenting.com ] - Prometheus.2012.CAM.RIP.XVID-AT", "Prometheus", 2012)]
		[TestCase("{www.scenetime.com}Prometheus.2012.CAM.XviD-HOPE", "Prometheus", 2012)]
		[TestCase("The.Avengers.2012.TS.XviD.AC3-ADTRG", "The Avengers", 2012)]
		[TestCase("Next.Avengers-Heroes.Of.Tomorrow[2008]DvDrip-aXXo", "Next Avengers Heroes Of Tomorrow", 2008)]
		[TestCase("The.Avengers.2012.CAM.READNFO.XviD-HOPE", "The Avengers", 2012)]
		[TestCase("WRATH OF THE TITANS (2012) DVDRip [MKV 6ch AC3][RoB]", "WRATH OF THE TITANS", 2012)]
		[TestCase("Wrath of the Titans - 2012 DVDRip XViD Ac3-playXD", "Wrath of the Titans", 2012)]
		[TestCase("Wrath of the Titans 2012 - DvDrip XviD [OPTiC]", "Wrath of the Titans", 2012)]
		[TestCase("Wrath.of.the.titans.2012.SWESUB.DVDrip-SC666", "Wrath of the titans", 2012)]
		[TestCase("countdown.to.zero.2010.xvid-submerge.avi", "countdown to zero", 2010)]
		[TestCase("DrJn.2010.BRRip_mediafiremoviez.com.mkv", "DrJn", 2010)]
		[TestCase("Nim’s.Island[2008]DvDrip-aXXo.avi", "Nim’s Island", 2008)]
		[TestCase("Adoration 2008 DvdRip ExtraScene RG.avi", "Adoration", 2008)]
		[TestCase("America.2009.STV.DVDRip.XviD-ViSiON.avi", "America", 2009)]
		[TestCase("Antibodies.2005.GERMAN.DVDRip.XviD.AC3.CD1-AFO.avi", "Antibodies", 2005)]
		[TestCase("Balls of Fury[2007]DvDrip[Eng]-FXG.avi", "Balls of Fury", 2007)]
		[TestCase("Bruno (2009) DVDRip-MAXSPEED www.torentz.3xforum.ro.avi", "Bruno", 2009)]
		[TestCase("Defiance [2009] ( 10rating ).avi", "Defiance", 2009)]
		[TestCase("Einstein.And.Eddington.2008.DVDRip.XviD.avi", "Einstein And Eddington", 2008)]
		[TestCase("Transformers Revenge of the Fallen[2009]DvDrip[Eng]-FXG.avi", "Transformers Revenge of the Fallen", 2009)]
		[TestCase("Inception [2010] DvDRiP XviD - ExtraTorrentRG.avi", "Inception", 2010)]
		[TestCase("Salt {2010} DVDRIP. Jaybob.avi", "Salt", 2010)]
		[TestCase("The Sorcerer's Apprentice[2010]DvDrip[Eng]-FXG.avi", "The Sorcerer's Apprentice", 2010)]
		[TestCase("Toy.Story.3.2010.R5.XviD.AC3-NYDIC.avi", "Toy Story 3", 2010)]
		[TestCase("(www.dustorrents.com) Patthar Ke Phool (1991) DVDRip_XviD_MP3 (Dustorrents).avi", "Patthar Ke Phool", 1991)]
		[TestCase("Fanaa.2006.720p.BluRay.nHD.x264-NhaNc3.mkv", "Fanaa", 2006)]
		[TestCase("Force [2011] 720p Upscaled 1.2DVDr x264 06Ch AAC ESubs By NhaNc3.mkv", "Force", 2011)]
		[TestCase("ROWDY RATHORE (2012)- 720p- DVDRip- x264- AC3 5.1 - MSubs- Team Ictv.mkv", "ROWDY RATHORE", 2012)]
		[TestCase("Rascals [2011] 720p Upscaled nHD x264 06Ch AAC Subs By NhaNc3.mkv", "Rascals", 2011)]
		[TestCase("Band Baaja Baaraat  - 2010 - 720p DVDRip X264 Untouch AC3 5.1 Audio ESubs {Team DUS}.mkv", "Band Baaja Baaraat", 2010)]
		[TestCase("Bbuddah Hoga Terra Baap (2011) 720p DVDRip X264 AAC imamzafar[TDBB].mkv", "Bbuddah Hoga Terra Baap", 2011)]
		[TestCase("Dum Maaro Dum (2011) 2CD DVD Rip X264 AC3 imamzafar[TDBB].mkv", "Dum Maaro Dum", 2011)]
		public void ParseTest(string SearchInput, string Name, int Year)
		{
			_movieDetailsRegex.ExtractDetails(SearchInput);
			Assert.AreEqual(Name, _movieDetailsRegex.Name);
			Assert.AreEqual(Year, _movieDetailsRegex.Year);
		}
	}
}
