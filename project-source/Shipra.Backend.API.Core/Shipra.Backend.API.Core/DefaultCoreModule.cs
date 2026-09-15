using Autofac;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Services;

namespace Shipra.Backend.API.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();

    builder.RegisterType<DeleteContributorService>()
        .As<IDeleteContributorService>().InstancePerLifetimeScope();
    
  }
}
