using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Services;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Implementation.Factory;
using Shipra.Backend.API.Application.Services.Implementation.Shopify;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Application.Services.Interfaces.Shopify;
using ShopifySharp;
using ShopifySharp.Extensions.DependencyInjection;

namespace Shipra.Backend.API.Application;
public static class ServiceCollectionExtensions
{
  /// <summary>
  ///     Adds Security Authentication.
  /// </summary>


  public static void InstallApplicationServices(
      this IServiceCollection services, IConfiguration configuration
     )
  {
    //services.AddTransient<ICurrentTenantService, CurrentTenantService>();
    services.AddTransient<IExceptionHelper, ExceptionHelper>();
    services.AddTransient<IEmailHandler, EmailHandler>();
    services.AddTransient<IEmailServiceProvider, EmailServiceProvider>();
    services.AddTransient<IBarcodeGenerate, BarcodeGenerate>();
    services.AddTransient<IApplicationUrls, ApplicationUrls>(); 
    services.AddTransient<ISecrets, Secrets>();
    services.AddTransient<IKeyGeneratorService, KeyGeneratorService>();
    services.AddTransient<IOtpService, OtpService>();
    //services.AddTransient<IKeyGeneratorService, KeyGeneratorService>();
    services.AddShopifySharpServiceFactories();
    // Add ShopifySharp's service factories and the LeakyBucketExecutionPolicy to your DI container
    services.AddShopifySharpRequestExecutionPolicy<LeakyBucketExecutionPolicy>();
    services.AddShopifySharp<LeakyBucketExecutionPolicy>();
    services.AddScoped<DynamicPermissionAppService>(); 
    services.AddScoped<ExceptionHandlerService>();
    services.AddScoped<CarrierServiceFactory>();
  }
}
