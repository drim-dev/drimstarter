using Drimstarter.ServiceDefaults;

const string postgresVersion = "16.3";

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag(postgresVersion)
    .WithPgAdmin();

var projectServiceDb = postgres.AddDatabase(ResourceNames.ProjectServiceDb);

var projectServiceSetup = builder.AddProject<Projects.Drimstarter_ProjectService_Setup>(ResourceNames.ProjectServiceSetup)
    .WithReference(projectServiceDb);

var projectService = builder.AddProject<Projects.Drimstarter_ProjectService>(ResourceNames.ProjectService)
    .WithReference(projectServiceDb)
    .WaitForCompletion(projectServiceSetup);

var accountServiceDb = postgres.AddDatabase(ResourceNames.AccountServiceDb);

var accountServiceSetup = builder.AddProject<Projects.Drimstarter_AccountService_Setup>(ResourceNames.AccountServiceSetup)
    .WithReference(accountServiceDb);

var accountService = builder.AddProject<Projects.Drimstarter_AccountService>(ResourceNames.AccountService)
    .WithReference(accountServiceDb)
    .WaitForCompletion(accountServiceSetup);

var paymentService = builder.AddProject<Projects.Drimstarter_PaymentService>("payment-service");

var notificationServiceDb = postgres.AddDatabase("notification-service-db");

var notificationService = builder.AddProject<Projects.Drimstarter_NotificationService>("notification-service")
    .WithReference(notificationServiceDb);

var messagingService = builder.AddProject<Projects.Drimstarter_MessagingService>("messaging-service");

var blockchainServiceDb = postgres.AddDatabase(ResourceNames.BlockchainServiceDb);

var blockchainServiceSetup = builder.AddProject<Projects.Drimstarter_BlockchainService_Setup>(ResourceNames.BlockchainServiceSetup)
    .WithReference(blockchainServiceDb);

var blockchainService = builder.AddProject<Projects.Drimstarter_BlockchainService>(ResourceNames.BlockchainService)
    .WithReference(blockchainServiceDb)
    .WaitForCompletion(blockchainServiceSetup);

var apiGateway = builder.AddProject<Projects.Drimstarter_ApiGateway>("api-gateway")
    .WithReference(projectService)
    .WithReference(accountService)
    .WithReference(paymentService)
    .WithReference(blockchainService)
    .WithReference(notificationService);

builder.Build().Run();
