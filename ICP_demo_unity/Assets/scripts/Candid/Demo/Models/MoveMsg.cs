using EdjCase.ICP.Candid.Mapping;

namespace Candid.Demo.Models
{
	public class MoveMsg
	{
		[CandidName("dir")]
		public sbyte Dir { get; set; }

		public MoveMsg(sbyte dir)
		{
			this.Dir = dir;
		}

		public MoveMsg()
		{
		}
	}
}