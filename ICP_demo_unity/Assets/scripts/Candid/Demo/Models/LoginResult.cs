using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;
using EdjCase.ICP.Candid.Models;

namespace Candid.Demo.Models
{
	public class LoginResult
	{
		[CandidName("ok")]
		public OptionalValue<Login> Ok { get; set; }

		[CandidName("error")]
		public OptionalValue<Error> Error { get; set; }

		public LoginResult(OptionalValue<Login> ok, OptionalValue<Error> error)
		{
			this.Ok = ok;
			this.Error = error;
		}

		public LoginResult()
		{
		}
	}
}