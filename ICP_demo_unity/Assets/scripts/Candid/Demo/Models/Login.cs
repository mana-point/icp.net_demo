using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;

namespace Candid.Demo.Models
{
	public class Login
	{
		[CandidName("map")]
		public Map Map { get; set; }

		[CandidName("player")]
		public Player Player { get; set; }

		public Login(Map map, Player player)
		{
			this.Map = map;
			this.Player = player;
		}

		public Login()
		{
		}
	}
}