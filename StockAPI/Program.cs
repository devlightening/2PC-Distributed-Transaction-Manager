var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Stock Service is Readyyyyy *_- ");
    return true;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock Service is Committed");
    return true;
});


app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock Service is Rollbacked");
});


app.Run();
