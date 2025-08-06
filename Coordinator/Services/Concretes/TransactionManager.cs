using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;

namespace Coordinator.Services.Concretes
{
    public class TransactionManager(IHttpClientFactory _httpClientFactory, TwoPhaseCommitContext _context) : ITransactionService
    {

        //Klasik Yöntem Injecktion için...

        //TwoPhaseCommitContext _context;

        //public TransactionManager(TwoPhaseCommitContext context)
        //{
        //    _context = context;
        //}

        HttpClient _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");
        HttpClient _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");
        HttpClient _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");


        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();

            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enum.ReadyType.Pending,
                    TransactionState = Enum.TransactionStateType.Pending
                }
            });

            await _context.SaveChangesAsync();
            return transactionId;
        }


        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("ready"),
                        "StockAPI" => _stockHttpClient.GetAsync("ready"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("ready")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enum.ReadyType.Ready : Enum.ReadyType.NotReady;
                }
                catch (Exception ex)
                {
                    transactionNode.IsReady = Enum.ReadyType.NotReady;
                }
            }
            // Tüm değişiklikleri döngü dışında tek bir seferde kaydet
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckReadyTransactionAsync(Guid transactionId) => (await _context.NodeStates
                  .Where(ns => ns.TransactionId == transactionId)
                  .ToListAsync()).TrueForAll(ns => ns.IsReady == Enum.ReadyType.Ready);

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("commit"),
                        "StockAPI" => _stockHttpClient.GetAsync("commit"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("commit")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? Enum.TransactionStateType.Done : Enum.TransactionStateType.Aborted;
                }
                catch
                {
                    transactionNode.TransactionState = Enum.TransactionStateType.Aborted;
                }
            }
            // Tüm değişiklikleri döngü dışında tek bir seferde kaydet
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId) => (await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns => ns.TransactionState == Enum.TransactionStateType.Done);

        public async Task RollbackAsync(Guid transactionId)
        {
            // Öncelikle, bu işlemle ilişkili tüm node'ları getir.
            // Bu, hem başarılı hem de başarısız olan node'ları içerir.
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                // Yalnızca başarılı bir şekilde hazırlık aşamasını tamamlamış veya
                // commit olmuş olan servisler için rollback talimatı gönderilir.
                if (transactionNode.IsReady == Enum.ReadyType.Ready || transactionNode.TransactionState == Enum.TransactionStateType.Done)
                {
                    try
                    {
                        var response = await (transactionNode.Node.Name switch
                        {
                            "OrderAPI" => _orderHttpClient.GetAsync("rollback"),
                            "StockAPI" => _stockHttpClient.GetAsync("rollback"),
                            "PaymentAPI" => _paymentHttpClient.GetAsync("rollback")
                        });

                        var result = bool.Parse(await response.Content.ReadAsStringAsync());

                        // Rollback başarılı olursa durumu Aborted olarak güncelle.
                        // Başarısız olursa yine de Aborted olarak işaretleyebiliriz,
                        // çünkü işlem zaten iptal ediliyor.
                        transactionNode.TransactionState = result ? Enum.TransactionStateType.Aborted : Enum.TransactionStateType.Done;
                    }
                    catch
                    {
                        // Bir hata oluşsa bile, işlem zaten iptal edildiği için durumu Aborted olarak işaretle.
                        // Bu, son durumu doğru yansıtır.
                        transactionNode.TransactionState = Enum.TransactionStateType.Aborted;
                    }
                }
                else
                {
                    // Zaten başarısız olan veya hazırlık aşamasına bile geçemeyen node'lara
                    // rollback talimatı göndermeye gerek yoktur. Sadece durumunu Aborted olarak işaretle.
                    transactionNode.TransactionState = Enum.TransactionStateType.Aborted;
                }
            }

            // Tüm değişiklikleri döngü bittikten sonra tek seferde veritabanına kaydet.
            await _context.SaveChangesAsync();
        }
    }
}
