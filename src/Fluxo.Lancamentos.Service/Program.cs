using Fluxo.Lancamentos.Service.Infra;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.UseLancamentos(builder.Configuration);

var app = builder.Build();

await app.AplicarMigrationsAsync();

await app.SeedCompetenciaAsync();

app.MapDefaultEndpoints();

app.MapOpenApi();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseLancamentosEndpoints();

await app.RunAsync();