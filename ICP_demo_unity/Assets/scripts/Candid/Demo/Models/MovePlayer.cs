using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;

namespace Candid.Demo.Models
{
	public class MovePlayer
	{
		[CandidName("map")]
		public Map Map { get; set; }

		[CandidName("player")]
		public Player Player { get; set; }

		[CandidName("time")]
		public ulong Time { get; set; }

		public MovePlayer(Map map, Player player, ulong time)
		{
			this.Map = map;
			this.Player = player;
			this.Time = time;
		}

		public MovePlayer()
		{
		}
	}
}