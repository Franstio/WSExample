using WSExample.Server.WSHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IWSHandler, MockOutputWebSocketHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("p1",
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});
var app = builder.Build();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
};
webSocketOptions.AllowedOrigins.Add("*");
app.UseWebSockets(webSocketOptions);
app.Map("/ws", async (HttpContext ctx, IWSHandler IWSHandler) =>
{

    if (!ctx.WebSockets.IsWebSocketRequest)
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

    using (var webSocket = await ctx.WebSockets.AcceptWebSocketAsync())
    {
        await IWSHandler.AddWSClient(webSocket);

    }
});
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("p1");
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
