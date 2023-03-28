using EdjCase.ICP.Candid.Mapping;

namespace Candid.Demo.Models
{
	public class MoveMsg
	{
		[CandidName("dir")]
		public int Dir { get; set; }

		public MoveMsg(int dir)
		{
			this.Dir = dir;
		}

		public MoveMsg()
		{
		}
	}
}