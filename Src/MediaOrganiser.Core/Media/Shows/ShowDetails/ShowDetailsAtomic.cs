using System;
using System.IO.Abstractions;
using System.Linq;
using System.Collections.Generic;
using MediaOrganiser.AtomicParsley;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsAtomic : IShowDetailsBasic, IShowDetailsAdditional
	{
		private String _ShowName;
		public String ShowName
		{
			get
			{
				return _ShowName;
			}
		}

		private Int32? _SeasonNumber;
		public Int32? SeasonNumber
		{
			get
			{
				return _SeasonNumber;
			}
		}

		private Int32? _EpisodeNumber;
		public Int32? EpisodeNumber
		{
			get
			{
				return _EpisodeNumber;
			}
		}

		private String _EpisodeName;
		public String EpisodeName
		{
			get
			{
				return _EpisodeName;
			}
		}

		private DateTime? _AiredDate;
		public DateTime? AiredDate
		{
			get
			{
				return _AiredDate;
			}
		}

		private String _Overview;
		public String Overview
		{
			get
			{
				return _Overview;
			}
		}

		private String _TVNetwork;
		public String TVNetwork
		{
			get
			{
				return _TVNetwork;
			}
		}

		private IEnumerable<FileInfoBase> _Artworks;
		public IEnumerable<FileInfoBase> Artworks
		{
			get
			{
				return _Artworks;
			}
		}

		private Boolean _HasDetails = false;
		public Boolean HasDetails
		{
			get
			{
				return _HasDetails;
			}
		}

		public Boolean ExtractDetails(FileInfoBase ShowFile)
		{
			// Run AtomicParsley to get details.
			Dictionary<String, String> Details = AtomicParsley.AtomicParsley.ExtractDetails(ShowFile.FullName);

			// Extract the details out of the output.
			if(!Details.ContainsKey("tvsh") || !Details.ContainsKey("tves"))
			{
				return false;
			}

			_ShowName = Details["tvsh"];
			_SeasonNumber = null;
			_EpisodeNumber = Convert.ToInt32(Details["tves"]);
			_EpisodeName = null;
			_AiredDate = null;
			_Overview = null;
			_TVNetwork = null;
			_Artworks = null;

			if(Details.ContainsKey("tvsn"))
			{
				_SeasonNumber = Convert.ToInt32(Details["tvsn"]);
			}
			if(Details.ContainsKey("@nam"))
			{
				_EpisodeName = Details["@nam"];
			}
			if(Details.ContainsKey("@day"))
			{
				_AiredDate = DateTime.Parse(Details["@day"]);
			}
			if(Details.ContainsKey("@desc"))
			{
				_Overview = Details["desc"];
			}
			if(Details.ContainsKey("tvnn"))
			{
				_TVNetwork = Details["tvnn"];
			}

			_HasDetails = true;
			return true;
		}
	}
}

