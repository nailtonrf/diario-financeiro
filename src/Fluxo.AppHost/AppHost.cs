using DotNetEnv;

Env.Load();

var builder = DistributedApplication.CreateBuilder(args);

var posgres = ConfigurePostgres(builder);

var mongo = ConfigureMongoDb(builder);

var rabbitMq = ConfigureRabbitMq(builder);

builder
    .AddProject<Projects.Fluxo_Lancamentos_Service>("fluxo-lancamentos-service")
    .WithReference(posgres)
    .WithReference(rabbitMq);

builder
    .AddProject<Projects.Fluxo_Saldos_Service>("fluxo-saldos-service")
    .WithReference(posgres)
    .WithReference(mongo)
    .WithReference(rabbitMq);

builder.Build().Run();

return;

IResourceBuilder<PostgresDatabaseResource> ConfigurePostgres(IDistributedApplicationBuilder app)
{
    var postgresUser = app.AddParameter(
        "postgres-user",
        Environment.GetEnvironmentVariable("POSTGRES_USER")!);

    var postgresPassword = app.AddParameter(
        "postgres-password",
        Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!,
        secret: true);

    var postgresDb =
        Environment.GetEnvironmentVariable("POSTGRES_DB")!;

    var postgresPort = int.Parse(
        Environment.GetEnvironmentVariable("POSTGRES_PORT")!);

    var postgres = app
        .AddPostgres(
            "postgres",
            postgresUser,
            postgresPassword)
        .WithContainerName("postgres")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithEndpoint(
            postgresPort,
            5432,
            name: "tcp");

    var resourceBuilder = postgres.AddDatabase(postgresDb);

    return resourceBuilder;
}

IResourceBuilder<MongoDBDatabaseResource> ConfigureMongoDb(IDistributedApplicationBuilder app)
{
    var mongoUser = app.AddParameter(
        "mongo-user",
        Environment.GetEnvironmentVariable("MONGODB_USER")!);

    var mongoPassword = app.AddParameter(
        "mongo-password",
        Environment.GetEnvironmentVariable("MONGODB_PASSWORD")!,
        secret: true);

    var mongoPort = int.Parse(
        Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017");

    var mongoDb = app
        .AddMongoDB(
            "mongo",
            mongoPort,
            mongoUser,
            mongoPassword)
        .WithContainerName("mongo")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithEndpoint(
            mongoPort,
            27017,
            name: "tcp");

    var resourceBuilder = mongoDb.AddDatabase("saldodb");

    return resourceBuilder;
}

IResourceBuilder<RabbitMQServerResource> ConfigureRabbitMq(IDistributedApplicationBuilder app)
{
    var rabbitUser = builder.AddParameter(
        "rabbitmq-user",
        Environment.GetEnvironmentVariable("RABBITMQ_USER")!);

    var rabbitPassword = builder.AddParameter(
        "rabbitmq-password",
        Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!,
        secret: true);

    var rabbitPort = int.Parse(
        Environment.GetEnvironmentVariable("RABBITMQ_PORT")!);

    var rabbitManagementPort = int.Parse(
        Environment.GetEnvironmentVariable("RABBITMQ_MANAGEMENT_PORT")!);

    return builder
        .AddRabbitMQ(
            "rabbitmq",
            rabbitUser,
            rabbitPassword)
        .WithContainerName("rabbitmq")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithEndpoint(
            rabbitPort,
            5672,
            name: "tcp")
        .WithManagementPlugin();
}