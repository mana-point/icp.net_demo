using System;
using UnityEngine;
using System.Threading.Tasks;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using UnityEditor;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Agent.Models;
using System.Collections.Generic;
using System.Linq;
using Candid.Demo;
using LitJson;
using Candid.Demo.Models;
using EdjCase.ICP.Agent;

namespace Candid
{
	public class BackendConnector
	{
		public static bool IsConnected = false;

		// ICP.net 
		public static DelegationIdentity identity;
		public static DemoApiClient mainClient;
		public static IAgent agent;

		public static ulong anchorId = 1;

		public static async Task createIdentityByJsonAndConnect(string mainCanisterId, string json, bool useLocalHost = false)
		{
			Debug.Log("Connect via ICPNET");

			try
			{
				identity = await SetIdentityAsync(json);

				if (useLocalHost)
					agent = new HttpAgent(identity, new Uri("http://localhost:4943"));
				else
					agent = new HttpAgent(identity);

				// connect to main
				var mainId = Principal.FromText(mainCanisterId);
				mainClient = new DemoApiClient(agent, mainId);

				IsConnected = true;

				Debug.Log("Connected");
			}
			catch (Exception e) 
			{
				Debug.Log(e.Message);
			}
		}

		public static async Task<DelegationIdentity> SetIdentityAsync(string json)
		{
			DelegationIdentity Identity = null;
			IIIdentity info = JsonMapper.ToObject<IIIdentity>(json);

			if (info != null)
			{
				byte[] privateKey = FromHexString(info.identity[1]);
				var identity = Ed25519Identity.FromPrivateKey(privateKey);
				DelegationChain chain = info.Delegation.ToCommon();
				Identity = new DelegationIdentity(identity, chain);
			}

			return Identity;
		}

		public static byte[] FromHexString(string hexString)
		{
			var bytes = new byte[hexString.Length / 2];
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}
			return bytes;
		}

		[Serializable]
		public class IIIdentity
		{
			[JsonAlias("_inner")]
			public string[] identity;

			[JsonAlias("_delegation")]
			public DelegationChainModel Delegation;
		}

		[Serializable]
		public class DelegationChainModel
		{
			[JsonAlias("delegations")]
			public List<SignedDelegationModel> Delegations { get; set; } = new List<SignedDelegationModel>();
			[JsonAlias("publicKey")]
			public string PublicKey { get; set; }

			public DelegationChain ToCommon()
			{
				List<SignedDelegation> delegations = this.Delegations
					.Select(d => d.ToCommon())
					.ToList();
				byte[] publicKeyBytes = FromHexString(this.PublicKey);
				SubjectPublicKeyInfo publicKey = SubjectPublicKeyInfo.FromDerEncoding(publicKeyBytes);
				return new DelegationChain(publicKey, delegations);
			}
		}

		[Serializable]
		public class SignedDelegationModel
		{
			[JsonAlias("delegation")]
			public DelegationModel Delegation { get; set; }

			[JsonAlias("signature")]
			public string Signature { get; set; }

			public SignedDelegation ToCommon()
			{
				Delegation delegation = this.Delegation.ToCommon();
				byte[] signature = FromHexString(this.Signature);
				return new SignedDelegation(delegation, signature);
			}
		}

		[Serializable]
		public class DelegationModel
		{
			[JsonAlias("expiration")]
			public string Expiration { get; set; }

			[JsonAlias("pubkey")]
			public string PubKey { get; set; }

			public Delegation ToCommon()
			{
				byte[] publicKeyBytes = FromHexString(this.PubKey);
				SubjectPublicKeyInfo publicKey = SubjectPublicKeyInfo.FromDerEncoding(publicKeyBytes);
				ulong nanosecondsFromNow = (ulong)ToBigInteger(FromHexString(this.Expiration), isUnsigned: true, isBigEndian: true);
				ICTimestamp expiration = ICTimestamp.FromNanoSeconds(nanosecondsFromNow);
				return new Delegation(publicKey, expiration, targets: null);
			}

