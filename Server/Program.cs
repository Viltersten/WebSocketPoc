using Server.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddScoped<IService, Service>();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
//app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();
app.Run();
