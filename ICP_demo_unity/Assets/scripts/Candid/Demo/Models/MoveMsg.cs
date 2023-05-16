using EdjCase.ICP.Candid.Mapping;

namespace Candid.Demo.Models
{
	public class moveMsg
	{
		[CandidName("dir")]
		public int Dir { get; set; }

		public moveMsg(int dir)
		{
			this.Dir = dir;
		}

		public moveMsg()
		{
		}
	}
}