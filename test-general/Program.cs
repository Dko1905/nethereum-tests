using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Signer;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.JsonRpc.Client;

namespace test_general
{
	class Program
	{
		/* BNB chain id. */
		static private readonly BigInteger chainId = 97;
		/* RPC uri for use in client. */
		static private readonly Uri testnetRpcUri = new Uri("https://data-seed-prebsc-1-s1.binance.org:8545/");
		/* Special description of bytecode that you can find in remix. */
		static private readonly string danAbi = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"balances\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
		static private readonly string danId = "0x0f93c5261de33f320d50a48ebfc909c28df9da88";
		/* Wallet address of wallet with DAN coins. */
		static private readonly string exampleAddress = "0x5Adc942A25E1551b39977dB8EA9C21cCd9417298";

		static void Main(string[] args)
		{
			// printBalance(exampleAddress).Wait();
			// printBalance(generateAccount().Address).Wait();
			moveBackAndForthDan().Wait();
		}

		/* Print balance of BNB and DAN of address. This function also shows
		 * creation of rpc client of web3, normally you would only create them
		 * once.
		 */
		static async Task printBalance(string address) {
			/* Address with a bit under 2 BNB and 10.000 DAN on testnet. */
			var anonRpcClient = new RpcClient(testnetRpcUri);
			var anonWeb3 = new Web3(anonRpcClient);
			/* Fetch DAN coin contract. The second param is the contract id. */
			var danContract = anonWeb3.Eth.GetContract(danAbi, danId);
			var danBalanceOfFunc = danContract.GetFunction("balanceOf");
			/* Lookup balance using web3. */
			var balanceBnb = Web3.Convert.FromWeiToBigDecimal(await anonWeb3.Eth.GetBalance.SendRequestAsync(address));
			/* Lookup balance using DAN coin contract. */
			var balanceDan = Web3.Convert.FromWeiToBigDecimal(await danBalanceOfFunc.CallAsync<BigInteger>(address));

			Console.WriteLine($"address has {balanceBnb}BNB");
			Console.WriteLine($"address has {balanceDan}DAN");
		}

		static async Task moveBackAndForthDan() {
			IClient client = new RpcClient(testnetRpcUri);
			Account alice = generateAccount();
			Web3 aliceWeb3 = new Web3(alice, client);
			Contract aliceContract = aliceWeb3.Eth.GetContract(danAbi, danId);
			Function aliceBalanceFunc = aliceContract.GetFunction("balanceOf");

			Console.WriteLine($"Please deposit 1 DAN to alice: {alice.Address}");
			Console.WriteLine("Press anything to continue");
			Console.ReadLine();

			while (true) {
				var aliceBalance = aliceBalanceFunc.CallAsync<BigInteger>(alice.Address);

				Console.WriteLine($"Balance of alice is {Web3.Convert.FromWeiToBigDecimal(await aliceBalance)} DAN");

				/* Ask for confirmation. */
				Console.WriteLine("Continue (y/n/a)?");
				string ret = Console.ReadLine();
				if (ret == "n") {
					Console.WriteLine("Cancelling");
					goto cancel;
				} else if (ret == "a") {
					continue;
				}

			}

			cancel:
			Console.Write("");
		}

		static Account generateAccount() {
			/* Generate EC private and public keys. */
			var ecKey = Nethereum.Signer.EthECKey.GenerateKey();

			/* Return new account with the newly generated keys. */
			return new Account(ecKey, chainId);
		}

	}
}
