using Fluxo.Saldos.Service.Infra;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.UseSaldos(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseSaldosEndpoints();

app.Run();