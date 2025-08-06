var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Payment Service is Readyyyyy *_- ");
    return true;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Payment Service is Committed");
    return true;
});


app.MapGet("/rollback", () =>
{
    Console.WriteLine("Payment Service is Rollbacked");
});


app.Run();
