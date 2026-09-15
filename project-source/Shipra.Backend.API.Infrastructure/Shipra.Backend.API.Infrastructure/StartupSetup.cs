using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Grpc;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure.Data;
using Shipra.Backend.API.Infrastructure.Grpc;
using Shipra.GrpcContracts;

namespace Shipra.Backend.API.Infrastructure;

public static class StartupSetup
{
  public static void AddDbContext(this IServiceCollection services, IConfiguration configuration, string masterDbconnectionString)
  {
    //services.AddDbContext<ShipraCatalogueDbContext>(options =>
    //options.UseSqlServer(shippraCatalougeConnection)); // will be created in web project root
    // Configure CacheSettings using the configuration parameter 
    // Add MemoryCache service
    services.AddMemoryCache();
    services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

    services.AddDbContext<ShipraMasterDbContext>(options =>
        options.UseSqlServer(masterDbconnectionString)); // will be created in web project root

   services.AddDbContext<ShipperInvoiceDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("ShipperInvoiceConnection"))); // will be created in web project root


    //services.AddDbContext<AppDbContext>(options =>
    //    options.UseSqlServer(connectionString)); // will be created in web project root
    services.AddTransient<ICurrentTenantService, CurrentTenantService>();
    //services.AddScoped<ICurrentTenantRepository, CurrentTenantRepository>(); 
    services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    {
      var tenantProvider = serviceProvider.GetRequiredService<ICurrentTenantService>();
      var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
      var tenant = tenantProvider.GetTenantAsync(httpContextAccessor.HttpContext!);
      
      // Use tenant-specific connection string
      options.UseSqlServer(tenant);
    });
    services.AddTransient<DapperAppDbContext>();
    //#region hangire
    //services.AddHangfireServices(configuration);  // Add Hangfire configuration
    //#endregion
    #region grpc config
    GrpcSettings? grpcSettings = configuration.GetSection("GrpcSettings").Get<GrpcSettings>();
    if (grpcSettings != null && !string.IsNullOrEmpty(grpcSettings.BaseAddress))
    {
      // Register the gRPC client factory
      services.AddSingleton<GrpcClientFactory>(sp =>
      {
        var baseAddress = grpcSettings?.BaseAddress ?? throw new InvalidOperationException("gRPC base address not configured.");
        return new GrpcClientFactory(baseAddress);
      });
       
      // Register the notification service
      services.AddScoped<IGrpcClientService, GrpcClientService>();
    }
    #endregion
  }

}
