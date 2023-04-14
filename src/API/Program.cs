using API.Endpoint;
using API.Extension;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHubAPISwagger(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseHubExceptionHandler(app.Logger);

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DisplayRequestDuration();
});

app.UseCors();
app.UseHttpsRedirection();

// add authentication middle-ware
app.UseAuthentication();
app.UseAuthorization();

app.MapSuperHeroEndpoint();
app.MapAccountEndpoint();

await app.UseInfrastructureServicesAsync(app.Environment);

app.Run();
