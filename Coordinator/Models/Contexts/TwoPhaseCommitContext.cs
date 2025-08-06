using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Contexts
{

    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions<TwoPhaseCommitContext> options) : base(options)
        {
        }


        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasData(
                new Node("OrderAPI") { Id = Guid.NewGuid() },
                new Node("StockAPI") { Id = Guid.NewGuid() },
                new Node("PaymentAPI") { Id = Guid.NewGuid() }
            );
        }

    }
}
