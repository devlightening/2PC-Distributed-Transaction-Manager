var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Order Service is Readyyyyy *_- ");
    return false;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Order Service is Committed");
    return true;
});


app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order Service is Rollbacked");
});


app.Run();
