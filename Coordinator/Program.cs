using Coordinator.Models.Contexts;
using Coordinator.Services.Abstracts;
using Coordinator.Services.Concretes;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddHttpClient("OrderAPI", client => client.BaseAddress = new("https://localhost:7287/"));
builder.Services.AddHttpClient("StockAPI", client => client.BaseAddress = new("https://localhost:7263/"));
builder.Services.AddHttpClient("PaymentAPI", client => client.BaseAddress = new("https://localhost:7014/"));

builder.Services.AddTransient<ITransactionService, Coordinator.Services.Concretes.TransactionManager>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/created-order-transaction", async (ITransactionService transactionService) =>
{
    // Phase 1: Prepare

    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyTransactionAsync(transactionId);

    if (transactionState)
    {
        // Phase 2: Commit
        await transactionService.CommitAsync(transactionId);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
        return Results.Ok("Transaction committed successfully.");
    }
    else
    {
        // Phase 3: Rollback
        await transactionService.RollbackAsync(transactionId);
        return Results.BadRequest("Transaction failed, rolled back.");
    }
});
app.Run();
