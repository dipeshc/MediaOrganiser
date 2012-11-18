using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;
using TagLib.Mpeg4;

namespace MediaOrganiser.Core
{
	public static class TagModifier
	{
		public static bool SetTVShowDetails(string filePath, string showName, int? seasonNumber, int episodeNumber, string episodeName, DateTime? airedDate, string description, string tvNetwork, IEnumerable<string> artworkPaths=null)
		{
			// Create the file and get the tag.
			var file = TagLib.File.Create(filePath);
			var tag = (TagLib.Mpeg4.AppleTag) file.Tag;

			// Set tvshow tag type and name.
			var tvShowData = new AppleDataBox(ByteVector.FromString(showName, StringType.UTF8), (int) AppleDataBox.FlagType.ContainsText);
			tag.SetData("tvsh", new AppleDataBox[] {tvShowData});

			// Set tags.
			tag.Title = episodeName;
			tag.Track = (uint) episodeNumber;
			var episodeNumberData = new AppleDataBox(ByteVector.FromInt(episodeNumber), (int) AppleDataBox.FlagType.ContainsText);
			tag.SetData("TVEpisodeNum", new AppleDataBox[] {episodeNumberData});

			// Set season number.
			if(seasonNumber.HasValue)
			{
				var seasonNumberData = new AppleDataBox(ByteVector.FromInt(seasonNumber.Value), (int) AppleDataBox.FlagType.ContainsText);
				tag.SetData("TVSeasonNum", new AppleDataBox[] {seasonNumberData});
			}

			// If airdate provided.
			if(airedDate.HasValue)
			{
				tag.Year = (uint) airedDate.Value.Year;
			}

			// Set long description.
			var longDescriptionData = new AppleDataBox(ByteVector.FromString(description, StringType.UTF8), (int) AppleDataBox.FlagType.ContainsText);
			tag.SetData("ldes", new AppleDataBox[] {longDescriptionData});

			// Set TVNetwork.
			var tvNetworkData = new AppleDataBox(ByteVector.FromString(tvNetwork, StringType.UTF8), (int) AppleDataBox.FlagType.ContainsText);
			tag.SetData("TVNetwork", new AppleDataBox[] {tvNetworkData});

			// Add artwork if it exists.
			if(artworkPaths!=null)
			{
				file.Tag.Pictures = artworkPaths.Select(artworkPath =>
				{
					return new Picture(artworkPath);
				}).ToArray();
			}

			// Save the file.
			file.Save();

			// Return true.
			return true;
		}
	}
}

