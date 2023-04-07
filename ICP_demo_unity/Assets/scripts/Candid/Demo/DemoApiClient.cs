using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid;
using Candid.Demo;
using EdjCase.ICP.Agent.Responses;
using System.Threading;
using System.Diagnostics;

namespace Candid.Demo
{
	public class DemoApiClient
	{
		public IAgent Agent { get; }

		public Principal CanisterId { get; }

		public EdjCase.ICP.Candid.CandidConverter? Converter { get; }

		public DemoApiClient(IAgent agent, Principal canisterId, CandidConverter? converter = default)
		{
			this.Agent = agent;
			this.CanisterId = canisterId;
			this.Converter = converter;
		}

		public async System.Threading.Tasks.Task<Models.LoginResult> Login()
		{
			UnityEngine.Debug.Log("Login");
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAndWaitAsync(this.CanisterId, "login", arg);
			return reply.ToObjects<Models.LoginResult>(this.Converter);
		}

		public async System.Threading.Tasks.Task<Models.MovePlayerResult> MovePlayer(Models.MoveMsg arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0));
			CandidArg reply = await this.Agent.CallAndWaitAsync(this.CanisterId, "movePlayer", arg);
			return reply.ToObjects<Models.MovePlayerResult>(this.Converter);
		}

		public async System.Threading.Tasks.Task<Models.UpdateMapResult> UpdateWorld()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "updateWorld", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.UpdateMapResult>(this.Converter);
		}
	}
}