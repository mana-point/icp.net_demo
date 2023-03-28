using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;
using EdjCase.ICP.Candid.Models;

namespace Candid.Demo.Models
{
	public class MovePlayerResult
	{
		[CandidName("ok")]
		public OptionalValue<MovePlayer> Ok { get; set; }

		[CandidName("error")]
		public OptionalValue<Error> Error { get; set; }

		public MovePlayerResult(OptionalValue<MovePlayer> ok, OptionalValue<Error> error)
		{
			this.Ok = ok;
			this.Error = error;
		}

		public MovePlayerResult()
		{
		}
	}
}