			public static System.Numerics.BigInteger ToBigInteger(byte[] bytes, bool isUnsigned, bool isBigEndian)
			{
				// BigInteger takes a twos compliment little endian value
				if (isUnsigned || isBigEndian)
				{
					BinarySequence bits = BinarySequence.FromBytes(bytes, isBigEndian);
					if (isUnsigned && bits.MostSignificantBit)
					{
						// Convert unsigned to signed
						bits = bits.ToTwosCompliment();
					}
					bytes = bits.ToByteArray(bigEndian: false);
				}
				return new System.Numerics.BigInteger(bytes);
			}

			internal class BinarySequence
			{
				// Least signifcant bit (index 0) => Most signifcant bit (index n - 1)
				private readonly bool[] bits;

				public bool MostSignificantBit => this.bits[this.bits.Length - 1];

				/// <param name="bits">Least signifcant to most ordered bits</param>
				public BinarySequence(bool[] bits)
				{
					this.bits = bits;
				}

				public BinarySequence ToTwosCompliment()
				{
					// If value most significant bit is `1`, the 2's compliment needs to be 1 bit larger to hold sign bit
					if (this.MostSignificantBit)
					{
						bool[] newBits = new bool[this.bits.Length + 1];
						this.bits.CopyTo(newBits, 0);
						return new BinarySequence(newBits);
					}
					bool[] bits = this.ToTwosComplimentInternal().ToArray();
					return new BinarySequence(bits);
				}

				public BinarySequence ToReverseTwosCompliment()
				{
					if (this.bits.Last())
					{
						throw new InvalidOperationException("Cannot reverse two's compliment on a negative number");
					}
					bool[] bits = this.ToTwosComplimentInternal().ToArray();
					return new BinarySequence(bits);
				}

				public byte[] ToByteArray(bool bigEndian = false)
				{
					IEnumerable<byte> bytes = this.bits
						.Chunk(8)
						.Select(BitsToByte);
					// Reverse if need big endian
					if (bigEndian)
					{
						bytes = bytes.Reverse();
					}

					return bytes.ToArray();

					byte BitsToByte(bool[] bits)
					{
						if (bits.Length > 8)
						{
							throw new ArgumentException("Bit length must be less than or equal to 8");
						}
						// Bits are in least significant first order
						int value = 0;
						for (int i = 0; i < bits.Length; i++)
						{
							bool b = bits[i];
							if (b)
							{
								value |= 1 << i;
							}
						}
						return (byte)value;
					}
				}

				public override string ToString()
				{
					var stringBuilder = new System.Text.StringBuilder();
					foreach (bool bit in this.bits.Reverse()) // Reverse to show LSB on right (normal display)
					{
						stringBuilder.Append(bit ? "1" : "0");
					}
					return stringBuilder.ToString();
				}

				public static BinarySequence FromBytes(byte[] bytes, bool isBigEndian)
				{
					IEnumerable<byte> byteSeq = bytes;
					if (isBigEndian)
					{
						byteSeq = byteSeq.Reverse();
					}
					bool[] bits = byteSeq
						.SelectMany(ByteToBits)
						.ToArray();
					return new BinarySequence(bits);

					IEnumerable<bool> ByteToBits(byte b)
					{
						// Least significant first
						yield return (b & 0b00000001) == 0b00000001;
						yield return (b & 0b00000010) == 0b00000010;
						yield return (b & 0b00000100) == 0b00000100;
						yield return (b & 0b00001000) == 0b00001000;
						yield return (b & 0b00010000) == 0b00010000;
						yield return (b & 0b00100000) == 0b00100000;
						yield return (b & 0b01000000) == 0b01000000;
						yield return (b & 0b10000000) == 0b10000000;
					}
				}

				private IEnumerable<bool> ToTwosComplimentInternal()
				{
					// Invert all numbers left of the right-most `1` to get 2's compliment

					bool flipBits = false;
					foreach (bool bit in this.bits.Reverse())
					{
						yield return flipBits ? !bit : bit;
						// If bit is `1`, all bits to left are flipped
						if (bit)
						{
							flipBits = true;
						}
					}
				}
			}
		}
	}
}
