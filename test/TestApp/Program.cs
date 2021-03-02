using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.BalanceHistory.Client;
using Service.BalanceHistory.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new BalanceHistoryClientFactory("http://localhost:80");
            var client = factory.GetWalletBalanceUpdateService();

            var resp = await client.GetBalanceUpdatesAsync(new GetBalanceUpdateRequest() { WalletId = "test--default", Take = 20 });
            foreach (var update in resp.BalanceUpdates)
            {
                Console.WriteLine($"{update.EventType}: {update.Symbol} {update.AmountBalance} {update.AmountReserve} | {update.Id} | {update.SequenceId}");
            }

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
