using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Prometheus;
using Serilog;
using Shipra.Backend.API.Application;
using Shipra.Backend.API.Application.Middlewares;
using Shipra.Backend.API.Core;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure;
using Shipra.Backend.API.Web;
using Shipra.Backend.API.Web.Models;
using Stripe;
using static Shipra.Backend.API.Application.Features.ExampleFeatures.Commands.ExampleCommand;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

string? masterDbconnectionString = builder.Configuration.GetConnectionString("ShipraMasterConnection");  //Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext(builder.Configuration, masterDbconnectionString!);

builder.Services.AddHttpContextAccessor();
#region stripe
var stripeSecretKey = builder.Configuration.GetSection("StripeSettings:StripeAPIKey").Get<StripeAPIKey>();
StripeConfiguration.ApiKey = stripeSecretKey!.Secretkey;
#endregion

builder.Services.InstallServices(builder.Configuration);
builder.Services.InstallApplicationServices(builder.Configuration);

builder.Services.AddControllersWithViews()
  .AddNewtonsoftJson();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ExampleCommandValidator>(); 
#region new changes cognito
// 1) Load base + environment-specific appsettings
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 2) Read AWS settings from config
var awsSection = builder.Configuration.GetSection("AWS");
var awsRegionName = awsSection["Region"];

var cognitoSection = awsSection.GetSection("Cognito");
var accessKey = cognitoSection["AccessKey"];
var secretKey = cognitoSection["SecretKey"];

var regionEndpoint = RegionEndpoint.GetBySystemName(awsRegionName);

// 3) Register Cognito client with those credentials
builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
{
  var credentials = new BasicAWSCredentials(accessKey, secretKey);

  var config = new AmazonCognitoIdentityProviderConfig
  {
    RegionEndpoint = regionEndpoint
  };

  return new AmazonCognitoIdentityProviderClient(credentials, config);
});
#endregion
#region MyRegion
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<SmtpSettingsForException>(builder.Configuration.GetSection("SmtpSettingsForException"));
#endregion
builder.Services.AddRazorPages();
//builder.Services.AddFastEndpoints();
//builder.Services.AddFastEndpointsApiExplorer();
#region swagger
if (builder.Environment.EnvironmentName == "Development")
{
  builder.Services.AddSwaggerGen(config =>
  {
    config.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebAPI", Version = "v1" });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
          {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
          });
  });
}
#endregion
// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure
builder.Services.AddCors();

 
// Ensure HttpClient is registered in ConfigureServices
builder.Services.AddHttpClient();
builder.Services.AddScopedJwtAuthentication(builder.Configuration); 
//#endregion


var app = builder.Build();
app.UseCors(builder =>
{
  builder
  .AllowAnyOrigin()
  .AllowAnyMethod()
  .AllowAnyHeader();
}); 
 

//app.UseMiddleware<CustomUserSuspendedExceptionHandlerMiddleware>(); // Register before other middlewares
app.UseMiddleware<SuspendedClientMiddleware>(); // Register before other middlewares

//app.UseMiddleware<JwtAuthenticationMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseSerilogRequestLogging();
app.UseRouting();
// For Prometheus add below UseRouting()
app.UseHttpMetrics();
//app.UseFastEndpoints();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

#region MyRegion
if (builder.Environment.EnvironmentName == "Development")
{
  // Enable middleware to serve generated Swagger as a JSON endpoint.
  app.UseSwagger();
  // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

}
#endregion
app.MapDefaultControllerRoute();
app.MapRazorPages();

// For Prometheus add below MapControllers()
app.MapMetrics();

// Seed Database
//using (var scope = app.Services.CreateScope())
//{
//  var services = scope.ServiceProvider;

//  try
//  {
//    var context = services.GetRequiredService<AppDbContext>();
//    //                    context.Database.Migrate();
//    context.Database.EnsureCreated();
//    SeedData.Initialize(services);
//  }
//  catch (Exception ex)
//  {
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
//  }
//}

app.Run();
