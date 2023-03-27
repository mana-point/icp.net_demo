using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;

namespace Candid.Demo.Models
{
	public class Player
	{
		[CandidName("username")]
		public string Username { get; set; }

		[CandidName("owner")]
		public string Owner { get; set; }

		[CandidName("position")]
		public Position Position { get; set; }

		[CandidName("lastTime")]
		public ulong LastTime { get; set; }

		public Player(string username, string owner, Position position, ulong lastTime)
		{
			this.Username = username;
			this.Owner = owner;
			this.Position = position;
			this.LastTime = lastTime;
		}

		public Player()
		{
		}
	}
}