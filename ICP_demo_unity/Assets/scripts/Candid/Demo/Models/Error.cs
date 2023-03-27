using EdjCase.ICP.Candid.Mapping;

namespace Candid.Demo.Models
{
	public class Error
	{
		[CandidName("msg")]
		public string Msg { get; set; }

		public Error(string msg)
		{
			this.Msg = msg;
		}

		public Error()
		{
		}
	}
}