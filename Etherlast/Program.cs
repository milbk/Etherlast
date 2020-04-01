using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Etherlast
{
    class Program
    {
        static void Main(string[] args)
        {
            var w3 = new Web3("https://cloudflare-eth.com");

            var nowBlockNumber = BigInteger.Zero;
            while (true)
            {
                var lastBlockNumber = w3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.Value;
                if (lastBlockNumber == nowBlockNumber) continue;
                if (nowBlockNumber == 0)
                    nowBlockNumber = w3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.Value - 1;
                for (var i = nowBlockNumber - 1; i <= lastBlockNumber; i++)
                {
                    try
                    {
                        var blockWithTransactions = w3.Eth.Blocks.GetBlockWithTransactionsByNumber
                            .SendRequestAsync(i.ToHexBigInteger()).Result;
                        nowBlockNumber = blockWithTransactions.Number.Value;
                        //Task.Run(()=> { })
                        //Parallel.ForEach(blockWithTransactions.Transactions, itemTransaction => {})
                        foreach (var itemTransaction in blockWithTransactions.Transactions)
                        {
                            Console.WriteLine(blockWithTransactions.Number + " - " + itemTransaction.TransactionIndex +
                                              " / " + blockWithTransactions.Transactions.Length);
                            Console.WriteLine("-----------------------------");
                            Console.WriteLine("TransactionHash: " + itemTransaction.TransactionHash);
                            Console.WriteLine("From: " + itemTransaction.From);
                            Console.WriteLine("To: " + itemTransaction.To);
                            Console.WriteLine("Val: " + Web3.Convert.FromWei(itemTransaction.Value.Value));
                            //Console.WriteLine("InputText " + itemTransaction.Input);
                            Console.WriteLine("-----------------------------");
                            Console.WriteLine("LastBlockNumber: " + lastBlockNumber);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                Thread.Sleep(10000);
            }
        }
    }
}
