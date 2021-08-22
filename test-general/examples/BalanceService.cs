using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Signer;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.JsonRpc.Client;

namespace test_general.examples {
	class BalanceService {
		private readonly BigInteger chainId;
		private readonly Web3 anonWeb3;

		public BalanceService(BigInteger chainId, Uri rpcClientUri) {
			RpcClient rpcClient = new RpcClient(rpcClientUri);
			anonWeb3 = new Web3(rpcClient);
		}

		public async Task<BigInteger> fetchBalance(string address) {
			return await anonWeb3.Eth.GetBalance.SendRequestAsync(address);
		}
	}
}
