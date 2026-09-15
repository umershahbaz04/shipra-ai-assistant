using MediatR;

namespace Shipra.Backend.API.Web;
public static class ServiceCollectionExtensions
{



  public static void InstallServices(this IServiceCollection services, IConfiguration Configuration)
  {
    services.AddHttpContextAccessor();
    var assembly = AppDomain.CurrentDomain.Load("Shipra.Backend.API.Application");
    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    services.AddMediatR(assembly);
    services.AddAutoMapper(assembly);
    //services.AddMediatR(Assembly.GetExecutingAssembly());
  }
}
