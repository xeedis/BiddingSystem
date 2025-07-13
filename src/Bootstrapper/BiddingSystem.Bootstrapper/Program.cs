var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => Results.Text("BiddingSystem API!"));

await app.RunAsync();
