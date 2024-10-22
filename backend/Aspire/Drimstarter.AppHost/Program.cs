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
    .WithReference(projectServiceDb);

var accountServiceDb = postgres.AddDatabase(ResourceNames.AccountServiceDb);

var accountServiceSetup = builder.AddProject<Projects.Drimstarter_AccountService_Setup>(ResourceNames.AccountServiceSetup)
    .WithReference(accountServiceDb);

var accountService = builder.AddProject<Projects.Drimstarter_AccountService>(ResourceNames.AccountService)
    .WithReference(accountServiceDb);

var paymentService = builder.AddProject<Projects.Drimstarter_PaymentService>("payment-service");

var notificationServiceDb = postgres.AddDatabase("notification-service-db");

var notificationService = builder.AddProject<Projects.Drimstarter_NotificationService>("notification-service")
    .WithReference(notificationServiceDb);

var messagingService = builder.AddProject<Projects.Drimstarter_MessagingService>("messaging-service");

var apiGateway = builder.AddProject<Projects.Drimstarter_ApiGateway>("api-gateway")
    .WithReference(projectService)
    .WithReference(accountService)
    .WithReference(paymentService)
    .WithReference(notificationService);

builder.Build().Run();
