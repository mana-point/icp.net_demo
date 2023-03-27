using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;
using System.Collections.Generic;

namespace Candid.Demo.Models
{
	public class Map
	{
		[CandidName("players")]
		public List<Player> Players { get; set; }

		public Map(List<Player> players)
		{
			this.Players = players;
		}

		public Map()
		{
		}
	}
}