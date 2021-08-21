using System;
using System.Numerics;
using System.Diagnostics;

using Nethereum.Web3;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

namespace test_opret_wallet
{
    class Program
    {
        static void Main(string[] args)
        {
            /* BNB testnet id */
            var chainId = 97;
            /* Keep track of time used by generating new key. */
            var sw = new Stopwatch();

            sw.Start();
            var account = generateAccount(chainId);
            sw.Stop();

            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms to generate new account");

            printAccountInfo(account);
        }

        static void printAccountInfo(Account account) {
            Console.WriteLine($"Public key {account.PublicKey}");
            Console.WriteLine($"Private key {account.PrivateKey}");
        }

        static Account generateAccount(BigInteger chainId) {
            /* Generate EC private and public keys. */
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();

            /* Return new account with the newly generated keys. */
            return new Account(ecKey, chainId);
        }
    }
}
