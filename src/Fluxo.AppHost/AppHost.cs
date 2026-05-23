using DotNetEnv;

Env.Load();

var builder = DistributedApplication.CreateBuilder(args);

var postgresUser = builder.AddParameter(
    "postgres-user",
    Environment.GetEnvironmentVariable("POSTGRES_USER")!);

var postgresPassword = builder.AddParameter(
    "postgres-password",
    Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!,
    secret: true);

var postgresDb =
    Environment.GetEnvironmentVariable("POSTGRES_DB")!;

var postgresPort = int.Parse(
    Environment.GetEnvironmentVariable("POSTGRES_PORT")!);

var postgres = builder
    .AddPostgres(
        "postgres",
        postgresUser,
        postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(
        postgresPort,
        5432,
        name: "tcp");

var db = postgres.AddDatabase(postgresDb);

builder
    .AddProject<Projects.Fluxo_Lancamentos_Service>("fluxo-lancamentos-service")
    .WithReference(db);

builder.Build().Run();