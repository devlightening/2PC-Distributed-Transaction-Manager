using Coordinator.Enum;

namespace Coordinator.Models
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 1.Aşamanın Durumunu İfade Eder..
        /// </summary>
        public ReadyType IsReady { get; set; }

        /// <summary>
        /// 2.Aşamanın Neticesinde İşlemin Başarıyla Tamamlanıp Tamamlanmadığını İfade Eder..
        /// </summary>
        public TransactionStateType TransactionState { get; set; }


        public Node Node { get; set; }


    }
}
