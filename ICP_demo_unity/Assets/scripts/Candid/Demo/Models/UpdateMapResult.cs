using EdjCase.ICP.Candid.Mapping;
using Candid.Demo.Models;
using EdjCase.ICP.Candid.Models;

namespace Candid.Demo.Models
{
	public class UpdateMapResult
	{
		[CandidName("ok")]
		public OptionalValue<Map> Ok { get; set; }

		[CandidName("error")]
		public OptionalValue<Error> Error { get; set; }

		public UpdateMapResult(OptionalValue<Map> ok, OptionalValue<Error> error)
		{
			this.Ok = ok;
			this.Error = error;
		}

		public UpdateMapResult()
		{
		}
	}
}