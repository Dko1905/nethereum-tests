using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;

namespace danCoinExchange
{
	class Program
	{
		/* Testnet chainId */
		static private readonly string danAbi = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"balances\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
		static private readonly string danContractAddress = "0x0f93c5261de33f320d50a48ebfc909c28df9da88";
		/**/
		static private readonly Account alice = new Account("0xc8194491228e1db78df028be7d0eefef453719c545e00587fd74b90983d72e2e", chainId: 97);
		static private readonly Account bob = new Account(Environment.GetEnvironmentVariable("PRVKEY")!, chainId: 97);

		static void Main(string[] args)
		{
			transfer().Wait();
		}

		static async Task transfer() {
			var chainId = new HexBigInteger(97);
			var rpcUri = new Uri("https://data-seed-prebsc-1-s1.binance.org:8545/");
			var rpcClient = new RpcClient(rpcUri);

			/* EXAMPLE: Send money to other address. */
			/* Create web3 client. */
			var web3 = new Web3(alice, rpcClient);
			/* Print useful infomation. */
			var aliceBalance = web3.Eth.GetBalance.SendRequestAsync(alice.Address);
			var bobBalance = web3.Eth.GetBalance.SendRequestAsync(bob.Address);
			Console.WriteLine($"Alice: {alice.Address}");
			Console.WriteLine($"Alice: {Web3.Convert.FromWei(await aliceBalance)}BNB");
			Console.WriteLine($"Bob: {bob.Address}");
			Console.WriteLine($"Bob: {Web3.Convert.FromWei(await bobBalance)}BNB");
			/* Fetch contract using ID and ABI. */
			var danCoin = web3.Eth.GetContract(danAbi, danContractAddress);
			/* Get transfer function handle from bytecode. */
			var transferFunc = danCoin.GetFunction("transfer");
			/* Estimate gas (second argument must be 0). */
			var gas = transferFunc.EstimateGasAsync(new Object[]{bob.Address, 0});
			Console.WriteLine($"Gas: {await gas}");
			Console.ReadLine();
			/* Transfer the money. */
			Console.WriteLine("sending");
			var res = await transferFunc.CallAsync<bool>(new Object[]{bob.Address, 0});
			Console.WriteLine(res.ToString());
			Console.WriteLine("sent");

		}
	}
}
