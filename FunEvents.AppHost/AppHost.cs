var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var database = sqlServer.AddDatabase("funevents");

builder.AddProject<Projects.FunEvents_API>("funevents-api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();