using EdjCase.ICP.Candid.Mapping;

namespace Candid.Demo.Models
{
	public class Position
	{
		[CandidName("x")]
		public int X { get; set; }

		[CandidName("y")]
		public int Y { get; set; }

		public Position(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public Position()
		{
		}
	}
}